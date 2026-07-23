#!/bin/bash

# Full local documentation build:
#   xrefmap -> apiPage metadata -> fix apiPage src -> build -> serve.
# Usage: _scripts/local-doc-gen.sh [port]

set -e

readonly default_port=8080
readonly script_dir=$(dirname "$0")
readonly docs_dir=$(cd "$script_dir/.." && pwd)

# Read port number from the first argument, use default port if not provided
readonly port="${1:-$default_port}"

# Validate port
if ! [[ $port =~ ^[0-9]+$ ]] ; then
   echo "Error: Port must be a number" >&2
   exit 1
fi

cd "$docs_dir"

# Remove existing build artifacts
rm -rf _site api-docs _xref-gen xrefs obj

# 1. Generate the xref map (mref pass) so apiPage cross-references resolve to links
"$script_dir/generate-xrefmap.sh"

# 2. Generate API metadata (apiPage)
docfx metadata docfx.json

# 3. Encode "{T}" braces in src links (docfx apiPage workaround)
"$script_dir/fix-apipage-src.sh"

# 4. Build and serve
docfx build docfx.json
docfx serve _site --port "$port"
