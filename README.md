# Robotico.Outbox.Mishima

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![C#](https://img.shields.io/badge/C%23-latest-239120?logo=csharp)](https://learn.microsoft.com/dotnet/csharp/)
[![MishimaDocs](https://img.shields.io/badge/MishimaDocs-engine-5C4EE5)](https://www.nuget.org/packages/MishimaDocs)
[![NuGet](https://img.shields.io/badge/NuGet-Robotico.Outbox.Mishima-blue?logo=nuget)](https://github.com/robotico-dev/robotico-outbox-mishimadocs-csharp/packages)
[![Robotico](https://img.shields.io/badge/Robotico-Tier%203%20adapter-1f883d?logo=github)](https://github.com/robotico-dev)

NuGet package id: **`Robotico.Outbox.Mishima`** — `IOutbox` on **MishimaDocs** (one JSON document per message). Optional `IMishimaWriteBatch` defers persistence until `CommitAsync`.

**Target framework:** `net10.0` only (matches the MishimaDocs engine).

## Build (Robotico monorepo)

Place this repo under `csharp/robotico-outbox-mishimadocs-csharp` beside `mishima-suite/mishima-docs` for the **ProjectReference** to `MishimaDocs`.

```bash
dotnet restore Robotico.Outbox.Mishima.sln
dotnet build Robotico.Outbox.Mishima.sln -c Release
dotnet test Robotico.Outbox.Mishima.sln -c Release
```

## License

See [LICENSE](LICENSE).
