# DataAnalysis — Project Overview

## Summary

DataAnalysis is an **analytics platform template** for exploring a dataset
(a records/transactions table) across multiple dimensions, tracking weekly
trends and changes, running segment analyses, and exporting the results as
reports.

It is built with an enterprise-grade architecture: authentication (including
MFA), role/permission management, audit logging, caching, scheduled snapshot
generation, PDF/Excel export, and AI-assisted summary/segment interpretation.

> **Note:** In this template the actual data queries (SQL) have been removed and
> replaced with comment lines describing what each query computes. The
> architecture, data flow, and the full feature skeleton are intact — the only
> thing left to do is write the repository queries against your own data source.

---

## Tech Stack

**Backend**
- .NET 10 — Clean Architecture (API / Application / Domain / Infrastructure / Persistence)
- CQRS + **MediatR** (command/query separation), **FluentValidation** (input validation)
- **Dapper** — high-performance queries against the analytics source database (MSSQL)
- **EF Core + PostgreSQL (Npgsql)** — the application's own data (users, permissions, logs, saved segments)
- **StackExchange.Redis** — cache + snapshot store
- **JWT (JwtBearer)** + **BCrypt** (password hashing) + **Otp.NET / QRCoder** (MFA/TOTP)
- **QuestPDF** (PDF), **ClosedXML** (Excel) — report export
- **Scalar / OpenAPI** — API documentation

**Frontend**
- **React 19** + **Vite 7** + **Tailwind CSS 4**
- **Zustand** (global state), **React Router 7**, **Axios**
- **React Hook Form + Zod** (form/schema validation)
- **Recharts** (charts), **React Toastify** (notifications)

**Infrastructure / Delivery**
- **Azure DevOps** CI/CD pipeline (prod + test)

---

## Architecture

Dependencies flow inward; business rules are independent of infrastructure:

- **Domain** — Entities (user, permission, log, saved segment), interfaces.
- **Application** — Feature-based CQRS commands/queries/handlers, DTOs,
  validators, common behaviors, filter models.
- **Infrastructure** — Repository implementations (Dapper), the Redis cache and
  snapshot mechanism, services (export, email, MFA, token, AI).
- **Persistence** — EF Core context, migrations, repository, seed data.
- **API** — Controllers, middleware, filters; routing into MediatR.

**Two-database approach:**
- *Analytics source* (read-only, MSSQL, Dapper) — queries over a large fact table.
- *Application database* (PostgreSQL, EF Core) — users, permissions, logs, saved segments.

**Cache + Snapshot pattern:** Each analytics domain has a
`Repository` → `CachedRepository` → `SnapshotSource` chain. Heavy queries are run
on a schedule (weekly) and their results are stored as snapshots in Redis;
requests are served first from the snapshot/cache, so dashboards load fast.

---

## Modules & Capabilities

### Analytics Dimensions (Detail Pages)
Every dimension offers the same analysis set: **KPI summary**, **weekly trend**,
**heatmap**, and **distribution** analyses.

- **Region** — Region-level performance, top/bottom region, week-over-week (WoW) change.
- **City** — City-level analysis, city profile, top brands per city.
- **Brand** — Brand-level analysis, model distribution within a brand.
- **Agency** — Paginated agency list, agency profile, region distribution, top brands.
- **Vehicle** — Age / price / body type / segment distributions and trends.
- **Company** — Competitor/company comparison, movement-type breakdown, transition analysis.
- **Demographic** — Insured type, gender, age group, and city distributions.

### Dashboard
Top-level KPI cards, segment drift monitoring, brand/region/age distributions,
weekly totals, and a heatmap — all responsive to the global filter.

### Custom Segment
Lets a user define a segment from their own filter combination, save it, track
how the segment's weekly share changes over time (drift), compare two segments,
and get an **AI interpretation**.

### Exec Summary
Date-range segment drift, brand×age and age×tier matrices, risk segments,
various distributions, and an **AI-assisted executive summary**.

### Filtering
A shared, global filter across all pages: product group (a single global
selection), insured type, business source, item type, and product code. Filters
apply to both the queries and the exports.

### Export
Every analytics page can be exported as **PDF** (QuestPDF) and **Excel**
(ClosedXML); a summary of the active filter is added to the output header.

### Artificial Intelligence (Ollama)
Segment and executive-summary interpretation via local/remote LLMs (multiple
models selectable from configuration).

### Identity & Administration
- **Auth:** Login, logout, token refresh, change/reset/forgot password.
- **MFA:** TOTP-based two-factor authentication (QR setup + verification).
- **Users:** Create/update users, toggle status, unlock, reset MFA/password.
- **Permissions:** Define and assign roles/permissions.
- **Audit Logs / Auth Logs:** Operation and session audit trails.

---

## Typical Data Flow

1. The user opens an analytics page and selects filters (Zustand store).
2. The frontend calls the relevant API endpoint (Axios).
3. The controller forwards the request through **MediatR** to the matching **query handler**.
4. The handler asks for data via the **CachedRepository**; if available it comes
   from the Redis snapshot/cache, otherwise the **Repository** queries the source database.
5. The result is returned as a DTO and visualized with **Recharts** on the frontend.
6. On demand, the same data can be exported as **PDF/Excel**.

---

## Running (Summary)

1. Copy `API/appsettings.example.json` to `API/appsettings.json` and fill it in
   (connection strings, JWT, seed admin, email, AI settings).
2. Set the API address in `Client/.env.*`.
3. Start the backend with `dotnet run` (API project) and the frontend with
   `npm install` + `npm run dev` (Client).

---

## Filling In the Template

See `TEMPLATE.md`. In short:
- `OctopusConnection.PolicyTable` → your own table.
- `Infrastructure/Repositories/*Repository.cs` → write the SQL per the comment in each method.
- `FilterSqlBuilder.cs` → map the filter dimensions to your real columns.
