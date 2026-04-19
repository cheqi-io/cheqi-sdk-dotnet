ENV ?= test

SPEC_LOCAL_URL := http://localhost:8080/v3/api-docs.yaml
SPEC_TEST_URL := https://test.api.cheqi.io/v3/api-docs.yaml
SPEC_SANDBOX_URL := https://sandbox.api.cheqi.io/v3/api-docs.yaml
SPEC_PROD_URL := https://api.cheqi.io/v3/api-docs.yaml

ifeq ($(ENV),local)
SPEC_URL := $(SPEC_LOCAL_URL)
else ifeq ($(ENV),test)
SPEC_URL := $(SPEC_TEST_URL)
else ifeq ($(ENV),sandbox)
SPEC_URL := $(SPEC_SANDBOX_URL)
else ifeq ($(ENV),prod)
SPEC_URL := $(SPEC_PROD_URL)
else
$(error Unsupported ENV '$(ENV)'. Use one of: local, test, sandbox, prod)
endif

GENERATED_DIR := Models/Generated

.PHONY: fetch-spec fetch-spec-local generate build test pack spec-check

fetch-spec:
	curl -fsSL $(SPEC_URL) -o openapi.yaml
	@echo "Spec updated. Run 'make generate' to regenerate models."

fetch-spec-local:
	$(MAKE) fetch-spec ENV=local

generate:
	rm -rf /tmp/cheqi-dotnet-gen
	openapi-generator generate \
		-i openapi.yaml \
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
	rm -rf /tmp/cheqi-dotnet-gen
	@echo "Models generated in $(GENERATED_DIR)/"

build:
	dotnet build cheqi-sdk.csproj --configuration Release

test:
	dotnet test tests/cheqi-sdk.Tests/cheqi-sdk.Tests.csproj --configuration Release

pack:
	dotnet pack cheqi-sdk.csproj --configuration Release --no-build

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
