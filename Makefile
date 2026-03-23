SPEC_URL := https://test.api.cheqi.io/v3/api-docs.yaml
GENERATED_DIR := Models/Generated

.PHONY: fetch-spec generate build test spec-check

fetch-spec:
	curl -fsSL $(SPEC_URL) -o openapi.yaml
	@echo "Spec updated. Run 'make generate' to regenerate models."

generate:
	rm -rf /tmp/cheqi-dotnet-gen
	python3 -c "\
	import yaml; \
	d=yaml.safe_load(open('openapi.yaml')); \
	schemas=d.get('components',{}).get('schemas',{}); \
	[s.update({'enum': list(dict.fromkeys(s['enum']))}) for s in \
	 [schemas.get(n,{}) for n in schemas] + \
	 [schemas.get(n,{}).get('properties',{}).get(p,{}) \
	  for n in schemas for p in schemas.get(n,{}).get('properties',{})] \
	 if 'enum' in s and len(s['enum']) != len(set(s['enum']))]; \
	yaml.dump(d, open('/tmp/cheqi-openapi-deduped.yaml','w'), default_flow_style=False, sort_keys=False)"
	openapi-generator generate \
		-i /tmp/cheqi-openapi-deduped.yaml \
		-g csharp \
		-o /tmp/cheqi-dotnet-gen \
		--global-property models,supportingFiles=Option.cs \
		--model-package Models.Generated \
		--additional-properties=packageName=Cheqi.Sdk,sourceFolder=.,targetFramework=net8.0,nullableReferenceTypes=true,useDateTimeOffset=true,library=generichost,optionalProjectFile=false,optionalAssemblyInfo=false,validatable=false,hideGenerationTimestamp=true
	rm -rf $(GENERATED_DIR)
	mkdir -p $(GENERATED_DIR)
	cp /tmp/cheqi-dotnet-gen/Cheqi.Sdk/Client/Option.cs $(GENERATED_DIR)/
	cp /tmp/cheqi-dotnet-gen/Cheqi.Sdk/Models.Generated/*.cs $(GENERATED_DIR)/
	find $(GENERATED_DIR) -name '*.cs' -exec sed -i '' 's/using Cheqi\.Sdk\.Client;//' {} +
	find $(GENERATED_DIR) -name '*.cs' -exec sed -i '' 's/namespace Cheqi\.Sdk\.Client/namespace Cheqi.Sdk.Models.Generated/' {} +
	rm -rf /tmp/cheqi-dotnet-gen /tmp/cheqi-openapi-deduped.yaml
	@echo "Models generated in $(GENERATED_DIR)/"

build:
	dotnet build cheqi-sdk.csproj --configuration Release

test:
	dotnet test tests/cheqi-sdk.Tests/cheqi-sdk.Tests.csproj

spec-check:
	@curl -fsSL $(SPEC_URL) -o openapi.yaml.tmp
	@if diff -q openapi.yaml openapi.yaml.tmp > /dev/null 2>&1; then \
		echo "Spec is up to date."; \
	else \
		echo "ERROR: Local openapi.yaml differs from $(SPEC_URL)"; \
		echo "Run 'make fetch-spec && make generate' to update."; \
		diff openapi.yaml openapi.yaml.tmp || true; \
		rm openapi.yaml.tmp; \
		exit 1; \
	fi
	@rm -f openapi.yaml.tmp
