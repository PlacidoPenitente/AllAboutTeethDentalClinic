# Tech Stack & Architecture Plan

## All About Teeth Dental Clinic Management System (DCMS)

---

| Field            | Value              |
| ---------------- | ------------------ |
| Document Version | 1.1                |
| Date             | May 13, 2026       |
| Status           | Draft — For Review |
| Reference BRD    | BRD v4.3           |

---

## Table of Contents

1. [Architecture Overview](#1-architecture-overview)
2. [Database Decision](#2-database-decision)
3. [Frontend — Vue 3 + Vite](#3-frontend--vue-3--vite)
4. [Backend — ASP.NET Core 8](#4-backend--aspnet-core-8)
5. [Infrastructure & DevOps](#5-infrastructure--devops)
6. [Solution Structure](#6-solution-structure)
7. [MongoDB Document Model](#7-mongodb-document-model)
8. [Key Technical Decisions & Patterns](#8-key-technical-decisions--patterns)
9. [Security Implementation Plan](#9-security-implementation-plan)
10. [Full Dependency Reference](#10-full-dependency-reference)

---

## 1. Architecture Overview

The system is a **modular monolith** deployed as two separate process: a **REST API** and a **Single-Page Application**. Microservices are not warranted for a single-clinic deployment — they would add operational overhead without benefit at this scale.

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│  CLIENT LAYER                                                    │
│  Vue 3 + Vite SPA (TypeScript)                                  │
│  FullCalendar · PrimeVue · Custom SVG Dental Chart · vue-echarts│
└────────────────────────────┬────────────────────────────────────┘
                             │ HTTPS
                             │ REST (JSON)
                             │ WebSocket (SignalR — dashboard)
┌────────────────────────────▼────────────────────────────────────┐
│  API GATEWAY / REVERSE PROXY                                     │
│  Nginx — SSL termination · static file serving · rate limiting  │
└────────────────────────────┬────────────────────────────────────┘
                             │ HTTP (internal)
┌────────────────────────────▼────────────────────────────────────┐
│  APPLICATION LAYER                                               │
│  ASP.NET Core 8 Web API (Clean Architecture)                    │
│  JWT Auth · MediatR CQRS · FluentValidation · SignalR           │
│  Hangfire (background jobs) · PdfSharpCore + MigraDoc           │
└──────────┬─────────────────────────────┬───────────────────────┘
           │                             │
┌──────────▼──────────┐       ┌──────────▼──────────────────────┐
│  PRIMARY DATABASE    │       │  SECONDARY STORES                │
│  PostgreSQL 16       │       │  Valkey 8 (cache, rate limit,   │
│  (via EF Core 8)     │       │  token blacklist)               │
└─────────────────────┘       └─────────────────────────────────┘
           │
┌──────────▼──────────┐
│  FILE STORAGE        │
│  Local /uploads      │
│  (or MinIO / S3)     │
└─────────────────────┘
```

### Why This Architecture

| Decision                     | Rationale                                                                                                                 |
| ---------------------------- | ------------------------------------------------------------------------------------------------------------------------- |
| Modular monolith             | Single-clinic deployment; simple to run, deploy, and debug                                                                |
| REST over GraphQL            | BRD requirements map cleanly to REST resources; GraphQL adds frontend complexity without enough benefit for this use case |
| Clean Architecture           | Separates domain logic from infrastructure; enables unit testing without a database                                       |
| CQRS with MediatR            | Commands (state changes) and Queries (reads) have very different validation, authorization, and performance needs         |
| Separate frontend deployment | Vue 3 SPA served by Nginx; API and frontend can be versioned and scaled independently even on a single server             |

---

## 2. Database Decision

### The Honest Assessment: MongoDB vs PostgreSQL for This Domain

Before committing to a database, it is important to evaluate the domain characteristics against each option. This system should make this decision with full awareness of the trade-offs.

#### Domain Characteristics

| Characteristic                                                      | Implication                                                                        |
| ------------------------------------------------------------------- | ---------------------------------------------------------------------------------- |
| Financial calculations (balance = PatientShare − SUM(payments))     | Requires aggregate queries over a payment ledger; atomic multi-record transactions |
| Billing status auto-transitions based on payment sums               | Transactional writes spanning multiple collections/tables                          |
| Appointment conflict detection (dentist + overlapping time range)   | Range query over indexed datetime fields                                           |
| 13 standard reports (see FR-09) requiring cross-entity aggregation  | Multi-table joins or multi-collection lookups                                      |
| Patient medical history (allergies, medications, diseases as lists) | Nested, document-like data — naturally fits a document store                       |
| Tooth condition history (52 teeth, append-only per-tooth timeline)  | Embedded array with append-only semantics — excellent MongoDB fit                  |
| Activity audit log (immutable, append-only, variable schema)        | Perfect document store use case                                                    |
| Referential integrity (appointment must reference valid patient)    | No native enforcement in MongoDB; must be handled in application layer             |

#### Side-by-Side Comparison

| Concern                                      | PostgreSQL                                                   | MongoDB                                                                                                                  |
| -------------------------------------------- | ------------------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------ |
| Financial integrity (ACID)                   | Native, no setup required                                    | Supported since v4.0, but **requires a replica set** even for local dev; multi-document transactions have higher latency |
| Complex reports (JOINs)                      | Simple SQL with indexed JOINs                                | Aggregation pipeline with `$lookup` — correct and powerful, but more verbose                                             |
| Appointment range queries                    | Efficient with a `(dentist_id, scheduled_at)` compound index | Efficient with the same compound index                                                                                   |
| Nested data (allergies, conditions)          | JSONB columns handle this well                               | Native embedded documents — cleaner                                                                                      |
| Tooth condition history (append-only arrays) | Child table with append-only pattern                         | Embedded array — ideal                                                                                                   |
| Schema enforcement                           | Strong (migration-managed)                                   | None by default; application must validate                                                                               |
| Referential integrity enforcement            | Native FK constraints                                        | Application-layer only                                                                                                   |
| .NET ORM maturity                            | EF Core 8 — full LINQ, migrations, rich tooling              | MongoDB.Driver + MongoFramework or unofficial providers; no official EF Core provider                                    |
| Philippine hosting availability              | Available everywhere                                         | Available everywhere                                                                                                     |
| Stability for financial data                 | Proven at banking scale                                      | Proven, but requires careful transaction scope design                                                                    |

#### Recommendation

> **Use PostgreSQL as the primary data store.** The domain defined in BRD v4.3 is fundamentally relational: financial ledgers, multi-entity transactions, complex reporting joins, and referential integrity requirements are all native strengths of a relational database.
>
> **Use MongoDB as a secondary store** for the two collections where document semantics genuinely win: the **Activity Audit Log** (immutable, variable-schema, append-only) and optionally **Session/Token Blacklist** (or use Valkey for that instead).

#### If MongoDB Is Required for All Data

If there is a strong preference to use MongoDB for all storage, the following conditions are non-negotiable for the system to be stable:

1. **MongoDB 7.0+ in Replica Set mode** — even on a single server (`rs0` with one node and one arbiter). Multi-document transactions are not available on a standalone instance.
2. **Every financial operation (payment recording, billing generation, billing status update) must be wrapped in a multi-document session transaction** using `IClientSessionHandle`.
3. **Referential integrity must be enforced at the API layer** via FluentValidation and existence checks before every write.
4. **All financial report pipelines** must be tested against production-scale data volumes (10,000+ patients, 50,000+ appointments). MongoDB aggregation pipelines produce correct results but can be slow without proper index hints.

The rest of this document uses **PostgreSQL** as the primary database and **Valkey** as the cache/session store, which is the recommended configuration. A [MongoDB document model](#7-mongodb-document-model) is provided at the end for reference if the MongoDB route is taken.

---

## 3. Frontend — Vue 3 + Vite

### Core Framework

| Package      | Version | Purpose                                                                     |
| ------------ | ------- | --------------------------------------------------------------------------- |
| `vue`        | 3.x     | Core framework — Composition API, `<script setup>`, fine-grained reactivity |
| `vite`       | 5.x     | Build tool and dev server — near-instant HMR, optimized production bundles  |
| `typescript` | 5.4+    | Language                                                                    |
| `vue-router` | 4.x     | Client-side routing with lazy-loaded route components                       |
| `axios`      | 1.x     | HTTP client — interceptors for JWT attachment and 401 handling              |

### UI Component Library

**PrimeVue 4+** — same vendor as PrimeNG, feature-identical components rewritten for Vue 3.

PrimeVue is chosen because:

- Its DataTable component handles the complex paginated/filterable lists required throughout the BRD.
- It includes a DatePicker/Calendar component suitable for the appointment view.
- Its Dialog, AutoComplete, MultiSelect, and Timeline components cover nearly all BRD UI requirements out of the box.
- The API surface is nearly identical to PrimeNG, making knowledge transfer between the two straightforward.
- It is actively maintained and is the de-facto standard PUI library in the Vue ecosystem.

| Package            | Purpose                                                                             |
| ------------------ | ----------------------------------------------------------------------------------- |
| `primevue`         | UI component library (DataTable, DatePicker, Dialog, Form controls, Timeline, Tabs) |
| `primeicons`       | Icon set                                                                            |
| `@primevue/themes` | Design token-based theming system                                                   |
| `tailwindcss`      | Custom styling, responsive layouts, color system                                    |

#### PrimeVue UI Specification Standard

All UI screens in both the **Clinic SPA** and the **Patient Portal SPA** must strictly follow PrimeVue 4 component standards and the **Aura preset** (default theme). Deviating from these specifications — e.g., hand-rolling a custom input or button — is not permitted unless no PrimeVue equivalent exists.

##### Theme

| Setting                 | Value                                                                               |
| ----------------------- | ----------------------------------------------------------------------------------- |
| Preset                  | **Aura** (`@primevue/themes/aura`) — the PrimeVue 4 default                         |
| Primary colour override | Clinic teal: `#0E9DBE` (`primary-500`), hover `#0B88A4` (`primary-600`)             |
| Surface scale           | Aura default neutral greys (`surface-0` through `surface-900`)                      |
| Configuration           | `app.use(PrimeVue, { theme: { preset: Aura, options: { primaryColor: 'teal' } } })` |

The primary colour is the only override. All other Aura design tokens (border-radius, spacing, shadows, font sizes) are used as-is.

##### Component Sizing & Spacing Tokens

These Aura CSS custom properties govern component geometry. All components must respect them:

| CSS Token                      | Value            | Applies To                                                         |
| ------------------------------ | ---------------- | ------------------------------------------------------------------ |
| `--p-form-field-height`        | `2.5rem` (40px)  | InputText, Password, Dropdown, InputNumber, Textarea (single-line) |
| `--p-form-field-padding-x`     | `0.75rem` (12px) | Horizontal inner padding of all form fields                        |
| `--p-form-field-padding-y`     | `0.5rem` (8px)   | Vertical inner padding of all form fields                          |
| `--p-form-field-border-radius` | `6px`            | All form field borders                                             |
| `--p-form-field-border-color`  | `surface-200`    | Default border colour                                              |
| `--p-form-field-background`    | `surface-0`      | Field background                                                   |
| `--p-button-border-radius`     | `6px`            | All button variants                                                |
| `--p-button-padding-y`         | `0.5rem` (8px)   | Button vertical padding                                            |
| `--p-button-padding-x`         | `1rem` (16px)    | Button horizontal padding                                          |
| `--p-card-border-radius`       | `12px`           | Card container                                                     |
| `--p-card-background`          | `surface-0`      | Card background                                                    |

##### Core Component Standards

| Component                   | Usage Rule                                                                                                                                                                |
| --------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **InputText**               | All single-line text inputs. Height 40px, 6px radius, `surface-200` border, `surface-0` background. Always paired with a `<label>` (14px Inter Medium, `surface-700`).    |
| **Password**                | All password fields. Use the built-in `toggleMask` prop for show/hide toggle — never implement a custom eye icon separately.                                              |
| **Dropdown**                | All single-select lists (role, status, dentist, etc.). Never use a native `<select>`.                                                                                     |
| **MultiSelect**             | All multi-select lists (e.g., applicable tooth conditions).                                                                                                               |
| **DatePicker**              | All date and datetime fields. `showTime` prop for datetime; `dateFormat="mm/dd/yy"` for Philippines locale.                                                               |
| **InputNumber**             | All numeric fields (amounts, quantities). Use `mode="currency" currency="PHP" locale="en-PH"` for monetary fields.                                                        |
| **Textarea**                | Multi-line text. Always set `rows` explicitly; do not auto-resize unless UX requires it.                                                                                  |
| **Button**                  | All clickable actions. Variants: `severity` prop (`undefined` = primary filled, `"secondary"`, `"danger"`, `"text"`, `"outlined"`). Never use a plain `<button>` element. |
| **DataTable**               | All tabular data lists. Use `paginator`, `rows`, `sortField`, `filterDisplay="menu"` for standard tables.                                                                 |
| **Dialog**                  | All modal interactions (confirmations, forms, detail views). Use `modal`, `closable`, `draggable=false` by default.                                                       |
| **Card**                    | Top-level content containers. `p-card-header` for title + subtitle; `p-card-body` for content. Radius 12px.                                                               |
| **Tabs / TabPanel**         | Multi-section views (patient profile tabs, settings tabs, billing tabs).                                                                                                  |
| **Timeline**                | Appointment history, condition history, audit log — append-only chronological data.                                                                                       |
| **Toast**                   | All transient success, error, warning, and info notifications. Positioned `top-right`. Never use browser `alert()`.                                                       |
| **ConfirmDialog**           | All destructive action confirmations (delete, void, cancel). Driven by `useConfirm()` composable. Never use browser `confirm()`.                                          |
| **Message / InlineMessage** | Inline validation errors and form-level feedback. Paired with FluentValidation error responses from the API.                                                              |
| **ProgressSpinner**         | Loading states during API calls. Centred within the loading container.                                                                                                    |
| **FileUpload**              | Patient file attachment uploads (FR-03.7). `mode="basic"`, `maxFileSize="20971520"` (20 MB), `accept` restricted to `image/*,application/pdf`.                            |
| **Divider**                 | Section separators within cards and forms. `layout="horizontal"` default.                                                                                                 |

##### Typography

All text uses **Inter** (web-safe fallback: `system-ui, sans-serif`). PrimeVue Aura font tokens:

| Role              | Size | Weight           | Token                      |
| ----------------- | ---- | ---------------- | -------------------------- |
| Page / card title | 22px | Semi Bold        | `--p-card-title-font-size` |
| Section heading   | 18px | Semi Bold        | custom                     |
| Body / label      | 14px | Regular / Medium | `--p-form-field-font-size` |
| Caption / helper  | 12px | Regular          | custom                     |
| Button label      | 14px | Semi Bold        | `--p-button-font-size`     |

##### Page Background

| Context                                     | Colour                   |
| ------------------------------------------- | ------------------------ |
| Page background (Clinic SPA and Portal SPA) | `surface-50` (`#F8FAFB`) |
| Card / panel background                     | `surface-0` (`#FFFFFF`)  |
| Input background                            | `surface-0` (`#FFFFFF`)  |
| Input disabled background                   | `surface-100`            |

##### Figma Wireframe Compliance

All Figma wireframes produced for this project must reflect the PrimeVue Aura component specifications above. Component annotations in each wireframe frame must identify the PrimeVue component name, variant/props used, and any BRD requirement it satisfies. Wireframes serve as the developer handoff source of truth for component selection — not for pixel-perfect visual styling, which is governed by PrimeVue's Aura theme tokens at runtime.

### State Management

**Pinia** — the official Vue 3 state management library. Lightweight, fully typed, devtools-integrated, and far less boilerplate than NgRx or Vuex.

| Package / Pattern                          | Usage                                                                               |
| ------------------------------------------ | ----------------------------------------------------------------------------------- |
| `pinia`                                    | Global stores for cross-feature state (auth, active patient session, notifications) |
| Vue 3 `ref` / `computed` (Composition API) | Local component state — no store needed for UI-only state                           |
| `axios` interceptors                       | Attach JWT header on every request; redirect to login on 401                        |
| Route-level `beforeEnter` guards           | Pre-load patient/appointment data before component renders                          |

> **Active Patient Context — `patient.store.ts` (Patient Portal, Phase 6+):** The store must maintain an **`activePatientId`** field tracking which patient record is currently in context. When a Primary Account Holder switches from their own record to a Dependent's record (FR-12.1.7), `activePatientId` must be explicitly updated before any API call is dispatched. All booking, dental chart, history, and billing requests must use `activePatientId` — not the authenticated user's own patient ID. This prevents accidental cross-record writes (e.g., booking an appointment under the wrong family member's name). `activePatientId` is reset to the Primary Account Holder's own `patientId` on fresh login and on logout.

### Appointment Calendar

| Package                     | Purpose                                                           |
| --------------------------- | ----------------------------------------------------------------- |
| `@fullcalendar/vue3`        | Appointment calendar — day, week, month views with dentist filter |
| `@fullcalendar/daygrid`     | Month view plugin                                                 |
| `@fullcalendar/timegrid`    | Day / week time-slot view                                         |
| `@fullcalendar/interaction` | Drag-and-drop, click event handling                               |

### Charts & Analytics

| Package                   | Purpose                                                                          |
| ------------------------- | -------------------------------------------------------------------------------- |
| `echarts` + `vue-echarts` | Dashboard charts (appointment status breakdown, revenue trend, inventory levels) |

### Dental Chart

The dental chart (FR-05) is implemented as a **custom Vue 3 SVG component**. No third-party dental chart library exists for Vue that covers FDI notation correctly. The component is built as a set of Single-File Components (SFCs):

```
DentalChart/
├── DentalChart.vue          ← parent: holds 52 ToothViewModel[], handles selection
│                               SVG layout: Q1 top-right, Q2 top-left, Q3 bottom-left, Q4 bottom-right
├── Tooth.vue                ← individual tooth: 5 surfaces as SVG paths, condition → symbol/fill
│                               SVG path group for one tooth
├── ToothConditionLegend.vue ← always-visible legend (19 conditions + symbols)
└── ToothDetailPanel.vue     ← condition history timeline, remarks, linked records
```

Each tooth is an SVG `<g>` element with five `<path>` children (B, L, M, D, O surfaces). Condition codes map to fill color and pattern (e.g., DCF = outlined circle, FLD = horizontal line fill). Vue 3's fine-grained reactivity means only the affected tooth re-renders when a condition changes — no full chart re-render. This gives a clinically recognizable chart familiar to Filipino dentists.

### PDF / Print

| Approach                                | Implementation                                                                                                                             |
| --------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
| Structured documents (OR, SOA, reports) | Generated server-side by PdfSharpCore + MigraDoc; Vue downloads and opens the PDF blob via an `axios` response with `responseType: 'blob'` |
| Simple print views                      | `@media print` CSS on Vue components for simple list prints                                                                                |

### Authentication

| Package / Pattern               | Purpose                                                                                |
| ------------------------------- | -------------------------------------------------------------------------------------- |
| `jwt-decode`                    | Decode JWT payload client-side (read role, expiry) — no verification (server verifies) |
| `axios` request interceptor     | Attaches `Authorization: Bearer <token>` header on every outgoing request              |
| `axios` response interceptor    | Handles 401 responses — clears token from memory and redirects to login                |
| `vue-router` `beforeEach` guard | Global navigation guard — redirects unauthenticated users to login                     |
| Custom `requireRole` route meta | Per-route role restriction checked in the global navigation guard                      |

### Real-Time Dashboard

| Package              | Purpose                                                                                                        |
| -------------------- | -------------------------------------------------------------------------------------------------------------- |
| `@microsoft/signalr` | SignalR client — receives push updates for dashboard metrics, appointment status changes, and low-stock alerts |

### File Upload (Patient Photo, Logo)

| Approach                | Notes                                                                                                                                                        |
| ----------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| PrimeVue `<FileUpload>` | Webcam capture via `navigator.mediaDevices.getUserMedia()` in a custom Vue 3 composable (`useWebcam`); file upload via standard multipart form using `axios` |

---

## 4. Backend — ASP.NET Core 8

### Framework

| Package                          | Purpose                                          |
| -------------------------------- | ------------------------------------------------ |
| `Microsoft.AspNetCore.App` (8.x) | Core web framework, routing, middleware pipeline |
| `Swashbuckle.AspNetCore`         | Swagger / OpenAPI documentation                  |

### Authentication & Security

| Package                                         | Purpose                                                            |
| ----------------------------------------------- | ------------------------------------------------------------------ |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT Bearer token validation                                        |
| `BCrypt.Net-Next`                               | Password and security answer hashing (cost factor 12+)             |
| `Microsoft.AspNetCore.RateLimiting`             | Built-in rate limiter middleware (login endpoint, API-wide limits) |

### Application Layer (CQRS)

| Package                       | Purpose                                                                        |
| ----------------------------- | ------------------------------------------------------------------------------ |
| `MediatR`                     | CQRS mediator — all business operations dispatched as Commands or Queries      |
| `FluentValidation`            | Request validation pipeline — each Command/Query has a corresponding validator |
| `FluentValidation.AspNetCore` | Integrates validators into ASP.NET model binding pipeline                      |
| `AutoMapper`                  | Maps domain entities to DTOs and response models                               |

### Data Access — PostgreSQL

| Package                                 | Purpose                                               |
| --------------------------------------- | ----------------------------------------------------- |
| `Microsoft.EntityFrameworkCore` (8.x)   | ORM — LINQ-based queries, migrations, change tracking |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | PostgreSQL EF Core provider (Npgsql)                  |
| `Microsoft.EntityFrameworkCore.Design`  | Tooling for `dotnet ef migrations`                    |

EF Core configuration priorities for this domain:

- `HasColumnType("numeric(10,2)")` on all decimal money columns — never `double` or `float`
- Owned entities for value objects (e.g., `BloodPressure` as owned type with `Systolic` + `Diastolic` columns)
- Table-per-hierarchy or separate tables for polymorphic audit log entries
- Global query filters for soft-delete (`IsArchived = false`)
- Interceptors for `UpdatedAt` auto-stamping

### Caching & Session

| Package                                           | Purpose                                                                                           |
| ------------------------------------------------- | ------------------------------------------------------------------------------------------------- |
| `StackExchange.Redis`                             | Valkey/Redis-compatible .NET client (MIT) — `StackExchange.Redis` is fully compatible with Valkey |
| `Microsoft.Extensions.Caching.StackExchangeRedis` | IDistributedCache implementation backed by Valkey                                                 |

> **Phase note:** Valkey is a **Phase 1** infrastructure dependency — it is included in the Docker Compose stack from the first deployment for JWT token revocation (FR-01.2.5: logout must invalidate the session token server-side). Its role expands in **Phase 7** to add a gateway payment slot-lock for the **payment gateway path only** (FR-12.4.6: 15-minute TTL reservation while the patient completes an online gateway payment). The **manual deposit path** (FR-02.12a — GCash/bank transfer receipt upload) does **not** set a Valkey TTL; the appointment slot is held in `AwaitingDepositVerification` status indefinitely until a staff member clicks Verify or Reject — `SlotReleaseJob` is not triggered on the manual path. No new infrastructure is required for Phase 7 — the existing Valkey instance is reused under a new key namespace.

Valkey is used for:

- **JWT token blacklist** _(Phase 1)_ — stores invalidated tokens until their natural expiry so logout is truly enforced server-side (FR-01.2.5)
- **Login attempt counter** — tracks failed attempts per username for lockout logic (avoids a DB write per failed attempt)
- **Report caching** — expensive aggregation queries (RPT-04 Revenue, RPT-06 AR) are cached with a 5-minute TTL and invalidated on payment writes
- **Rate limiting state** — per-IP/per-user rate limit counters
- **Gateway payment slot-lock** _(Phase 7, gateway path only)_ — holds a 15-minute TTL reservation on an appointment slot while the patient completes an online gateway payment (FR-12.4.6); key expires automatically after 15 minutes. **Manual deposit path (FR-02.12a) does not use a Valkey TTL** — the slot is held in `AwaitingDepositVerification` status with no expiry until staff action; `SlotReleaseJob` is not involved on this path.

### Background Jobs

| Package               | Purpose                                       |
| --------------------- | --------------------------------------------- |
| `Hangfire.AspNetCore` | Background job scheduling and processing      |
| `Hangfire.PostgreSql` | PostgreSQL persistence for Hangfire job store |

Hangfire jobs:
| Job | Trigger | Description |
|---|---|---|
| `CreateTeethForPatientJob` | Patient registered | Creates 52 Tooth records in background; registration returns immediately |
| `UpdateBillingStatusJob` | Payment recorded | Recomputes billing status (PartiallyPaid / FullyPaid) after every payment write |
| `LowStockCheckJob` | Scheduled: every 6 hours | Queries items at or below CriticalQuantity; pushes alerts via SignalR |
| `NearExpiryCheckJob` | Scheduled: daily 06:00 | Queries items within expiry warning window; pushes alerts via SignalR |
| `AppointmentReminderJob` _(v2)_ | Scheduled: daily | Sends SMS/email reminders for next-day confirmed appointments |

### Real-Time (SignalR)

| Package                        | Purpose                               |
| ------------------------------ | ------------------------------------- |
| `Microsoft.AspNetCore.SignalR` | WebSocket hub for real-time dashboard |

`DashboardHub` pushes:

- New appointment created / status changed
- Low-stock alert triggered
- Near-expiry alert triggered
- Daily metrics updates (appointment count, revenue)

### PDF Generation

| Package                | Purpose                                                                                               |
| ---------------------- | ----------------------------------------------------------------------------------------------------- |
| `PdfSharpCore`         | MIT-licensed .NET Core port of PdfSharp — low-level PDF creation engine                               |
| `MigraDoc.NetStandard` | MIT-licensed document model layer on top of PdfSharpCore — tables, sections, headers, footers, images |

PdfSharpCore + MigraDoc is chosen because:

- Both packages are MIT licensed — fully open source, no usage restrictions
- Fully managed .NET; no native binary dependency or external process
- MigraDoc provides a document-model API (paragraphs, tables, page numbering, images) suitable for OR, SOA, and all FR-09 reports
- Widely used in .NET projects; well-documented and actively maintained community fork

### Email _(v2 — prepared but not wired in v1)_

| Package   | Purpose                                                            |
| --------- | ------------------------------------------------------------------ |
| `MailKit` | SMTP client for appointment reminders and password recovery emails |

### Logging

| Package                           | Purpose                                                          |
| --------------------------------- | ---------------------------------------------------------------- |
| `Serilog.AspNetCore`              | Structured logging host integration                              |
| `Serilog.Sinks.PostgreSQL`        | Writes structured JSON logs to a PostgreSQL `app_logs` table     |
| `Serilog.Sinks.Console`           | Console output for development                                   |
| `Serilog.Enrichers.CorrelationId` | Attaches a correlation ID to every request log for trace linking |

### Testing

| Package                            | Purpose                                                       |
| ---------------------------------- | ------------------------------------------------------------- |
| `xUnit`                            | Test framework                                                |
| `Moq`                              | Mocking for unit tests                                        |
| `FluentAssertions`                 | Readable assertion syntax                                     |
| `Testcontainers.PostgreSql`        | Spins up a real PostgreSQL container for integration tests    |
| `Microsoft.AspNetCore.Mvc.Testing` | In-process ASP.NET Core test server for API integration tests |
| `Bogus`                            | Fake data generation for test fixtures                        |

---

## 5. Infrastructure & DevOps

### Containerization

```yaml
# docker-compose.yml (development)
services:
  api:
    build: ./backend
    ports: ["5000:8080"]
    environment:
      - ConnectionStrings__DefaultConnection=...
      - ConnectionStrings__Valkey=...
      - Jwt__PrivateKey=...
    depends_on: [postgres, valkey]

  frontend:
    build: ./frontend
    ports: ["5173:80"] # 5173 = Vite default dev port; 80 in production container
    depends_on: [api]

  postgres:
    image: postgres:16-alpine
    environment:
      POSTGRES_DB: allaboutteeth
      POSTGRES_USER: dcms_user
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
    volumes:
      - pgdata:/var/lib/postgresql/data
    ports: ["5432:5432"]

  valkey:
    image: valkey/valkey:8-alpine
    command: valkey-server --requirepass ${VALKEY_PASSWORD}
    volumes:
      - valkeydata:/data

  nginx:
    image: nginx:alpine
    ports: ["80:80", "443:443"]
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf
      - ./certs:/etc/nginx/certs
    depends_on: [api, frontend]
```

### Production Server Minimum Spec

| Resource   | Minimum                        | Recommended                                   |
| ---------- | ------------------------------ | --------------------------------------------- |
| CPU        | 2 vCPU                         | 4 vCPU                                        |
| RAM        | 4 GB                           | 8 GB                                          |
| Storage    | 40 GB SSD                      | 100 GB SSD                                    |
| OS         | Ubuntu 22.04 LTS               | Ubuntu 24.04 LTS                              |
| PostgreSQL | On same server (containerized) | Managed DB (e.g., Supabase, Neon, or AWS RDS) |

### CI/CD — GitHub Actions

```
.github/workflows/
├── ci.yml          ← On PR: build + test (unit + integration with Testcontainers)
├── staging.yml     ← On push to develop: build Docker images, push to registry, deploy to staging
└── production.yml  ← On push to main (after PR review): deploy to production VPS via SSH
```

Pipeline steps:

1. `dotnet build` + `dotnet test` (backend)
2. `vite build` (frontend — outputs to `dist/`)
3. Docker image build and tag
4. Push to GitHub Container Registry (`ghcr.io`)
5. SSH to server → `docker compose pull && docker compose up -d`
6. Run `dotnet ef database update` migration

### Nginx Configuration (Key Points)

```nginx
# Key directives
server {
    listen 443 ssl http2;

    # Patient file attachments (FR-03.7) allow up to 20 MB per file; 25 M gives a safe overhead
    # above the BRD limit and prevents Nginx returning 413 before the API can validate the request.
    # Without this directive Nginx defaults to 1 MB and rejects every X-ray upload silently.
    client_max_body_size 25M;

    # SSL — Let's Encrypt via Certbot
    ssl_certificate     /etc/letsencrypt/live/.../fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/.../privkey.pem;
    ssl_protocols       TLSv1.2 TLSv1.3;

    # Vue 3 SPA — serve index.html for all non-API routes (history mode routing)
    location / {
        root /usr/share/nginx/html;
        try_files $uri $uri/ /index.html;
    }

    # API proxy
    location /api/ {
        proxy_pass http://api:8080;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    }

    # SignalR WebSocket
    location /hubs/ {
        proxy_pass http://api:8080;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
    }

    # Security headers
    add_header X-Frame-Options DENY;
    add_header X-Content-Type-Options nosniff;
    add_header Referrer-Policy strict-origin-when-cross-origin;
    add_header Content-Security-Policy "default-src 'self'; ...";
}
```

---

## 6. Solution Structure

### Repository Layout

```
AllAboutTeethDentalClinic/          ← repo root
├── backend/
│   ├── AllAboutTeeth.sln
│   ├── src/
│   │   ├── AllAboutTeeth.Domain/
│   │   ├── AllAboutTeeth.Application/
│   │   ├── AllAboutTeeth.Infrastructure/
│   │   └── AllAboutTeeth.Api/
│   └── tests/
│       ├── AllAboutTeeth.UnitTests/
│       └── AllAboutTeeth.IntegrationTests/
├── frontend/
│   └── allaboutteeth-web/          ← Vue 3 + Vite project
│       ├── src/
│       │   ├── main.ts             ← app entry point (createApp, router, pinia, primevue)
│       │   ├── App.vue
│       │   ├── core/
│       │   │   ├── api/            ← axios instance + interceptors
│       │   │   ├── auth/           ← useAuth composable, router guards
│       │   │   ├── layout/         ← AppShell.vue, Sidebar.vue, TopBar.vue
│       │   │   └── signalr/        ← useDashboardHub composable
│       │   ├── shared/
│       │   │   ├── components/     ← shared presentational components
│       │   │   ├── composables/    ← useWebcam, usePagination, useConfirm, etc.
│       │   │   └── utils/
│       │   ├── stores/             ← Pinia stores
│       │   │   ├── auth.store.ts
│       │   │   ├── patient.store.ts
│       │   │   └── notification.store.ts
│       │   ├── features/
│       │   │   ├── dashboard/
│       │   │   ├── patients/
│       │   │   ├── appointments/
│       │   │   ├── dental-chart/
│       │   │   ├── treatments/
│       │   │   ├── billing/
│       │   │   ├── payments/
│       │   │   ├── inventory/
│       │   │   ├── reports/
│       │   │   ├── settings/
│       │   │   └── users/
│       │   ├── router/
│       │   │   └── index.ts        ← vue-router routes with lazy imports + meta roles
│       │   └── assets/
│       ├── vite.config.ts
│       ├── tsconfig.json
│       └── package.json
├── docker-compose.yml
├── docker-compose.override.yml     ← dev overrides (volume mounts, debug ports)
├── nginx.conf
├── BRD.md
├── DOMAIN_KNOWLEDGE.md
└── TECH_STACK_AND_ARCHITECTURE.md
```

### Backend Project Structure (Clean Architecture)

```
AllAboutTeeth.Domain/
├── Entities/
│   ├── User.cs
│   ├── Patient.cs
│   │   ├── PatientAllergy.cs       ← owned entity
│   │   ├── PatientMedication.cs    ← owned entity
│   │   └── PatientDisease.cs       ← owned entity
│   ├── Tooth.cs
│   ├── ToothConditionEntry.cs
│   ├── Treatment.cs
│   ├── Appointment.cs
│   ├── AppointmentTreatment.cs
│   ├── AppointmentProcedure.cs
│   ├── TreatmentRecord.cs
│   ├── Billing.cs
│   ├── Payment.cs
│   ├── SupplyItem.cs
│   ├── SupplyStockLedger.cs
│   ├── Supplier.cs
│   ├── Provider.cs
│   ├── ProviderTreatmentCoverage.cs
│   ├── ActivityLog.cs
│   └── ClinicSettings.cs
├── Enumerations/
│   ├── UserRole.cs
│   ├── AppointmentStatus.cs
│   ├── ToothCondition.cs
│   ├── ToothSurface.cs
│   ├── TreatmentCategory.cs
│   ├── BillingStatus.cs
│   ├── PaymentMethod.cs
│   ├── ActivityAction.cs
│   ├── SupplyLedgerChangeType.cs
│   └── PatientSex.cs
├── ValueObjects/
│   ├── BloodPressure.cs            ← Systolic + Diastolic
│   ├── FdiToothNumber.cs           ← validated "11"–"85" string
│   └── Money.cs                    ← decimal wrapper with currency
├── DomainEvents/
│   ├── PatientRegisteredEvent.cs   ← triggers CreateTeethForPatientJob
│   ├── AppointmentCompletedEvent.cs← triggers Billing creation
│   └── PaymentRecordedEvent.cs     ← triggers billing status recompute
├── Exceptions/
│   ├── AppointmentConflictException.cs
│   ├── InsufficientStockException.cs
│   └── InvalidStatusTransitionException.cs
└── Interfaces/
    ├── IRepository.cs
    ├── IUnitOfWork.cs
    └── ICurrentUserService.cs
```

```
AllAboutTeeth.Application/
├── Common/
│   ├── Behaviors/
│   │   ├── ValidationBehavior.cs   ← MediatR pipeline: runs FluentValidation before handler
│   │   ├── LoggingBehavior.cs      ← MediatR pipeline: logs command/query name + duration
│   │   └── TransactionBehavior.cs  ← MediatR pipeline: wraps Commands in a DB transaction
│   ├── Exceptions/
│   │   ├── NotFoundException.cs
│   │   ├── ForbiddenException.cs
│   │   └── ValidationException.cs
│   └── Mappings/
│       └── AutoMapperProfile.cs
└── Features/
    ├── Patients/
    │   ├── Commands/
    │   │   ├── CreatePatient/
    │   │   │   ├── CreatePatientCommand.cs
    │   │   │   ├── CreatePatientCommandHandler.cs
    │   │   │   └── CreatePatientCommandValidator.cs
    │   │   └── UpdatePatient/ ...
    │   └── Queries/
    │       ├── GetPatient/
    │       └── ListPatients/
    ├── Appointments/
    │   ├── Commands/
    │   │   ├── CreateAppointment/   ← includes conflict detection logic
    │   │   ├── ConfirmAppointment/
    │   │   ├── StartAppointment/
    │   │   ├── CompleteAppointment/ ← triggers AppointmentCompletedEvent → billing
    │   │   └── CancelAppointment/
    │   └── Queries/
    │       ├── GetAppointmentsByDate/
    │       └── GetAppointmentsByDentist/
    ├── DentalChart/
    │   ├── Commands/
    │   │   └── RecordToothCondition/ ← creates new ToothConditionEntry
    │   └── Queries/
    │       ├── GetPatientChart/
    │       └── GetToothHistory/
    ├── Billing/
    │   ├── Commands/
    │   │   ├── GenerateBilling/    ← called by AppointmentCompletedEvent handler
    │   │   ├── FinalizeBilling/
    │   │   └── VoidBilling/
    │   └── Queries/
    │       └── GetBillingForAppointment/
    ├── Payments/
    │   ├── Commands/
    │   │   ├── RecordPayment/      ← validates amount ≤ balance; triggers status update
    │   │   └── VoidPayment/
    │   └── Queries/
    │       └── GetPaymentsForBilling/
    ├── Inventory/
    │   ├── Commands/
    │   │   ├── ReceiveStock/
    │   │   └── ConsumeStock/
    │   └── Queries/
    │       ├── GetLowStockItems/
    │       └── GetNearExpiryItems/
    └── Reports/
        └── Queries/
            ├── GetRevenueReport/
            ├── GetAccountsReceivable/
            ├── GetDentistProduction/
            └── ... (one query per RPT-xx)
```

```
AllAboutTeeth.Infrastructure/
├── Persistence/
│   ├── ApplicationDbContext.cs
│   ├── Migrations/
│   ├── Configurations/             ← IEntityTypeConfiguration<T> per entity
│   │   ├── PatientConfiguration.cs
│   │   ├── AppointmentConfiguration.cs
│   │   ├── BillingConfiguration.cs ← defines computed Balance as [NotMapped]
│   │   └── ...
│   └── Repositories/
│       └── Repository.cs           ← generic IRepository<T> implementation
├── Identity/
│   ├── JwtTokenService.cs
│   ├── PasswordHasher.cs           ← wraps BCrypt.Net-Next
│   └── CurrentUserService.cs       ← reads UserId from JWT claims
├── BackgroundJobs/
│   ├── HangfireJobService.cs
│   ├── CreateTeethJob.cs
│   ├── UpdateBillingStatusJob.cs
│   ├── LowStockCheckJob.cs
│   └── NearExpiryCheckJob.cs
├── RealTime/
│   └── DashboardNotificationService.cs ← calls SignalR hub
├── Services/
│   ├── PdfService.cs               ← MigraDoc/PdfSharpCore templates: OR, SOA, reports
│   ├── EmailService.cs             ← MailKit (stubbed in v1)
│   └── FileStorageService.cs       ← patient photo, clinic logo storage
├── Caching/
│   └── ValkeyCacheService.cs
└── DependencyInjection.cs          ← Infrastructure services registration
```

```
AllAboutTeeth.Api/
├── Controllers/
│   ├── AuthController.cs
│   ├── UsersController.cs
│   ├── PatientsController.cs
│   ├── AppointmentsController.cs
│   ├── DentalChartController.cs
│   ├── TreatmentsController.cs
│   ├── BillingController.cs
│   ├── PaymentsController.cs
│   ├── InventoryController.cs
│   ├── ProvidersController.cs
│   ├── SuppliersController.cs
│   ├── ReportsController.cs
│   ├── ActivityLogController.cs
│   └── ClinicSettingsController.cs
├── Hubs/
│   └── DashboardHub.cs             ← SignalR hub
├── Middleware/
│   ├── ExceptionHandlingMiddleware.cs ← maps domain exceptions to HTTP status codes
│   └── AuditLogMiddleware.cs          ← intercepts responses to write ActivityLog entries
├── Extensions/
│   └── ServiceCollectionExtensions.cs
└── Program.cs
```

---

## 7. MongoDB Document Model

> This section is provided for reference in case MongoDB is chosen over PostgreSQL. If using PostgreSQL, skip this section.

### Requirements for MongoDB on This System

- **MongoDB 7.0+**
- **Replica set mode required** (single-node `rs0` is acceptable) — transactions don't work on standalone instances
- **MongoDB.Driver 3.x** (.NET official driver)
- No official EF Core provider for MongoDB; use Repository pattern with the native driver directly

### Collection Design

#### `users`

```json
{
  "_id": ObjectId,
  "username": "string (unique index)",
  "passwordHash": "string",
  "role": "Administrator | Dentist | Staff",
  "status": "Active | Archived",
  "firstName": "string",
  "lastName": "string",
  "prcLicenseNumber": "string (dentists only)",
  "prcLicenseExpiry": ISODate,
  "failedLoginCount": 0,
  "lockedUntil": ISODate | null,
  "lastLoginAt": ISODate | null,
  "createdAt": ISODate,
  "updatedAt": ISODate
}
// Indexes: { username: 1 } unique
```

#### `patients`

```json
{
  "_id": ObjectId,
  "patientNumber": "string (sequential, unique)",
  "lastName": "string",
  "firstName": "string",
  "birthdate": ISODate,
  "sex": "Male | Female | Other",
  "status": "Active | Archived",
  "contactNo": "string",
  "consentGiven": true,
  "consentDate": ISODate,
  "philhealthMemberId": "string",
  "providerId": ObjectId | null,
  "insuranceMemberId": "string",
  // Embedded arrays — these are DOCUMENT-like; good MongoDB fit
  "allergies": [
    { "allergen": "Penicillin", "reaction": "rash", "severity": "Moderate" }
  ],
  "medications": [
    { "name": "Metformin", "dosage": "500mg", "frequency": "twice daily", "reason": "DM2" }
  ],
  "diseases": [
    { "condition": "Hypertension", "isActive": true, "diagnosedYear": 2019 }
  ],
  "bloodPressure": { "systolic": 120, "diastolic": 80 },
  "bloodType": "O+",
  "isTobaccoUser": false,
  "isAlcoholUser": false,
  "isDangerousDrugUser": false,
  "isPregnant": false,
  "createdAt": ISODate,
  "createdBy": ObjectId
}
// Indexes: { lastName: "text", firstName: "text" }, { "contactNo": 1 }, { status: 1 }
```

#### `teeth`

```json
{
  "_id": ObjectId,
  "patientId": ObjectId,
  "toothNumber": "16",
  "toothType": "Permanent | Primary",
  "remarks": "string",
  // Embedded condition history — APPEND-ONLY, excellent MongoDB fit
  "conditionHistory": [
    {
      "condition": "PNT",
      "surfaces": ["B", "O"],
      "isActive": true,
      "recordedAt": ISODate,
      "recordedBy": ObjectId,
      "appointmentId": ObjectId | null,
      "notes": "string"
    }
  ],
  "updatedAt": ISODate,
  "updatedBy": ObjectId
}
// Indexes: { patientId: 1, toothNumber: 1 } unique
// 52 documents per patient
```

#### `appointments`

```json
{
  "_id": ObjectId,
  "patientId": ObjectId,
  "dentistId": ObjectId,
  "status": "Pending | Confirmed | InProgress | Completed | Cancelled | NoShow | AwaitingApproval | AwaitingDepositVerification | Rejected",
  "scheduledAt": ISODate,
  "durationMinutes": 30,
  "chiefComplaint": "string",
  "loaNumber": "string",
  // Multiple treatments per appointment — correct model
  "plannedTreatments": [
    { "treatmentId": ObjectId, "negotiatedPrice": 500.00 }
  ],
  // Status history for audit
  "statusHistory": [
    { "status": "Pending", "changedAt": ISODate, "changedBy": ObjectId }
  ],
  "cancellationReason": "string | null",
  "createdAt": ISODate,
  "createdBy": ObjectId
}
// Indexes: { dentistId: 1, scheduledAt: 1 }, { patientId: 1 }, { status: 1, scheduledAt: 1 }
// Conflict detection query: { dentistId: dentistId, status: { $nin: ["Cancelled"] },
//   scheduledAt: { $lt: newEnd }, endAt: { $gt: newStart } }
// Note: store computed endAt = scheduledAt + durationMinutes for efficient range query
```

#### `billings`

```json
{
  "_id": ObjectId,
  "appointmentId": ObjectId,
  "status": "Draft | Final | PartiallyPaid | FullyPaid | Voided",
  "subTotal": NumberDecimal("1500.00"),
  "discount": NumberDecimal("0.00"),
  "taxAmount": NumberDecimal("0.00"),
  "totalAmount": NumberDecimal("1500.00"),
  "providerId": ObjectId | null,
  "hmoCoverageAmount": NumberDecimal("0.00"),
  "patientShare": NumberDecimal("1500.00"),
  // Balance is NEVER stored. Computed as patientShare - SUM(non-voided payments)
  "procedures": [
    {
      "toothId": ObjectId,
      "toothNumber": "16",
      "treatmentId": ObjectId,
      "treatmentName": "Composite Resin Filling",
      "amountCharged": NumberDecimal("1500.00"),
      "conditionBefore": "DCF",
      "conditionAfter": "CCF"
    }
  ],
  "createdAt": ISODate,
  "createdBy": ObjectId
}
// Use NumberDecimal (Decimal128) — NEVER Double for money
// Indexes: { appointmentId: 1 } unique, { status: 1, createdAt: -1 }
```

#### `payments`

```json
{
  "_id": ObjectId,
  "billingId": ObjectId,
  "amountPaid": NumberDecimal("500.00"),
  "paymentMethod": "Cash | GCash | ...",
  "orNumber": "OR-000001",
  "isVoided": false,
  "voidedAt": ISODate | null,
  "voidedBy": ObjectId | null,
  "voidReason": "string | null",
  "paidAt": ISODate,
  "createdBy": ObjectId
}
// Separate collection — needed to compute SUM for balance
// Indexes: { billingId: 1 }, { orNumber: 1 } unique, { isVoided: 1, billingId: 1 }
```

#### `activityLogs`

```json
{
  "_id": ObjectId,
  "userId": ObjectId,
  "userFullName": "string (denormalized for query performance)",
  "action": "Create | Update | Delete | Login | ...",
  "entityType": "Patient | Appointment | Billing | ...",
  "entityId": ObjectId | null,
  "description": "string",
  "oldValues": { /* JSON object */ } | null,
  "newValues": { /* JSON object */ } | null,
  "ipAddress": "string",
  "occurredAt": ISODate
}
// Immutable — no updates ever
// Indexes: { occurredAt: -1 }, { userId: 1 }, { entityType: 1, entityId: 1 }
// TTL index is NOT set — logs must be retained per BRD Section 9
```

### Critical MongoDB Transaction Pattern (Financial Operations)

All financial writes MUST use a session transaction:

```csharp
// Example: RecordPayment command handler (MongoDB version)
using var session = await _mongoClient.StartSessionAsync();
session.StartTransaction();
try
{
    // 1. Load billing and verify balance
    var billing = await _billings.Find(session, b => b.Id == command.BillingId).FirstOrDefaultAsync();
    var existingPayments = await _payments.Find(session, p => p.BillingId == command.BillingId && !p.IsVoided).ToListAsync();
    var currentBalance = billing.PatientShare - existingPayments.Sum(p => p.AmountPaid);

    if (command.AmountPaid > currentBalance)
        throw new ValidationException("Payment exceeds outstanding balance.");

    // 2. Insert payment
    var payment = new Payment { ... };
    await _payments.InsertOneAsync(session, payment);

    // 3. Recompute and update billing status
    var newTotalPaid = existingPayments.Sum(p => p.AmountPaid) + command.AmountPaid;
    var newStatus = newTotalPaid >= billing.PatientShare ? BillingStatus.FullyPaid : BillingStatus.PartiallyPaid;
    await _billings.UpdateOneAsync(session,
        b => b.Id == billing.Id,
        Builders<Billing>.Update.Set(b => b.Status, newStatus));

    // 4. Write activity log
    await _activityLogs.InsertOneAsync(session, new ActivityLog { ... });

    await session.CommitTransactionAsync();
}
catch
{
    await session.AbortTransactionAsync();
    throw;
}
```

---

## 8. Key Technical Decisions & Patterns

### 8.1 CQRS with MediatR Pipeline

Every API request is handled by a dedicated Command or Query. The MediatR pipeline applies behaviors in order:

```
Request
  → LoggingBehavior       (logs request name + args)
  → ValidationBehavior    (FluentValidation; throws if invalid)
  → TransactionBehavior   (wraps Commands only in a DB transaction)
  → Handler               (actual business logic)
  → Domain Event Dispatch (processes domain events after save)
```

This means controllers are thin — they only translate HTTP to MediatR and MediatR back to HTTP:

```csharp
[HttpPost]
public async Task<IActionResult> CreateAppointment(CreateAppointmentCommand command)
{
    var result = await _mediator.Send(command);
    return CreatedAtAction(nameof(GetAppointment), new { id = result.Id }, result);
}
```

### 8.2 Computed Balance — Never Stored

`Billing.Balance` is never a column in the database. It is always computed on read:

```csharp
// EF Core — billing response DTO
public class BillingResponse
{
    public decimal SubTotal { get; init; }
    public decimal Discount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal HMOCoverageAmount { get; init; }
    public decimal PatientShare { get; init; }
    public decimal TotalPaid { get; init; }           // SUM of non-voided payments
    public decimal OutstandingBalance => PatientShare - TotalPaid;  // computed property
    public BillingStatus Status { get; init; }
}
```

The query fetches billing with its payments in one JOIN and the DTO computes the balance in-process — no stale data possible.

### 8.3 Appointment Conflict Detection

Implemented in the `CreateAppointmentCommandHandler` before any database write:

```csharp
// EF Core version
var newEnd = command.ScheduledAt.AddMinutes(command.DurationMinutes);

var hasConflict = await _context.Appointments
    .Where(a =>
        a.DentistId == command.DentistId &&
        a.Status != AppointmentStatus.Cancelled &&
        a.Status != AppointmentStatus.NoShow &&
        a.ScheduledAt < newEnd &&
        a.ScheduledAt.AddMinutes(a.DurationMinutes) > command.ScheduledAt)
    .AnyAsync();

if (hasConflict)
    throw new AppointmentConflictException(command.DentistId, command.ScheduledAt);
```

This query is covered by a compound index on `(dentist_id, scheduled_at)`.

### 8.4 Domain Events for Side Effects

Domain events decouple the primary write from its side effects:

```
Patient registered
  → PatientRegisteredEvent dispatched after SaveChanges
  → PatientRegisteredEventHandler enqueues CreateTeethJob in Hangfire
  → CreateTeethJob creates 52 Tooth rows/documents in background
  → Registration endpoint returns immediately (fast response)

Appointment marked Completed
  → AppointmentCompletedEvent dispatched
  → Handler calls GenerateBillingCommand (creates Billing with SubTotal)
  → Billing created synchronously (billing must exist before response returns)
```

### 8.5 Tooth Condition — Append-Only Pattern

```csharp
// EF Core entity
public class Tooth
{
    public int Id { get; private set; }
    public int PatientId { get; private set; }
    public string ToothNumber { get; private set; }     // FDI: "16"
    public IReadOnlyCollection<ToothConditionEntry> ConditionHistory => _conditionHistory.AsReadOnly();
    private readonly List<ToothConditionEntry> _conditionHistory = [];

    public void RecordCondition(ToothCondition condition, IEnumerable<ToothSurface> surfaces,
                                int recordedByUserId, int? appointmentId = null, string notes = null)
    {
        // Mark all active entries as historical
        foreach (var entry in _conditionHistory.Where(e => e.IsActive))
            entry.Deactivate();

        // Append new entry
        _conditionHistory.Add(new ToothConditionEntry
        {
            Condition = condition,
            SurfacesAffected = surfaces.ToList(),
            IsActive = true,
            RecordedAt = DateTime.UtcNow,
            RecordedBy = recordedByUserId,
            AppointmentId = appointmentId,
            Notes = notes
        });
    }
}
```

### 8.6 Treatment Applicable Conditions Filter

```csharp
// EF Core — treatments filtered by current tooth condition
// The ToothCondition enum is stored as flags in a join table (Treatment → ApplicableConditions)
var applicableTreatments = await _context.Treatments
    .Where(t =>
        t.Status == TreatmentStatus.Active &&
        t.ApplicableConditions.Any(tc => tc.Condition == currentToothCondition))
    .ToListAsync();
```

On the Vue 3 side, this query is triggered when a tooth is selected during an In Progress appointment session:

```typescript
// AppointmentProcedure.vue <script setup>
const selectedTooth = ref<ToothViewModel | null>(null);
const applicableTreatments = ref<TreatmentDto[]>([]);

async function onToothSelected(tooth: ToothViewModel) {
  selectedTooth.value = tooth;
  applicableTreatments.value = await treatmentsApi.getApplicableFor(
    tooth.activeCondition,
  );
}
```

### 8.7 Audit Logging

Implemented as a MediatR pipeline behavior, not a controller concern:

```csharp
// AuditBehavior<TRequest, TResponse>
public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
                                    CancellationToken cancellationToken)
{
    var before = CaptureStateSnapshot(request);           // serialize relevant state
    var response = await next();
    var after = CaptureStateSnapshot(response);

    await _activityLogRepository.AddAsync(new ActivityLog
    {
        UserId = _currentUser.Id,
        Action = DetermineAction(request),                // Create / Update / Delete
        EntityType = DetermineEntityType(request),        // "Patient", "Appointment", ...
        EntityId = DetermineEntityId(request, response),
        OldValues = before,
        NewValues = after,
        OccurredAt = DateTime.UtcNow,
        IPAddress = _currentUser.IPAddress
    });

    return response;
}
```

> **Sensitive-field masking in `AuditBehavior`:** A generic MediatR interceptor that serializes the full request/response payload will capture raw `PasswordHash`, `SecurityAnswerHash`, and similar fields unless explicitly excluded. `Serilog.Destructuring` policies only mask _log file output_ — they do not affect the JSON payload written to the `AuditLog` PostgreSQL table. The `CaptureStateSnapshot` method must use a **dedicated `JsonSerializerOptions` instance** (or a custom `JsonConverter` / contract resolver) with `[JsonIgnore]` applied to all sensitive properties before writing `OldValues` / `NewValues` to the database. Fields that must never appear in `OldValues` or `NewValues`: `PasswordHash`, `SecurityAnswer1Hash`, `SecurityAnswer2Hash`, any `ApiKey` field, any `PrivateKey` field. These must serialize as `"[REDACTED]"` or be omitted from the snapshot entirely.

### 8.8 OR Number — Concurrent Duplicate Prevention

OR numbers in this system are **manually entered** by staff from a physical BIR ATP booklet (FR-02.7). They are not auto-generated. Because multiple staff members may operate simultaneously, two users could attempt to save the same OR number at the same time — a scenario the application-layer validation cannot reliably catch on its own.

**Required database constraint:**

```csharp
// PaymentConfiguration.cs (IEntityTypeConfiguration<Payment>)
public void Configure(EntityTypeBuilder<Payment> builder)
{
    // ...
    // The unique index must cover ALL rows — including voided payments.
    // FR-07.4.5: "The OR number of a voided payment is retained as voided and is never reused
    // or reassigned." A partial index filtered to is_voided = false would allow a voided OR number
    // to be re-entered, directly violating BIR sequential receipt compliance.
    builder.HasIndex(p => p.ORNumber)
           .IsUnique();  // no filter — voided OR numbers are permanently reserved
}
```

**Required handler-level catch in `RecordPaymentCommandHandler`:**

```csharp
try
{
    await _context.SaveChangesAsync(cancellationToken);
}
catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg && pg.SqlState == "23505"
                                    && pg.ConstraintName == "ix_payments_or_number")
{
    throw new ValidationException("OR number is already recorded in the system. Please verify the OR register and enter the correct number.");
}
```

This pattern ensures that even if two staff members submit the same OR number within the same millisecond, exactly one will succeed and the other will receive a clear, actionable validation error. The `TransactionBehavior` in the MediatR pipeline wraps the entire command — the unique constraint violation is surfaced within the transaction boundary and rolled back cleanly.

### 8.9 Payment Gateway Webhook — On-Premises Access

The BRD mandates an **on-premises LAN deployment** (the clinic runs on a private IP, e.g., `192.168.1.x`) so the system remains usable during internet outages. Phase 7 introduces payment gateway integration (PayMongo / Maya / Paynamics) which depends on **incoming webhook callbacks** from the external gateway to confirm payment and release Valkey slot-locks. An external gateway cannot POST a webhook to a private LAN address.

**Required solution: Cloudflare Tunnel**

| Option                                | Recommendation               | Rationale                                                                                                                                                           |
| ------------------------------------- | ---------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Cloudflare Tunnel** (`cloudflared`) | **Preferred for production** | Zero-port-forward outbound tunnel; no static public IP required; end-to-end TLS; free tier; Cloudflare relays inbound webhook calls to the local API container      |
| Ngrok                                 | Development / staging only   | Free tier changes URL on each restart — not stable for production webhook registration                                                                              |
| Active API polling (fallback)         | If tunnel is not viable      | Backend polls the gateway's payment-status API on a Hangfire schedule (every 30 s, up to 15 min); no inbound path required; slightly higher gateway API call volume |

**Docker Compose addition (production `docker-compose.prod.yml`):**

```yaml
cloudflared:
  image: cloudflare/cloudflared:latest
  command: tunnel --no-autoupdate run --token ${CLOUDFLARE_TUNNEL_TOKEN}
  restart: unless-stopped
  depends_on: [nginx]
```

The `CLOUDFLARE_TUNNEL_TOKEN` is stored in the production `.env` file (gitignored) and injected as a Docker environment variable. The tunnel is configured in the Cloudflare dashboard to route only the webhook path (`/api/webhooks/payment-gateway`) to the internal Nginx → API container; all other clinic traffic continues on the LAN.

**Polling fallback implementation note:**

If Cloudflare Tunnel is not deployed, `SlotReleaseJob` (Hangfire) is extended with a polling loop: call `IPaymentGatewayClient.GetPaymentStatus(paymentIntentId)` every 30 seconds. On gateway success, confirm the slot. On 15-minute timeout with no success, release the slot. Each gateway adapter must implement `GetPaymentStatus` in addition to `InitiatePayment`.

**Webhook signature validation is always required** regardless of delivery mechanism — validate the gateway-specific HMAC or RSA signature on every inbound payload before processing any state change (NFR-S04).

---

## 9. Security Implementation Plan

| Threat                                  | Mitigation                                                                                                                                     |
| --------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------- |
| SQL/NoSQL Injection                     | EF Core parameterized queries only; no raw SQL strings with user input; MongoDB driver parameterized filter builders only                      |
| XSS                                     | Vue 3's built-in HTML escaping on `{{ }}` interpolation; `v-html` is never used with user-supplied content; CSP header via Nginx               |
| CSRF                                    | SameSite=Strict on auth cookie if used; `axios` does not send cookies by default (JWT in memory only)                                          |
| IDOR (Insecure Direct Object Reference) | Every API handler validates that the requesting user has access to the requested resource (not just that the ID exists); tenant-scoped queries |
| Credential Exposure                     | BCrypt cost factor 12 for passwords and security answers; security answers stored as separate hashes; no plaintext ever logged                 |
| Token Theft                             | JWT stored in memory (not localStorage); `httpOnly` refresh token cookie for token refresh only; token blacklist in Valkey on logout           |
| Brute Force                             | Rate limiter on `/api/auth/login` (10 requests/minute per IP); account lockout after 5 failures                                                |
| Sensitive Data in Logs                  | `Serilog.Destructuring` policies exclude `PasswordHash`, `SecurityAnswer1Hash`, `SecurityAnswer2Hash` from all log output                      |
| Dependency Vulnerabilities              | `dotnet list package --vulnerable` in CI pipeline; `npm audit` in CI pipeline; Dependabot alerts on GitHub                                     |
| Secrets in Source Control               | `.env` files gitignored; Docker secrets or environment variables for all credentials; CI secrets stored in GitHub Actions encrypted secrets    |

---

## 10. Full Dependency Reference

### Backend NuGet Packages

```xml
<!-- AllAboutTeeth.Api.csproj -->
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.*" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.*" />
<PackageReference Include="Serilog.AspNetCore" Version="8.*" />
<PackageReference Include="Serilog.Sinks.PostgreSQL" Version="4.*" />
<PackageReference Include="Serilog.Enrichers.CorrelationId" Version="3.*" />
<PackageReference Include="Hangfire.AspNetCore" Version="1.*" />
<PackageReference Include="Hangfire.PostgreSql" Version="1.*" />
<PackageReference Include="Microsoft.AspNetCore.SignalR" Version="1.*" />

<!-- AllAboutTeeth.Application.csproj -->
<PackageReference Include="MediatR" Version="12.*" />
<PackageReference Include="FluentValidation.AspNetCore" Version="11.*" />
<PackageReference Include="AutoMapper" Version="13.*" />

<!-- AllAboutTeeth.Infrastructure.csproj -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.*" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.*" />
<PackageReference Include="BCrypt.Net-Next" Version="4.*" />
<PackageReference Include="StackExchange.Redis" Version="2.*" />          <!-- MIT; compatible with Valkey -->
<PackageReference Include="Microsoft.Extensions.Caching.StackExchangeRedis" Version="8.*" />
<PackageReference Include="PdfSharpCore" Version="1.*" />                  <!-- MIT -->
<PackageReference Include="MigraDoc.NetStandard" Version="1.*" />          <!-- MIT -->
<PackageReference Include="MailKit" Version="4.*" />

<!-- AllAboutTeeth.UnitTests.csproj -->
<PackageReference Include="xUnit" Version="2.*" />
<PackageReference Include="Moq" Version="4.*" />
<PackageReference Include="FluentAssertions" Version="6.*" />
<PackageReference Include="Bogus" Version="35.*" />

<!-- AllAboutTeeth.IntegrationTests.csproj -->
<PackageReference Include="Testcontainers.PostgreSql" Version="3.*" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.*" />
```

### Frontend NPM Packages

```json
{
  "dependencies": {
    "vue": "^3.4.0",
    "vue-router": "^4.3.0",
    "pinia": "^2.1.0",

    "axios": "^1.7.0",
    "jwt-decode": "^4.0.0",

    "primevue": "^4.0.0",
    "@primevue/themes": "^4.0.0",
    "primeicons": "^7.0.0",

    "@fullcalendar/vue3": "^6.1.0",
    "@fullcalendar/daygrid": "^6.1.0",
    "@fullcalendar/timegrid": "^6.1.0",
    "@fullcalendar/interaction": "^6.1.0",

    "echarts": "^5.4.0",
    "vue-echarts": "^7.0.0",

    "@microsoft/signalr": "^8.0.0",

    "tailwindcss": "^3.4.0"
  },
  "devDependencies": {
    "vite": "^5.0.0",
    "@vitejs/plugin-vue": "^5.0.0",
    "typescript": "^5.4.0",
    "vue-tsc": "^2.0.0",
    "vitest": "^1.6.0",
    "@vue/test-utils": "^2.4.0",
    "@testing-library/vue": "^8.0.0",
    "@types/node": "^20.0.0"
  }
}
```

### Infrastructure Versions

| Component               | Version                | Notes                                                                                                |
| ----------------------- | ---------------------- | ---------------------------------------------------------------------------------------------------- |
| PostgreSQL              | 16 (Alpine)            | Primary data store                                                                                   |
| Valkey                  | 8 (Alpine)             | Cache, token blacklist, rate limit counters — BSD 3-Clause open-source Redis fork (Linux Foundation) |
| Nginx                   | Latest stable (Alpine) | Reverse proxy, SSL, static SPA serving                                                               |
| Docker                  | 25+                    | Container runtime                                                                                    |
| Docker Compose          | v2                     | Multi-container orchestration                                                                        |
| Ubuntu Server           | 22.04 LTS              | Production host OS                                                                                   |
| .NET SDK                | 8 (LTS)                | API runtime                                                                                          |
| Node.js                 | 20 LTS                 | Vue 3 + Vite build toolchain                                                                         |
| Let's Encrypt / Certbot | Latest                 | SSL certificate — free, auto-renewing                                                                |
