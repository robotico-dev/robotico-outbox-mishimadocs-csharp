# Changelog

All notable changes to **Robotico.Outbox.Mishima** will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Dual Mishima dependency: **ProjectReference** when engine sources exist at `MishimaDocsProjectPath`, else **PackageReference** `MishimaDocs` (central version in `Directory.Packages.props`).
- `publish.yml` (build, test, coverage gate, pack/push to GitHub Packages) and Dependabot for NuGet + GitHub Actions.
- `.editorconfig` enforcing explicit types (`IDE0008`) per Robotico Library 10/10 standard.
- `tests/coverlet.runsettings` and GitHub Actions CI (restore, build, test, coverage gate, `verify-robotico-library-bar.ps1` when run from umbrella `csharp/` layout).
- Central `MishimaDocsProjectPath` in `Directory.Build.props` for the MishimaDocs engine project reference.
- Test coverage for enqueue JSON serialization failure (cyclic graph).
- CsCheck property test for **MishimaOutboxMessageDocument** JSON round-trip (production `MishimaOutboxJsonOptions`).

### Changed

- Test code uses explicit types (no `var`) per Robotico conventions.
