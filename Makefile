ENV ?= test
PYTHON ?= python3

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
	$(PYTHON) scripts/generate-models.py

build:
	dotnet build cheqi-sdk.csproj --configuration Release

test:
	dotnet test tests/cheqi-sdk.Tests/cheqi-sdk.Tests.csproj --configuration Release

pack:
	dotnet pack cheqi-sdk.csproj --configuration Release --no-build

spec-check:
	@curl -fsSL $(SPEC_URL) -o openapi.yaml.tmp
	@$(PYTHON) scripts/check-spec.py openapi.yaml openapi.yaml.tmp; result=$$?; rm -f openapi.yaml.tmp; exit $$result
