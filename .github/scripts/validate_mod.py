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
ACTIVE_REQUIRED_FILES = (
    "Patches/PlantReedsSowable.xml",
    "Patches/SeedsPleaseReeds.xml",
    "Languages/English/DefInjected/SeedsPlease.SeedDef/HSKAlgaeCultivation.xml",
    "Languages/Russian/DefInjected/SeedsPlease.SeedDef/HSKAlgaeCultivation.xml",
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
    required_files = list(REQUIRED_FILES)
    if version == "1.6":
        required_files.extend(ACTIVE_REQUIRED_FILES)
    for relative in required_files:
        path = version_root / relative
        if not path.is_file():
            fail(f"Missing required file: {path.relative_to(ROOT)}")
        elif path.suffix == ".xml":
            parse_xml(path)

# Translation keys must match between English and Russian within each version.
translation_keys = {}
for version in VERSIONS:
    keys_by_language = {}
    for language in LANGUAGES:
        path = ROOT / version / "Languages" / language / "Keyed" / "HSKAlgaeCultivation.xml"
        root = parse_xml(path) if path.is_file() else None
        if root is not None:
            keys_by_language[language] = {child.tag for child in list(root)}
    if "English" in keys_by_language and "Russian" in keys_by_language:
        if keys_by_language["English"] != keys_by_language["Russian"]:
            fail(
                f"English/Russian translation keys differ for RimWorld {version}: "
                f"English-only={sorted(keys_by_language['English'] - keys_by_language['Russian'])}; "
                f"Russian-only={sorted(keys_by_language['Russian'] - keys_by_language['English'])}"
            )
    translation_keys[version] = keys_by_language

# New C# translation keys must exist in the active 1.6 translations.
source_dir = ROOT / "Source" / "HSKAlgaeCultivation"
translate_pattern = re.compile(r'"(HSKAlgaeCultivation_[A-Za-z0-9_]+)"\s*\.Translate')
source_keys = set()
for source_path in sorted(source_dir.glob("*.cs")):
    try:
        source_keys.update(translate_pattern.findall(source_path.read_text(encoding="utf-8")))
    except OSError as exc:
        fail(f"Cannot read source file {source_path.relative_to(ROOT)} ({exc})")

for language in LANGUAGES:
    keys = translation_keys.get("1.6", {}).get(language, set())
    missing = source_keys - keys
    if missing:
        fail(f"Active 1.6 {language} translations missing C# keys: {sorted(missing)}")

if ERRORS:
    print("Repository validation FAILED:")
    for error in ERRORS:
        print(f" - {error}")
    sys.exit(1)

print("Repository validation passed: XML parsed, legacy/active folders checked, active translations are consistent.")
