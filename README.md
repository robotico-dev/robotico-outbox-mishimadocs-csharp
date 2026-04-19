# Robotico.Outbox.Mishima

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![C#](https://img.shields.io/badge/C%23-latest-239120?logo=csharp)](https://learn.microsoft.com/dotnet/csharp/)
[![MishimaDocs](https://img.shields.io/badge/MishimaDocs-engine-5C4EE5)](https://www.nuget.org/packages/MishimaDocs)
[![NuGet](https://img.shields.io/badge/NuGet-Robotico.Outbox.Mishima-blue?logo=nuget)](https://github.com/robotico-dev/robotico-outbox-mishimadocs-csharp/packages)
[![Robotico](https://img.shields.io/badge/Robotico-Tier%203%20adapter-1f883d?logo=github)](https://github.com/robotico-dev)

NuGet package id: **`Robotico.Outbox.Mishima`** — `IOutbox` on **MishimaDocs** (one JSON document per message). Optional `IMishimaWriteBatch` / `IMishimaAsyncWriteBatch` defers persistence until commit when you use MishimaDocs batch APIs in application code.

**Target framework:** `net10.0` only (matches the MishimaDocs engine).

## Persistence and transactions

| Topic | Behavior |
|-------|----------|
| Default outbox writes | Maps to MishimaDocs document operations; consult package XML docs for immediate vs batched paths |
| Cross-message atomicity | Use MishimaDocs write batches (`BeginWriteBatch` / `BeginAsyncWriteBatch`) where you need all-or-nothing multi-document commits |
| Robotico `IUnitOfWork` | Pair with link:../robotico-repository-mishimadocs-csharp/README.md[`Robotico.Repository.Mishima`] `MishimaUnitOfWork` only for API shape — that UoW is `NoOpCommitSuccess` per link:../build/ROBOTICO_REPOSITORY_ADAPTER_IO_INVENTORY.adoc[adapter I/O inventory]; do not assume EF-style deferred flush unless you implement it with Mishima batches |

Application-level guidance: link:../build/ROBOTICO_APPLICATION_ARCHITECTURE.adoc[Robotico application architecture].

## Build (Robotico monorepo)

**MishimaDocs dependency (dual mode):** when `mishima-suite/mishima-docs` exists at `MishimaDocsProjectPath` (two levels up from this adapter folder — typical Robotico monorepo layout under `csharp/`), MSBuild uses a **ProjectReference** to the engine sources. Otherwise restore uses **`MishimaDocs`** from NuGet at the version pinned in `Directory.Packages.props` (publish that package to your feed, or use `ROBOTICO_USE_LOCAL_NUGETS` / `csharp/nugets` per `csharp/build/ROBOTICO_NUGET_DUAL_MODE_PLAN.adoc`).

```bash
dotnet restore Robotico.Outbox.Mishima.sln
dotnet build Robotico.Outbox.Mishima.sln -c Release
dotnet test Robotico.Outbox.Mishima.sln -c Release
```

## CI and quality bar

GitHub Actions runs build, tests, coverlet (adapter assembly), coverage threshold, and — when this repo lives under `csharp/` next to `csharp/tools` — `verify-robotico-library-bar.ps1` (one top-level type per `.cs` file, no `var` in `robotico-*` sources). See `csharp/build/ROBOTICO_LIBRARY_10_STANDARD.adoc` in the Robotico workspace.

## License

See [LICENSE](LICENSE).
