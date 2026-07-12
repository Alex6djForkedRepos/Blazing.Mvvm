#!/bin/bash

# Workaround for a docfx (2.78.x) apiPage defect.
#
# docfx's apiPage renderer treats "{...}" in a page's `src:` URL as a template
# token. Source files named with the {T} generic convention (e.g. IView{T}.cs)
# therefore produce a src URL like ".../IView{T}.cs", which fails to render and
# silently drops the page, emitting "Unable to find file ...-1.yml" TOC errors.
#
# Fix: URL-encode the curly braces ({ -> %7B, } -> %7D) on `src:` lines only.
# The page then renders and GitHub still resolves the encoded link to the real
# file. Restricting to `src:` lines avoids touching code samples.
#
# Run AFTER `docfx metadata` and BEFORE `docfx build`.

set -e

script_dir=$(dirname "$0")
docs_dir=$(cd "$script_dir/.." && pwd)
api_dir="$docs_dir/api-docs"

if [ ! -d "$api_dir" ]; then
  echo "fix-apipage-src: '$api_dir' not found. Run 'docfx metadata' first." >&2
  exit 1
fi

find "$api_dir" -type f -name '*.yml' -print0 \
  | xargs -0 sed -i '/^[[:space:]]*src:/ { s/{/%7B/g; s/}/%7D/g }'

echo "fix-apipage-src: encoded curly braces in src URLs under $api_dir"
