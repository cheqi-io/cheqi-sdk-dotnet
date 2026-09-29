"""Compare OpenAPI contracts while ignoring YAML mapping order."""
from pathlib import Path
import sys
import yaml

local, remote = (Path(path) for path in sys.argv[1:])
if yaml.safe_load(local.read_text()) != yaml.safe_load(remote.read_text()):
    sys.exit("ERROR: OpenAPI contract has changed. Run 'make fetch-spec && make generate'.")
print("OpenAPI contract is up to date (mapping order ignored).")
