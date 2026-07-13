# Documentation

The documentation is powered by [DocFX](https://dotnet.github.io/docfx/). Conceptual docs are written in Markdown and live in the `site` folder (with `index.md` as the landing page), while the API reference is generated from the library source by DocFX. The documentation is built and deployed to GitHub Pages using GitHub Actions ([ci-cd.yml](../../.github/workflows/ci-cd.yml)).

## How to build the documentation

### Prerequisites

- Install the .NET SDK that the project is using.
- Install [DocFX](https://dotnet.github.io/docfx/) as a global tool. The project version can be found in the [ci-cd.yml](../../.github/workflows/ci-cd.yml) file (currently `2.78.5`):

  ```bash
  dotnet tool update -g docfx --version 2.78.5
  ```

- Bash. If you are on Windows, you can use Git Bash or WSL.

### Build and preview

To build the documentation, run the following command in the `docs/github-pages` folder:

```bash
_scripts/local-doc-gen.sh
```

The script performs some clean up, generates the API metadata and cross-reference map, runs `docfx` to build the site, and then serves it on `http://localhost:8080`. You can then preview the documentation in your browser.

To serve the documentation on a different port, pass the port number as an argument to the script:

```bash
_scripts/local-doc-gen.sh 8081
```

## What the build does

`_scripts/local-doc-gen.sh` runs the full local pipeline:

1. Removes previous build artifacts (`_site`, `api-docs`, `xrefs`, and intermediate folders).
2. **`generate-xrefmap.sh`** — produces an xref map from a classic `mref` pass so that in-prose `<xref>` links inside the generated API pages resolve to links instead of plain text.
3. **`docfx metadata`** — generates the API reference (apiPage) from the `Blazing.Mvvm.Base`, `Blazing.Mvvm`, and `Blazing.Mvvm.Analyzers` projects.
4. **`fix-apipage-src.sh`** — URL-encodes `{T}` braces in `src:` links, a workaround for a DocFX 2.78 apiPage defect that would otherwise drop generic-typed pages (e.g. `IView{T}.cs`).
5. **`docfx build`** — builds the static site into `_site`.
6. **`docfx serve`** — serves `_site` on the chosen port.

> [!NOTE]
> The GitHub Pages deployment (in `ci-cd.yml`) passes the hosting sub path (`/Blazing.Mvvm/`) to `generate-xrefmap.sh` so cross-references resolve under the project-site URL. The local build uses the default root sub path (`/`).

## Folder layout

| Path | Purpose |
|------|---------|
| `index.md` | Landing page |
| `site/` | Conceptual docs (Markdown) and their `toc.yml` |
| `api-docs/` | Generated API reference (produced by the build) |
| `xrefs/` | Generated cross-reference map (`xrefmap.yml`) used to resolve API links |
| `template/` | Custom theme overrides (`public/main.css`, `public/main.js`) |
| `images/` | Logo, favicon, and other assets |
| `docfx.json` | DocFX build configuration |
| `docfx-xref.json` | DocFX configuration used to generate the cross-reference map |
| `_scripts/` | Local build helper scripts |
