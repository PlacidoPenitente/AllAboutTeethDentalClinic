# Development Phase Plan

## All About Teeth Dental Clinic Management System (DCMS)

---

| Field            | Value                                      |
| ---------------- | ------------------------------------------ |
| Document Version | 1.1                                        |
| Date             | May 13, 2026                               |
| Basis            | BRD v4.3                                   |
| Repository       | PlacidoPenitente/AllAboutTeethDentalClinic |

---

## Purpose

This document splits the BRD v4.3 functional requirements into seven sequential development phases ordered by clinical priority and technical dependency. Each phase ends in a working, deployable increment. Subsequent phases add value without breaking the previous state.

**Guiding priority rules used for ordering:**

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

| Component | BRD Justification | Phase |
| --------- | ----------------- | ----- |
| Vue 3 + Vite + TypeScript | NFR-U01 (responsive web; Chrome/Edge/Firefox latest 2 versions) | 1 |
| PrimeVue 4+ | NFR-U03 (required-field indicator; inline validation errors); NFR-U04 (confirmation dialogs before destructive actions) | 1 |
| Pinia | State management for real-time widget data (FR-09.1.A.2) | 1 |
| vue-router | SPA routing for clinic app; reused in Patient Portal SPA (Phase 6) | 1 |
| Tailwind CSS | NFR-U01 (desktop full-function; tablet usable; mobile readable) | 1 |
| Vitest | Automated tests validating OWASP mitigations (NFR-S04) | 1 |
| FullCalendar | FR-04.3 (day/week/month calendar view); NFR-P02 (calendar loads ≤ 1 s) | 2 |
| SignalR JS client | FR-09.1.A.2 (WebSocket real-time push; widgets update without page reload) | 2 |
| echarts | FR-09.1.C–D (revenue trend bar, appointment donut, top-5 treatments bar); FR-12.8.6 (rating distribution chart — Phase 6) | 3 |
| Patient Portal SPA (separate Vue 3 project) | FR-12 (public-facing interface separate from clinic app; `portal` JWT audience — BR-07.2) | 6 |

### Backend API

| Component | BRD Justification | Phase |
| --------- | ----------------- | ----- |
| ASP.NET Core 8 Web API | NFR-M01 (containerized); NFR-M03 (`/health` endpoint) | 1 |
| Clean Architecture + CQRS + MediatR 12 | NFR-R02 (every multi-table mutation in one atomic transaction); NFR-M02 (EF Core migrations as sole schema change path) | 1 |
| FluentValidation 11 | NFR-S04 (input validation at all API boundaries — SQL injection, XSS, IDOR); NFR-U03 (validation error detail returned to caller) | 1 |
| AutoMapper 13 | NFR-S08 (API responses never expose internal domain model or stack traces — DTOs projected from domain entities) | 1 |
| EF Core 8 + Npgsql | NFR-M02 (migrations only; no manual SQL in production); NFR-R02 (atomic transactions); NFR-R03 (FK constraints at DB level) | 1 |
| BCrypt (cost ≥ 12) | NFR-S01; FR-01.1.5 (staff passwords); FR-01.3.1 (security answer hashes); FR-12.1.3 (portal passwords) | 1 |
| JWT RS256 | NFR-S03; FR-01.2.4 (clinic staff token); FR-12.2.2 (`portal` audience claim — same signing infrastructure, separate audience claim) | 1 |
| Serilog | NFR-M04 (structured JSON logging: timestamp, level, request ID); NFR-S08 (errors server-logged only — never in HTTP response body) | 1 |
| SignalR (server hub) | FR-09.1.A.2 (WebSocket push to all dashboard widgets) | 2 |
| PdfSharpCore + MigraDoc | FR-07.2.3 (SOA PDF); FR-07.3.6 (BIR-compliant OR PDF); FR-09.2 (all report PDFs); FR-09.3.2 (OR register PDF); FR-15.9 (prescription PDF); FR-16.7 (Medical Certificate PDF) | 3 |
| Hangfire | FR-10.7–10.8 (async notification delivery + 3-retry exponential backoff); FR-13.5 (async bulk export); FR-07.5.4 (nightly installment status job); FR-10.10 (daily recall job); FR-12.4.6 (15-min slot-release timeout job) | 4 |
| MailKit | FR-02.11 (SMTP config); FR-10.3–10.6 (appointment lifecycle emails); FR-12.1.4 (portal verify email); FR-12.2.3 (password reset email); BR-02.12 (AR deposit confirmation email) | 4 |
| SMS gateway HTTP client (Semaphore / Globe Labs / ITEXMO) | FR-02.10 (generic HTTP REST; Philippine providers configurable in Clinic Settings); FR-10.3–10.10 (all patient SMS notifications) | 4 |
| Payment gateway HTTP client (PayMongo / Maya / Paynamics) | FR-02.12 (online deposit collection); FR-12.4.6 (gateway deposit flow); NFR-S04 (webhook signature validation); NFR-S05–S06 (API keys encrypted at rest; never in source control); NFR-S08 (keys never in logs) | 7 |

### Infrastructure

| Component | BRD Justification | Phase |
| --------- | ----------------- | ----- |
| PostgreSQL 16 Alpine | NFR-R01 (`DECIMAL(10,2)` for all monetary fields; FLOAT/DOUBLE prohibited); NFR-R03 (FK constraints enforced at DB level) | 1 |
| Docker + Docker Compose v2 | NFR-M01 (full stack containerized; `docker compose up` brings all services); NFR-M06 (on-premises LAN — clinic operates during internet outage) | 1 |
| Nginx | NFR-S02 (HTTPS/TLS 1.2+ termination for clinic SPA, Patient Portal SPA, and API) | 1 |
| Ubuntu 22.04 LTS | NFR-M05 (production server ≥ 2 vCPUs, 4 GB RAM) | 1 |
| GitHub Actions | NFR-M02 (automated EF Core migration CI; Vitest runs on every push) | 1 |
| **Valkey 8** (StackExchange.Redis compatible) | FR-01.2.5 (server-side token revocation set for logout invalidation); FR-12.4.6 (ephemeral 15-min slot lock for gateway payments). **No BRD NFR mandates a cache layer** — Valkey is an implementation choice for these two specific requirements. Must be in Docker Compose to satisfy NFR-M01 and NFR-M06. | 1 (token store) / 7 (slot lock) |

---

## Phase Overview

| Phase | Name                                 | End-State Capability                                       |
| ----- | ------------------------------------ | ---------------------------------------------------------- |
| 1     | Foundation & Infrastructure          | Staff can log in; admin can configure the clinic           |
| 2     | Patient Management & Scheduling      | Staff can register patients and manage the appointment book|
| 3     | Clinical Core & Financial Operations | Dentist can chart and treat; staff can bill and receipt    |
| 4     | Inventory, Reports & Notifications   | Stock is tracked; all reports work; patients get SMS/email |
| 5     | Specialized Clinical Modules         | Prescriptions, certs, ortho notes, file attachments, installments, recall |
| 6     | Patient Portal                       | Patients self-register, book online, view history          |
| 7     | Online Booking Deposits              | Portal collects deposits via gateway or manual GCash       |

---

## Phase 1 — Foundation & Infrastructure

### Goal
The application is runnable, secure, and configurable. Staff can authenticate. The Administrator can manage user accounts and configure clinic identity, financial, and operating settings. The audit log captures every action from day one.

### BRD Coverage

| Module                      | Subsections / Items Included                                                                                    |
| --------------------------- | --------------------------------------------------------------------------------------------------------------- |
| **FR-01: User & Access Management** | FR-01.1 (user accounts), FR-01.2 (authentication, lockout), FR-01.3 (password recovery), FR-01.4 (RBAC table) |
| **FR-02: Clinic Settings**  | FR-02.1 – FR-02.9 (identity, compliance numbers, operating schedule, default slot duration, VAT, OR number tracking, security config, expiry warning threshold) |
| **FR-11: Audit Log**        | FR-11.1 – FR-11.5 (all; cross-cutting concern — scaffolded once and consumed by every subsequent phase)         |

### Business Rules
- BR-04.1 – BR-04.4 (user management rules)

### Non-Functional Requirements
- NFR-S01 – NFR-S08 (all security requirements — implemented from day one, not retrofitted)
- NFR-R01 – NFR-R03 (DECIMAL(10,2), atomic transactions, FK constraints — database schema standards established)
- NFR-M01 – NFR-M06 (Docker Compose stack, EF Core migrations, health check, structured logging, on-premises deployment)

### New Database Entities
`User`, `ClinicSettings`, `AuditLog`

### Tech Stack Components First Introduced
- **ASP.NET Core 8 + Clean Architecture + CQRS/MediatR 12 + FluentValidation 11 + AutoMapper 13** — API scaffold; all input validated at boundaries (NFR-S04); responses never expose internals (NFR-S08)
- **EF Core 8 + Npgsql + PostgreSQL 16 Alpine** — `DECIMAL(10,2)` enforced from migration 1 (NFR-R01); FK constraints at DB level (NFR-R03); EF Core migrations only — no manual SQL (NFR-M02)
- **Docker Compose v2 + Nginx** — full stack containerized; Nginx terminates TLS 1.2+ (NFR-S02, NFR-M01, NFR-M06)
- **JWT RS256** — clinic staff tokens; `portal` audience claim infrastructure established here (reused Phase 6) (NFR-S03, FR-01.2.4)
- **BCrypt (cost ≥ 12)** — staff password and security answer hashes (NFR-S01, FR-01.1.5, FR-01.3.1)
- **Valkey 8** — token revocation set for server-side logout invalidation (FR-01.2.5)
- **Serilog** — structured JSON logging with timestamp, level, and request ID; errors never returned in HTTP responses (NFR-M04, NFR-S08)
- **Vue 3 + Vite + TypeScript + PrimeVue 4 + Pinia + vue-router + Tailwind CSS + Vitest** — clinic SPA scaffold and automated test suite (NFR-U01, NFR-U03, NFR-U04, NFR-S04)
- **GitHub Actions** — CI pipeline: EF Core migration validation and Vitest runs (NFR-M02)

### Phase-End Acceptance Criteria
- An Administrator can log in, create Dentist and Staff accounts, and configure clinic name, TIN, operating hours, and VAT status.
- A user locked after 5 failed attempts is automatically unlocked after 30 minutes; an Administrator can unlock it immediately.
- Password recovery via security questions produces a new valid session.
- Every login, logout, account creation, and settings change appears in the Audit Log with correct actor, timestamp, and entity.
- The Docker Compose stack starts cleanly with `docker compose up` and the `/health` endpoint returns 200.

---

## Phase 2 — Patient Management & Scheduling

### Goal
The clinic can operate its front desk. Staff can register patients with full medical and dental history, manage the appointment calendar, handle check-ins, and record vital signs. A minimal dashboard shows today's schedule.

### BRD Coverage

| Module                         | Subsections / Items Included                                                                                                             |
| ------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------- |
| **FR-03: Patient Management**  | FR-03.1 (registration + age-based tooth initialization), FR-03.2 (personal & contact), FR-03.3 (dental history), FR-03.4 (medical history incl. pregnancy flag FR-03.4.12), FR-03.5 (search & list), FR-03.6 (Active / Archived status) |
| **FR-04: Appointment Scheduling** | FR-04.1 (create, conflict detection, planned treatments, LOA number), FR-04.2 (full status lifecycle: Pending → Confirmed → In Progress → Completed, plus No Show, Cancelled, Awaiting Approval, Rejected), FR-04.3 (calendar view with color-coded status), FR-04.4 (appointment list view), FR-04.5 (pre-operative vitals + pregnancy × X-ray warning FR-04.5.6) |
| **FR-09.1: Dashboard (partial)** | FR-09.1.A (general behavior — **SignalR WebSocket connection scaffolded here**; FR-09.1.A.2 mandates real-time push from the first dashboard widget onward), FR-09.1.B (all-user widgets: today's appointments, appointment counts, active patient count, pending count) |
| **FR-09.2 (partial)**          | RPT-01 (Daily Appointment List) — the one report needed before billing exists, for day-one printed schedules                             |

### Business Rules
- BR-01.1 – BR-01.8 (all appointment scheduling rules including Awaiting Approval and Rejected portal booking states)
- BR-03.3 (tooth records created at registration)
- BR-06.1 – BR-06.2 (privacy consent required; patient access logged)

### New Database Entities
`Patient`, `Tooth` (52 per patient, initialized per FR-03.1.6 age brackets), `MedicalHistory`, `DentalHistory`, `Appointment`, `PlannedTreatment`, `AppointmentVitalSigns`, `Provider` (HMO/insurance reference, needed for dental history in FR-03.3.4)

### Tech Stack Components First Introduced
- **FullCalendar** — appointment calendar (day/week/month views); must meet ≤ 1 s load target (FR-04.3, NFR-P02)
- **SignalR (server hub + JS client)** — real-time WebSocket infrastructure scaffolded here; required by FR-09.1.A.2 from the first dashboard widget onward

### Phase-End Acceptance Criteria
- A new patient can be registered with all demographic, dental history, and medical history fields. Privacy consent is required to save.
- A 7-year-old patient's tooth chart is automatically initialized with the correct mixed-dentition state: primary canines and molars (53–55, 63–65, 73–75, 83–85) PNT; primary incisors (51, 52, 61, 62, 71, 72, 81, 82) SHD; permanent first molars (16, 26, 36, 46) and permanent central incisors (11, 21, 31, 41) PNT; remaining permanent teeth UNE (per FR-03.1.6).
- Booking a second appointment for the same dentist in an overlapping time window is blocked with a conflict message.
- An appointment created as a walk-in starts directly in In Progress status.
- The calendar view shows today's appointments color-coded by status and updates when status changes.
- RPT-01 (Daily Appointment List) is printable and exportable as PDF for a selected date.

---

## Phase 3 — Clinical Core & Financial Operations

### Goal
The dentist can examine, chart, and record procedures. Staff can generate a billing after the visit, record payment, and print a BIR-compliant Official Receipt. The clinic can operate end-to-end from patient arrival to payment receipt, including HMO-covered visits.

### BRD Coverage

| Module                              | Subsections / Items Included                                                                                                                             |
| ----------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **FR-05: Dental Charting**          | FR-05.1 (SVG chart display), FR-05.2 (tooth detail: condition history, procedure history), FR-05.3 (update tooth conditions, append-only history, Quick Set tool), FR-05.4 (treatment filtering from chart — applicable conditions) |
| **FR-06: Treatment Records**        | FR-06.1 (treatment catalog: tooth-specific vs. global, categories, pricing, applicable/resulting conditions, interim condition, required specialization, procedure kit reference), FR-06.2 (procedure recording per appointment: per-tooth, completion flag, resulting/interim condition update, in-progress visual flag), FR-06.3 (immutable treatment records / clinical history per patient) |
| **FR-07: Billing & Payments**       | FR-07.1 (billing generation, draft→final, subtotal/discount/tax/HMO share), FR-07.2 (Statement of Account / Invoice — printable PDF), FR-07.3 (payment recording: manual OR number entry, multiple payment methods, Acknowledgment Receipt for deposits), FR-07.4 (payment voiding — Administrator only, mandatory reason) |
| **FR-14: HMO Claim Recording**      | FR-14.1 – FR-14.6 (LOA entry with tariff-derived authorized amount note, coverage cap enforcement, per-patient LOA history, disclaimer, multi-provider patient links, RPT-13 stub) |
| **FR-09.1 (additions)**             | FR-09.1.C (Administrator revenue widgets: today/week/month revenue, outstanding receivables, revenue trend chart, new patients this month), FR-09.1.D (appointment status breakdown donut, top-5 treatments chart), FR-09.1.E (dentist's own schedule and procedure count widgets) |
| **FR-09.2 (partial)**               | RPT-04 (Revenue Report), RPT-05 (Billing Register), RPT-06 (Accounts Receivable), RPT-07 (Dentist Production), RPT-08 (Treatment Frequency), RPT-13 (HMO Claims Summary), RPT-14 (Daily Collection) |
| **FR-09.3: OR Register**            | FR-09.3.1 – FR-09.3.2 (sequential OR register, PDF/CSV export)                                                                                          |

### Business Rules
- BR-02.1 – BR-02.13 (all financial rules: balance computation, OR uniqueness, void rules, fully-paid transition, installment matching, deposit/AR receipting — BR-02.11 and BR-02.12 are scaffolded here; installment plan UI is Phase 5)
- BR-03.1 – BR-03.7 (all clinical rules: immutable records, append-only chart history, FDI notation, condition constraints, applicable-condition enforcement, read-only chart outside In Progress)
- BR-08.1 – BR-08.4 (HMO claim recording rules)

### Non-Functional Requirements
- NFR-R04 (balance always computed from payment records — enforced in all billing API endpoints)
- NFR-U05 (SVG dental chart — not a table)
- NFR-P01 – NFR-P04 (search 500ms, calendar 1s, chart 1s, financial reports 5s)

### New Database Entities
`TreatmentCatalog`, `ProcedureKit`, `ToothConditionEntry`, `AppointmentProcedure`, `AppointmentGlobalProcedure`, `TreatmentRecord`, `Billing`, `BillingLineItem`, `Payment`, `LOA`, `PatientProvider`

### Tech Stack Components First Introduced
- **PdfSharpCore + MigraDoc** — PDF output for OR (FR-07.3.6, BIR-compliant), SOA (FR-07.2.3), standard reports (FR-09.2), and OR register (FR-09.3.2)
- **echarts** — revenue trend bar chart, appointment status donut chart, and top-5 treatments bar chart (FR-09.1.C–D)

### Phase-End Acceptance Criteria
- The dental chart displays all 52 FDI teeth as an SVG diagram with color-coded current conditions.
- Updating a tooth condition creates a new history entry; the previous entry is marked historical and never overwritten.
- When selecting a treatment for a tooth, the system prioritizes (surfaces first) treatments whose Applicable Conditions include the tooth's current active surface condition; all active treatments remain selectable regardless of the tooth's current condition (FR-05.4.1 — the filter is a productivity aid, not a clinical gate).
- After an appointment is marked Completed, all treatment records are read-only.
- A billing is generated from a completed appointment showing subtotal, HMO coverage (if applicable), patient share, and outstanding balance.
- A payment with manual OR number entry produces a printable PDF Official Receipt matching the BIR format.
- The OR Register lists all ORs sequentially; a voided OR shows its number with a "Voided" marker.
- RPT-04, RPT-05, RPT-06, RPT-13, and RPT-14 are all viewable, printable, and PDF-exportable.
- The LOA entry form on billing includes a **"View Provider Tariff"** read-only look-up panel showing the current tariff schedule for the patient's assigned provider (FR-14.1). Staff can verify the authorized amount and coverage cap without leaving the billing screen before manually entering the LOA figure.

---

## Phase 4 — Inventory, Reports & Notifications

### Goal
Supply stock is tracked with receipts and consumption. Low-stock and near-expiry alerts appear on the dashboard. All 14 standard reports are available. Patients receive SMS and email notifications for appointment lifecycle events. Bulk data export is available.

### BRD Coverage

| Module                            | Subsections / Items Included                                                                                                          |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------- |
| **FR-08: Inventory**              | FR-08.1 (supply item catalog: categories, units, critical quantity, Tracked vs. Bulk-Managed tier), FR-08.2 (stock ledger: receipts, adjustments, lot numbers, expiry dates), FR-08.3 (consumption during treatment: optional per-appointment, procedure kit auto-suggestion, Tracked vs. Bulk-Managed policy, stock warning, weekly adjustment workflow), FR-08.4 (supplier catalog) |
| **FR-02 (additions)**             | FR-02.10 (SMS gateway configuration: Semaphore / Globe Labs / ITEXMO), FR-02.11 (SMTP email configuration)                           |
| **FR-10: Notifications & Alerts** | FR-10.1 – FR-10.9 (in-app badges, appointment confirmation / reschedule / cancellation SMS+email, 24-hour reminder, async Hangfire delivery, 3-retry with exponential backoff, per-patient notification preferences) |
| **FR-09.1 (completion)**          | FR-09.1.F (low-stock and near-expiry dashboard alerts with badge counts); SignalR push extended to inventory alert badge widgets (SignalR infrastructure introduced in Phase 2) |
| **FR-09.2 (completion)**          | RPT-02 (Patient Masterlist), RPT-03 (Patient Treatment History), RPT-09 (Inventory Status), RPT-10 (Inventory Consumption), RPT-11 (No-Show & Cancellation), RPT-12 (New Patient Acquisition) — all remaining standard reports |
| **FR-13: Data Export**            | FR-13.1 – FR-13.9 (bulk CSV/XLSX export via Hangfire, time-limited secure download link, configurable date-range scope, audit-logged) |

### Business Rules
- BR-05.1 – BR-05.5 (all inventory rules: computed stock, expiry/lot requirements, over-consumption warning, dashboard alerts)
- BR-06.3 – BR-06.4 (bulk export restricted to Administrator; 10-year retention before purge)
- BR-07.6 (dentist/staff reschedule → mandatory patient notification)
- BR-07.10 (notification delivery failure does not roll back the appointment action)
- BR-09.1 – BR-09.4 (export rules: Administrator-only, audit-logged, no secrets in exports, 24h link expiry)

### Non-Functional Requirements
- NFR-R05 (automated daily database backups, configurable retention)
- NFR-P05 (10 concurrent users without degradation)
- NFR-M04 (structured JSON logging via Serilog)

### New Database Entities
`SupplyItem`, `SupplyStockLedger`, `SupplierCatalog`, `ProcedureConsumption`, `NotificationLog`, `DataExportJob`

### Tech Stack Components First Introduced
- **Hangfire** — background job server: async notification delivery with 3-retry exponential backoff (FR-10.7–10.8); async bulk data export (FR-13.5); nightly installment status transition job (FR-07.5.4, activated Phase 5); daily recall reminder job (FR-10.10, activated Phase 5); 15-min slot-release job (FR-12.4.6, activated Phase 7)
- **MailKit** — SMTP email delivery for appointment lifecycle notifications (FR-10.3–10.6) and portal email flows (FR-12.1.4, FR-12.2.3 — activated Phase 6); SMTP credentials stored per NFR-S06
- **SMS gateway HTTP client** (Semaphore / Globe Labs / ITEXMO) — generic HTTP REST adapter for all patient SMS (FR-02.10, FR-10.3–10.10)

### Phase-End Acceptance Criteria
- Receiving a supply item adds a positive ledger entry; recording consumption on an appointment deducts from stock. Current stock is always `SUM(ledger entries)`.
- Items at or below critical quantity show a badge on the dashboard with a direct link to the inventory page.
- An Anesthetic or Medication receipt without a lot number and expiry date is rejected by the API.
- When an appointment is confirmed, the patient receives an SMS and email within the Hangfire job cycle. Delivery status is recorded as Sent or Failed.
- A failed notification is retried 3 times with exponential backoff before being flagged for administrator review.
- All 14 standard reports (RPT-01 through RPT-14) are fully functional, printable, and exportable as PDF.
- A bulk data export triggered by the Administrator produces a ZIP of CSV files for all entities and is downloadable within the time-limited link period. The action is recorded in the Audit Log.
- All dashboard widgets — including revenue totals, appointment counts, and inventory alert badge counts — refresh in real time via SignalR push; no page reload is required to see updated values.
- End-to-end SMS delivery is verified against **at least one live Philippine SMS gateway** (Semaphore, Globe Labs, or ITEXMO) for every notification event type (confirmation, reschedule, cancellation, 24-hour reminder). Gateway response latency is measured per event; the Hangfire retry policy (3 retries, exponential backoff, per FR-10.7–10.8) is confirmed to recover from transient gateway timeouts before Phase 4 is signed off. Configuration instructions for each supported gateway are documented for clinic onboarding.

---

## Phase 5 — Specialized Clinical Modules

### Goal
Dentists can generate prescriptions and medical/dental certificates from within the system. Orthodontic appointments have a structured progress note. Patient files (including X-ray JPEGs) can be attached to the patient record. Multi-session treatments can be managed with installment payment schedules. Automated recall reminders run daily.

### BRD Coverage

| Module                                  | Subsections / Items Included                                                                                                                |
| --------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------- |
| **FR-03.7: Patient File Attachments**   | FR-03.7.1 – FR-03.7.5 (Attachments tab, JPEG/PNG/PDF upload, label + note, reverse-chron display, download, audit-logged deletion, server-filesystem storage, auth-gated access) |
| **FR-06.4: Orthodontic Progress Notes** | FR-06.4.1 – FR-06.4.6 (triggered by Orthodontic-category appointment; structured fields: upper/lower wire, elastics, bracket changes, appliance status; Orthodontic History tab; tooth chart accessible but not required) |
| **FR-07.5: Installment Payment Plans**  | FR-07.5.1 – FR-07.5.8 (plan creation on a billing, configurable schedule, Hangfire reminders N days before due date, waive installment, match payments to earliest unpaid installment) |
| **FR-02.15**                            | Installment plan reminder lead-time setting in Clinic Settings                                                                              |
| **FR-15: Electronic Prescription**      | FR-15.1 – FR-15.11 (Dentist/Admin only, linked to patient + optional appointment, drug items, PRC/PTR/S2 auto-fill, blank wet-signature line, no digital signature image, read-only after save, void with reason, PDF auto-saved to Attachments tab as System-Generated, audit-logged, prescription history on patient profile) |
| **FR-16: Medical / Dental Certificate** | FR-16.1 – FR-16.9 (Dentist/Admin only, linked to patient + optional appointment, diagnosis/procedure/rest period fields, selectable cert title, blank wet-signature line, no digital signature image, void with reason, PDF auto-saved to Attachments tab as System-Generated, audit-logged, certificate history on patient profile) |
| **FR-10.10: Recall Reminders**          | Hangfire daily job, configurable 6-month threshold, 30-day re-send guard, respects per-patient opt-out (FR-10.9), SMS + email delivery        |
| **FR-02 (additions)**                   | FR-02.15 already listed above; recall threshold and re-send interval settings added to Clinic Settings                                       |

### Business Rules
- BR-02.11 (installment plan payment matching against schedule)
- BR-02.13 (waived installment excluded from balance but does not reduce TotalAmount)
- BR-03.6 (applicable conditions — already implemented Phase 3; ortho procedures follow same rule)
- BR-10.1 – BR-10.6 (all prescription/certificate rules: wet-signature-only, no digital image, PRC/S2 warnings, same policy applies to certs, system-generated PDFs stored in Attachments, staff manual email for digital copy requests)

### New Database Entities
`PatientFileAttachment`, `OrthoProgressNote`, `InstallmentPlan`, `InstallmentEntry`, `Prescription`, `PrescriptionItem`, `MedicalCertificate`

### Audit Log Entity Coverage Added
`PatientFileAttachment`, `Prescription`, `MedicalCertificate`, `OrthoProgressNote` (as specified in FR-11.2)

### Tech Stack Components First Introduced
No new stack components. Existing components are extended:
- **PdfSharpCore + MigraDoc** (introduced Phase 3): generates prescription PDFs (FR-15.9) and Medical Certificate PDFs (FR-16.7), both auto-saved as System-Generated entries in the patient Attachments tab
- **Hangfire** (introduced Phase 4): adds installment due-date reminder jobs (FR-07.5.6) and the daily recall reminder job (FR-10.10)

### Phase-End Acceptance Criteria
- A JPEG file uploaded to a patient's Attachments tab is downloadable by authenticated clinic staff but inaccessible via direct URL without a valid session token.
- Deleting an attachment requires a mandatory reason and creates an Audit Log entry; the physical file is removed from storage.
- An Orthodontic-category appointment displays the Ortho Progress Note panel; the structured fields (wire gauge, elastics, appliance status) are saved and visible in the Orthodontic History tab.
- A prescription PDF is generated on demand and automatically appears in the patient's Attachments tab labeled "Prescription — [date] — Dr. [name]" as a System-Generated entry that cannot be deleted by staff.
- A Medical Certificate PDF follows the same auto-save behavior with label "Medical Certificate — [date] — Dr. [name]".
- Neither the prescription nor the certificate embeds or renders a dentist signature image.
- An installment plan on a billing generates Hangfire-scheduled reminder notifications N days before each due date.
- Patients with no upcoming appointment whose last completed appointment was more than 6 months ago receive a recall reminder SMS and email; no patient receives more than one recall reminder within the 30-day re-send window.

---

## Phase 6 — Patient Portal

### Goal
Patients can self-register, book appointments online, view their treatment history and billing, submit feedback, and rate their dentist — all through a separate public-facing SPA. A parent can manage dependent (child) profiles under a single account.

### BRD Coverage

| Module                                    | Subsections / Items Included                                                                                                                                                                                  |
| ----------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **FR-02 (additions)**                     | FR-02.13 (booking approval mode: Immediate / Awaiting Approval), FR-02.16 (online booking appointment types: label, description, duration, required specialization tag)                                       |
| **FR-12.1: Portal Accounts**              | FR-12.1.1 – FR-12.1.6 (self-registration, email verification, portal-registered patient record, staff invitation link), FR-12.1.7 – FR-12.1.8 (Dependent / family accounts: add dependent form, Primary Account Holder session governs access, max 10 dependents configurable) |
| **FR-12.2: Portal Authentication**        | FR-12.2.1 – FR-12.2.4 (email+password login, separate `portal` JWT audience and signing key, 5-attempt lockout, staff unlock + password reset email from clinic dashboard, cross-patient IDOR rejection)       |
| **FR-12.3: Self-Registration Intake**     | FR-12.3.1 – FR-12.3.5 (guided medical/dental history intake, Patient-Submitted status, staff review merge flow, Pending Staff Review indicator, contact-only self-edit after confirmation)                    |
| **FR-12.4: Online Appointment Booking**   | FR-12.4.1 – FR-12.4.5 and FR-12.4.9 – FR-12.4.10 (booking calendar, appointment type selection, conflict check, approval mode handling, confirmation email, staff in-app flag) — **deposit flow (FR-12.4.6–12.4.8) is Phase 7** |
| **FR-12.5: My Appointments**              | FR-12.5.1 – FR-12.5.6 (view upcoming and past appointments, cancel, guided reschedule as atomic cancel+rebook)                                                                                                |
| **FR-12.6: My Treatment History**         | FR-12.6.1 – FR-12.6.5 (completed appointment procedures in plain language; no FDI codes; read-only; date-range and dentist filter)                                                                            |
| **FR-12.7: My Billing & Payment Summary** | FR-12.7.1 – FR-12.7.5 (billing list, patient-friendly statuses, download own OR PDF, download own SOA PDF; IDOR enforced at API)                                                                              |
| **FR-12.8: Dentist Ratings**              | FR-12.8.1 – FR-12.8.8 (post-completion prompt, 1–5 stars + comment, one rating per appointment, anonymous to dentist — name never disclosed, Admin-only full view, 7-day edit window, Admin delete with reason) |
| **FR-12.9: Patient Feedback**             | FR-12.9.1 – FR-12.9.8 (complaint/suggestion/recommendation/general, staff inbox, lifecycle: New → Under Review → Resolved → Closed, Complaint priority badge, patient sees status not staff notes, in-portal status change notification) |
| **FR-10.9 (portal-side)**                 | Notification preference controls (SMS/email opt-in/out per event type) in portal profile settings                                                                                                             |

### Business Rules
- BR-01.7 – BR-01.8 (Awaiting Approval portal bookings; Rejected is terminal)
- BR-07.1 – BR-07.13 (all portal rules: audience isolation, patient-scope IDOR enforcement, cancel/reschedule atomicity, mandatory reschedule notification, rating anonymity, edit window, complaint acknowledgment, deposit forfeiture — BR-07.12–13 are scaffolded here, fully exercised in Phase 7)
- BR-09.1 already in scope (portal accounts are not exported by individual patients)

### Architecture Note
The Patient Portal is a **separate Vue 3 SPA** (`/portal/` or subdomain). It shares the existing ASP.NET Core API but authenticates via a distinct JWT audience (`portal`). The API already enforces audience separation (introduced Phase 1/3). The Portal SPA is a new frontend project within the repository.

### New Database Entities
`PortalAccount`, `PatientDependentLink`, `PortalAccountLockout`, `DentistRating`, `PatientFeedback`

### Tech Stack Components First Introduced
- **Patient Portal SPA** — a separate Vue 3 + Vite + TypeScript project within the repository, consuming the same ASP.NET Core API via the `portal` JWT audience claim (FR-12.2.2, BR-07.2). Reuses PrimeVue, Pinia, vue-router, Tailwind CSS, FullCalendar (online booking calendar, FR-12.4.2), echarts (rating distribution chart, FR-12.8.6), and the SignalR JS client.

### Phase-End Acceptance Criteria
- A patient self-registers on the portal, receives a verification email, activates their account, and completes the guided medical/dental history intake. The patient record appears in the clinic system flagged "Pending Staff Review."
- A Primary Account Holder adds a dependent child profile. The child's appointments appear under the parent's portal session; the child has no independent login.
- A portal booking in Awaiting Approval mode does not block the time slot; a conflict booking by staff before approval forces staff to reject the portal request.
- A patient can cancel a Pending or Confirmed appointment from the portal. Cancelling In Progress or Completed is rejected by the API.
- The guided reschedule flow (cancel + rebook) is executed as a single atomic transaction; if the new slot is taken, the cancellation is rolled back.
- A completed appointment prompts the patient to rate the dentist. The dentist's dashboard shows the star average and distribution; the patient's name is never shown to the dentist.
- A Complaint submission appears with a red badge in the staff feedback inbox.
- The portal JWT with `portal` audience is rejected by all clinic-staff API endpoints. A staff JWT is rejected by all portal API endpoints.

---

## Phase 7 — Online Booking Deposits

### Goal
Patients can pay a configurable booking deposit during the portal booking flow — either through a payment gateway (PayMongo / Maya / Paynamics) or by uploading a GCash/bank transfer receipt. Staff have a verification inbox for manual deposits. The clinic is protected from no-shows without requiring any particular payment gateway.

### BRD Coverage

| Module                                 | Subsections / Items Included                                                                                                                                              |
| -------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **FR-02.12**                           | Payment gateway configuration (PayMongo / Maya / Paynamics / Other, API keys encrypted at rest, sandbox/production toggle, connection-test button)                        |
| **FR-02.12a**                          | Manual deposit verification configuration (toggle, instruction text, receipt upload field; can coexist with gateway)                                                      |
| **FR-02.14**                           | Booking deposit settings (PHP amount — 0 disables feature, label, forfeiture policy description text shown to patient before payment)                                     |
| **FR-12.4.6 – FR-12.4.8**             | Full deposit flow: gateway path (15-minute slot reservation, Awaiting Payment status, auto-release on timeout/failure) and manual path (Awaiting Deposit Verification status, receipt image upload); deposit applied to final billing; forfeiture / refund recording |
| **FR-12.4.6a**                         | Staff inbox: Awaiting Deposit Verification queue with Verify and Reject actions; rejected deposit notifies patient and releases slot; Forfeited recording on late cancellation |
| **FR-12.4.6b**                         | Uploaded receipt image stored on appointment record; accessible to Admin and Staff; not visible to patient after upload                                                    |

### Business Rules
- BR-02.12 (deposit recorded as Deposit payment type; AR issued immediately; BIR OR recorded at visit completion; AR number and OR number stored on deposit payment record)
- BR-07.12 – BR-07.13 (deposit non-refundable by default per forfeiture policy; forfeited amount recorded as revenue; refunded amount reversed from placeholder billing)

### Architecture Note
The payment gateway integration introduces a new outbound HTTP dependency. The gateway webhook receiver must validate signatures before trusting any payload (OWASP input validation). API keys must be stored encrypted at rest (NFR-S05, NFR-S06) and must never appear in logs (NFR-S08). The 15-minute slot reservation for gateway payments requires a Hangfire job that releases the slot on timeout.

### New Database Entities
`BookingDepositReceipt` (stores uploaded image path + appointment reference), `AcknowledgmentReceipt`

### Tech Stack Components First Introduced
- **Payment gateway HTTP client** (PayMongo / Maya / Paynamics) — outbound integration for automated deposit collection (FR-02.12); webhook receiver must validate gateway signatures before trusting payloads (NFR-S04); API keys encrypted at rest and never logged (NFR-S05, NFR-S06, NFR-S08)
- **Valkey 8 (slot reservation)** — extends the existing Valkey instance (introduced Phase 1 for token revocation) to hold the ephemeral 15-minute slot lock for gateway payments (FR-12.4.6); lock released by Hangfire timeout job or on payment confirmation

### Phase-End Acceptance Criteria
- With deposit amount set to ₱500 and gateway configured, a portal booking routes through the gateway; the slot is reserved for 15 minutes; payment confirmation saves the appointment and records the deposit.
- If the gateway payment times out, the slot reservation is released and the appointment is not saved.
- With Manual Deposit enabled, a patient uploads a GCash screenshot; the appointment enters Awaiting Deposit Verification; staff can Verify (advances to Pending) or Reject (patient notified, slot released).
- The uploaded receipt image is accessible to staff from the appointment detail page; it is not returned by any patient-facing portal API endpoint.
- A verified deposit generates a system-produced Acknowledgment Receipt (non-BIR) that is emailed to the patient and saved to their Attachments tab.
- When the visit is completed and the final billing is created, the deposit is automatically applied as the first payment reducing the outstanding balance.
- An appointment cancelled after deposit verification shows the deposit as Forfeited or Refunded; Forfeited deposits appear as revenue in RPT-04.
- Gateway API keys return `[REDACTED]` from all API responses and do not appear in server logs.

---

## Cross-Cutting Requirements (All Phases)

The following apply throughout every phase from Phase 1 onward. They are not deferred.

| Requirement                                                                  | Source           |
| ---------------------------------------------------------------------------- | ---------------- |
| BCrypt cost ≥ 12 for all password and security answer hashes                 | NFR-S01, FR-01   |
| HTTPS (TLS 1.2+) on all endpoints                                            | NFR-S02          |
| RS256-signed JWTs; `portal` audience separation from day portal work begins  | NFR-S03, BR-07.2 |
| OWASP Top 10 mitigations (SQL injection via ORM, XSS, CSRF, IDOR)           | NFR-S04          |
| `DECIMAL(10,2)` for all monetary fields; no FLOAT/DOUBLE                    | NFR-R01          |
| Every multi-table mutation wrapped in a single database transaction           | NFR-R02          |
| Foreign key constraints enforced at database level                           | NFR-R03          |
| No API endpoint exposes stack traces or internal error details               | NFR-S08          |
| No secrets committed to source control                                       | NFR-S06          |
| EF Core migrations only for schema changes; no manual SQL in production      | NFR-M02          |
| Audit log entry created for every in-scope action from Phase 1 onward        | FR-11, BR-06.2   |

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
