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
MIRRORED_XML = (
    "Defs/SeedsPlease_Algae.xml",
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

# Check that both game-version folders are complete.
for version in VERSIONS:
    version_root = ROOT / version
    for relative in REQUIRED_FILES:
        path = version_root / relative
        if not path.is_file():
            fail(f"Missing required file: {path.relative_to(ROOT)}")
        elif path.suffix == ".xml":
            parse_xml(path)

# Defs and game patches should stay in sync between supported versions.
for relative in MIRRORED_XML:
    left = ROOT / VERSIONS[0] / relative
    right = ROOT / VERSIONS[1] / relative
    if left.is_file() and right.is_file():
        if left.read_bytes() != right.read_bytes():
            fail(f"Version-specific files unexpectedly differ: {VERSIONS[0]}/{relative} vs {VERSIONS[1]}/{relative}")

# Each locale should expose the same keyed entries in both game versions.
translation_keys = {}
for language in LANGUAGES:
    keys_by_version = {}
    for version in VERSIONS:
        path = ROOT / version / "Languages" / language / "Keyed" / "HSKAlgaeCultivation.xml"
        root = parse_xml(path) if path.is_file() else None
        if root is not None:
            keys = {child.tag for child in list(root)}
            keys_by_version[version] = keys
    if len(keys_by_version) == len(VERSIONS):
        expected = keys_by_version[VERSIONS[0]]
        for version in VERSIONS[1:]:
            if keys_by_version[version] != expected:
                fail(f"Translation keys differ for {language}: {VERSIONS[0]}={sorted(expected)}, {version}={sorted(keys_by_version[version])}")
        translation_keys[language] = expected

if "English" in translation_keys and "Russian" in translation_keys:
    if translation_keys["English"] != translation_keys["Russian"]:
        fail(
            "English and Russian translation keys differ: "
            f"English-only={sorted(translation_keys['English'] - translation_keys['Russian'])}; "
            f"Russian-only={sorted(translation_keys['Russian'] - translation_keys['English'])}"
        )

# Ensure translation keys used by the C# source are defined in every locale.
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

print(f"Repository validation passed: XML parsed, {len(VERSIONS)} game versions checked, translations are consistent.")
