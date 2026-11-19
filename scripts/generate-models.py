"""Generate the published C# models in an isolated temporary directory."""
from pathlib import Path
import shutil
import subprocess
import sys
import tempfile

root = Path(__file__).resolve().parent.parent
with tempfile.TemporaryDirectory(prefix="cheqi-dotnet-gen-") as directory:
    subprocess.run([
        "openapi-generator", "generate", "-i", str(root / "openapi.yaml"),
        "-g", "csharp", "-o", directory,
        "--global-property", "models,supportingFiles=Option.cs",
        "--model-package", "Models.Generated",
        "--additional-properties",
        "packageName=Cheqi.Sdk,sourceFolder=.,targetFramework=net8.0,nullableReferenceTypes=true,"
        "useDateTimeOffset=true,library=generichost,optionalProjectFile=false,optionalAssemblyInfo=false,"
        "validatable=false,hideGenerationTimestamp=true",
    ], cwd=root, check=True)
    generated = Path(directory) / "Cheqi.Sdk"
    sources = list((generated / "Models.Generated").glob("*.cs"))
    sources.append(generated / "Client/Option.cs")
    if len(sources) < 2 or any(not path.exists() for path in sources):
        sys.exit("Generator did not produce the expected model files")
    destination = root / "Models/Generated"
    destination.mkdir(parents=True, exist_ok=True)
    for path in destination.glob("*.cs"):
        path.unlink()
    for path in sources:
        shutil.copyfile(path, destination / path.name)
subprocess.run([sys.executable, str(root / "scripts/prepare-generated-models.py")], cwd=root, check=True)
print("Models generated in Models/Generated/")
