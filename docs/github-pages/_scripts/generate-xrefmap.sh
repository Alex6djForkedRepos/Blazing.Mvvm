#!/bin/bash

# Generates an xref map (UID -> href) for the API surface.
#
# apiPage renders internal <xref> cross-references in prose (summaries, type
# parameter descriptions, etc.) as plain unlinked text unless an xref map is
# fed back into the main build. This script produces that map from a classic
# mref pass and copies it to the xrefs folder for docfx.json to consume.
#
# The map hrefs are made absolute (prefixed with the hosting sub path) so the
# links resolve from API pages at any folder depth. Pass the sub path the site
# is hosted under:
#   - local preview served at the root:   "/"  (default)
#   - GitHub Pages project site:          "/Blazing.Mvvm/"
#
# Run BEFORE generate-metadata.sh / docfx build.

set -e

# Sub path (default "/") with guaranteed leading and trailing slash
sub_path="${1:-/}"
[[ "${sub_path}" != /* ]] && sub_path="/${sub_path}"
[[ "${sub_path}" != */ ]] && sub_path="${sub_path}/"

readonly sub_path
readonly script_dir=$(dirname "$0")
readonly docs_dir=$(cd "$script_dir/.." && pwd)
readonly gen_folder="_xref-gen"
readonly xref_dir="xrefs"

cd "$docs_dir"

docfx metadata docfx-xref.json
docfx build docfx-xref.json

mkdir -p "$xref_dir"
cp -f "$gen_folder/xrefmap.yml" "$xref_dir/xrefmap.yml"

# Make hrefs absolute so cross-references resolve from any page depth
sed -i.bak "s|href: |href: ${sub_path}|g" "$xref_dir/xrefmap.yml"
rm -f "$xref_dir/xrefmap.yml.bak"

# Clean up intermediate artifacts (the main build regenerates api-docs as apiPage)
rm -rf api-docs "$gen_folder"

echo "generate-xrefmap: wrote $xref_dir/xrefmap.yml (sub path '${sub_path}')"
