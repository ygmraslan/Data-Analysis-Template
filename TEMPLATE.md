# DataAnalysis — Template

This is a generic **data-analysis platform template** derived from a working
.NET 10 Clean Architecture + React 19 application. All business-specific data
access (SQL queries, source-table and column names) has been removed and
replaced with explanatory comments and compilable stubs, so you can plug in
your own data source without rewriting the architecture.

The Clean Architecture skeleton is intact: API / Application / Domain /
Infrastructure / Persistence, CQRS + MediatR, Dapper-based repositories with a
cached-repository + snapshot pattern, JWT auth, PDF/Excel export, and a
React + Vite + Tailwind + Zustand client.

## What was templated (you must fill these in)

1. **Source table** — `Infrastructure/Octopus/OctopusConnection.cs`
   `PolicyTable` is set to `"dbo.YOUR_FACT_TABLE"`. Replace with your table.

2. **Repository queries** — `Infrastructure/Repositories/*Repository.cs`
   Each data method has its SQL removed. A `// SQL REMOVED (template)` comment
   block describes exactly what that query should compute (purpose, grouping,
   returned shape). The body throws `NotImplementedException` until you
   implement it. Implement against the matching DTO in
   `Application/Features/<Feature>/Dtos`.

3. **Dynamic filter mapping** — `Infrastructure/Octopus/FilterSqlBuilder.cs`
   Filter dimensions map to `YOUR_*_COLUMN` placeholders. Replace with real
   columns and keep using Dapper parameters (do not concatenate user input).

4. **Product catalog mapping** — `Application/Common/Filters/FilterCatalog.cs`
   Maps product codes to source values for your domain.

## Configuration (no secrets are committed)

Copy `API/appsettings.example.json` to `API/appsettings.json` and fill in:
connection strings, `JwtSettings`, `SeedData` admin user, `EmailSettings`,
and any AI/`OllamaSettings`. Client API URL lives in `Client/.env.*`.

## Notes

- The example feature set (Region / Brand / City / Agency / Vehicle / Company /
  Demographic / ExecSummary / Dashboard) is kept as a reference implementation
  of the pattern. Rename or remove features you don't need.
- UI/report labels are still in Turkish in places; translate as needed.
