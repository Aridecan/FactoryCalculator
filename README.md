# FactoryCalculator

Factory game production-rate calculator

Projects:
- `FactoryCalculator.Core` — core domain, serializers, calculation engine (NET 8)
- `FactoryCalculator` — WinUI front-end
- `FactoryCalculator.Core.Tests` — unit tests (xUnit)

Quick start (CLI):
1. dotnet build FactoryCalculator.sln
2. dotnet test FactoryCalculator.sln

CI: GitHub Actions run build, tests and format checks on push/PR. Release workflow packs `FactoryCalculator.Core` and pushes to NuGet when you push a `vX.Y.Z` tag.