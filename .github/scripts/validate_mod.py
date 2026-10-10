#!/usr/bin/env python3
"""Static repository checks for HSK Algae Cultivation."""

from pathlib import Path
import re
import sys
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
VERSIONS = ("1.5", "1.6")
REQUIRED_FILES = (
    "Assemblies/HSKAlgaeCultivation.dll",
    "Defs/SeedsPlease_Algae.xml",
    "Languages/English/Keyed/HSKAlgaeCultivation.xml",
    "Languages/Russian/Keyed/HSKAlgaeCultivation.xml",
    "Patches/GrowZonePatch.xml",
    "Patches/PlantAlgaeSowable.xml",
)
LANGUAGES = ("English", "Russian")
ERRORS = []


def fail(message: str) -> None:
    ERRORS.append(message)


def parse_xml(path: Path):
    try:
        return ET.parse(path).getroot()
    except (ET.ParseError, OSError) as exc:
        fail(f"Invalid or unreadable XML: {path.relative_to(ROOT)} ({exc})")
        return None


# Validate all XML definitions, patches, translations, and mod metadata.
xml_files = sorted(ROOT.glob("About/*.xml"))
for version in VERSIONS:
    xml_files.extend(sorted((ROOT / version).rglob("*.xml")))
for xml_path in xml_files:
    parse_xml(xml_path)

metadata_path = ROOT / "About" / "About.xml"
metadata = parse_xml(metadata_path)
if metadata is not None:
    supported = {
        node.text for node in metadata.findall("./supportedVersions/li") if node.text
    }
    if supported != set(VERSIONS):
        fail(f"About/About.xml supportedVersions should be exactly {VERSIONS}, got {sorted(supported)}")
    package_id = metadata.findtext("./packageId")
    if package_id != "smokerdl.hsk.algaecultivation":
        fail(f"Unexpected packageId in About/About.xml: {package_id!r}")

# The 1.5 folder is retained as a legacy snapshot; new features target 1.6.
for version in VERSIONS:
    version_root = ROOT / version
    for relative in REQUIRED_FILES:
        path = version_root / relative
        if not path.is_file():
            fail(f"Missing required file: {path.relative_to(ROOT)}")
        elif path.suffix == ".xml":
            parse_xml(path)

# Translation keys must match within each version and between locales.
translation_keys = {}
for language in LANGUAGES:
    keys_by_version = {}
    for version in VERSIONS:
        path = ROOT / version / "Languages" / language / "Keyed" / "HSKAlgaeCultivation.xml"
        root = parse_xml(path) if path.is_file() else None
        if root is not None:
            keys = {child.tag for child in list(root)}
            keys_by_version[version] = keys
    for version, keys in keys_by_version.items():
        if keys != keys_by_version.get("1.5", keys):
            fail(f"Translation keys differ across version folders for {language}: {version}={sorted(keys)}, 1.5={sorted(keys_by_version.get('1.5', set()))}")
    if "1.6" in keys_by_version:
        translation_keys[language] = keys_by_version["1.6"]
    elif "1.5" in keys_by_version:
        translation_keys[language] = keys_by_version["1.5"]

if "English" in translation_keys and "Russian" in translation_keys:
    if translation_keys["English"] != translation_keys["Russian"]:
        fail(
            "English and Russian translation keys differ: "
            f"English-only={sorted(translation_keys['English'] - translation_keys['Russian'])}; "
            f"Russian-only={sorted(translation_keys['Russian'] - translation_keys['English'])}"
        )

# Ensure translation keys used by the C# source are defined in every active locale.
source_dir = ROOT / "Source" / "HSKAlgaeCultivation"
translate_pattern = re.compile(r'"(HSKAlgaeCultivation_[A-Za-z0-9_]+)"\s*\.Translate')
source_keys = set()
for source_path in sorted(source_dir.glob("*.cs")):
    try:
        source_keys.update(translate_pattern.findall(source_path.read_text(encoding="utf-8")))
    except OSError as exc:
        fail(f"Cannot read source file {source_path.relative_to(ROOT)} ({exc})")

for language, keys in translation_keys.items():
    missing = source_keys - keys
    if missing:
        fail(f"{language} translations missing C# keys: {sorted(missing)}")

if ERRORS:
    print("Repository validation FAILED:")
    for error in ERRORS:
        print(f" - {error}")
    sys.exit(1)

print("Repository validation passed: XML parsed, legacy/active folders checked, active translations are consistent.")
