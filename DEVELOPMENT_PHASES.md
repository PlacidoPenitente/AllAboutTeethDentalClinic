# Development Phase Plan

## All About Teeth Dental Clinic Management System (DCMS)

---

| Field            | Value                                      |
| ---------------- | ------------------------------------------ |
| Document Version | 2.0                                        |
| Date             | May 13, 2026                               |
| Basis            | BRD v4.3                                   |
| Repository       | PlacidoPenitente/AllAboutTeethDentalClinic |

---

## Purpose

This document organises BRD v4.3 development following standard software development lifecycle (SDLC) practice. Each functional area is split into a **Backend (A)** phase — database schema, API endpoints, business logic, CQRS handlers, and backend tests — and a **Frontend (B)** phase — Vue 3 components, Pinia stores, API integration, and frontend tests. SDLC framing phases (project setup, system testing, UAT, deployment, and post-launch support) precede and follow the functional work.

**Phase sequence:**

```
Phase 0   → Project Setup & Architecture Design
Phase 1A  → Backend:  Foundation & Infrastructure
Phase 1B  → Frontend: Foundation & Infrastructure
Phase 2A  → Backend:  Patient Management & Scheduling
Phase 2B  → Frontend: Patient Management & Scheduling
Phase 3A  → Backend:  Clinical Core & Financial Operations
Phase 3B  → Frontend: Clinical Core & Financial Operations
Phase 4A  → Backend:  Inventory, Reports & Notifications
Phase 4B  → Frontend: Inventory, Reports & Notifications
Phase 5A  → Backend:  Specialized Clinical Modules
Phase 5B  → Frontend: Specialized Clinical Modules
Phase 6A  → Backend:  Patient Portal API
Phase 6B  → Frontend: Patient Portal SPA
Phase 7A  → Backend:  Online Booking Deposits
Phase 7B  → Frontend: Online Booking Deposits UI
Phase 8   → System & Integration Testing
Phase 9   → User Acceptance Testing (UAT)
Phase 10  → Deployment & Go-Live
Phase 11  → Post-Launch Monitoring & Support
```

**Sequencing rule:** Each Backend (A) phase must reach its acceptance criteria before the corresponding Frontend (B) phase begins. Phases 8–11 are sequential and follow completion of all Phase 7 work.

**Guiding clinical priority rules (unchanged):**

1. If it blocks patient registration or appointment creation, it is Phase 1 or 2.
2. If the dentist cannot do clinical work without it, it belongs in Phase 3 at the latest.
3. If the clinic cannot collect money without it, it belongs in Phase 3.
4. Inventory, reports, and notifications are operational improvements — they come after the clinic can run at all.
5. Specialized clinical modules (prescriptions, ortho notes, certificates, installments) serve specific workflows — they come after the core is stable.
6. The Patient Portal is a separate SPA and depends on the full backend being complete. It is Phase 6.
7. Online deposits are a portal sub-feature requiring third-party gateway integration — they are isolated to Phase 7.

---

## Technology Stack

The table below maps every technology in the chosen stack to the BRD v4.3 requirement that justifies it. The **Phase** column shows when each component is first activated.

### Clinic SPA & Patient Portal SPA

| Component                                   | BRD Justification                                                                                                         | Phase |
| ------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------- | ----- |
| Vue 3 + Vite + TypeScript                   | NFR-U01 (responsive web; Chrome/Edge/Firefox latest 2 versions)                                                           | 1     |
| PrimeVue 4+                                 | NFR-U03 (required-field indicator; inline validation errors); NFR-U04 (confirmation dialogs before destructive actions)   | 1     |
| Pinia                                       | State management for real-time widget data (FR-09.1.A.2)                                                                  | 1     |
| vue-router                                  | SPA routing for clinic app; reused in Patient Portal SPA (Phase 6)                                                        | 1     |
| Tailwind CSS                                | NFR-U01 (desktop full-function; tablet usable; mobile readable)                                                           | 1     |
| Vitest                                      | Automated tests validating OWASP mitigations (NFR-S04)                                                                    | 1     |
| FullCalendar                                | FR-04.3 (day/week/month calendar view); NFR-P02 (calendar loads ≤ 1 s)                                                    | 2     |
| SignalR JS client                           | FR-09.1.A.2 (WebSocket real-time push; widgets update without page reload)                                                | 2     |
| echarts                                     | FR-09.1.C–D (revenue trend bar, appointment donut, top-5 treatments bar); FR-12.8.6 (rating distribution chart — Phase 6) | 3     |
| Patient Portal SPA (separate Vue 3 project) | FR-12 (public-facing interface separate from clinic app; `portal` JWT audience — BR-07.2)                                 | 6     |

### Backend API

| Component                                                 | BRD Justification                                                                                                                                                                                                           | Phase |
| --------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----- |
| ASP.NET Core 8 Web API                                    | NFR-M01 (containerized); NFR-M03 (`/health` endpoint)                                                                                                                                                                       | 1     |
| Clean Architecture + CQRS + MediatR 12                    | NFR-R02 (every multi-table mutation in one atomic transaction); NFR-M02 (EF Core migrations as sole schema change path)                                                                                                     | 1     |
| FluentValidation 11                                       | NFR-S04 (input validation at all API boundaries — SQL injection, XSS, IDOR); NFR-U03 (validation error detail returned to caller)                                                                                           | 1     |
| AutoMapper 13                                             | NFR-S08 (API responses never expose internal domain model or stack traces — DTOs projected from domain entities)                                                                                                            | 1     |
| EF Core 8 + Npgsql                                        | NFR-M02 (migrations only; no manual SQL in production); NFR-R02 (atomic transactions); NFR-R03 (FK constraints at DB level)                                                                                                 | 1     |
| BCrypt (cost ≥ 12)                                        | NFR-S01; FR-01.1.5 (staff passwords); FR-01.3.1 (security answer hashes); FR-12.1.3 (portal passwords)                                                                                                                      | 1     |
| JWT RS256                                                 | NFR-S03; FR-01.2.4 (clinic staff token); FR-12.2.2 (`portal` audience claim — same signing infrastructure, separate audience claim)                                                                                         | 1     |
| Serilog                                                   | NFR-M04 (structured JSON logging: timestamp, level, request ID); NFR-S08 (errors server-logged only — never in HTTP response body)                                                                                          | 1     |
| SignalR (server hub)                                      | FR-09.1.A.2 (WebSocket push to all dashboard widgets)                                                                                                                                                                       | 2     |
| PdfSharpCore + MigraDoc                                   | FR-07.2.3 (SOA PDF); FR-07.3.6 (BIR-compliant OR PDF); FR-09.2 (all report PDFs); FR-09.3.2 (OR register PDF); FR-15.9 (prescription PDF); FR-16.7 (Medical Certificate PDF)                                                | 3     |
| Hangfire                                                  | FR-10.7–10.8 (async notification delivery + 3-retry exponential backoff); FR-13.5 (async bulk export); FR-07.5.4 (nightly installment status job); FR-10.10 (daily recall job); FR-12.4.6 (15-min slot-release timeout job) | 4     |
| MailKit                                                   | FR-02.11 (SMTP config); FR-10.3–10.6 (appointment lifecycle emails); FR-12.1.4 (portal verify email); FR-12.2.3 (password reset email); BR-02.12 (AR deposit confirmation email)                                            | 4     |
| SMS gateway HTTP client (Semaphore / Globe Labs / ITEXMO) | FR-02.10 (generic HTTP REST; Philippine providers configurable in Clinic Settings); FR-10.3–10.10 (all patient SMS notifications)                                                                                           | 4     |
| Payment gateway HTTP client (PayMongo / Maya / Paynamics) | FR-02.12 (online deposit collection); FR-12.4.6 (gateway deposit flow); NFR-S04 (webhook signature validation); NFR-S05–S06 (API keys encrypted at rest; never in source control); NFR-S08 (keys never in logs)             | 7     |

### Infrastructure

| Component                                     | BRD Justification                                                                                                                                                                                                                                                                                            | Phase                           |
| --------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------------------------------- |
| PostgreSQL 16 Alpine                          | NFR-R01 (`DECIMAL(10,2)` for all monetary fields; FLOAT/DOUBLE prohibited); NFR-R03 (FK constraints enforced at DB level)                                                                                                                                                                                    | 1                               |
| Docker + Docker Compose v2                    | NFR-M01 (full stack containerized; `docker compose up` brings all services); NFR-M06 (on-premises LAN — clinic operates during internet outage)                                                                                                                                                              | 1                               |
| Nginx                                         | NFR-S02 (HTTPS/TLS 1.2+ termination for clinic SPA, Patient Portal SPA, and API)                                                                                                                                                                                                                             | 1                               |
| Ubuntu 22.04 LTS                              | NFR-M05 (production server ≥ 2 vCPUs, 4 GB RAM)                                                                                                                                                                                                                                                              | 1                               |
| GitHub Actions                                | NFR-M02 (automated EF Core migration CI; Vitest runs on every push)                                                                                                                                                                                                                                          | 1                               |
| **Valkey 8** (StackExchange.Redis compatible) | FR-01.2.5 (server-side token revocation set for logout invalidation); FR-12.4.6 (ephemeral 15-min slot lock for gateway payments). **No BRD NFR mandates a cache layer** — Valkey is an implementation choice for these two specific requirements. Must be in Docker Compose to satisfy NFR-M01 and NFR-M06. | 1 (token store) / 7 (slot lock) |

---

## Phase Overview

| Phase | Type     | Name                                 | End-State Capability                                                                   |
| ----- | -------- | ------------------------------------ | -------------------------------------------------------------------------------------- |
| 0     | SDLC     | Project Setup & Architecture Design  | Dev environment running; ER diagram agreed; OpenAPI skeleton committed; CI stubs live  |
| 1A    | Backend  | Foundation & Infrastructure          | Auth API live; Clinic Settings API live; Audit Log writing; Docker stack healthy       |
| 1B    | Frontend | Foundation & Infrastructure          | Login UI; user/settings management screens; Audit Log viewer                           |
| 2A    | Backend  | Patient Management & Scheduling      | Patient CRUD API; Appointment API; conflict detection; SignalR hub scaffolded          |
| 2B    | Frontend | Patient Management & Scheduling      | Patient registration forms; appointment calendar; real-time dashboard shell            |
| 3A    | Backend  | Clinical Core & Financial Operations | Dental chart API; treatment record API; billing API; payment API; report endpoints     |
| 3B    | Frontend | Clinical Core & Financial Operations | SVG dental chart; procedure recording UI; billing & payment screens; financial reports |
| 4A    | Backend  | Inventory, Reports & Notifications   | Inventory API; all 14 report queries; Hangfire jobs; SMS/email delivery                |
| 4B    | Frontend | Inventory, Reports & Notifications   | Inventory management UI; all 14 report views; notification log                         |
| 5A    | Backend  | Specialized Clinical Modules         | Prescription API; certificate API; attachment API; ortho note API; installment API     |
| 5B    | Frontend | Specialized Clinical Modules         | Prescription/certificate forms; attachment tab; ortho note UI; installment plan UI     |
| 6A    | Backend  | Patient Portal API                   | Portal auth endpoints; self-registration API; online booking API; feedback/rating API  |
| 6B    | Frontend | Patient Portal SPA                   | Public-facing SPA; booking calendar; my appointments; billing summary; rating/feedback |
| 7A    | Backend  | Online Booking Deposits              | Payment gateway integration; deposit command; Valkey slot-lock; webhook handler        |
| 7B    | Frontend | Online Booking Deposits UI           | Gateway redirect flow; deposit confirmation; staff verification inbox                  |
| 8     | SDLC     | System & Integration Testing         | All API endpoints tested E2E; Playwright suite passes; load test baseline met          |
| 9     | SDLC     | User Acceptance Testing (UAT)        | Clinic staff complete all critical-path scenarios; written sign-off obtained           |
| 10    | SDLC     | Deployment & Go-Live                 | Production server live; TLS active; staff trained; smoke test passed                   |
| 11    | SDLC     | Post-Launch Monitoring & Support     | Uptime monitored; P1/P2 bugs resolved; 30-day review completed                         |

---

## Phase 0 — Project Setup & Architecture Design

### Goal

All developers share a working local environment. The repository structure follows the documented Clean Architecture layout. Architecture decisions are confirmed against BRD v4.3. The entity-relationship model is drafted and agreed before any production code is written. CI/CD pipeline stubs are in place so quality gates operate from the first commit.

### Deliverables

| Deliverable               | Description                                                                                                                                                                                                                                                                          |
| ------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Dev environment checklist | Docker Desktop, .NET 8 SDK, Node.js 20 LTS, PostgreSQL GUI (DBeaver / TablePlus), VS Code with C# Dev Kit + Vue Language Features extensions verified on all developer machines                                                                                                      |
| Repository structure      | `backend/` (4-project Clean Architecture solution stubs), `frontend/allaboutteeth-web/` (Vue 3 + Vite scaffold), `frontend/allaboutteeth-portal/` (portal SPA scaffold — Phase 6B), `docker-compose.yml`, `docker-compose.override.yml`, `nginx.conf`, `.editorconfig`, `.gitignore` |
| Code style baseline       | `.editorconfig` for C# naming and indentation; ESLint + Prettier config for TypeScript/Vue; enforced in CI                                                                                                                                                                           |
| Draft ER diagram          | All BRD v4.3 entities and relationships diagrammed (Patient, Tooth, Appointment, Billing, Payment, etc.); reviewed and team-approved before Phase 1A begins                                                                                                                          |
| OpenAPI contract skeleton | Controller stubs with route definitions, request/response shapes, and security annotations — not implemented, used for API-first alignment between backend and frontend teams                                                                                                        |
| CI/CD stubs               | `.github/workflows/ci.yml` (triggers on PR: `dotnet build`, `dotnet test`, `vite build`), `staging.yml` stub, `production.yml` stub                                                                                                                                                  |
| OWASP review              | OWASP Top 10 checklist completed against the planned architecture; documented mitigations per threat confirmed before Phase 1A begins                                                                                                                                                |

### Phase-End Acceptance Criteria

- `docker compose up` starts all services (stub API, PostgreSQL, Valkey, Nginx) and `/health` returns HTTP 200.
- The CI pipeline runs on a PR without errors (`dotnet build` succeeds on solution stubs; `vite build` succeeds on scaffold).
- Draft ER diagram is reviewed and written approval noted by the development team before Phase 1A begins.
- All developer machines have confirmed working local environments.

---

## Phase 1A — Backend: Foundation & Infrastructure

### Goal

The security layer, authentication API, user management, clinic configuration, and audit log are production-ready. Every subsequent backend phase builds on this security foundation.

### BRD Coverage

| Module                              | Subsections / Items Included                                                                                                                                                                                                     |
| ----------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **FR-01: User & Access Management** | FR-01.1 (user accounts: roles, status), FR-01.2 (authentication: login, logout, lockout after 5 failures, 30-min auto-unlock), FR-01.3 (password recovery via security questions), FR-01.4 (RBAC: Administrator, Dentist, Staff) |
| **FR-02: Clinic Settings**          | FR-02.1 – FR-02.9 (identity, compliance numbers, operating schedule, default slot duration, VAT config, OR number tracking field, security config, expiry warning threshold)                                                     |
| **FR-11: Audit Log**                | FR-11.1 – FR-11.5 (all — cross-cutting; scaffolded once and consumed by every subsequent phase)                                                                                                                                  |

### Business Rules

- BR-04.1 – BR-04.4 (user management rules)

### Non-Functional Requirements

- NFR-S01 – NFR-S08 (all security requirements — implemented from day one, never retrofitted)
- NFR-R01 – NFR-R03 (DECIMAL(10,2), atomic transactions, FK constraints — schema standards established in Migration 1)
- NFR-M01 – NFR-M06 (Docker Compose stack, EF Core migrations only, `/health` endpoint, structured logging, on-premises deployment)

### New Database Entities

`User`, `ClinicSettings`, `AuditLog`

### API Endpoints

- `POST /api/auth/login`, `POST /api/auth/logout`, `POST /api/auth/refresh`
- `POST /api/auth/recover/initiate`, `POST /api/auth/recover/confirm`
- `GET /api/users`, `POST /api/users`, `PUT /api/users/{id}`, `DELETE /api/users/{id}` (archive), `POST /api/users/{id}/unlock`
- `GET /api/clinic-settings`, `PUT /api/clinic-settings`
- `GET /api/audit-logs` (paginated, filterable by date, actor, entity type)

### Backend Tech Introduced

- **ASP.NET Core 8 + Clean Architecture + CQRS/MediatR 12 + FluentValidation 11 + AutoMapper 13** — API scaffold; all input validated at boundaries (NFR-S04); responses never expose internals (NFR-S08)
- **EF Core 8 + Npgsql + PostgreSQL 16 Alpine** — Migration 1 establishes `DECIMAL(10,2)` baseline (NFR-R01); FK constraints at DB level (NFR-R03); EF Core migrations only — no manual SQL (NFR-M02)
- **Docker Compose v2 + Nginx** — full stack containerized; Nginx terminates TLS 1.2+ (NFR-S02, NFR-M01, NFR-M06)
- **JWT RS256** — clinic staff tokens; `portal` audience claim infrastructure established here for reuse in Phase 6A (NFR-S03, FR-01.2.4)
- **BCrypt (cost ≥ 12)** — staff password and security answer hashes (NFR-S01, FR-01.1.5, FR-01.3.1)
- **Valkey 8** — token revocation set for server-side logout invalidation (FR-01.2.5); included in Docker Compose from this phase onward
- **Serilog** — structured JSON logging with timestamp, level, and request ID; errors never returned in HTTP responses (NFR-M04, NFR-S08)
- **GitHub Actions CI** — `dotnet build` + `dotnet test` on every PR (NFR-M02)

### Backend Tests

- Unit: authentication flow, BCrypt hashing, Valkey token revocation, lockout counter logic
- Integration (Testcontainers): login → JWT issued; lockout after 5 failures; logout → token blacklisted; audit log written on every mutation

### Phase-End Acceptance Criteria

- `POST /api/auth/login` returns a signed JWT on valid credentials; returns 401 on invalid credentials.
- Account locks automatically after 5 failed login attempts; unlocks after 30 minutes or via Administrator action.
- `POST /api/auth/logout` blacklists the token in Valkey; subsequent requests with that token return 401.
- All user management and clinic settings mutations write a corresponding `AuditLog` entry.
- `docker compose up` starts all services; `/health` returns 200; `dotnet ef migrations list` shows Migration 1 applied.

---

## Phase 1B — Frontend: Foundation & Infrastructure

### Goal

Clinic staff have a working web application they can log in to. The Administrator can manage user accounts and configure clinic identity and settings through the UI.

### Frontend Tech Introduced

- **Vue 3 + Vite + TypeScript** — clinic SPA scaffold (`frontend/allaboutteeth-web/`)
- **PrimeVue 4+** — component library (form controls, DataTable, Dialog, Toast — NFR-U03, NFR-U04)
- **Pinia** — global state management
- **vue-router** — SPA routing with lazy-loaded routes, `beforeEach` auth guard, per-route `requireRole` meta
- **Tailwind CSS** — responsive layout (NFR-U01: desktop full-function, tablet usable)
- **Vitest + Vue Test Utils** — component unit tests; runs in CI on every PR (NFR-S04)
- **axios** — HTTP client with request interceptor (JWT attachment) and response interceptor (401 → redirect to login)

### Pinia Stores

`auth.store.ts` (JWT token, decoded role, expiry), `notification.store.ts` (toast queue)

### Pages & Components

| Component                | Description                                                                                        |
| ------------------------ | -------------------------------------------------------------------------------------------------- |
| `LoginView.vue`          | Email + password form, inline error messages, locked-account state with time remaining             |
| `AppShell.vue`           | Sidebar navigation (role-gated menu links), top bar (username, logout button), main content outlet |
| `UsersView.vue`          | Paginated DataTable of user accounts; create/edit/archive/unlock actions; Administrator only       |
| `UserFormDialog.vue`     | Create / edit user dialog with role selection and inline FluentValidation messages                 |
| `ClinicSettingsView.vue` | Tabbed settings form: clinic identity, compliance numbers, operating hours, VAT, security config   |
| `AuditLogView.vue`       | Date-range + actor + entity-type filters; paginated read-only log table                            |

### Phase-End Acceptance Criteria

- Staff can log in; incorrect credentials show an inline error; locked accounts show the lockout message with approximate time remaining.
- After logout, navigating to any protected route redirects to the login page (token revoked server-side).
- Administrator can create a Dentist account, change its role, and archive it; all actions appear in the Audit Log view.
- Clinic Settings form saves all fields and reflects saved values on reload.
- Audit Log view lists entries with actor, timestamp, entity, and action; date-range filter narrows results.
- All form fields with required or format constraints display inline validation errors matching the API's FluentValidation messages (NFR-U03).
- Vitest component test suite passes in CI with no failures.

---

## Phase 2A — Backend: Patient Management & Scheduling

### Goal

Patient records, appointment lifecycle, conflict detection, vitals recording, and dashboard data APIs are fully functional. The SignalR hub is scaffolded for real-time push from this phase onward.

### BRD Coverage

| Module                            | Subsections / Items Included                                                                                                                                                                                                                                                                                                                   |
| --------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **FR-03: Patient Management**     | FR-03.1 (registration + age-based tooth initialization per FR-03.1.6 mixed dentition rules), FR-03.2 (personal & contact), FR-03.3 (dental history), FR-03.4 (medical history incl. pregnancy flag FR-03.4.12), FR-03.5 (search & list), FR-03.6 (Active / Archived status)                                                                    |
| **FR-04: Appointment Scheduling** | FR-04.1 (create, conflict detection, planned treatments, LOA number), FR-04.2 (full status lifecycle: Pending → Confirmed → In Progress → Completed, plus No Show, Cancelled, Awaiting Approval, Rejected), FR-04.3 (calendar data endpoint), FR-04.4 (appointment list), FR-04.5 (pre-operative vitals + pregnancy × X-ray warning FR-04.5.6) |
| **FR-09.1: Dashboard (partial)**  | FR-09.1.A (general — **SignalR hub scaffolded**; FR-09.1.A.2 mandates real-time push from the first dashboard widget onward), FR-09.1.B (all-user widgets: today's appointments, counts, pending count)                                                                                                                                        |
| **FR-09.2 (partial)**             | RPT-01 (Daily Appointment List) — the one report needed before billing exists                                                                                                                                                                                                                                                                  |

### Business Rules

- BR-01.1 – BR-01.8 (all appointment scheduling rules including Awaiting Approval and Rejected portal booking states)
- BR-03.3 (tooth records created at registration)
- BR-06.1 – BR-06.2 (privacy consent required; patient access logged)

### New Database Entities

`Patient`, `Tooth` (52 per patient, initialized per FR-03.1.6 age brackets), `MedicalHistory`, `DentalHistory`, `Appointment`, `PlannedTreatment`, `AppointmentVitalSigns`, `Provider` (HMO/insurance reference, needed for dental history in FR-03.3.4)

### API Endpoints

- `GET /api/patients` (search: name, patient number), `POST /api/patients`, `GET /api/patients/{id}`, `PUT /api/patients/{id}`, `DELETE /api/patients/{id}` (archive)
- `GET /api/patients/{id}/teeth`
- `GET /api/appointments`, `POST /api/appointments`, `GET /api/appointments/{id}`, `PUT /api/appointments/{id}`
- `POST /api/appointments/{id}/confirm`, `/start`, `/complete`, `/cancel`, `/no-show`
- `GET /api/appointments/calendar?date=&dentistId=`
- `POST /api/appointments/{id}/vitals`
- `GET /api/dashboard/widgets`
- `GET /api/reports/rpt-01`
- SignalR: `DashboardHub` — accepts WebSocket connections; broadcasts `AppointmentStatusChanged` event

### Backend Tech Introduced

- **SignalR (server hub)** — `DashboardHub` scaffolded; broadcasts real-time events required by FR-09.1.A.2

### Backend Tests

- Unit: conflict detection algorithm, mixed dentition initialization (7-year-old fixture), appointment status machine (valid and invalid transitions)
- Integration (Testcontainers): patient registration + 52 tooth records created; overlapping appointment rejected (409); walk-in → InProgress; status lifecycle; SignalR connection accepted

### Phase-End Acceptance Criteria

- `POST /api/patients` registers a patient and creates 52 `Tooth` records per FR-03.1.6 (7-year-old fixture must produce the correct PNT / SHD / UNE distribution including permanent central incisors 11, 21, 31, 41 as PNT).
- `POST /api/appointments` with an overlapping dentist-slot returns 409 Conflict.
- Walk-in appointment creation returns `status: InProgress` without requiring a Confirm step.
- `DashboardHub` accepts WebSocket connections; `AppointmentStatusChanged` broadcast verified in integration test.
- `GET /api/reports/rpt-01?date=` returns a structured appointment list that is PDF-exportable.

---

## Phase 2B — Frontend: Patient Management & Scheduling

### Goal

Front-desk staff can register patients with full medical and dental history, manage the full appointment calendar, and see a live-updating dashboard — all through the UI.

### Frontend Tech Introduced

- **FullCalendar** (`@fullcalendar/vue3`, daygrid, timegrid, interaction) — day/week/month appointment calendar; must meet ≤ 1 s load target (NFR-P02)
- **SignalR JS client** (`@microsoft/signalr`) — connects to `DashboardHub`; drives live calendar and dashboard widget updates without page reload (FR-09.1.A.2)

### Pinia Stores

`patient.store.ts` (active patient, search results), `appointment.store.ts` (calendar events, selected appointment)

### Pages & Components

| Component                     | Description                                                                                            |
| ----------------------------- | ------------------------------------------------------------------------------------------------------ |
| `PatientListView.vue`         | Search bar + paginated DataTable; name / patient number search; Archive action (Admin)                 |
| `PatientRegisterView.vue`     | Multi-tab registration form: demographics, dental history, medical history, consent toggle             |
| `PatientEditView.vue`         | Same tabs for editing; fields that cannot change after creation are read-only                          |
| `PatientDetailView.vue`       | Read-only patient profile with navigation to related records                                           |
| `AppointmentCalendarView.vue` | FullCalendar instance; dentist filter; click-to-open appointment detail; real-time updates via SignalR |
| `AppointmentListView.vue`     | Tabs: Today / Upcoming / Past; status badges                                                           |
| `AppointmentFormDialog.vue`   | Create / edit: patient search, dentist, date/time, duration, planned treatments, LOA number field      |
| `AppointmentDetailView.vue`   | Status action buttons (Confirm, Start, Complete, Cancel, No-Show); pre-op vitals sub-form              |
| `DashboardView.vue`           | Today's schedule widget, appointment count cards; live updates via SignalR                             |

### Phase-End Acceptance Criteria

- Staff search for a patient by name; results appear within 500 ms for ≥ 1,000 seeded patients (NFR-P01).
- Patient registration form validates the privacy consent toggle as required before save (BR-06.1); all required fields show inline errors.
- Appointment calendar renders today's appointments color-coded by status and loads within 1 s (NFR-P02).
- Changing an appointment's status in one browser tab updates the calendar in a second open tab within 2 s via SignalR — no page reload required (FR-09.1.A.2).
- Booking a conflicting appointment displays a user-friendly conflict message (not a raw HTTP error).
- Walk-in booking flow bypasses Pending status and opens the appointment In Progress immediately.
- Pre-operative vitals form is visible on In Progress appointments only; an empty required vital shows an inline error.
- RPT-01 renders correctly and the Print / Export PDF action opens the PDF blob.

---

## Phase 3A — Backend: Clinical Core & Financial Operations

### Goal

The dental chart, treatment records, billing, payment, OR generation, and HMO claim APIs are all production-ready. The full end-to-end patient visit can be run entirely through the API.

### BRD Coverage

| Module                         | Subsections / Items Included                                                                                                                                                                                                                                                                                                                                           |
| ------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **FR-05: Dental Charting**     | FR-05.1 (chart data endpoint), FR-05.2 (tooth detail: condition history, procedure history), FR-05.3 (update tooth conditions — append-only history, Quick Set tool), FR-05.4 (applicable-conditions filter — productivity aid, not a gate; FR-05.4.1)                                                                                                                 |
| **FR-06: Treatment Records**   | FR-06.1 (treatment catalog: tooth-specific vs. global, categories, pricing, applicable/resulting conditions, interim condition, required specialization, procedure kit reference), FR-06.2 (procedure recording per appointment: per-tooth, completion flag, resulting/interim condition update), FR-06.3 (immutable treatment records / clinical history per patient) |
| **FR-07: Billing & Payments**  | FR-07.1 (billing generation, draft→final, subtotal/discount/tax/HMO share), FR-07.2 (SOA PDF), FR-07.3 (payment recording: manual OR number entry, multiple payment methods, OR PDF), FR-07.4 (payment voiding — Administrator only, mandatory reason)                                                                                                                 |
| **FR-14: HMO Claim Recording** | FR-14.1 – FR-14.6 (LOA entry, tariff look-up endpoint, coverage cap, per-patient LOA history, disclaimer, RPT-13 stub)                                                                                                                                                                                                                                                 |
| **FR-09.1 (additions)**        | FR-09.1.C (Administrator revenue widgets: today/week/month revenue, outstanding receivables, revenue trend, new patients), FR-09.1.D (appointment status donut, top-5 treatments bar), FR-09.1.E (dentist's own schedule and procedure count)                                                                                                                          |
| **FR-09.2 (partial)**          | RPT-04 (Revenue), RPT-05 (Billing Register), RPT-06 (AR), RPT-07 (Dentist Production), RPT-08 (Treatment Frequency), RPT-13 (HMO Claims), RPT-14 (Daily Collection)                                                                                                                                                                                                    |
| **FR-09.3: OR Register**       | FR-09.3.1 – FR-09.3.2 (sequential OR register, PDF/CSV export)                                                                                                                                                                                                                                                                                                         |
| **FR-03.7: Patient File Attachments (manual uploads)** | FR-03.7.1 – FR-03.7.5 (upload JPEG/PNG/PDF up to 20 MB, label + note, reverse-chron list, auth-gated download, audit-logged deletion, server-filesystem storage) — promoted to this phase so X-rays and imaging are available to clinicians during the Clinical Core. System-Generated entries (prescription/certificate PDFs from Phase 5A) reuse this same infrastructure; no additional API endpoints are needed in Phase 5A. |

### Business Rules

- BR-02.1 – BR-02.13 (all financial rules: balance computation, OR uniqueness, void rules, fully-paid transition — BR-02.11/BR-02.12 scaffolded here; installment plan UI is Phase 5B)
- BR-03.1 – BR-03.7 (all clinical rules: immutable records, append-only chart history, FDI notation, applicable-condition filter, read-only chart outside In Progress)
- BR-08.1 – BR-08.4 (HMO claim recording rules)

### Non-Functional Requirements

- NFR-R04 (balance always computed from payment records — never stored as a column)
- NFR-U05 (SVG dental chart — not a table)
- NFR-P01 – NFR-P04 (search 500 ms, calendar 1 s, chart 1 s, financial reports 5 s)

### New Database Entities

`TreatmentCatalog`, `ProcedureKit`, `ToothConditionEntry`, `AppointmentProcedure`, `AppointmentGlobalProcedure`, `TreatmentRecord`, `Billing`, `BillingLineItem`, `Payment`, `LOA`, `PatientProvider`, `PatientFileAttachment`

### Audit Log Entity Coverage Added

`PatientFileAttachment` (FR-11.2)

### API Endpoints

- `GET /api/patients/{id}/dental-chart`, `GET /api/patients/{id}/teeth/{toothNumber}`
- `POST /api/patients/{id}/teeth/{toothNumber}/conditions` (append-only)
- `GET /api/treatments`, `POST /api/treatments`, `PUT /api/treatments/{id}`, `DELETE /api/treatments/{id}`
- `GET /api/treatments/applicable?condition={code}` (filter, not gate — FR-05.4.1)
- `POST /api/appointments/{id}/procedures` (per-tooth), `POST /api/appointments/{id}/global-procedures`
- `GET /api/appointments/{id}/billing`, `POST /api/billings`, `PUT /api/billings/{id}/finalize`
- `POST /api/billings/{id}/payments` (OR number uniqueness enforced at DB level — TECH_STACK_AND_ARCHITECTURE.md §8.8)
- `DELETE /api/payments/{id}` (void — Administrator only)
- `GET /api/providers`, `POST /api/patients/{id}/providers`
- `GET /api/providers/{id}/tariff` (tariff look-up for LOA entry — FR-14.1)
- `POST /api/billings/{id}/loa`
- `GET /api/reports/rpt-{04,05,06,07,08,13,14}`, `GET /api/reports/or-register`
- PDF: `GET /api/billings/{id}/pdf/soa`, `GET /api/payments/{id}/pdf/or`, `GET /api/reports/or-register/pdf`
- `POST /api/patients/{id}/attachments`, `GET /api/patients/{id}/attachments`, `GET /api/attachments/{id}/download` (auth-gated; served via authenticated endpoint — not a public filesystem path), `DELETE /api/attachments/{id}` (requires reason body; audit-logged; **returns 403 if `IsSystemGenerated == true`** — system-generated PDFs may only be removed by voiding the parent prescription or certificate, not by direct deletion; FR-15.9, FR-16.7)

### Backend Tech Introduced

- **PdfSharpCore + MigraDoc** — OR PDF (BIR-compliant, FR-07.3.6), SOA PDF (FR-07.2.3), standard report PDFs, OR register PDF (FR-09.3.2)
- **echarts data endpoints** — revenue trend, appointment donut, top-5 treatments data returned as JSON for frontend chart rendering (FR-09.1.C–D)

### Backend Tests

- Unit: balance computation (computed on read — no stored column), OR UNIQUE constraint exception handler (§8.8), applicable-conditions filter (filter not gate), append-only tooth condition logic, billing status transitions
- Integration (Testcontainers): appointment → complete → billing generated → payment recorded → OR PDF returned; duplicate OR number returns 422 with user-readable message; voided payment excluded from balance; `GET /api/providers/{id}/tariff` returns tariff schedule; attachment upload → auth-gated download returns 401 without token; delete without `reason` returns 422

### Phase-End Acceptance Criteria

- `POST /api/patients/{id}/teeth/{tooth}/conditions` appends a new `ToothConditionEntry`; the previous active entry is deactivated but never deleted.
- `GET /api/treatments/applicable?condition=DCF` returns a filtered list; all active treatments remain accessible via the main treatments endpoint (filter is a suggestion, not a gate — FR-05.4.1).
- `POST /api/billings/{id}/payments` with a duplicate OR number returns 422 with message "OR number is already recorded in the system" (DB UNIQUE constraint — TECH_STACK_AND_ARCHITECTURE.md §8.8).
- `GET /api/billings/{id}` returns `OutstandingBalance` computed as `PatientShare − TotalPaid`; no `Balance` column exists in the database (NFR-R04).
- `GET /api/providers/{id}/tariff` returns the provider tariff schedule for HMO LOA entry (FR-14.1).
- OR PDF returns a well-formed document with clinic name, TIN, OR number, patient name, amounts, and payment method.
- RPT-04 through RPT-14 (Phase 3A subset) respond within 5 s for a 12-month range on seeded data (NFR-P04).
- `GET /api/attachments/{id}/download` returns 401 without a valid JWT; returns the file with a correct `Content-Disposition` header with a valid token.
- `DELETE /api/attachments/{id}` without a `reason` field in the request body returns 422; with a reason it succeeds and creates an audit log entry.
- `DELETE /api/attachments/{id}` on a record where `IsSystemGenerated == true` returns 403 with message "System-generated attachments cannot be deleted directly. Void the related prescription or certificate instead." (FR-15.9, FR-16.7)
- Files up to 20 MB are accepted; Nginx `client_max_body_size 25M` must be confirmed configured (TECH_STACK_AND_ARCHITECTURE.md §5).

---

## Phase 3B — Frontend: Clinical Core & Financial Operations

### Goal

Dentists can chart conditions, record procedures, and complete visits. Staff can generate billings, record payments, print BIR-compliant ORs, and handle HMO-covered visits — all through the UI.

### Frontend Tech Introduced

- **echarts + vue-echarts** — revenue trend bar chart, appointment status donut, top-5 treatments bar chart (FR-09.1.C–D)
- **Custom SVG dental chart components** — `DentalChart.vue`, `Tooth.vue`, `ToothConditionLegend.vue`, `ToothDetailPanel.vue` (FR-05.1, NFR-U05)

### Pages & Components

| Component                       | Description                                                                                                                                                         |
| ------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `DentalChart.vue`               | SVG FDI chart (52 teeth, 4 quadrants — Q1 top-right, Q2 top-left, Q3 bottom-left, Q4 bottom-right); condition-coded fill and symbol; selects tooth for detail panel |
| `Tooth.vue`                     | Individual tooth: 5 SVG surface paths (B, L, M, D, O); condition → fill color + pattern (e.g., DCF = outlined circle)                                               |
| `ToothConditionLegend.vue`      | Always-visible 19-condition legend with symbols                                                                                                                     |
| `ToothDetailPanel.vue`          | Condition history timeline, remarks, linked procedure records                                                                                                       |
| `TreatmentCatalogView.vue`      | Treatment list with applicable/resulting conditions, pricing, procedure kit builder                                                                                 |
| `AppointmentProceduresView.vue` | In-progress appointment: tooth selection → applicable-treatment filter panel (suggestion only, not a gate) → add per-tooth procedure; global procedures panel       |
| `BillingView.vue`               | Billing detail: line items, HMO share, patient share, computed outstanding balance; Draft → Final actions                                                           |
| `LOAFormDialog.vue`             | LOA number + authorized amount entry; "View Provider Tariff" read-only look-up panel (FR-14.1)                                                                      |
| `PaymentFormDialog.vue`         | Payment method selection, amount, manual OR number field, change computation for cash                                                                               |
| `OfficialReceiptView.vue`       | Read-only receipt preview; Print / Download PDF actions                                                                                                             |
| `ORRegisterView.vue`            | Sequential OR list; voided ORs flagged; PDF/CSV export                                                                                                              |
| `PatientAttachmentsTab.vue`     | Reverse-chron attachment list; upload (JPEG/PNG/PDF up to 20 MB); label + note; auth-gated download; delete with mandatory reason confirmation dialog — available from Phase 3B so X-rays are accessible during clinical charting and oral surgery visits |
| `ReportViewer.vue`              | Shared report shell: date-range picker, filters, DataTable, Print / Export PDF                                                                                      |
| `DashboardView.vue` (extended)  | Revenue trend bar chart, appointment donut, top-5 treatments chart (echarts) added to Phase 2B dashboard shell                                                      |

### Phase-End Acceptance Criteria

- The SVG dental chart renders all 52 FDI teeth with correct quadrant layout and condition-coded fills.
- Selecting a tooth with an active condition shows a filtered treatment suggestion list first; scrolling or clearing the filter reveals all active treatments — the filter never blocks selection (FR-05.4.1).
- Marking an appointment as Completed locks all procedure fields to read-only.
- The billing screen shows subtotal, HMO coverage amount, patient share, and computed outstanding balance; the balance updates immediately when a payment is added without a page reload.
- The LOA entry form shows the "View Provider Tariff" look-up panel for the patient's assigned provider before the authorized amount is manually entered (FR-14.1).
- Printing an OR opens a PDF matching the BIR format (clinic name, TIN, OR number, amounts, payment method).
- The OR Register lists ORs sequentially; voided ORs display a "Voided" label next to their number.
- RPT-04 (Revenue), RPT-06 (AR), and RPT-13 (HMO Claims Summary) render correctly and export as PDF.
- X-ray images (JPEG/PNG) can be uploaded to a patient’s Attachments tab from the Clinical Core; the file appears in the list immediately and is downloadable by authenticated clinic staff. Accessing the file without a valid session token returns an authentication error.

---

## Phase 4A — Backend: Inventory, Reports & Notifications

### Goal

Inventory tracking, all 14 standard reports, Hangfire background job infrastructure, SMS/email delivery, and bulk export APIs are fully functional.

### BRD Coverage

| Module                            | Subsections / Items Included                                                                                                                                                                                                                                                                                                                         |
| --------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **FR-08: Inventory**              | FR-08.1 (supply item catalog: categories, units, critical quantity, Tracked vs. Bulk-Managed tier), FR-08.2 (stock ledger: receipts, adjustments, lot numbers, expiry dates), FR-08.3 (consumption during treatment: optional per-appointment, procedure kit auto-suggestion, stock warning, weekly adjustment workflow), FR-08.4 (supplier catalog) |
| **FR-02 (additions)**             | FR-02.10 (SMS gateway config: Semaphore / Globe Labs / ITEXMO), FR-02.11 (SMTP email config)                                                                                                                                                                                                                                                         |
| **FR-10: Notifications & Alerts** | FR-10.1 – FR-10.9 (in-app badges, appointment confirmation / reschedule / cancellation SMS+email, 24-hour reminder, async Hangfire delivery, 3-retry with exponential backoff, per-patient notification preferences)                                                                                                                                 |
| **FR-09.1 (completion)**          | FR-09.1.F (low-stock and near-expiry dashboard alerts via SignalR; infrastructure introduced Phase 2A)                                                                                                                                                                                                                                               |
| **FR-09.2 (completion)**          | RPT-02 (Patient Masterlist), RPT-03 (Patient Treatment History), RPT-09 (Inventory Status), RPT-10 (Inventory Consumption), RPT-11 (No-Show & Cancellation), RPT-12 (New Patient Acquisition) — all remaining standard reports                                                                                                                       |

### Business Rules

- BR-05.1 – BR-05.5 (inventory rules: computed stock, expiry/lot requirements, over-consumption warning, dashboard alerts)
- BR-07.6 (dentist/staff reschedule → mandatory patient notification)
- BR-07.10 (notification delivery failure does not roll back the appointment action)

### Non-Functional Requirements

- NFR-R05 (automated daily database backups, configurable retention)
- NFR-P05 (10 concurrent users without degradation)
- NFR-M04 (structured JSON logging via Serilog)

### New Database Entities

`SupplyItem`, `SupplyStockLedger`, `SupplierCatalog`, `ProcedureConsumption`, `NotificationLog`

### API Endpoints

- `GET /api/inventory`, `POST /api/inventory`, `PUT /api/inventory/{id}`
- `POST /api/inventory/{id}/receive`, `POST /api/appointments/{id}/consumption`
- `GET /api/suppliers`, and full CRUD
- `GET /api/clinic-settings/sms`, `PUT`; `GET /api/clinic-settings/smtp`, `PUT`
- `GET /api/notifications/{patientId}/preferences`, `PUT`
- `GET /api/reports/rpt-{02,03,09,10,11,12}`

### Backend Tech Introduced

- **Hangfire** — background job server: async notification delivery (3-retry exponential backoff, FR-10.7–10.8); `LowStockCheckJob` (every 6 h); `NearExpiryCheckJob` (daily 06:00); `AppointmentReminderJob` (daily SMS/email); installment status job stub (activated Phase 5A); recall reminder job stub (activated Phase 5A); bulk export job stub (activated Phase 6A); 15-min slot-release job stub (activated Phase 7A)
- **MailKit** — SMTP email for appointment lifecycle (FR-10.3–10.6); SMTP credentials stored per NFR-S06; portal email flows (FR-12.1.4, FR-12.2.3) activated Phase 6A
- **SMS gateway HTTP client** — generic HTTP REST adapter; configurable for Semaphore / Globe Labs / ITEXMO (FR-02.10); all patient SMS notifications (FR-10.3–10.10)

### Backend Tests

- Unit: stock balance always `SUM(ledger entries)` — no stored column; Hangfire retry policy; notification preference gate
- Integration (Testcontainers): receive stock → ledger entry; consume on appointment → stock decremented; low-stock check pushes to `DashboardHub`; notification delivered and logged; failed notification retried 3× then flagged

### Phase-End Acceptance Criteria

- Stock balance is always `SUM(SupplyStockLedger)` — no `CurrentStock` column in the database.
- An Anesthetic or Medication receipt without lot number and expiry date returns 422.
- Items at or below critical quantity appear in `GET /api/dashboard/widgets` with a badge count pushed via SignalR.
- All 14 standard report endpoints (RPT-01 through RPT-14) respond within 5 s on seeded data (NFR-P04).
- Appointment confirmation triggers a Hangfire notification job; `NotificationLog` records Sent or Failed.
- Failed notification is retried 3× with exponential backoff; final failure creates an admin-reviewable flag.

---

## Phase 4B — Frontend: Inventory, Reports & Notifications

### Goal

Staff can manage inventory through the UI. All 14 standard reports are viewable and exportable. Administrators can configure SMS/email gateways, view notification delivery status, and trigger bulk data exports.

### Pages & Components

| Component                           | Description                                                                                                                           |
| ----------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------- |
| `InventoryListView.vue`             | Supply item catalog; Tracked vs. Bulk-Managed tier badges; critical quantity indicator; low-stock alert badge (real-time via SignalR) |
| `InventoryFormDialog.vue`           | Create / edit supply item: category, unit, critical quantity, tier                                                                    |
| `ReceiveStockDialog.vue`            | Stock receipt: quantity, lot number, expiry date, supplier; lot/expiry required for Anesthetic/Medication                             |
| `ConsumeStockDialog.vue`            | Appointment-linked consumption entry; over-consumption warning                                                                        |
| `SupplierView.vue`                  | Supplier catalog CRUD                                                                                                                 |
| `NotificationLogView.vue`           | Per-appointment/patient delivery log; Sent / Failed / Retrying status                                                                 |
| `NotificationPreferencesDialog.vue` | Per-patient SMS/email opt-in per event type (FR-10.9)                                                                                 |
| `SMSGatewaySettingsView.vue`        | Gateway selection, API key input (masked after save), test-send button                                                                |
| `ReportViewer.vue` (extended)       | Remaining reports added: RPT-02, RPT-03, RPT-09, RPT-10, RPT-11, RPT-12                                                               |
| `DashboardView.vue` (extended)      | Low-stock and near-expiry badge widgets added; real-time via SignalR (FR-09.1.F)                                                      |

### Phase-End Acceptance Criteria

- Receiving stock adds an entry to the supply ledger; the stock count on the inventory list updates immediately.
- Receiving Anesthetic or Medication without lot number / expiry shows an inline error before save.
- The low-stock badge on the dashboard shows the correct count and links to the inventory page; badge updates in real time without page reload (SignalR).
- All 14 standard reports (RPT-01 through RPT-14) are viewable, filterable, printable, and PDF-exportable.
- The SMS gateway settings screen has a "Send Test SMS" button; the result (Sent / Error) is shown inline; API key is masked after save.
- End-to-end SMS delivery is verified against **at least one live Philippine SMS gateway** (Semaphore, Globe Labs, or ITEXMO) for every notification event type (confirmation, reschedule, cancellation, 24-hour reminder); gateway latency is measured; the Hangfire retry policy recovers from transient timeouts; gateway onboarding instructions are documented before Phase 4B is signed off (FR-02.10, FR-10.7–10.8).

---

## Phase 5A — Backend: Specialized Clinical Modules

### Goal

Prescription, medical certificate, orthodontic progress note, and installment payment plan APIs are production-ready. Recall reminder and installment reminder Hangfire jobs are activated. Prescription/certificate PDF generation auto-saves System-Generated entries into the attachment infrastructure introduced in Phase 3A.

### BRD Coverage

| Module                                  | Subsections / Items Included                                                                                                                                                                                                                                                       |
| --------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **FR-03.7 (System-Generated entries only)** | Prescription and certificate PDFs are auto-saved as System-Generated `PatientFileAttachment` records using the attachment upload infrastructure introduced in Phase 3A. No new API endpoints or entities are required for this phase. |
| **FR-06.4: Orthodontic Progress Notes** | FR-06.4.1 – FR-06.4.6 (triggered by Orthodontic-category appointment; structured fields: upper/lower wire, elastics, bracket changes, appliance status; Orthodontic History tab)                                                                                                   |
| **FR-07.5: Installment Payment Plans**  | FR-07.5.1 – FR-07.5.8 (plan creation on a billing, configurable schedule, Hangfire reminders N days before due date, waive installment, match payments to earliest unpaid installment)                                                                                             |
| **FR-02.15**                            | Installment plan reminder lead-time setting in Clinic Settings                                                                                                                                                                                                                     |
| **FR-15: Electronic Prescription**      | FR-15.1 – FR-15.11 (Dentist/Admin only, linked to patient + optional appointment, drug items, PRC/PTR/S2 auto-fill, blank wet-signature line, no digital signature image, read-only after save, void with reason, PDF auto-saved to Attachments as System-Generated, audit-logged) |
| **FR-16: Medical / Dental Certificate** | FR-16.1 – FR-16.9 (same governance as prescription; selectable cert title, diagnosis/procedure/rest period, void with reason, PDF auto-saved to Attachments as System-Generated)                                                                                                   |
| **FR-10.10: Recall Reminders**          | Hangfire daily job activated; configurable 6-month threshold; 30-day re-send guard; respects per-patient opt-out (FR-10.9); SMS + email delivery. **Edge case:** patients with zero completed appointments use `RegistrationDate` as the threshold baseline instead of `LastCompletedAppointmentDate`. |

### Business Rules

- BR-02.11 (installment payment matching to earliest unpaid entry)
- BR-02.13 (waived installment excluded from balance but does not reduce TotalAmount)
- BR-03.6 (applicable conditions — already enforced Phase 3A; ortho procedures follow same filter-not-gate rule)
- BR-10.1 – BR-10.6 (prescription/certificate: wet-signature only, no digital image, PRC/S2 warnings, system-generated PDFs in Attachments)

### New Database Entities

`OrthoProgressNote`, `InstallmentPlan`, `InstallmentEntry`, `Prescription`, `PrescriptionItem`, `MedicalCertificate`

### Audit Log Entity Coverage Added

`Prescription`, `MedicalCertificate`, `OrthoProgressNote` (FR-11.2)

### API Endpoints

- `GET /api/appointments/{id}/ortho-note`, `POST /api/appointments/{id}/ortho-note`
- `POST /api/billings/{id}/installment-plan`, `GET /api/billings/{id}/installment-plan`, `PUT /api/installment-entries/{id}/waive` (requires reason)
- `POST /api/patients/{id}/prescriptions`, `GET /api/patients/{id}/prescriptions`, `DELETE /api/prescriptions/{id}` (void with reason)
- `GET /api/prescriptions/{id}/pdf`
- `POST /api/patients/{id}/certificates`, `GET /api/patients/{id}/certificates`, `DELETE /api/certificates/{id}` (void with reason)
- `GET /api/certificates/{id}/pdf`

### Backend Tech Introduced (Extensions Only)

- **PdfSharpCore + MigraDoc** (introduced Phase 3A): generates prescription PDFs (FR-15.9) and Medical Certificate PDFs (FR-16.7); both auto-saved as System-Generated `PatientFileAttachment` entries
- **Hangfire** (introduced Phase 4A): `InstallmentReminderJob` (N days before each due date, FR-07.5.6) and `RecallReminderJob` (daily, FR-10.10) activated

### Backend Tests

- Unit: installment plan payment matching to earliest unpaid entry; waived installment excluded from balance; recall 30-day re-send guard logic; recall threshold uses `RegistrationDate` when patient has zero completed appointments
- Integration (Testcontainers): prescription created → System-Generated attachment auto-saved to the existing attachment store (Phase 3A infrastructure); void prescription with reason returns 200; installment Hangfire jobs fire on correct schedule

### Phase-End Acceptance Criteria

- `POST /api/patients/{id}/prescriptions` creates a prescription and immediately creates a System-Generated `PatientFileAttachment` with label matching "Prescription — [date] — Dr. [name]".
- Neither the prescription nor the certificate PDF embeds or renders a dentist signature image (BR-10.1).
- `POST /api/billings/{id}/installment-plan` creates the plan and schedules Hangfire reminder jobs for each installment due date.
- `RecallReminderJob` runs daily; no patient receives more than one recall SMS/email within the 30-day re-send window (FR-10.10).
- A patient with zero completed appointments whose `RegistrationDate` is ≥ 6 months ago is included in the recall batch; a patient registered less than 6 months ago is not.

---

## Phase 5B — Frontend: Specialized Clinical Modules

### Goal

Dentists can issue prescriptions and certificates through the UI. Orthodontic visit notes are structured. Staff can create and manage installment payment plans. The Attachments tab (introduced Phase 3B) now also receives System-Generated PDF entries from prescriptions and certificates.

### Pages & Components

| Component                   | Description                                                                                                                               |
| --------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------- |
| `OrthoProgressNoteView.vue` | Structured ortho fields (wire, elastics, brackets, appliance); Orthodontic History tab; appears only on Orthodontic-category appointments |
| `InstallmentPlanView.vue`   | Plan creation form (schedule, amounts); installment entry list with status (Unpaid/Paid/Waived); waive action with mandatory reason       |
| `PrescriptionFormView.vue`  | Dentist/Admin only: patient link, drug items, dosage, PRC/PTR/S2 auto-fill, blank signature line label; submit → read-only                |
| `PrescriptionListView.vue`  | Patient prescription history; Download PDF; Void with reason                                                                              |
| `CertificateFormView.vue`   | Dentist/Admin only: cert title selection, diagnosis/procedure/rest period, blank signature line label; submit → read-only                 |
| `CertificateListView.vue`   | Patient certificate history; Download PDF; Void with reason                                                                               |

### Phase-End Acceptance Criteria

- An Orthodontic-category appointment shows the Ortho Progress Note panel; a non-Orthodontic appointment does not.
- The prescription form is accessible to Dentist and Administrator roles only; Staff users do not see the "New Prescription" button (route meta role guard + API enforcement).
- After saving a prescription, all fields are read-only; the Void button requires a reason.
- Prescription and certificate PDFs render correctly and do not show a dentist signature image (BR-10.1).
- A new System-Generated attachment appears in the Attachments tab immediately after a prescription or certificate PDF is generated.
- An installment plan on a billing shows each entry with due date, amount, and status; waiving an entry requires a reason and updates the installment list immediately.
- Patients due for recall (no appointment for ≥ 6 months) appear in the Administrator recall report view.

---

## Phase 6A — Backend: Patient Portal API

### Goal

All portal-specific API endpoints are production-ready: portal authentication with audience isolation, self-registration, online booking (without deposit), dependent management, feedback, and ratings.

### BRD Coverage

| Module                                       | Subsections / Items Included                                                                                                                                                                                                                    |
| -------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **FR-02 (additions)**                        | FR-02.13 (booking approval mode: Immediate / Awaiting Approval), FR-02.16 (online booking appointment types: label, description, duration, required specialization tag)                                                                         |
| **FR-12.1: Portal Accounts**                 | FR-12.1.1 – FR-12.1.6 (self-registration, email verification, portal patient record, staff invitation link), FR-12.1.7 – FR-12.1.8 (Dependent / family accounts, Primary Account Holder session governs access, max 10 dependents configurable) |
| **FR-12.2: Portal Authentication**           | FR-12.2.1 – FR-12.2.4 (email+password login, separate `portal` JWT audience, 5-attempt lockout, staff unlock + password reset email, cross-patient IDOR rejection)                                                                              |
| **FR-12.3: Self-Registration Intake**        | FR-12.3.1 – FR-12.3.5 (guided medical/dental history intake, Patient-Submitted status, staff review merge flow, Pending Staff Review indicator, contact-only self-edit after confirmation)                                                      |
| **FR-12.4: Online Booking (Phase 6A scope)** | FR-12.4.1 – FR-12.4.5, FR-12.4.9 – FR-12.4.10 (booking calendar, appointment type selection, conflict check, approval mode handling, confirmation email, staff in-app flag) — deposit flow (FR-12.4.6–12.4.8) is Phase 7A                       |
| **FR-12.5 – FR-12.9**                        | My Appointments (view, cancel, atomic reschedule), My Treatment History, My Billing & Payment Summary, Dentist Ratings (7-day edit window, rating anonymity), Patient Feedback (lifecycle: New → Under Review → Resolved → Closed)              |
| **FR-10.9 (portal-side)**                    | Notification preference controls                                                                                                                                                                                                                |
| **FR-13: Bulk Data Export**                  | FR-13.1 – FR-13.9 (bulk CSV/XLSX export via Hangfire, time-limited 24 h secure download link, configurable date-range scope, audit-logged) — deferred from Phase 4 to this phase because the export must include Installment Plans, Prescriptions, Medical Certificates, and Ortho Progress Notes which are not available until Phase 5A. The export ZIP must contain a separate CSV for each of the following entity types: Patients, Appointments, Treatment Records, Billings, Payments, Installment Plans, Prescriptions, Medical Certificates, and Ortho Progress Notes (FR-13.2). |

### Business Rules

- BR-01.7 – BR-01.8 (Awaiting Approval; Rejected is terminal)
- BR-07.1 – BR-07.13 (all portal rules: `portal` audience isolation, patient-scope IDOR enforcement, reschedule atomicity, rating anonymity, complaint acknowledgment, deposit forfeiture scaffolded for Phase 7A)
- BR-06.3 – BR-06.4 (bulk export — Administrator only; 10-year retention before purge)
- BR-09.1 – BR-09.4 (export rules: Administrator-only, audit-logged, no secrets in exports, 24 h link expiry)

### New Database Entities

`PortalAccount`, `PatientDependentLink`, `PortalAccountLockout`, `DentistRating`, `PatientFeedback`, `DataExportJob`

### API Endpoints (portal JWT audience — rejected by clinic-staff endpoints; NFR-S03, BR-07.2)

- `POST /api/portal/auth/register`, `/verify-email`, `/login`, `/logout`, `/forgot-password`, `/reset-password`
- `GET /api/portal/accounts/me`, `PUT /api/portal/accounts/me` (contact info only after staff confirmation)
- `POST /api/portal/accounts/me/dependents`, `GET`, `DELETE`
- `GET /api/portal/booking/calendar?date=&appointmentType=`
- `POST /api/portal/booking/appointments` (conflict check; Awaiting Approval or Pending per FR-02.13)
- `GET /api/portal/appointments`, `DELETE /api/portal/appointments/{id}` (cancel — accepted statuses: `Pending`, `Confirmed`, `AwaitingApproval`, `AwaitingDepositVerification`; FR-12.5.2 extended to include portal-only statuses)
- `POST /api/portal/appointments/{id}/reschedule` (atomic cancel + rebook)
- `GET /api/portal/treatment-history`
- `GET /api/portal/billing-summary`, `GET /api/portal/billing/{id}/pdf/or`, `GET /api/portal/billing/{id}/pdf/soa`
- `POST /api/portal/appointments/{id}/rating`, `PUT` (7-day edit window; server-side enforcement only)
- `GET /api/portal/appointments/{id}/rating-status` — returns `{ canEditRating: bool, ratedAt: ISO8601 | null }`; the frontend must rely on this flag to render the Edit button and must not compute the 7-day window client-side (FR-12.8.7)
- `POST /api/portal/feedback`, `GET /api/portal/feedback` (own submissions)
- `GET /api/admin/portal-accounts` (staff unlock and management), `POST /api/admin/portal-accounts/{id}/unlock`
- `GET /api/admin/feedback` (staff feedback inbox with Complaint priority badge)
- `POST /api/exports` (enqueue Hangfire bulk export job — Administrator only; audit-logged), `GET /api/exports/{id}/download` (time-limited 24 h signed URL)

### Backend Tests

- Unit: `portal` audience rejection on clinic-staff endpoints and vice versa; dependent IDOR prevention; `canEditRating` returns `false` after 7 days and `true` within 7 days; reschedule atomicity rollback on conflict; cancellation accepted for `AwaitingApproval` and `AwaitingDepositVerification` statuses; bulk export job enqueued and download link expires after 24 h
- Integration (Testcontainers): full portal registration → email verification → login → booking → confirmation email; dependent booking under primary account; reschedule rolls back cancellation if new slot taken; portal JWT rejected by `/api/users` endpoint (clinic-staff only); bulk export job completes and includes rows from all entities through Phase 5A; `GET /api/exports/{id}/download` returns 410 after 24 h

### Phase-End Acceptance Criteria

- A portal JWT with `portal` audience is rejected by all clinic-staff API endpoints (BR-07.2); a clinic-staff JWT is rejected by all portal API endpoints.
- A Primary Account Holder can add a dependent; booking with `patientId = dependentId` succeeds; attempting to book for another account holder's patient returns 403.
- A portal booking in Awaiting Approval mode does not create a time-slot conflict; a conflicting staff booking forces staff to reject the portal request.
- `POST /api/portal/appointments/{id}/reschedule` rolls back the cancellation if the new slot is taken (returns 409).
- `POST /api/portal/appointments/{id}/rating` is rejected after the 7-day edit window closes (server-side check — `canEditRating: false` returned by `GET /api/portal/appointments/{id}/rating-status`).
- A Complaint-type feedback submission is flagged in `GET /api/admin/feedback` with priority indicator.
- `GET /api/portal/billing/{id}/pdf/or` returns 403 if the authenticated portal user does not own that billing (IDOR enforcement).
- `DELETE /api/portal/appointments/{id}` returns 200 for appointments in `AwaitingApproval` and `AwaitingDepositVerification` statuses in addition to `Pending` and `Confirmed` — patients must be able to cancel at any pre-visit stage (FR-12.5.2).
- `POST /api/exports` enqueues the bulk export job; `GET /api/exports/{id}/download` returns the ZIP file within the 24 h window containing CSV rows for all entities through Phase 5A; requests after expiry return 410 Gone.

---

## Phase 6B — Frontend: Patient Portal SPA

### Goal

Patients have a public-facing web application for self-registration, online booking, treatment history, billing, feedback, and family account management.

### Architecture Note

The Patient Portal is a **separate Vue 3 SPA** project (`frontend/allaboutteeth-portal/`) within the repository. It shares the same ASP.NET Core API via a distinct JWT audience (`portal`). It reuses PrimeVue, Pinia, vue-router, Tailwind CSS, FullCalendar (booking calendar), echarts (rating distribution), and the SignalR JS client.

### Frontend Tech Introduced

- **Patient Portal SPA** — separate `frontend/allaboutteeth-portal/` Vue 3 + Vite + TypeScript project; built and deployed independently from the clinic SPA
- **`patient.store.ts` — `activePatientId` field** — tracks which patient context is active (Primary Account Holder's own record or a Dependent's record). When the user switches to a Dependent, `activePatientId` is explicitly updated before any API call. Reset to the Primary Account Holder's own `patientId` on fresh login and on logout (FR-12.1.7; TECH_STACK_AND_ARCHITECTURE.md §3 State Management)

### Pages & Components

| Component                           | Description                                                                                                    |
| ----------------------------------- | -------------------------------------------------------------------------------------------------------------- |
| `PortalLoginView.vue`               | Email + password, forgot-password link, "Create Account" CTA                                                   |
| `PortalRegisterView.vue`            | Self-registration: basic details, email verification step                                                      |
| `IntakeFormView.vue`                | Guided medical/dental history intake (FR-12.3); submits as Patient-Submitted record                            |
| `PortalDashboardView.vue`           | Upcoming appointments, notification count, family context switcher                                             |
| `FamilySwitcherDropdown.vue`        | Switches `activePatientId`; shows active patient name in top bar; all subsequent API calls use the new context |
| `ManageDependentsView.vue`          | Add/remove dependent profiles (up to 10 per FR-12.1.7)                                                         |
| `OnlineBookingView.vue`             | FullCalendar booking calendar; appointment type selection; conflict feedback; Awaiting Approval mode message   |
| `MyAppointmentsView.vue`            | Upcoming and past appointment list; Cancel and Reschedule actions                                              |
| `MyTreatmentHistoryView.vue`        | Plain-language completed procedure history; no FDI codes (FR-12.6); date-range and dentist filter              |
| `MyBillingView.vue`                 | Billing list with patient-friendly statuses; Download OR PDF; Download SOA PDF                                 |
| `DentistRatingDialog.vue`           | 1–5 star + comment; post-completion prompt; **Edit button visibility driven solely by `canEditRating` flag from `GET /api/portal/appointments/{id}/rating-status`** — the frontend never computes the 7-day window independently (FR-12.8.7) |
| `FeedbackView.vue`                  | Submit feedback (type selection); status timeline (New / Under Review / Resolved / Closed)                     |
| `PortalNotificationPreferences.vue` | SMS/email opt-in per event type (FR-10.9)                                                                      |

### Clinic SPA Additions (Phase 6B scope — `allaboutteeth-web`)

The following view is added to the clinic SPA during Phase 6B because its backend dependency (FR-13 bulk export API) first becomes available in Phase 6A. It is part of the clinic SPA, not the patient portal SPA.

| Component | Description |
| --------- | ----------- |
| `DataExportView.vue` | Administrator-only: trigger bulk export; progress indicator; download button when ready; 24 h expiry countdown; shows which entity types are included in the export |

### Phase-End Acceptance Criteria

- A new patient completes self-registration, receives a verification email, activates the account, and submits the guided intake; the clinic dashboard shows the record flagged "Pending Staff Review."
- The Family Switcher dropdown updates `activePatientId` in `patient.store.ts`; all subsequent booking and history API calls use the switched patient context; the top bar shows the active patient name.
- A Primary Account Holder cannot view or book on behalf of another account holder's patient (IDOR — 403 returned and shown as a friendly error).
- The online booking calendar shows available slots; selecting a taken slot shows a conflict message before submission.
- After a completed appointment a rating prompt appears; the "Edit Rating" button is shown or hidden based exclusively on the `canEditRating` boolean returned by the backend — the Vue component does not calculate dates client-side.
- A Complaint submission shows "Your complaint has been received" with a reference; the patient sees status updates when staff progress the ticket.
- A portal JWT is not accepted by any clinic-staff-facing endpoint (verified by attempting a staff-only action from the portal app).
- Downloading an OR or SOA PDF from the billing view opens the correct document belonging to the authenticated portal user.
- In the clinic SPA, a triggered bulk data export shows a progress indicator; a download button appears when the file is ready; the download link shows an expiry countdown; the link returns a "Link Expired" message after 24 hours.

---

## Phase 7A — Backend: Online Booking Deposits

### Goal

Payment gateway integration, deposit command, Valkey slot reservation, Hangfire slot-release timeout job, and manual deposit verification APIs are production-ready.

### BRD Coverage

| Module                    | Subsections / Items Included                                                                                                                                                                                                  |
| ------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **FR-02.12**              | Payment gateway config (PayMongo / Maya / Paynamics; API keys AES-encrypted at rest; sandbox/production toggle; connection-test button)                                                                                       |
| **FR-02.12a**             | Manual deposit verification config (toggle, instruction text, receipt upload; can coexist with gateway)                                                                                                                       |
| **FR-02.14**              | Deposit amount setting (PHP; 0 disables feature), label, forfeiture policy text shown to patient before payment                                                                                                               |
| **FR-12.4.6 – FR-12.4.8** | **Gateway path**: 15-min Valkey slot reservation → Awaiting Payment status → auto-release by `SlotReleaseJob` on timeout or payment failure. **Manual path**: receipt upload sets `AwaitingDepositVerification` status with **no Valkey TTL and no time limit** — the slot is held indefinitely until staff Verify or Reject; `SlotReleaseJob` is not triggered on the manual path. Deposit applied to final billing; forfeiture / refund recording. |
| **FR-12.4.6a**            | Staff inbox: Awaiting Deposit Verification queue; Verify and Reject actions; patient notified on rejection                                                                                                                    |
| **FR-12.4.6b**            | Uploaded receipt image stored on appointment record; accessible to Admin/Staff only; not returned by patient-facing API                                                                                                       |

### Business Rules

- BR-02.12 (deposit recorded as Deposit payment type; AR issued immediately; BIR OR at visit completion; AR number and OR number stored on deposit payment record)
- BR-07.12 – BR-07.13 (deposit forfeiture and refund recording; forfeited amounts as revenue)

### New Database Entities

`BookingDepositReceipt`, `AcknowledgmentReceipt`

### API Endpoints

- `POST /api/portal/booking/appointments/{id}/deposit/gateway` — initiates gateway payment; sets Valkey slot lock; returns redirect URL
- `POST /api/portal/booking/appointments/{id}/deposit/manual` — receipt image upload; sets Awaiting Deposit Verification
- `POST /api/webhooks/payment-gateway` — webhook receiver; **validates gateway signature before trusting any payload** (NFR-S04); confirms or fails deposit
- `GET /api/admin/deposit-verifications` — staff inbox (Awaiting Deposit Verification queue)
- `POST /api/admin/deposit-verifications/{id}/verify`, `/reject`
- `GET /api/clinic-settings/gateway`, `PUT` (API keys AES-encrypted at rest — NFR-S05; raw key never returned in response or logs — NFR-S08)

### Backend Tech Introduced

- **Payment gateway HTTP client** (PayMongo / Maya / Paynamics) — outbound deposit initiation (FR-02.12); webhook signature validation required before processing any payload (NFR-S04); API keys AES-encrypted at rest (NFR-S05, NFR-S06) and never appear in logs (NFR-S08)
- **Valkey 8 (slot reservation, gateway path only)** — extends the existing Valkey instance (running since Phase 1A) with a new key namespace for 15-min TTL slot locks on **gateway payments only** (FR-12.4.6); lock TTL auto-expires; also released by `SlotReleaseJob` (activated this phase) or on successful webhook confirmation. **Manual deposit path does not set a Valkey key** — `AwaitingDepositVerification` holds the slot in the database with no expiry until staff action (TECH_STACK_AND_ARCHITECTURE.md §4 Caching & Session).

### Backend Tests

- Unit: webhook signature validation (tampered payloads rejected); slot-lock TTL calculation; deposit forfeiture revenue categorisation; gateway API key encryption/decryption round-trip
- Integration (Testcontainers + Valkey): gateway deposit initiated → Valkey key set with ≤ 15-min TTL; `SlotReleaseJob` fires on timeout → appointment not committed; successful webhook → slot released + deposit payment recorded + AR generated; manual upload → no Valkey key set → slot held indefinitely → Verify → Pending; Reject → notification sent + slot released

### Phase-End Acceptance Criteria

- `POST /api/portal/booking/appointments/{id}/deposit/gateway` sets a Valkey slot-lock key with ≤ 15-min TTL and returns a gateway redirect URL.
- A webhook request with an invalid gateway signature returns 400 and is logged as a security warning; no state change occurs (NFR-S04).
- If no payment webhook arrives within 15 minutes, `SlotReleaseJob` fires, deletes the Valkey lock, and the appointment record is not committed. **`SlotReleaseJob` is not triggered for manual deposits.**
- Manual deposit path: `POST /api/portal/booking/appointments/{id}/deposit/manual` sets `AwaitingDepositVerification` status; **no Valkey key is created**; the slot remains held until staff Verify (advances to Pending) or Reject (slot released + patient notified).
- `GET /api/clinic-settings/gateway` returns only the last 4 characters of the API key masked; raw key never appears in any response or log (NFR-S08).
- A verified manual deposit generates an `AcknowledgmentReceipt`, emails the patient, and creates a System-Generated attachment.
- The deposit is applied to the billing as the first payment when the visit is completed.
- Forfeited deposit amounts appear in RPT-04 (Revenue Report) in the correct revenue category.

---

## Phase 7B — Frontend: Online Booking Deposits UI

### Goal

The patient portal's booking flow includes the deposit step. Staff have a verification inbox for manual receipt uploads. Administrators can configure and test the payment gateway.

### Pages & Components

| Component                          | Description                                                                                                                                          |
| ---------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- |
| `DepositStepView.vue`              | Post-booking deposit step: gateway option (redirect) or manual option (upload receipt); forfeiture policy disclosure shown before payment (FR-02.14) |
| `AwaitingPaymentView.vue`          | 15-min countdown timer for slot reservation; "Complete Payment" redirect link; slot-expired fallback message                                         |
| `DepositVerificationInboxView.vue` | Staff: paginated queue of Awaiting Deposit Verification appointments; receipt image preview; Verify / Reject actions                                 |
| `GatewaySettingsView.vue`          | Admin: gateway selection, API key entry (masked after save), sandbox/production toggle, "Test Connection" button                                     |
| `DepositConfirmationView.vue`      | Post-payment confirmation: booking confirmed, AR PDF download link, next steps                                                                       |

### Phase-End Acceptance Criteria

- After booking (with deposit enabled), the patient sees the deposit step with the forfeiture policy disclosure before proceeding.
- Gateway path: patient is redirected to the gateway; a 15-min countdown is visible; on payment success the booking confirmation page shows the AR PDF download link.
- If the 15-min timer expires without payment the patient sees "Slot no longer reserved — please book again."
- Manual path: receipt image upload succeeds; patient sees "Awaiting staff verification"; status updates when staff verify or reject.
- Staff deposit verification inbox shows the receipt image preview and Verify / Reject buttons; Reject sends a notification to the patient.
- Gateway API key field shows only masked characters after initial save; "Test Connection" returns success or error inline without revealing the full key.
- Forfeited deposit shows "Deposit Forfeited" on the patient's appointment history with the forfeiture date.
- A patient who submitted a manual deposit receipt but has not yet been verified can cancel the appointment from the portal (`AwaitingDepositVerification` is an accepted cancel status per Phase 6A); cancellation releases the slot and discards the receipt; the patient receives a cancellation confirmation.

---

## Phase 8 — System & Integration Testing

### Goal

All integrated components are tested end-to-end in the staging environment. No critical or high-severity defects remain open before UAT begins.

### Activities

| Activity                       | Description                                                                                                                                                                                     |
| ------------------------------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| API integration test sweep     | All API endpoints exercised against staging PostgreSQL + Valkey; response codes, payloads, and error messages verified                                                                          |
| E2E browser tests (Playwright) | Automated scripts covering all critical-path user journeys (see below)                                                                                                                          |
| Performance / load test        | 10 concurrent users for 5 minutes (NFR-P05); report generation under 5 s (NFR-P04)                                                                                                              |
| Security penetration test      | OWASP Top 10 checklist: SQL injection (EF Core parameterised), XSS (Vue escaping + CSP header via Nginx), CSRF (SameSite cookies), IDOR (resource ownership checks), secrets in logs (log scan) |
| Regression sweep               | All Phase 0–7B acceptance criteria re-verified in staging                                                                                                                                       |

### Critical-Path E2E Scenarios (Playwright)

1. Staff registers patient → books appointment → dentist charts conditions → records procedures → completes appointment → staff bills → records payment → prints OR PDF
2. HMO patient: assigns provider → enters LOA with tariff look-up → applies coverage → prints SOA
3. Inventory: receives stock → appointment procedure consumes stock → low-stock badge appears on dashboard in real time
4. Portal booking: patient self-registers → book online → staff approves → confirmation email/SMS received
5. Online deposit (gateway): portal booking → gateway redirect → payment webhook → booking confirmed → deposit applied on billing at visit completion
6. Data export: Administrator triggers bulk export → downloads ZIP within 24 h time-limited link → link returns 410 after expiry

### Phase-End Acceptance Criteria

- All Phase 1A–7B acceptance criteria pass in the staging environment with no regressions.
- All 6 E2E Playwright critical-path scenarios pass with zero failures.
- No SQL injection, XSS, or IDOR vulnerabilities found in penetration test.
- RPT-04 (Revenue Report) generates within 5 s for a 12-month range with ≥ 10,000 seeded patient records (NFR-P04).
- Load test confirms no degradation at 10 concurrent users for 5 consecutive minutes (NFR-P05).
- All P1 (system-blocking) defects found during this phase are resolved before Phase 9 begins.

---

## Phase 9 — User Acceptance Testing (UAT)

### Goal

Actual clinic staff complete all critical-path scenarios in the staging environment and provide written sign-off before production deployment proceeds.

### Participants

Administrator, Dentist, and Staff roles — real users from the target clinic, not developers.

### UAT Scenarios

| Scenario                | Role                | Pass Condition                                                                                                                |
| ----------------------- | ------------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| Full patient journey    | Staff + Dentist     | Register → book → chart → treat → bill → pay → print OR — completed without developer assistance                              |
| HMO patient visit       | Staff               | Assign provider → enter LOA with tariff look-up → apply coverage → print SOA                                                  |
| Inventory management    | Staff               | Receive stock → record procedure consumption → verify low-stock badge on dashboard                                            |
| Prescription            | Dentist             | Issue prescription → PDF appears in Attachments tab; signature line is blank                                                  |
| Medical certificate     | Dentist             | Issue certificate → print PDF; no signature image                                                                             |
| Installment plan        | Staff               | Create plan on billing → view schedule → waive one entry with reason                                                          |
| Patient portal booking  | Patient (simulated) | Self-register → book online → receive confirmation SMS and email                                                              |
| Online deposit          | Patient (simulated) | Gateway deposit flow → booking confirmed → deposit shown on billing at visit completion                                       |
| All 14 standard reports | Administrator       | View, filter, print, and PDF-export each of the 14 reports                                                                    |
| Bulk data export        | Administrator       | Trigger export → download ZIP → verify contents include all entity CSV files                                                  |
| Audit log               | Administrator       | Verify all key actions appear in audit log with correct actor, timestamp, and action                                          |
| Role enforcement        | All roles           | Staff cannot access Administrator-only screens; Dentist cannot void payments; portal user cannot reach clinic-staff endpoints |

### Phase-End Acceptance Criteria

- All UAT scenarios completed by actual clinic staff without workflow-blocking errors.
- No P1 (system-blocking) or P2 (workflow-impacting) defects remain open at sign-off.
- Clinic staff (non-technical users) rate the UI as usable for day-to-day operations.
- Written sign-off from clinic Administrator is obtained before Phase 10 begins.

---

## Phase 10 — Deployment & Go-Live

### Goal

The production server is provisioned, the application is deployed, TLS is live, and clinic staff are trained and ready to operate.

### Activities

| Activity              | Description                                                                                                                                                                           |
| --------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Server provisioning   | Ubuntu 22.04 LTS VPS: ≥ 2 vCPU, 4 GB RAM, 100 GB SSD (NFR-M05); firewall: ports 80/443 open; 5432/6379 internal only                                                                  |
| TLS configuration     | Nginx with Let's Encrypt certificate via Certbot; HTTP → HTTPS redirect; TLS 1.2+ only (NFR-S02)                                                                                      |
| Secrets configuration | Production `.env` with PostgreSQL password, Valkey password, JWT RS256 private key, SMTP credentials, SMS key, payment gateway key — gitignored; stored in encrypted secrets manager  |
| EF Core migrations    | `dotnet ef database update` on production database; verified with `dotnet ef migrations list`                                                                                         |
| Valkey persistence    | RDB snapshot (`save 60 1`); password-protected (NFR-S05)                                                                                                                              |
| Automated backups     | Daily `pg_dump` cron job; 30-day retention; restoration tested before go-live (NFR-R05)                                                                                               |
| Gateway configuration | Production SMS gateway API key entered in Clinic Settings (sandbox toggle off); production payment gateway key if FR-02.12 is enabled                                                 |
| Staff training        | Role-based sessions: Administrator (user management, settings, reports, export), Dentist (charting, prescriptions, certificates), Staff (registration, scheduling, billing, payments) |
| Smoke test            | Create patient → book appointment → record payment → print OR on production HTTPS URL                                                                                                 |
| DNS cutover           | Point clinic domain to production server; verify propagation; monitor for 24 hours                                                                                                    |

### Phase-End Acceptance Criteria

- `/health` returns HTTP 200 on the production HTTPS URL; HTTP requests redirect to HTTPS.
- TLS 1.2+ enforced; TLS 1.0/1.1 connections rejected by Nginx.
- Administrator can log in, configure clinic settings, and create user accounts on production.
- First automated daily database backup completes and is confirmed restorable.
- Smoke test passes: patient registered, appointment booked, payment recorded, OR PDF printed — all on production.
- No secrets appear in source control or in production log output.

---

## Phase 11 — Post-Launch Monitoring & Support

### Goal

The system is stable in production. Issues are triaged and resolved promptly. Performance baselines are established and monitored ongoing.

### Activities

| Activity                  | Cadence               | Description                                                                                     |
| ------------------------- | --------------------- | ----------------------------------------------------------------------------------------------- |
| Log monitoring            | Daily (first 30 days) | Review Serilog structured logs for errors and warnings; alert threshold set on error rate spike |
| Uptime monitoring         | Continuous            | External uptime monitor (e.g., UptimeRobot) on `/health`; SMS/email alert on downtime           |
| Performance baselines     | Week 1                | Record median API response times and PostgreSQL query durations under real clinic load          |
| Hangfire job review       | Weekly                | Check notification delivery rates, retry counts, and failed export jobs                         |
| SMS gateway review        | Week 2                | Confirm delivery rates per Philippine SMS provider; switch provider if latency is unacceptable  |
| Bug triage                | Ongoing               | P1 (system-blocking) resolved within 24 h; P2 (workflow-impacting) resolved within 72 h         |
| Dependency security audit | Monthly               | `dotnet list package --vulnerable`; `npm audit`; apply patches promptly                         |
| 30-day post-launch review | Day 30                | Gather staff feedback; document pain points; prioritise Version 2 backlog                       |

### Phase-End Acceptance Criteria

- System uptime ≥ 99% in the first 30 days of production operation.
- All P1 defects resolved within 24 hours; all P2 defects resolved within 72 hours.
- No OWASP Top 10 vulnerability remains unmitigated.
- 30-day post-launch review document completed and Version 2 backlog items captured.

---

## Cross-Cutting Requirements (All Phases)

The following apply throughout every phase from Phase 1 onward. They are not deferred.

| Requirement                                                                 | Source           |
| --------------------------------------------------------------------------- | ---------------- |
| BCrypt cost ≥ 12 for all password and security answer hashes                | NFR-S01, FR-01   |
| HTTPS (TLS 1.2+) on all endpoints                                           | NFR-S02          |
| RS256-signed JWTs; `portal` audience separation from day portal work begins | NFR-S03, BR-07.2 |
| OWASP Top 10 mitigations (SQL injection via ORM, XSS, CSRF, IDOR)           | NFR-S04          |
| `DECIMAL(10,2)` for all monetary fields; no FLOAT/DOUBLE                    | NFR-R01          |
| Every multi-table mutation wrapped in a single database transaction         | NFR-R02          |
| Foreign key constraints enforced at database level                          | NFR-R03          |
| No API endpoint exposes stack traces or internal error details              | NFR-S08          |
| No secrets committed to source control                                      | NFR-S06          |
| EF Core migrations only for schema changes; no manual SQL in production     | NFR-M02          |
| Audit log entry created for every in-scope action from Phase 1 onward       | FR-11, BR-06.2   |

---

## Deferred to Version 2 (Outside All Phases)

These items are explicitly out of scope for Version 1 per BRD Section 5.2.

| Item                                                      | BRD Reference |
| --------------------------------------------------------- | ------------- |
| PhilHealth e-Claims portal submission                     | §5.2          |
| DICOM viewer, CBCT analysis, advanced radiograph tools    | §5.2          |
| Full periodontal charting (probing depth, BOP, furcation) | §5.2          |
| Multi-branch / multi-clinic support                       | §5.2          |
| Payroll and HR management                                 | §5.2          |
| Supplier ordering and purchase orders                     | §5.2          |
| Teledentistry / video consultation                        | §5.2          |
| Patient-requested reschedule pending staff approval       | §5.2          |
| Publicly visible doctor profiles or ratings               | §5.2          |
| Patient access to raw FDI dental chart condition codes    | §5.2          |
