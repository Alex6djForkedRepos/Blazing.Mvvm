# Contributing

We love your input! We want to make contributing to this project as easy and transparent as possible, whether it's:

- Reporting a bug
- Discussing the current state of the code
- Submitting a fix
- Proposing new features

When contributing to this repository, please first discuss the change you wish to make via an issue, discussion, or any other method with the maintainers of this repository before making a change. You can also pick up an existing issue by looking for those [marked with `help wanted`](https://github.com/gragra33/Blazing.Mvvm/issues?q=is%3Aissue+is%3Aopen+label%3A%22help+wanted%22).

Please note we have a [Code of Conduct](CODE_OF_CONDUCT.md); follow it in all your interactions with the project.

## Project Prerequisites

- The [.NET SDK](https://dotnet.microsoft.com/download) for the target frameworks the project builds against.
- Any modern .NET IDE can also be used.

## Change Guidelines

- Follow the existing code style and conventions of the project.
- Add or update unit tests for the feature, bugfix, or hotfix.
- Add code comments where necessary to explain hard-to-understand code.
- Add XML documentation comments to any new public APIs.
- Update the documentation (in `docs/github-pages`) when behaviour or public APIs change.

## Pull Requests and Git Flow

> [!IMPORTANT]
> Follow the branch and target-branch guidelines below. Pull requests that do not follow these guidelines may not be accepted.

### Branch and target rules

| Change type | Branch prefix | Create from | Pull request target | Merge method |
| --- | --- | --- | --- | --- |
| Feature, enhancement, bug fix, or maintenance | `feature/`, `feat/`, `fix/`, `bugfix/`, or `chore/` | `develop` | `develop` | Squash merge |
| Release | `release/` | `develop` | `master` | Merge commit |
| Hotfix | `hotfix/` | `master` | `master` | Merge commit |

```mermaid
---
title: Git Flow
---
gitGraph
    commit tag: "1.0.0"
    branch develop
    branch feature
    commit
    commit
    checkout develop
    merge feature id: "feature merge" tag: "1.1.0-beta"
    branch bugfix
    commit
    commit
    checkout develop
    merge bugfix id: "bugfix merge" tag: "1.1.1-beta"
    branch release
    commit id: "prepare release"
    checkout master
    merge release tag: "1.1.3"
    checkout develop
    merge master
    checkout master
    branch hotfix
    commit
    checkout master
    merge hotfix tag: "1.1.4"
    checkout develop
    merge master
```

### Submitting a pull request

1. Fork the repository.
2. Create your branch using the prefix and base branch shown in the table above.
3. Implement your change and add or update tests and documentation as needed.
4. Rebase your branch onto its base branch. Do not merge the base branch into your working branch.
5. Open a pull request against the target branch shown in the table.

### Maintainer responsibilities

- When `develop` reaches a release milestone, create a `release/*` branch and open its pull request.
- Use the merge method shown in the table for each type of pull request.
- Delete release and hotfix branches after they are merged.
- After a stable package is published, approve the workflow run on the automatic `master`-to-`develop` synchronization pull request, then confirm that the pull request is merged.

### Tests

Run the tests via your IDE's test explorer or the .NET CLI from the `src` folder:

```bash
dotnet test
```

### Documentation

The documentation site is built with DocFX and lives in [`docs/github-pages`](docs/github-pages). See its [README](docs/github-pages/README.md) for how to build and preview the docs locally.

## Versioning

Blazing.Mvvm uses [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) (nbgv) to derive the package and assembly versions from Git history. Versioning is configured in [`src/version.json`](src/version.json).

- The `version` field sets the base version, such as `3.3`.
- Nerdbank.GitVersioning calculates the patch number from the Git height. Each qualifying commit produces a new version, such as `3.3.5` and then `3.3.6`.
- The `pathFilters` setting means only changes under `src` contribute to the Git height. Changes under `src/Tests` are excluded, as are changes to documentation, samples, and repository metadata.
- The `publicReleaseRefSpec` setting marks only `master` as a public release branch.
- [`src/Directory.Build.props`](src/Directory.Build.props) gives non-public packages the `-beta` suffix.

For example, if Nerdbank.GitVersioning calculates `3.3.5`:

| Branch | Package version | Release type |
| --- | --- | --- |
| `develop` | `3.3.5-beta` | Prerelease |
| `master` | `3.3.5` | Stable release |

Do not manually increment the patch number for each release. To start a new minor or major version line, update the `version` field in `src/version.json`, for example from `3.3` to `3.4` or `4.0`, and commit that change. Changing `src/version.json` is not the only release signal; any qualifying production-source commit gets a new patch version from Nerdbank.GitVersioning.

## CI/CD

The [CI/CD workflow](.github/workflows/ci-cd.yml) runs for pull requests targeting `develop` or `master`, and for relevant pushes to those branches.

### Pull requests

For a pull request, the workflow:

1. Restores and builds the solution in the `Release` configuration.
2. Runs both test projects against the configured .NET version.
3. Uploads the TRX test results.

Pull-request builds never publish packages or create releases.

### Pushes to `develop` and `master`

After building and testing, the workflow checks `should_publish`. It compares the commits before and after the push, looks at changed files under `src`, and ignores files under `src/Tests`.

- If only tests, documentation, samples, or workflow files changed, the workflow does not publish.
- If at least one production file under `src` changed, the workflow packages and publishes the libraries.

The publishing result depends on the target branch:

| Push branch | NuGet package | GitHub release |
| --- | --- | --- |
| `develop` | Version ending in `-beta` | Prerelease |
| `master` | Stable version | Latest stable release |

The workflow publishes `.nupkg` and `.snupkg` files to NuGet.org and attaches the NuGet packages to the generated GitHub release. Duplicate NuGet versions are skipped, and an existing GitHub release is not created again.

After a successful stable publish from `master`, the workflow creates a `master`-to-`develop` synchronization pull request and enables auto-merge. Documentation deployment is separate and must be started manually from `master` with the `publish_docs` workflow input.

## Important Pull Request Labels

The following labels categorize pull requests and define what is documented in each release. See [release.yml](.github/release.yml) for the release notes configuration.

- `hotfix`: Pull request fixes a major bug in the latest release and needs to be merged ASAP.
- `breaking-change`: Pull request introduces a breaking change.
- `feature`: Pull request adds a new feature.
- `bugfix`: Pull request fixes a bug.
- `enhancement`: Pull request improves an existing feature.
- `ignore-for-release`: Pull request should not be included in the release notes.

See the [full list of labels](https://github.com/gragra33/Blazing.Mvvm/labels) for more details.
