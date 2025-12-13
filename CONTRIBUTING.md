# Contributing Guidelines

## Purpose
This CONTRIBUTING.md documents the project's preferred workflow, coding standards, static analysis, and release/packaging expectations to ensure consistent, testable, and maintainable code for the factory game calculator targeting .NET 8.

## Getting Started
- Target framework: .NET 8.
- Use Visual Studio 2026 for development.
- Follow the rules in `.editorconfig` for formatting and style.

## Development Workflow
1. Start with high-level feature requirements (user stories / use cases).
   - Capture the goal, inputs, outputs, and success criteria for each feature.
   - Keep feature descriptions concise and testable.
2. Design the domain model and core data structures next.
   - Translate user stories into domain entities, value objects, and service interfaces.
   - Define clear responsibilities and invariants for each type.
3. Implement in small vertical slices (feature → model → tests → implementation → review).
   - Create a failing unit test that expresses the behavior (Arrange/Act/Assert).
   - Implement the minimal code to make the test pass.
   - Refactor with tests in place before expanding.
4. Iterate: evolve features and data structures as needed. Favor readability and testability.

## Coding Standards
- Follow `.editorconfig` rules strictly.
- Prefer explicit types for public APIs and use `var` for local variables when the type is obvious.
- Use PascalCase for public types and methods; use camelCase for private fields with an underscore prefix for backing fields.
- Keep methods small and classes focused on a single domain concern.
- Write XML documentation for public types and members.

## Static Analysis and Formatting
- Required tools:
  - `Microsoft.CodeAnalysis.NetAnalyzers` (added via `Directory.Build.props`).
  - Enforce formatting with `dotnet format`.
  - Optional: `StyleCop.Analyzers`.
- CI enforcement:
  - CI runs `dotnet format --verify-no-changes` and `dotnet build` with analyzers enabled.
  - On CI, the project sets __TreatWarningsAsErrors__ to true so analyzer warnings fail the build.

## Testing
- Unit tests are required for core domain logic and calculator algorithms.
- Use xUnit (current choice) and put tests in a separate test project.
- Prefer TDD where practical.

## Release & Packaging
- Publish `FactoryCalculator.Core` as a NuGet package (pack/push triggered by git tags: `v*.*.*`).
- Use semantic versioning. Store `NUGET_API_KEY` in GitHub repository secrets for publishing.
- CI release workflow performs pack and `dotnet nuget push` when a release tag is pushed.

## Branching & PRs
- Feature branches: `feature/<short-description>`.
- Open PRs with clear description and tests for behavior.

## CI / GitHub Actions
- CI runs on push and PR and includes:
  - `dotnet restore`, `dotnet build`, `dotnet test`
  - `dotnet format --verify-no-changes`
  - Analyzer enforcement and optional CodeQL scans

## Minimal first vertical slice
- Implement Game Setup, Items, Machines, Recipes, Desired Amount, and the calculation engine to compute machine counts for a single product.