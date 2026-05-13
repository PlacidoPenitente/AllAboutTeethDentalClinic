# API Endpoint Reference — All About Teeth DCMS

**Version:** 1.0  
**BRD Reference:** v4.3  
**Development Phases Reference:** DEVELOPMENT_PHASES.md v2.0  
**Architecture Reference:** TECH_STACK_AND_ARCHITECTURE.md  
**Wireframe Reference:** WIREFRAMES.md

---

## Table of Contents

1. [Conventions](#1-conventions)
2. [Infrastructure](#2-infrastructure)
3. [Authentication — Clinic Staff](#3-authentication--clinic-staff)
4. [User Management](#4-user-management)
5. [Clinic Settings](#5-clinic-settings)
6. [Audit Log](#6-audit-log)
7. [Patient Management](#7-patient-management)
8. [Appointment Scheduling](#8-appointment-scheduling)
9. [Dental Charting](#9-dental-charting)
10. [Treatment Catalog](#10-treatment-catalog)
11. [Appointment Procedures](#11-appointment-procedures)
12. [Billing](#12-billing)
13. [Payments](#13-payments)
14. [HMO Providers & LOA](#14-hmo-providers--loa)
15. [Patient File Attachments](#15-patient-file-attachments)
16. [Inventory & Supply Management](#16-inventory--supply-management)
17. [Suppliers](#17-suppliers)
18. [Notifications & Preferences](#18-notifications--preferences)
19. [Dashboard](#19-dashboard)
20. [Reports & Analytics](#20-reports--analytics)
21. [Orthodontic Progress Notes](#21-orthodontic-progress-notes)
22. [Installment Payment Plans](#22-installment-payment-plans)
23. [Prescriptions](#23-prescriptions)
24. [Medical/Dental Certificates](#24-medicaldental-certificates)
25. [Data Export](#25-data-export)
26. [Patient Portal — Authentication](#26-patient-portal--authentication)
27. [Patient Portal — Account & Profile](#27-patient-portal--account--profile)
28. [Patient Portal — Dependents](#28-patient-portal--dependents)
29. [Patient Portal — Online Booking](#29-patient-portal--online-booking)
30. [Patient Portal — My Appointments](#30-patient-portal--my-appointments)
31. [Patient Portal — Treatment History](#31-patient-portal--treatment-history)
32. [Patient Portal — Billing & Payments](#32-patient-portal--billing--payments)
33. [Patient Portal — Ratings](#33-patient-portal--ratings)
34. [Patient Portal — Feedback](#34-patient-portal--feedback)
35. [Online Booking Deposits](#35-online-booking-deposits)
36. [Admin — Portal Account Management](#36-admin--portal-account-management)
37. [Admin — Feedback Inbox](#37-admin--feedback-inbox)
38. [Admin — Ratings Management](#38-admin--ratings-management)
39. [Admin — Deposit Verifications](#39-admin--deposit-verifications)
40. [Webhooks](#40-webhooks)
41. [SignalR Hubs](#41-signalr-hubs)

---

## 1. Conventions

### Base URLs

| Context                   | Base URL                        |
| ------------------------- | ------------------------------- |
| Clinic Staff API          | `https://{host}/api/`           |
| Patient Portal API        | `https://{host}/api/portal/`    |
| Admin (Portal Management) | `https://{host}/api/admin/`     |
| Webhooks                  | `https://{host}/api/webhooks/`  |
| Health                    | `https://{host}/health`         |
| SignalR Hub               | `https://{host}/hubs/dashboard` |

### Authentication Types

| Code     | Meaning                                                                                                                     |
| -------- | --------------------------------------------------------------------------------------------------------------------------- |
| `Bearer` | Clinic-staff JWT (RS256, `clinic` audience). Issued by `POST /api/auth/login`. Default expiry: 8 h (configurable).          |
| `Portal` | Patient portal JWT (RS256, `portal` audience). Issued by `POST /api/portal/auth/login`. Default expiry: 2 h (configurable). |
| `Public` | No authentication required.                                                                                                 |

> **Audience isolation (NFR-S03, BR-07.2):** A `portal` audience JWT is rejected with `401` by every clinic-staff endpoint. A `Bearer` (clinic) JWT is rejected with `401` by every portal endpoint. This is enforced via separate `AddAuthentication` schemes with audience validation in the API middleware.

### Role Codes

| Code        | Meaning                      |
| ----------- | ---------------------------- |
| `Admin`     | Administrator (clinic staff) |
| `Dentist`   | Dentist (clinic staff)       |
| `Staff`     | Staff (clinic staff)         |
| `Patient`   | Portal-authenticated patient |
| `All Staff` | Admin + Dentist + Staff      |

### Standard HTTP Status Codes

| Code                        | Meaning                                                                                                                           |
| --------------------------- | --------------------------------------------------------------------------------------------------------------------------------- |
| `200 OK`                    | Successful GET, PUT, DELETE, PATCH                                                                                                |
| `201 Created`               | Successful POST that creates a resource                                                                                           |
| `204 No Content`            | Successful DELETE/action with no response body                                                                                    |
| `400 Bad Request`           | Malformed request                                                                                                                 |
| `401 Unauthorized`          | Missing or invalid JWT                                                                                                            |
| `403 Forbidden`             | Authenticated but insufficient role or ownership                                                                                  |
| `404 Not Found`             | Resource not found                                                                                                                |
| `409 Conflict`              | Business conflict (e.g., overlapping appointment, duplicate username)                                                             |
| `410 Gone`                  | Time-limited resource expired (e.g., export download link after 24 h)                                                             |
| `422 Unprocessable Entity`  | FluentValidation failure; body: `{ errors: { field: [message] } }`                                                                |
| `429 Too Many Requests`     | Rate limit exceeded (token bucket on login, global API rate limits). Response includes `Retry-After` header (FR-02.10a, NFR-S06). |
| `500 Internal Server Error` | Generic server error (no internal details exposed — NFR-S08)                                                                      |

### Common Conventions

- **Pagination:** All list endpoints accept `?page=1&pageSize=20` (offset-based). Response wraps data in `{ items, totalCount, page, pageSize }`. **Future optimization:** The Audit Log endpoint may eventually migrate to cursor-based (keyset) pagination if the table grows beyond millions of rows, to avoid O(n) index scans on deep pages. For now, offset pagination is sufficient for a single-clinic deployment.
- **Soft Delete:** Archiving (`DELETE`) sets `IsArchived = true` and `ArchivedAt`. Records are preserved in full.
- **Monetary values:** All amounts are `DECIMAL(10,2)`, Philippine Peso (PHP ₱). Never floating-point (NFR-R01).
- **Computed balance:** `OutstandingBalance = PatientShare − SUM(non-voided payments)`. Never stored as a column (NFR-R04, BR-02.1).
- **Computed stock:** `CurrentStock = SUM(SupplyStockLedger.QuantityChange)`. Never stored (BR-05.1).
- **CQRS:** All state-mutating endpoints map to a MediatR **Command**; all read endpoints map to a MediatR **Query**.
- **Validation:** FluentValidation pipeline runs on every request. Returns `422` with field-level messages.
- **Audit logging:** All create/update/archive/restore/delete/login/logout/password-change actions are automatically recorded via an EF Core `SaveChanges` interceptor (FR-11).
- **OR number uniqueness:** Enforced at the database via a UNIQUE constraint on `Payments.ORNumber` (non-voided). Duplicate returns `422` with message `"OR number is already recorded in the system"`.
- **Transactions:** All multi-entity mutations are wrapped in a single EF Core transaction (NFR-R02).
- **Error format:** `{ type, title, status, traceId, errors? }` (Problem Details RFC 7807).

### Soft Delete & Archive Semantics

In this API, `DELETE` operations perform **soft deletes (archiving)**, not permanent record destruction:

- `DELETE /api/patients/{id}` archives the patient; the record remains in the database with `IsArchived = true`.
- Correspondingly, `POST /api/patients/{id}/restore` restores the archived patient.
- This pattern is consistently applied across all archivable entities (users, treatments, inventory items, suppliers, etc.).
- **Rationale:** EF Core Global Query Filters suppress archived records from normal queries (NFR-R03). The database retains full audit history for compliance (FR-11, BR-05.3).
- **Frontend implication:** DELETE does not permanently destroy data. Confirmation dialogs should say "Archive" not "Delete", and a restore option must remain available (at least to Administrators).

### Concurrency Control & Optimistic Locking (ETags)

Financial and clinical mutations run concurrently (multiple staff may edit a draft billing simultaneously). While EF Core transactions (NFR-R02) guarantee atomicity within a single request, **optimistic concurrency prevents lost updates across concurrent requests**:

- **Approach:** `RowVersion` (SQL Server) or PostgreSQL's built-in row versioning via EF Core's `[Timestamp]` attribute.
- **Implementation:** On successful GET of a resource (e.g., `GET /api/billings/{id}`), the response includes an `ETag` header or a `rowVersion` field in the body.
- **Usage on PUT:** The client sends the same `ETag` value (or `rowVersion`) in an `If-Match` request header. If the resource has changed since the GET, the server returns `409 Conflict` and the client must retry (refresh, merge changes, or notify the user).
- **Applies to:** All `PUT` endpoints on financial data (`PUT /api/billings/{id}`, `PUT /api/installment-entries/{id}/waive`) and clinical mutations (`PUT /api/appointments/{id}`, `PUT /api/treatments/{id}`).
- **Not required for:** POST (create) endpoints, which are inherently safe from lost updates, and status-transition endpoints (see **Idempotency** section below).
- **Example request/response:**

  ```http
  GET /api/billings/123
  Response:
    200 OK
    ETag: "W/\"12345\""
    { id: 123, discountAmount: 500, rowVersion: "12345" }

  PUT /api/billings/123
  If-Match: "W/\"12345\""
  { id: 123, discountAmount: 600, rowVersion: "12345" }
  Response: 200 OK (or 409 Conflict if rowVersion has changed)
  ```

### Idempotency & Safe State Transitions

State transition endpoints (e.g., `POST /api/appointments/{id}/complete`) **must be idempotent** to prevent double-effects if a client retries due to network timeout:

- **Requirement:** If the same state-transition request is received twice (same user, same endpoint, same parameters), the second call must return `200 OK` with the same result as the first, **not** re-trigger the associated business logic (e.g., do not generate billing twice).
- **Implementation:** Before executing the state transition, check the target state. If the appointment is already `Completed`, return `200 OK` with the existing result (billing ID). If the state is incompatible with the transition (e.g., attempting to Complete an already-Cancelled appointment), return `409 Conflict` with a clear message.
- **Applies to all transitions:**
  - `POST /api/appointments/{id}/confirm` — idempotent (Pending → Confirmed)
  - `POST /api/appointments/{id}/start` — idempotent (Confirmed → In Progress)
  - `POST /api/appointments/{id}/complete` — idempotent (In Progress → Completed); must return the billing ID on both calls
  - `POST /api/appointments/{id}/cancel` — idempotent (Pending/Confirmed → Cancelled)
  - `POST /api/billings/{id}/finalize` — idempotent (Draft → Final)
  - Similar for all other state transitions.
- **Error cases:** If a transition is impossible from the current state, always return `409 Conflict` (never `200` with a misleading message).
- **Consequence:** Background jobs (e.g., reminder SMS on appointment confirmation) and downstream events (e.g., SignalR broadcasts) must **not** be re-triggered on idempotent re-calls. Implement these as side-effects of the state transition action itself; query the database to ensure the event has not already fired.

### Caching & ETags for Read-Heavy Endpoints

Endpoints that return infrequently-changing reference data (e.g., `GET /api/treatments`, `GET /api/inventory`, `GET /api/providers`, `GET /api/clinic-settings`) should include HTTP caching headers and/or ETag responses to reduce unnecessary database queries:

- **Cache-Control Strategy:**
  - Read-only reference data (`GET /api/treatments`, `GET /api/inventory`): `Cache-Control: public, max-age=3600` (1-hour client-side cache). Invalidate by version bump or manual cache clear on admin settings change.
  - Configuration data (`GET /api/clinic-settings`): `Cache-Control: private, max-age=300` (5-minute cache); more volatile due to admin changes.
  - Patient lists and appointment calendars: `Cache-Control: private, no-cache` (revalidate on every request, but allow conditional GET via ETag).

- **ETag Implementation:** Return an `ETag` header (hash of the response body or a version field) on all GET responses. The client sends `If-None-Match: {ETag}` on subsequent requests. If the ETag matches, return `304 Not Modified` (no body) instead of `200 OK` with full data. This saves bandwidth even if the response is not client-cached.

- **Vue 3 Frontend Implication:** Configure `axios` or `fetch` interceptors to:
  1. Store `ETag` headers from successful GET responses.
  2. Automatically include `If-None-Match` on repeat requests.
  3. Handle `304 Not Modified` by reusing the cached payload.
  4. On `200 OK`, update the local cache and `ETag` reference.

- **Example request/response cycle:**

  ```http
  GET /api/treatments
  Response:
    200 OK
    ETag: "\"v1:abc123\""
    Cache-Control: public, max-age=3600
    { items: [...], totalCount: 150 }

  GET /api/treatments (5 minutes later)
  If-None-Match: "\"v1:abc123\""
  Response:
    304 Not Modified
    (no body — client reuses cached data)

  GET /api/treatments (after admin edits a treatment)
  If-None-Match: "\"v1:abc123\""
  Response:
    200 OK
    ETag: "\"v1:abc124\""
    Cache-Control: public, max-age=3600
    { items: [...], totalCount: 151 }
  ```

### ASP.NET Core 8 Implementation Notes

#### SignalR WebSocket Authentication Gotcha

The query-string JWT extraction (documented in Section 41) **must** be configured in `Program.cs` via `JwtBearerEvents.OnMessageReceived`. Browsers cannot send standard HTTP `Authorization: Bearer` headers during WebSocket upgrade. If this handler is not implemented, WebSocket connections will fail with `401 Unauthorized` even though the token is valid:

```csharp
// Program.cs
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters { ... };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Extract JWT from query string for WebSocket upgrades
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken) && context.Request.Path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
```

Omitting this handler is a common cause of WebSocket connection failures in SignalR applications.

#### File Upload Buffer Alignment (Nginx + Kestrel)

The clinic correctly specifies `client_max_body_size 25M` in Nginx (TECH_STACK_AND_ARCHITECTURE.md). However, **both** Kestrel (ASP.NET Core) and the request handler must allow ≥ 20 MB uploads:

1. **Kestrel default limit:** ~30 MB (`RequestSizeLimit`). This is usually sufficient, but explicitly set it in `Program.cs` to ensure consistency:

   ```csharp
   builder.Services.Configure<FormOptions>(options =>
   {
       options.MultipartBodyLengthLimit = 26214400; // 25 MB
   });

   builder.WebHost.ConfigureKestrel(options =>
   {
       options.Limits.MaxRequestBodySize = 26214400;
   });
   ```

2. **Per-endpoint override:** For `POST /api/patients/{id}/attachments`, annotate the controller or action:

   ```csharp
   [RequestSizeLimit(26214400)] // 25 MB
   [HttpPost]
   public async Task<IActionResult> UploadAttachment(Guid patientId, IFormFile file)
   {
       // FluentValidation will enforce max 20 MB per file and 500 MB total per patient
       // If the request exceeds the [RequestSizeLimit], a 413 Payload Too Large is thrown
       // BEFORE the action method runs.
   }
   ```

3. **Validation sequence:** Nginx → Kestrel → `[RequestSizeLimit]` → Action method → FluentValidation. If misconfigured, the API returns `413 Payload Too Large` before FluentValidation runs, bypassing the business-logic error messages.

#### Output Caching with Tag Invalidation (ASP.NET Core 8)

For read-heavy endpoints like `GET /api/treatments` and `GET /api/inventory`, use ASP.NET Core 8's **OutputCache** middleware with cache-tag invalidation instead of manual ETag/304 handling:

```csharp
// Program.cs
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("TreatmentsCachePolicy", builder =>
        builder
            .Expire(TimeSpan.FromHours(1))
            .Tag("treatments-catalog"));

    options.AddPolicy("InventoryCachePolicy", builder =>
        builder
            .Expire(TimeSpan.FromHours(1))
            .Tag("inventory-stock"));
});

app.UseOutputCache();

// In TreatmentsController
[HttpGet]
[OutputCache(PolicyName = "TreatmentsCachePolicy")]
public async Task<IActionResult> GetTreatments(...)
{
    // Cached for 1 hour or until invalidated
}

// When Admin updates a treatment
[HttpPut]
public async Task<IActionResult> UpdateTreatment(Guid id, ...)
{
    var result = await _mediator.Send(new UpdateTreatmentCommand(...));

    // Invalidate the cache by tag
    _outputCacheStore.InvalidateByTagAsync("treatments-catalog", HttpContext.RequestAborted);

    return Ok(result);
}
```

**Benefits:**

- Automatic ETag and `304 Not Modified` response handling.
- No manual `If-None-Match` header management.
- Tag-based invalidation (invalidate the entire category when any treatment is updated).
- Built into ASP.NET Core 8; no third-party caching library required.

---

## 2. Infrastructure

> **Phase:** 0 (scaffolded at project creation)  
> **Controller:** None (ASP.NET Core Health Checks middleware)

| Method | Path      | Auth   | Roles | BRD     | Description                                                                                                                                                                       |
| ------ | --------- | ------ | ----- | ------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`  | `/health` | Public | —     | NFR-M03 | Returns `{ status: "Healthy" \| "Degraded" \| "Unhealthy", checks: [...] }`. Checks include PostgreSQL connectivity and Valkey connectivity. Suitable for use by uptime monitors. |

---

## 3. Authentication — Clinic Staff

> **Phase:** 1A  
> **Controller:** `AuthController`  
> **Base path:** `/api/auth`

| Method | Path                         | Auth   | Roles     | BRD                  | Description                                                                                                                                                                                                                                            |
| ------ | ---------------------------- | ------ | --------- | -------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `POST` | `/api/auth/login`            | Public | —         | FR-01.2.1, FR-01.2.4 | Authenticates a clinic staff user with `{ username, password }`. Returns `{ accessToken, refreshToken, expiresAt, user: { id, role, fullName } }`. Records `LastLoginAt` on the user. After 5 consecutive failures, the account is locked (FR-01.2.2). |
| `POST` | `/api/auth/logout`           | Bearer | All Staff | FR-01.2.5            | Invalidates the current access token by adding its JTI to the Valkey revocation set. Body: `{ refreshToken }`. Returns `204`.                                                                                                                          |
| `POST` | `/api/auth/refresh`          | Public | —         | FR-01.2.4            | Exchanges a valid, non-revoked refresh token for a new access token and refresh token pair. Returns `{ accessToken, refreshToken, expiresAt }`.                                                                                                        |
| `POST` | `/api/auth/recover/initiate` | Public | —         | FR-01.3.2            | Step 1 of password recovery. Body: `{ username }`. Returns the user's two security question texts (masked). Never reveals whether the username exists (prevents enumeration).                                                                          |
| `POST` | `/api/auth/recover/confirm`  | Public | —         | FR-01.3.2, FR-01.3.3 | Step 2 of password recovery. Body: `{ username, answer1, answer2, newPassword }`. Verifies both BCrypt security answers, then sets the new password hash. Returns `204`. Records the action in the audit log.                                          |

**Notes:**

- Passwords stored as BCrypt hash, cost factor ≥ 12 (NFR-S01). Plaintext never stored, logged, or returned.
- Password complexity: minimum 8 characters, at least one letter and one number (FR-01.3.3).
- Email-based recovery link (`/api/auth/recover/email-link`) is sent when a user has a valid email and clinic SMTP is configured (FR-01.3.5) — this endpoint triggers a time-limited reset token sent to the user's registered email address.

---

## 4. User Management

> **Phase:** 1A  
> **Controller:** `UsersController`  
> **Base path:** `/api/users`  
> **Access:** Administrator only for all mutating operations (FR-01.1.2).

| Method   | Path                                   | Auth   | Roles     | BRD                             | Description                                                                                                                                                                                                                                                                                          |
| -------- | -------------------------------------- | ------ | --------- | ------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`    | `/api/users`                           | Bearer | Admin     | FR-01.1                         | Returns paginated list of clinic users. Query params: `?search=` (name, username), `?role=` (Admin\|Dentist\|Staff), `?status=` (Active\|Archived).                                                                                                                                                  |
| `POST`   | `/api/users`                           | Bearer | Admin     | FR-01.1.3, FR-01.1.4            | Creates a new clinic user. Body includes: `username`, `password`, `role`, personal fields, `specialization`/`prcLicenseNumber`/`prcLicenseExpiry` (required when `role = Dentist`), `securityQuestion1`, `securityAnswer1`, `securityQuestion2`, `securityAnswer2`. Returns `201` with created user. |
| `GET`    | `/api/users/{id}`                      | Bearer | Admin     | FR-01.1                         | Returns full user record. Sensitive fields (password hash, security answer hashes) are never returned.                                                                                                                                                                                               |
| `PUT`    | `/api/users/{id}`                      | Bearer | Admin     | FR-01.1.3, FR-01.1.4            | Updates user profile fields. Does not change password.                                                                                                                                                                                                                                               |
| `DELETE` | `/api/users/{id}`                      | Bearer | Admin     | FR-01.1.6, FR-01.1.7, FR-01.1.8 | Archives (soft-deletes) the user. Returns `422` if this would remove the last active Administrator (FR-01.1.7). Returns `403` if the authenticated admin is archiving their own account (FR-01.1.8).                                                                                                 |
| `POST`   | `/api/users/{id}/restore`              | Bearer | Admin     | FR-01.1.6                       | Restores a previously archived user, setting status back to Active.                                                                                                                                                                                                                                  |
| `POST`   | `/api/users/{id}/unlock`               | Bearer | Admin     | FR-01.2.3                       | Clears the account lockout, resetting the failed-attempt counter. Returns `204`.                                                                                                                                                                                                                     |
| `PUT`    | `/api/users/me/password`               | Bearer | All Staff | FR-01.3.3, BR-04.3              | Allows the authenticated user to change their own password. Body: `{ currentPassword, newPassword }`. Validates BCrypt of `currentPassword` before accepting.                                                                                                                                        |
| `PUT`    | `/api/users/{id}/security-questions`   | Bearer | Admin     | FR-01.3.1                       | Administrator updates the security questions and hashed answers for a user. Body: `{ question1, answer1, question2, answer2 }`.                                                                                                                                                                      |
| `POST`   | `/api/users/{id}/force-reset-password` | Bearer | Admin     | BR-04.3                         | Administrator forces a password reset for any user. Body: `{ newPassword }`. Recorded in audit log.                                                                                                                                                                                                  |

---

## 5. Clinic Settings

> **Phase:** 1A (core fields); 4A (SMS/SMTP); 6A (booking, appointment types); 7A (payment gateway)  
> **Controller:** `ClinicSettingsController`  
> **Base path:** `/api/clinic-settings`  
> **Access:** Administrator only for all PUT operations (FR-02.1).

### 5.1 Core Settings

| Method | Path                   | Auth   | Roles     | Phase | BRD                       | Description                                                                                                                                                                                             |
| ------ | ---------------------- | ------ | --------- | ----- | ------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`  | `/api/clinic-settings` | Bearer | All Staff | 1A    | FR-02.1–FR-02.9, FR-02.15 | Returns the single clinic settings record: identity, compliance fields, operating schedule, slot duration, VAT config, security config, inventory expiry threshold, and installment reminder lead-time. |
| `PUT`  | `/api/clinic-settings` | Bearer | Admin     | 1A    | FR-02.1–FR-02.9, FR-02.15 | Updates core clinic settings fields.                                                                                                                                                                    |

### 5.2 SMS Gateway Settings

| Method | Path                            | Auth   | Roles | Phase | BRD      | Description                                                                                                                                |
| ------ | ------------------------------- | ------ | ----- | ----- | -------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
| `GET`  | `/api/clinic-settings/sms`      | Bearer | Admin | 4A    | FR-02.10 | Returns SMS gateway configuration (provider name, API base URL, sender name). API key is masked — only the last 4 characters are returned. |
| `PUT`  | `/api/clinic-settings/sms`      | Bearer | Admin | 4A    | FR-02.10 | Updates SMS gateway settings. Body: `{ providerName, apiBaseUrl, apiKey, senderName }`.                                                    |
| `POST` | `/api/clinic-settings/sms/test` | Bearer | Admin | 4A    | FR-02.10 | Sends a test SMS to the configured sender number. Returns `{ success: bool, message: string }`.                                            |

### 5.3 SMTP Email Settings

| Method | Path                             | Auth   | Roles | Phase | BRD      | Description                                                                                                      |
| ------ | -------------------------------- | ------ | ----- | ----- | -------- | ---------------------------------------------------------------------------------------------------------------- |
| `GET`  | `/api/clinic-settings/smtp`      | Bearer | Admin | 4A    | FR-02.11 | Returns SMTP settings (host, port, encryption, sender email, sender name, username). Password is never returned. |
| `PUT`  | `/api/clinic-settings/smtp`      | Bearer | Admin | 4A    | FR-02.11 | Updates SMTP settings. Body: `{ host, port, encryption, senderEmail, senderName, username, password }`.          |
| `POST` | `/api/clinic-settings/smtp/test` | Bearer | Admin | 4A    | FR-02.11 | Sends a test email to the configured sender address. Returns `{ success: bool, message: string }`.               |

### 5.4 Online Booking Settings

| Method   | Path                                                  | Auth   | Roles | Phase | BRD               | Description                                                                                                                                                  |
| -------- | ----------------------------------------------------- | ------ | ----- | ----- | ----------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `GET`    | `/api/clinic-settings/booking`                        | Bearer | Admin | 6A    | FR-02.13–FR-02.16 | Returns booking configuration: approval mode, deposit amount, deposit label, forfeiture policy text, max dependents, manual deposit toggle and instructions. |
| `PUT`    | `/api/clinic-settings/booking`                        | Bearer | Admin | 6A    | FR-02.13–FR-02.16 | Updates online booking configuration fields.                                                                                                                 |
| `GET`    | `/api/clinic-settings/booking/appointment-types`      | Bearer | Admin | 6A    | FR-02.16          | Returns the list of configured portal appointment types (label, description, duration, required specialization, status).                                     |
| `POST`   | `/api/clinic-settings/booking/appointment-types`      | Bearer | Admin | 6A    | FR-02.16          | Creates a new portal appointment type. At least one active type must remain for booking to be available. Returns `201`.                                      |
| `PUT`    | `/api/clinic-settings/booking/appointment-types/{id}` | Bearer | Admin | 6A    | FR-02.16          | Updates a portal appointment type.                                                                                                                           |
| `DELETE` | `/api/clinic-settings/booking/appointment-types/{id}` | Bearer | Admin | 6A    | FR-02.16          | Archives (deactivates) a portal appointment type. Returns `422` if deactivating would leave zero active types.                                               |

### 5.5 Payment Gateway Settings

| Method | Path                                | Auth   | Roles | Phase | BRD      | Description                                                                                                                                                                                              |
| ------ | ----------------------------------- | ------ | ----- | ----- | -------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`  | `/api/clinic-settings/gateway`      | Bearer | Admin | 7A    | FR-02.12 | Returns gateway config: provider, environment (sandbox\|production). API public key and secret key are masked — only last 4 characters returned. Raw keys never appear in any response or log (NFR-S08). |
| `PUT`  | `/api/clinic-settings/gateway`      | Bearer | Admin | 7A    | FR-02.12 | Updates payment gateway configuration. API keys are AES-encrypted at rest before storage (NFR-S05).                                                                                                      |
| `POST` | `/api/clinic-settings/gateway/test` | Bearer | Admin | 7A    | FR-02.12 | Tests connectivity to the configured payment gateway using stored credentials. Returns `{ success: bool, message: string }`.                                                                             |

---

## 6. Audit Log

> **Phase:** 1A  
> **Controller:** `AuditLogsController`  
> **Base path:** `/api/audit-logs`  
> **Access:** Administrator only (FR-11.5). Entries are immutable (FR-11.4).

| Method | Path                     | Auth   | Roles | BRD             | Description                                                                                                                                                                                                                                                                                                                                                                                                     |
| ------ | ------------------------ | ------ | ----- | --------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`  | `/api/audit-logs`        | Bearer | Admin | FR-11.1–FR-11.5 | Returns paginated audit log entries. Query params: `?startDate=`, `?endDate=`, `?userId=`, `?actionType=` (Create\|Update\|Archive\|Restore\|Delete\|Login\|Logout\|PasswordChange), `?entityType=` (Patient\|Appointment\|Billing\|Payment\|User\|...). Response includes: user name, action type, entity type, entity ID, human-readable description, changed fields (old/new values), IP address, timestamp. |
| `GET`  | `/api/audit-logs/export` | Bearer | Admin | FR-13.9         | Returns the audit log for a filtered range as a CSV download. Query params same as `GET /api/audit-logs`. Returns `Content-Disposition: attachment; filename="audit-log.csv"`.                                                                                                                                                                                                                                  |

---

## 7. Patient Management

> **Phase:** 2A  
> **Controller:** `PatientsController`  
> **Base path:** `/api/patients`

| Method   | Path                         | Auth   | Roles        | BRD                        | Description                                                                                                                                                                                                                                                    |
| -------- | ---------------------------- | ------ | ------------ | -------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`    | `/api/patients`              | Bearer | All Staff    | FR-03.5                    | Returns paginated patient list. Query params: `?search=` (last name, first name, contact number, patient ID), `?status=` (Active\|Archived, default: Active). Each item: patient ID, full name, birthdate, sex, mobile, status.                                |
| `POST`   | `/api/patients`              | Bearer | All Staff    | FR-03.1–FR-03.4, FR-03.1.6 | Registers a new patient. Required: `lastName`, `firstName`, `birthdate`, `sex`, `contactNumber`, `privacyConsentAcknowledged: true`, `consentDate`. On success, initializes 52 tooth records per age-bracket logic (FR-03.1.6). Returns `201` with patient ID. |
| `GET`    | `/api/patients/{id}`         | Bearer | All Staff    | FR-03.2–FR-03.4            | Returns full patient record including: personal info, contact info, emergency/guardian contact, dental history, medical history (allergies, medications, conditions, lifestyle, female fields), referral source, portal account status.                        |
| `PUT`    | `/api/patients/{id}`         | Bearer | All Staff    | FR-03.2–FR-03.4            | Updates patient profile fields (personal, contact, dental history, medical history).                                                                                                                                                                           |
| `DELETE` | `/api/patients/{id}`         | Bearer | Admin, Staff | FR-03.6.1–FR-03.6.3        | Archives the patient (soft delete). All linked records (appointments, billings, payments) are preserved. Only available when no In Progress appointments exist.                                                                                                |
| `POST`   | `/api/patients/{id}/restore` | Bearer | Admin, Staff | FR-03.6.1                  | Restores an archived patient to Active status.                                                                                                                                                                                                                 |

**Notes:**

- Medical history last-reviewed date and reviewed-by user are updated via `PUT /api/patients/{id}` by including `{ medicalHistoryReviewedAt, medicalHistoryReviewedById }` in the body (FR-03.4.13).
- Patient age is computed from `birthdate` on every read. No `Age` column is stored (FR-03.2.1).

---

## 8. Appointment Scheduling

> **Phase:** 2A  
> **Controller:** `AppointmentsController`  
> **Base path:** `/api/appointments`

### 8.1 Appointment CRUD

| Method | Path                     | Auth   | Roles     | BRD     | Description                                                                                                                                                                                                                                                                                                                                                                                      |
| ------ | ------------------------ | ------ | --------- | ------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `GET`  | `/api/appointments`      | Bearer | All Staff | FR-04.4 | Returns paginated appointment list. Query params: `?startDate=`, `?endDate=`, `?status=`, `?patientId=`, `?dentistId=`.                                                                                                                                                                                                                                                                          |
| `POST` | `/api/appointments`      | Bearer | All Staff | FR-04.1 | Creates a new appointment. Required: `patientId` (must be Active — BR-01.2), `dentistId` (must be Dentist or Admin role — BR-01.4), `scheduledAt`, `durationMinutes`, `plannedTreatments` (array, at least one — BR-01.3), `isWalkIn` (bool). Walk-in appointments may start directly In Progress (BR-01.5). Conflict check enforced before save (FR-04.1.4, BR-01.1). Returns `409` on overlap. |
| `GET`  | `/api/appointments/{id}` | Bearer | All Staff | FR-04.1 | Returns full appointment detail including: patient, dentist, status, planned treatments, vitals, procedures, billing reference, portal source flag, LOA number.                                                                                                                                                                                                                                  |
| `PUT`  | `/api/appointments/{id}` | Bearer | All Staff | FR-04.1 | Updates appointment fields (date/time, duration, chief complaint, LOA number) before it reaches Completed. Conflict check re-runs on date/time change.                                                                                                                                                                                                                                           |

### 8.2 Status Transitions

| Method | Path                                | Auth   | Roles          | BRD                       | Description                                                                                                                                                                          |
| ------ | ----------------------------------- | ------ | -------------- | ------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `POST` | `/api/appointments/{id}/confirm`    | Bearer | All Staff      | FR-04.2                   | Transitions: Pending → Confirmed. Triggers SMS/email confirmation notification (Hangfire job, Phase 4A). Returns `200`.                                                              |
| `POST` | `/api/appointments/{id}/start`      | Bearer | Dentist, Admin | FR-04.2                   | Transitions: Confirmed → In Progress. Patient is present and seated.                                                                                                                 |
| `POST` | `/api/appointments/{id}/complete`   | Bearer | Dentist, Admin | FR-04.2, FR-07.1.1        | Transitions: In Progress → Completed. Triggers automatic billing creation (FR-07.1.1). Treatment records become immutable (BR-03.1). Returns the created billing ID in the response. |
| `POST` | `/api/appointments/{id}/cancel`     | Bearer | All Staff      | FR-04.2, FR-04.2.2        | Transitions: Pending or Confirmed → Cancelled. Body: `{ reason }` (minimum 10 characters — FR-04.2.2). Triggers patient notification (Hangfire, Phase 4A).                           |
| `POST` | `/api/appointments/{id}/no-show`    | Bearer | Staff, Admin   | FR-04.2                   | Transitions: Confirmed → No Show.                                                                                                                                                    |
| `POST` | `/api/appointments/{id}/approve`    | Bearer | Staff, Admin   | FR-04.2, FR-02.13         | Transitions: Awaiting Approval → Pending (Approval Mode = Awaiting Approval). Slot is now blocked.                                                                                   |
| `POST` | `/api/appointments/{id}/reject`     | Bearer | Staff, Admin   | FR-04.2, BR-01.7, BR-01.8 | Transitions: Awaiting Approval → Rejected (terminal). Body: `{ reason }`. Patient is notified.                                                                                       |
| `POST` | `/api/appointments/{id}/reschedule` | Bearer | Staff, Admin   | FR-04.2                   | Creates a new Pending appointment (linked to this one) and marks this appointment as No Show. Body: `{ scheduledAt, durationMinutes }`. Patient is notified of new appointment.      |

### 8.3 Calendar & Vitals

| Method | Path                            | Auth   | Roles          | BRD     | Description                                                                                                                                                                                                                                                                                                                                   |
| ------ | ------------------------------- | ------ | -------------- | ------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`  | `/api/appointments/calendar`    | Bearer | All Staff      | FR-04.3 | Returns appointments for a date range formatted for calendar rendering. Query params: `?date=` (ISO date, used as anchor for day/week/month view), `?view=` (day\|week\|month), `?dentistId=`. Each appointment: id, patientName, scheduledAt, durationMinutes, status, treatments.                                                           |
| `PUT`  | `/api/appointments/{id}/vitals` | Bearer | Dentist, Staff | FR-04.5 | Records or updates pre-operative vital signs for a Confirmed or In Progress appointment (upsert semantics). Body: `{ systolic, diastolic, pulseRate, respiratoryRate, temperature?, spo2?, clinicalNotes? }`. Returns `200`. High-BP advisory (`systolic ≥ 180` or `diastolic ≥ 110`) is included in the response body as a flag (FR-04.5.5). |

---

## 9. Dental Charting

> **Phase:** 3A  
> **Controllers:** Nested under `PatientsController` for chart-level routes  
> **Base path:** `/api/patients/{patientId}/...`  
> **Access:** Dentist and Admin for mutations; All Staff for reads (FR-01.4, FR-05.3.1).

| Method | Path                                                       | Auth   | Roles          | BRD              | Description                                                                                                                                                                                                                                                                                                                                                                       |
| ------ | ---------------------------------------------------------- | ------ | -------------- | ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`  | `/api/patients/{patientId}/dental-chart`                   | Bearer | All Staff      | FR-05.1, FR-05.2 | Returns the full FDI dental chart for the patient: all 52 teeth with their active condition per surface (5 surfaces each), the In Progress procedure flag per tooth, and the active appointment ID if status = In Progress. Chart loads within 1 s (NFR-P03).                                                                                                                     |
| `GET`  | `/api/patients/{patientId}/teeth/{toothNumber}`            | Bearer | All Staff      | FR-05.2.1        | Returns tooth detail: FDI number, active condition per surface, full condition history (timeline of all `ToothConditionEntry` records per surface: date, condition, surfaces, recorded-by, appointment ID), remarks, and linked treatment records. `toothNumber` is an FDI two-digit integer (11–18, 21–28, 31–38, 41–48, 51–55, 61–65, 71–75, 81–85).                            |
| `POST` | `/api/patients/{patientId}/teeth/{toothNumber}/conditions` | Bearer | Dentist, Admin | FR-05.3, BR-03.2 | Appends a new `ToothConditionEntry` for the specified surface(s). Previous active entry for the **same surface(s)** is marked historical (inactive) — never deleted. Body: `{ surfaces: [string], condition: ConditionCode, remarks?: string, appointmentId?: guid }`. Returns `201`. Valid condition codes are the 19-condition FDI enumeration (UNE, PNT, SHD, DCF, RCT, etc.). |

---

## 10. Treatment Catalog

> **Phase:** 3A  
> **Controller:** `TreatmentsController`  
> **Base path:** `/api/treatments`  
> **Access:** Admin and Dentist for mutations; All Staff for reads (FR-06.1.8).

| Method   | Path                                               | Auth   | Roles          | BRD                 | Description                                                                                                                                                                                                                                                                                                                                     |
| -------- | -------------------------------------------------- | ------ | -------------- | ------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`    | `/api/treatments`                                  | Bearer | All Staff      | FR-06.1             | Returns paginated treatment catalog. Query params: `?search=`, `?category=`, `?scope=` (ToothSpecific\|Global), `?status=` (Active\|Archived, default Active). Each item includes: name, category, scope, base price, duration, requiresXRay, applicableConditions, resultingCondition, interimCondition, requiredSpecialization, procedureKit. |
| `POST`   | `/api/treatments`                                  | Bearer | Admin, Dentist | FR-06.1.1–FR-06.1.9 | Creates a treatment catalog entry. Body: `{ name, description, category, scope (ToothSpecific\|Global), basePrice, estimatedDurationMinutes, requiresXRay, applicableConditions?, resultingCondition?, interimCondition?, requiredSpecialization?, procedureKit?: [{ supplyItemId, defaultQuantity }] }`. Returns `201`.                        |
| `GET`    | `/api/treatments/{id}`                             | Bearer | All Staff      | FR-06.1             | Returns full treatment detail including procedure kit items.                                                                                                                                                                                                                                                                                    |
| `PUT`    | `/api/treatments/{id}`                             | Bearer | Admin, Dentist | FR-06.1             | Updates treatment catalog entry including procedure kit (replaces existing kit).                                                                                                                                                                                                                                                                |
| `DELETE` | `/api/treatments/{id}`                             | Bearer | Admin, Dentist | FR-06.1.7           | Archives the treatment. Archived treatments remain linked to historical records but do not appear in new appointment treatment selection.                                                                                                                                                                                                       |
| `GET`    | `/api/treatments/applicable`                       | Bearer | All Staff      | FR-05.4.1           | Returns filtered treatment list for a given tooth condition. Query: `?condition={code}&scope=ToothSpecific`. This is a **productivity filter only** — the main treatments endpoint continues to return all active treatments. The filter does not gate treatment selection (FR-05.4.1).                                                         |
| `GET`    | `/api/treatments/{id}/dentist-pricing`             | Bearer | Admin, Dentist | FR-06.1.6           | Returns all dentist-specific price overrides for this treatment.                                                                                                                                                                                                                                                                                |
| `PUT`    | `/api/treatments/{id}/dentist-pricing/{dentistId}` | Bearer | Admin, Dentist | FR-06.1.6           | Creates or updates a price override for a specific dentist–treatment pair. Body: `{ price }`.                                                                                                                                                                                                                                                   |
| `DELETE` | `/api/treatments/{id}/dentist-pricing/{dentistId}` | Bearer | Admin          | FR-06.1.6           | Removes the dentist-specific price override, reverting to base price.                                                                                                                                                                                                                                                                           |

---

## 11. Appointment Procedures

> **Phase:** 3A  
> **Controller:** `AppointmentProceduresController` (nested under Appointments)  
> **Access:** Dentist and Admin for mutations; All Staff for reads.

| Method   | Path                                                     | Auth   | Roles          | BRD                            | Description                                                                                                                                                                                                                                                                                                                                                                                       |
| -------- | -------------------------------------------------------- | ------ | -------------- | ------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`    | `/api/appointments/{id}/procedures`                      | Bearer | All Staff      | FR-06.2                        | Returns all tooth-specific procedures recorded for this appointment: tooth, surfaces, treatment, amount, conditionBefore, conditionAfter, completion (Complete\|InProgress), clinicalNotes.                                                                                                                                                                                                       |
| `POST`   | `/api/appointments/{id}/procedures`                      | Bearer | Dentist, Admin | FR-06.2.1, FR-06.2.3–FR-06.2.5 | Records a tooth-specific procedure. Body: `{ toothNumber, surfaces, treatmentId, amountCharged, conditionBefore, conditionAfter, completion (Complete\|InProgress), clinicalNotes? }`. Automatically applies resulting/interim condition to the tooth surface (FR-06.2.5). Appointment must be In Progress. Also auto-populates procedure kit consumption suggestions (FR-08.3.2). Returns `201`. |
| `PUT`    | `/api/appointments/{id}/procedures/{procedureId}`        | Bearer | Dentist, Admin | FR-06.2                        | Updates a procedure record before the appointment is Completed. After Completed, procedures are immutable (BR-03.1).                                                                                                                                                                                                                                                                              |
| `DELETE` | `/api/appointments/{id}/procedures/{procedureId}`        | Bearer | Dentist, Admin | FR-06.2                        | Removes a tooth-specific procedure from an In Progress appointment.                                                                                                                                                                                                                                                                                                                               |
| `GET`    | `/api/appointments/{id}/global-procedures`               | Bearer | All Staff      | FR-06.2.1                      | Returns all global (patient-level) procedures for this appointment.                                                                                                                                                                                                                                                                                                                               |
| `POST`   | `/api/appointments/{id}/global-procedures`               | Bearer | Dentist, Admin | FR-06.2.1                      | Records a global procedure (e.g., Oral Prophylaxis, Panoramic X-Ray). Body: `{ treatmentId, amountCharged, clinicalNotes? }`. No tooth or surface required. Returns `201`.                                                                                                                                                                                                                        |
| `DELETE` | `/api/appointments/{id}/global-procedures/{procedureId}` | Bearer | Dentist, Admin | FR-06.2                        | Removes a global procedure from an In Progress appointment.                                                                                                                                                                                                                                                                                                                                       |

---

## 12. Billing

> **Phase:** 3A  
> **Controller:** `BillingsController`  
> **Base path:** `/api/billings`  
> **Access:** Admin and Staff for create/finalize; Dentist for read; Admin only for void.

| Method   | Path                             | Auth   | Roles        | BRD       | Description                                                                                                                                                                                                                                                    |
| -------- | -------------------------------- | ------ | ------------ | --------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`    | `/api/appointments/{id}/billing` | Bearer | All Staff    | FR-07.1   | Returns the billing record associated with this appointment (if any), including computed `OutstandingBalance`. Returns `404` if no billing exists yet (appointment not yet Completed).                                                                         |
| `GET`    | `/api/billings/{id}`             | Bearer | All Staff    | FR-07.1   | Returns full billing detail: appointment reference, line items (procedures), subtotal, discount (amount + reason), taxAmount, totalAmount, hmoConverageAmount, patientShare, payments list, computed `OutstandingBalance`, status, installment plan reference. |
| `PUT`    | `/api/billings/{id}`             | Bearer | Admin, Staff | FR-07.1.3 | Updates a Draft billing (adjusts discount amount/reason). Returns `422` if billing is Finalized or beyond.                                                                                                                                                     |
| `PUT`    | `/api/billings/{id}/finalize`    | Bearer | Admin, Staff | FR-07.1.6 | Transitions billing from Draft → Final. Locks line items. Returns `200`.                                                                                                                                                                                       |
| `DELETE` | `/api/billings/{id}`             | Bearer | Admin        | FR-07.1.7 | Voids the billing. Body: `{ reason }` (required). Voided billing is retained with void timestamp, voided-by user, and reason. Cannot be un-voided.                                                                                                             |
| `GET`    | `/api/billings/{id}/pdf/soa`     | Bearer | All Staff    | FR-07.2   | Generates and returns the Statement of Account PDF for this billing (BIR-compliant format). `Content-Type: application/pdf`.                                                                                                                                   |

---

## 13. Payments

> **Phase:** 3A  
> **Controller:** `PaymentsController`  
> **Base path:** `/api/billings/{billingId}/payments` and `/api/payments/{id}`

| Method   | Path                          | Auth   | Roles        | BRD              | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 |
| -------- | ----------------------------- | ------ | ------------ | ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`    | `/api/billings/{id}/payments` | Bearer | All Staff    | FR-07.3          | Returns all payments (including voided) linked to this billing. Each payment: OR number, amount, paymentMethod, receivedAt, receivedBy, status (Valid\|Voided), voidedAt, voidReason.                                                                                                                                                                                                                                                                                                       |
| `POST`   | `/api/billings/{id}/payments` | Bearer | Admin, Staff | FR-07.3, FR-02.7 | Records a new payment. Body: `{ orNumber, amount, paymentMethod (Cash\|GCash\|Maya\|CreditCard\|DebitCard\|Check\|HMODirectPay\|BankTransfer), receivedAt, notes? }`. OR number uniqueness is enforced at the DB UNIQUE constraint level — duplicate returns `422 "OR number is already recorded in the system"`. Amount must not exceed current outstanding balance (BR-02.4). Billing transitions to Partially Paid or Fully Paid automatically (BR-02.8). Returns `201` with payment ID. |
| `GET`    | `/api/payments/{id}`          | Bearer | All Staff    | FR-07.3          | Returns a single payment record.                                                                                                                                                                                                                                                                                                                                                                                                                                                            |
| `DELETE` | `/api/payments/{id}`          | Bearer | Admin        | FR-07.4          | Voids the payment. Body: `{ reason }` (minimum 10 characters — FR-07.4.2). Voided payment is retained. Its OR number is retained as voided and never reused (FR-07.4.5). Outstanding balance is recomputed automatically. Returns `200`.                                                                                                                                                                                                                                                    |
| `GET`    | `/api/payments/{id}/pdf/or`   | Bearer | All Staff    | FR-07.3.6        | Generates and returns the Official Receipt PDF (BIR-compliant): clinic name, address, TIN, COR number, OR number, date, patient name, amount in words and figures, services description, payment method, cashier name. `Content-Type: application/pdf`.                                                                                                                                                                                                                                     |

---

## 14. HMO Providers & LOA

> **Phase:** 3A  
> **Controllers:** `ProvidersController`, nested under `PatientsController` and `BillingsController`

| Method   | Path                                           | Auth   | Roles        | BRD              | Description                                                                                                                                                                                                               |
| -------- | ---------------------------------------------- | ------ | ------------ | ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`    | `/api/providers`                               | Bearer | All Staff    | FR-14.5          | Returns list of HMO/insurance providers. Query: `?search=`, `?status=`.                                                                                                                                                   |
| `POST`   | `/api/providers`                               | Bearer | Admin        | FR-14.5          | Creates a new HMO provider record. Body: `{ name, address, contactNumber, email, notes? }`. Returns `201`.                                                                                                                |
| `GET`    | `/api/providers/{id}`                          | Bearer | All Staff    | FR-14.5          | Returns provider detail.                                                                                                                                                                                                  |
| `PUT`    | `/api/providers/{id}`                          | Bearer | Admin        | FR-14.5          | Updates provider information.                                                                                                                                                                                             |
| `DELETE` | `/api/providers/{id}`                          | Bearer | Admin        | FR-14.5          | Archives provider.                                                                                                                                                                                                        |
| `GET`    | `/api/providers/{id}/tariff`                   | Bearer | All Staff    | FR-14.1          | Returns the tariff schedule for this provider. Used as a reference during LOA entry when no explicit peso amount is printed on the LOA (FR-14.1 note). Response: list of `{ procedureName, authorizedAmount }`.           |
| `GET`    | `/api/patients/{patientId}/providers`          | Bearer | All Staff    | FR-14.5          | Returns HMO provider links for a patient: provider name, memberId, effectiveDate, note.                                                                                                                                   |
| `POST`   | `/api/patients/{patientId}/providers`          | Bearer | All Staff    | FR-14.5          | Links an HMO provider to the patient. Body: `{ providerId, memberId, effectiveDate, note? }`. Returns `201`.                                                                                                              |
| `DELETE` | `/api/patients/{patientId}/providers/{linkId}` | Bearer | Admin        | FR-14.5          | Removes the provider link from the patient.                                                                                                                                                                               |
| `GET`    | `/api/billings/{id}/loa`                       | Bearer | All Staff    | FR-14.1–FR-14.3  | Returns the LOA record linked to this billing (if any).                                                                                                                                                                   |
| `POST`   | `/api/billings/{id}/loa`                       | Bearer | Admin, Staff | FR-14.1, FR-14.2 | Records a Letter of Authorization for this billing. Body: `{ loaNumber, providerId, authorizedAmount, loaValidityDate, clinicalNotes? }`. HMO coverage amount may not exceed `authorizedAmount` (FR-14.2). Returns `201`. |
| `PUT`    | `/api/billings/{id}/loa`                       | Bearer | Admin, Staff | FR-14.1          | Updates an existing LOA record on a Draft billing.                                                                                                                                                                        |
| `GET`    | `/api/patients/{patientId}/loa-history`        | Bearer | All Staff    | FR-14.3          | Returns all LOA records ever recorded for this patient across all billings.                                                                                                                                               |

---

## 15. Patient File Attachments

> **Phase:** 3A  
> **Controller:** `AttachmentsController`  
> **Base paths:** `/api/patients/{patientId}/attachments`, `/api/attachments/{id}`  
> **Storage:** Server-local filesystem, patient-scoped directory. Access requires a valid clinic JWT.

| Method   | Path                                    | Auth   | Roles                                 | BRD                         | Description                                                                                                                                                                                                                                                                    |
| -------- | --------------------------------------- | ------ | ------------------------------------- | --------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `GET`    | `/api/patients/{patientId}/attachments` | Bearer | All Staff                             | FR-03.7.1–FR-03.7.4         | Returns the attachment list for this patient in reverse-chronological order: id, label, note, mimeType, fileSizeBytes, isSystemGenerated, uploadedAt, uploadedByName.                                                                                                          |
| `POST`   | `/api/patients/{patientId}/attachments` | Bearer | All Staff                             | FR-03.7.2–FR-03.7.3         | Uploads a file for this patient. Request: `multipart/form-data` with fields `file` (JPEG, PNG, or PDF; max 20 MB per file), `label` (required), `note?`. Returns `201` with the created attachment record. Returns `422` if total storage for the patient would exceed 500 MB. |
| `GET`    | `/api/attachments/{id}/download`        | Bearer | All Staff                             | FR-03.7.4–FR-03.7.5         | Returns the file with `Content-Disposition: attachment; filename="..."`. Requires a valid `Bearer` JWT. Unauthenticated requests return `401`. Files are NOT accessible via direct filesystem URL.                                                                             |
| `DELETE` | `/api/attachments/{id}`                 | Bearer | All Staff (own uploads) / Admin (any) | FR-03.7.4, FR-15.9, FR-16.7 | Deletes an attachment. Body: `{ reason }` (required, audit-logged). Returns `403` if `IsSystemGenerated == true` with message `"System-generated attachments cannot be deleted directly. Void the related prescription or certificate instead."`                               |

---

## 16. Inventory & Supply Management

> **Phase:** 4A  
> **Controller:** `InventoryController`  
> **Base path:** `/api/inventory`  
> **Access:** Admin and Staff for mutations; Dentist excluded (FR-01.4).

| Method   | Path                                 | Auth   | Roles                 | BRD                  | Description                                                                                                                                                                                                                                                                                |
| -------- | ------------------------------------ | ------ | --------------------- | -------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `GET`    | `/api/inventory`                     | Bearer | Admin, Staff          | FR-08.1              | Returns paginated supply item catalog. Query params: `?search=`, `?category=`, `?tier=` (Tracked\|BulkManaged), `?status=` (Active\|Archived), `?lowStock=true`, `?nearExpiry=true`. Each item includes computed `currentStock` (`SUM(ledger)`), `criticalQuantity`, tier, unit.           |
| `POST`   | `/api/inventory`                     | Bearer | Admin, Staff          | FR-08.1.1            | Creates a supply item. Body: `{ name, description, category, unit, preferredSupplierId?, unitCost, criticalQuantity, tier (Tracked\|BulkManaged) }`. Returns `201`.                                                                                                                        |
| `GET`    | `/api/inventory/{id}`                | Bearer | Admin, Staff          | FR-08.1              | Returns full supply item detail including current computed stock and ledger summary.                                                                                                                                                                                                       |
| `PUT`    | `/api/inventory/{id}`                | Bearer | Admin, Staff          | FR-08.1              | Updates supply item fields.                                                                                                                                                                                                                                                                |
| `DELETE` | `/api/inventory/{id}`                | Bearer | Admin                 | FR-08.1              | Archives the supply item.                                                                                                                                                                                                                                                                  |
| `POST`   | `/api/inventory/{id}/receive`        | Bearer | Admin, Staff          | FR-08.2, FR-08.2.3   | Records a stock receipt (ledger entry, `changeType = Receipt`). Body: `{ quantity, lotNumber, unitCost, supplierId, remarks? }`. `expiryDate` and `lotNumber` are **required** for items in Anesthetic and Medication categories (FR-08.2.3). Returns `201`.                               |
| `POST`   | `/api/inventory/{id}/adjust`         | Bearer | Admin, Staff          | FR-08.2.2, FR-08.3.5 | Records a manual stock adjustment (Physical Count or correction). Body: `{ quantityChange, changeType (Adjustment\|Expired\|Voided), reason }`. `reason` is required for Adjustment and Voided types. Returns `201`.                                                                       |
| `GET`    | `/api/inventory/{id}/ledger`         | Bearer | Admin, Staff          | FR-08.2              | Returns the full stock ledger for a supply item: each entry showing change type, quantity change, lot number, expiry date, reference, remarks, recorded by, recorded at.                                                                                                                   |
| `GET`    | `/api/appointments/{id}/consumption` | Bearer | All Staff             | FR-08.3              | Returns all supply items recorded as consumed during this appointment.                                                                                                                                                                                                                     |
| `POST`   | `/api/appointments/{id}/consumption` | Bearer | Dentist, Staff, Admin | FR-08.3.1, FR-08.3.4 | Records supply consumption for an In Progress appointment. Body: `{ items: [{ supplyItemId, quantity, remarks? }] }`. Items exceeding current stock return a warning in the response (not a blocking error) with message requiring a mandatory `remarks` field (FR-08.3.4). Returns `201`. |

---

## 17. Suppliers

> **Phase:** 4A  
> **Controller:** `SuppliersController`  
> **Base path:** `/api/suppliers`  
> **Access:** Admin and Staff.

| Method   | Path                  | Auth   | Roles        | BRD       | Description                                                                                                         |
| -------- | --------------------- | ------ | ------------ | --------- | ------------------------------------------------------------------------------------------------------------------- |
| `GET`    | `/api/suppliers`      | Bearer | Admin, Staff | FR-08.4   | Returns paginated supplier list. Query: `?search=`, `?status=`.                                                     |
| `POST`   | `/api/suppliers`      | Bearer | Admin, Staff | FR-08.4.1 | Creates a supplier. Body: `{ name, address, contactNumber, email, deliveryDays: [string], notes? }`. Returns `201`. |
| `GET`    | `/api/suppliers/{id}` | Bearer | Admin, Staff | FR-08.4   | Returns supplier detail.                                                                                            |
| `PUT`    | `/api/suppliers/{id}` | Bearer | Admin, Staff | FR-08.4   | Updates supplier information.                                                                                       |
| `DELETE` | `/api/suppliers/{id}` | Bearer | Admin        | FR-08.4   | Archives supplier.                                                                                                  |

---

## 18. Notifications & Preferences

> **Phase:** 4A  
> **Controller:** `NotificationsController`  
> **Base path:** `/api/notifications`

| Method | Path                                         | Auth   | Roles     | BRD              | Description                                                                                                                                                                                                                                    |
| ------ | -------------------------------------------- | ------ | --------- | ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`  | `/api/notifications/{patientId}/preferences` | Bearer | All Staff | FR-10.9          | Returns per-patient notification preferences: SMS opt-in per event type, email opt-in per event type. Event types: Confirmation, Reschedule, Cancellation, Reminder, InstallmentDue, RecallReminder.                                           |
| `PUT`  | `/api/notifications/{patientId}/preferences` | Bearer | All Staff | FR-10.9          | Updates notification preferences for a patient. Body: `{ smsEnabled: bool, emailEnabled: bool, preferences: [{ eventType, smsEnabled, emailEnabled }] }`.                                                                                      |
| `GET`  | `/api/notifications/log`                     | Bearer | Admin     | FR-10.7, FR-10.8 | Returns notification delivery log: appointment/patient reference, event type, channel (SMS\|Email), status (Sent\|Failed\|Pending), attempts, last attempt timestamp. Paginated. Query: `?patientId=`, `?status=`, `?startDate=`, `?endDate=`. |

**Background Jobs (Hangfire — not HTTP endpoints):**

- `AppointmentReminderJob` — daily at configurable time; sends 24-hour reminder SMS/email to patients with Confirmed/Pending appointments tomorrow (FR-10.6).
- `RecallReminderJob` — daily; evaluates all Active patients with no upcoming appointment whose last completed appointment (or registration date if none) is ≥ 6 months ago. Respects 30-day re-send guard and patient opt-out (FR-10.10).
- `InstallmentReminderJob` — daily; sends SMS/email to patients N days before each installment due date (FR-07.5.6, FR-02.15). Activated Phase 5A.
- `LowStockCheckJob` — every 6 hours; pushes low-stock alert counts to `DashboardHub` (FR-09.1.F).
- `NearExpiryCheckJob` — daily at 06:00; pushes near-expiry alert counts to `DashboardHub` (FR-09.1.F).

---

## 19. Dashboard

> **Phase:** 2A (base widgets); 3A (revenue, chart widgets); 4A (inventory alerts)  
> **Controller:** `DashboardController`  
> **Base path:** `/api/dashboard`

| Method | Path                     | Auth   | Roles     | BRD     | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
| ------ | ------------------------ | ------ | --------- | ------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `GET`  | `/api/dashboard/widgets` | Bearer | All Staff | FR-09.1 | Returns all dashboard widget data for the authenticated user's role. Response is a composite object with role-sensitive sections: `{ todayAppointments, countToday, countThisMonth, activePatientCount, pendingCount, revenue?: {...}, revenueTrend?: [...], newPatientsThisMonth?: number, statusBreakdown?: [...], topTreatments?: [...], lowStockAlerts?: [...], nearExpiryAlerts?: [...] }`. Role-inaccessible sections are omitted, not nulled (FR-09.1.A.6). |

---

## 20. Reports & Analytics

> **Phase:** 2A (RPT-01); 3A (RPT-04–08, 13–14, OR Register); 4A (RPT-02–03, 09–12)  
> **Controller:** `ReportsController`  
> **Base path:** `/api/reports`

All report endpoints accept common query parameters: `?startDate=`, `?endDate=`, `?format=` (json\|pdf\|csv). PDF format returns `Content-Type: application/pdf`. Reports must complete within 5 s for 12 months of data (NFR-P04).

| Method | Path                           | Auth   | Roles          | Phase | BRD       | Description                                                                                                                                                                                                   |
| ------ | ------------------------------ | ------ | -------------- | ----- | --------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`  | `/api/reports/rpt-01`          | Bearer | All Staff      | 2A    | RPT-01    | Daily Appointment List. Params: `?date=` (ISO date). Returns appointments for the date grouped by dentist, with status and treatments.                                                                        |
| `GET`  | `/api/reports/rpt-02`          | Bearer | Admin, Staff   | 4A    | RPT-02    | Patient Masterlist. Returns all active patients with demographics and contact info. Supports `?status=` filter.                                                                                               |
| `GET`  | `/api/reports/rpt-03`          | Bearer | Admin, Dentist | 4A    | RPT-03    | Patient Treatment History. Params: `?patientId=` (required), `?startDate=`, `?endDate=`. Complete treatment records for the patient.                                                                          |
| `GET`  | `/api/reports/rpt-04`          | Bearer | Admin          | 3A    | RPT-04    | Revenue Report. Total collections by period broken down by payment method, gross vs. net after HMO deductions.                                                                                                |
| `GET`  | `/api/reports/rpt-05`          | Bearer | Admin          | 3A    | RPT-05    | Billing Register. All billings for a date range with status.                                                                                                                                                  |
| `GET`  | `/api/reports/rpt-06`          | Bearer | Admin          | 3A    | RPT-06    | Accounts Receivable. Billings with outstanding balance (Final and Partially Paid), with aging.                                                                                                                |
| `GET`  | `/api/reports/rpt-07`          | Bearer | Admin          | 3A    | RPT-07    | Dentist Production Report. Procedures per dentist for the period with amounts and treatment categories. Params: `?dentistId=` (optional filter).                                                              |
| `GET`  | `/api/reports/rpt-08`          | Bearer | Admin, Dentist | 3A    | RPT-08    | Treatment Frequency Report. Most-performed procedures ranked by count and total revenue. Dentist sees only own records.                                                                                       |
| `GET`  | `/api/reports/rpt-09`          | Bearer | Admin, Staff   | 4A    | RPT-09    | Inventory Status. Current stock levels for all items highlighting low-stock and near-expiry.                                                                                                                  |
| `GET`  | `/api/reports/rpt-10`          | Bearer | Admin, Staff   | 4A    | RPT-10    | Inventory Consumption Report. Supply items consumed per period, linked to appointments.                                                                                                                       |
| `GET`  | `/api/reports/rpt-11`          | Bearer | Admin, Staff   | 4A    | RPT-11    | No-Show & Cancellation Report. Appointments with No-Show or Cancelled status with reasons.                                                                                                                    |
| `GET`  | `/api/reports/rpt-12`          | Bearer | Admin          | 4A    | RPT-12    | New Patient Acquisition. New patients registered per period with referral source breakdown.                                                                                                                   |
| `GET`  | `/api/reports/rpt-13`          | Bearer | Admin, Staff   | 3A    | RPT-13    | HMO / Provider Claims Summary. HMO-covered billings per provider: patient, LOA number, authorized amount, applied coverage, patient share, appointment date.                                                  |
| `GET`  | `/api/reports/rpt-14`          | Bearer | Admin          | 3A    | RPT-14    | Daily Collection Report. All payments received on a selected date: OR number, patient, services, amount, payment method, cashier. BIR-aligned daily totals with grand total row. Params: `?date=` (ISO date). |
| `GET`  | `/api/reports/or-register`     | Bearer | Admin          | 3A    | FR-09.3   | Official Receipt Register. Sequential list of all issued ORs with status (Valid/Voided). Paginated.                                                                                                           |
| `GET`  | `/api/reports/or-register/pdf` | Bearer | Admin          | 3A    | FR-09.3.2 | Exports the OR register as PDF. Same query params as `or-register`. BIR compliance export.                                                                                                                    |

---

## 21. Orthodontic Progress Notes

> **Phase:** 5A  
> **Controller:** Nested under `AppointmentsController`  
> **Base path:** `/api/appointments/{appointmentId}/ortho-note`  
> **Access:** Dentist and Admin only (FR-06.4.1).

| Method | Path                                           | Auth   | Roles          | BRD                 | Description                                                                                                                                                                                                                                                                                                                                                       |
| ------ | ---------------------------------------------- | ------ | -------------- | ------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`  | `/api/appointments/{appointmentId}/ortho-note` | Bearer | All Staff      | FR-06.4             | Returns the Ortho Progress Note for this appointment (if any). Returns `404` if none recorded.                                                                                                                                                                                                                                                                    |
| `POST` | `/api/appointments/{appointmentId}/ortho-note` | Bearer | Dentist, Admin | FR-06.4.2–FR-06.4.3 | Creates the Ortho Progress Note for this In Progress appointment. Body: `{ upperArchWire, lowerArchWire, upperArchElastics?, lowerArchElastics?, bracketChanges?, applianceStatus (Active\|Adjusted\|Completed\|RetentionPhase\|Interrupted), clinicalNotes? }`. Appointment must include at least one Orthodontic-category procedure (FR-06.4.1). Returns `201`. |
| `GET`  | `/api/patients/{patientId}/ortho-notes`        | Bearer | All Staff      | FR-06.4.4           | Returns all Ortho Progress Notes for a patient in chronological order (Orthodontic History tab).                                                                                                                                                                                                                                                                  |

---

## 22. Installment Payment Plans

> **Phase:** 5A  
> **Controllers:** Nested under `BillingsController`; `InstallmentEntriesController` for per-entry actions  
> **Base paths:** `/api/billings/{billingId}/installment-plan`, `/api/installment-entries/{id}`

| Method   | Path                                         | Auth   | Roles        | BRD                 | Description                                                                                                                                                                                                                                   |
| -------- | -------------------------------------------- | ------ | ------------ | ------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`    | `/api/billings/{billingId}/installment-plan` | Bearer | All Staff    | FR-07.5             | Returns the installment plan for this billing (if any): plan details, schedule entries with status (Upcoming\|Due\|Overdue\|Paid\|Waived).                                                                                                    |
| `POST`   | `/api/billings/{billingId}/installment-plan` | Bearer | Admin, Staff | FR-07.5.1–FR-07.5.2 | Creates an installment plan on a Final or Partially Paid billing. Body: `{ description, totalInstallments, installments: [{ number, dueDate, expectedAmount }] }`. Automatically schedules Hangfire reminder jobs (FR-07.5.6). Returns `201`. |
| `DELETE` | `/api/billings/{billingId}/installment-plan` | Bearer | Admin        | FR-07.5.8           | Deactivates the installment plan. Existing entries are preserved as historical. Outstanding balance computation reverts to the standard model.                                                                                                |
| `PUT`    | `/api/installment-entries/{id}/waive`        | Bearer | Admin        | FR-07.5.7           | Waives an individual installment entry. Body: `{ reason }` (required). Waived entry is excluded from balance computations (BR-02.13).                                                                                                         |

---

## 23. Prescriptions

> **Phase:** 5A  
> **Controller:** `PrescriptionsController`  
> **Base paths:** `/api/patients/{patientId}/prescriptions`, `/api/prescriptions/{id}`  
> **Access:** Dentist and Admin only for create/void (FR-15.1).

| Method   | Path                                      | Auth   | Roles          | BRD             | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                |
| -------- | ----------------------------------------- | ------ | -------------- | --------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`    | `/api/patients/{patientId}/prescriptions` | Bearer | All Staff      | FR-15.11        | Returns prescription history for this patient: date, prescribing dentist, item summary, status (Active\|Voided).                                                                                                                                                                                                                                                                                                                                                           |
| `POST`   | `/api/patients/{patientId}/prescriptions` | Bearer | Dentist, Admin | FR-15.2–FR-15.7 | Creates a prescription. Body: `{ appointmentId?, datePresigned, items: [{ drugName, strength, quantity, sig }], prcNumber (auto-filled from dentist profile, editable), ptrNumber (auto-filled, editable), s2LicenseNumber? }`. On creation: a PDF is generated using clinic letterhead (FR-15.9) and auto-saved as a System-Generated `PatientFileAttachment` with label `"Prescription — {date} — Dr. {name}"`. Returns `201`. Prescription is **read-only** after save. |
| `GET`    | `/api/prescriptions/{id}`                 | Bearer | All Staff      | FR-15           | Returns full prescription detail.                                                                                                                                                                                                                                                                                                                                                                                                                                          |
| `DELETE` | `/api/prescriptions/{id}`                 | Bearer | Admin          | FR-15.8         | Voids the prescription. Body: `{ reason }` (required). Voided prescription is retained. The System-Generated attachment is also flagged voided (but remains in the attachment list). Returns `200`.                                                                                                                                                                                                                                                                        |
| `GET`    | `/api/prescriptions/{id}/pdf`             | Bearer | All Staff      | FR-15.9         | Regenerates and returns the prescription PDF on demand. Blank wet-signature line is printed; **no digitized signature image is embedded** (FR-15.6). `Content-Type: application/pdf`.                                                                                                                                                                                                                                                                                      |

---

## 24. Medical/Dental Certificates

> **Phase:** 5A  
> **Controller:** `CertificatesController`  
> **Base paths:** `/api/patients/{patientId}/certificates`, `/api/certificates/{id}`  
> **Access:** Dentist and Admin only for create/void (FR-16.1).

| Method   | Path                                     | Auth   | Roles          | BRD             | Description                                                                                                                                                                                                                                                                                                                                                                                                                 |
| -------- | ---------------------------------------- | ------ | -------------- | --------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`    | `/api/patients/{patientId}/certificates` | Bearer | All Staff      | FR-16.9         | Returns Medical/Dental Certificate history for this patient: date, issuing dentist, diagnosis summary, status.                                                                                                                                                                                                                                                                                                              |
| `POST`   | `/api/patients/{patientId}/certificates` | Bearer | Dentist, Admin | FR-16.2–FR-16.4 | Creates a Medical/Dental Certificate. Body: `{ appointmentId?, title (MedicalCertificate\|DentalCertificate), consultationDate, diagnosisFinding, procedurePerformed, recommendedRestDays?, returnToDate?, remarks? }`. On creation: PDF is auto-generated (FR-16.7) and saved as a System-Generated `PatientFileAttachment` with label `"Medical Certificate — {date} — Dr. {name}"`. Returns `201`. Read-only after save. |
| `GET`    | `/api/certificates/{id}`                 | Bearer | All Staff      | FR-16           | Returns full certificate detail.                                                                                                                                                                                                                                                                                                                                                                                            |
| `DELETE` | `/api/certificates/{id}`                 | Bearer | Admin          | FR-16.6         | Voids the certificate. Body: `{ reason }` (required). Retained in database. Returns `200`.                                                                                                                                                                                                                                                                                                                                  |
| `GET`    | `/api/certificates/{id}/pdf`             | Bearer | All Staff      | FR-16.7         | Returns the certificate PDF on demand. Blank wet-signature line; **no digital signature image** (FR-16.5). `Content-Type: application/pdf`.                                                                                                                                                                                                                                                                                 |

---

## 25. Data Export

> **Phase:** 6A  
> **Controller:** `ExportsController`  
> **Base path:** `/api/exports`  
> **Access:** Administrator only (FR-13.1).

| Method | Path                         | Auth   | Roles | BRD             | Description                                                                                                                                                                                                                                                                                                                                                                    |
| ------ | ---------------------------- | ------ | ----- | --------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `POST` | `/api/exports`               | Bearer | Admin | FR-13.1–FR-13.7 | Enqueues a Hangfire bulk export job. Body: `{ format (csv\|xlsx), scope (full\|dateRange), startDate?, endDate? }`. Entities exported: Patients, Appointments, Treatment Records, Billings, Payments, Installment Plans, Prescriptions, Medical Certificates, Ortho Progress Notes (FR-13.2). Action is audit-logged (FR-13.7). Returns `202 Accepted` with `{ exportJobId }`. |
| `GET`  | `/api/exports/{id}`          | Bearer | Admin | FR-13.5         | Returns export job status: `{ status (Pending\|Processing\|Completed\|Failed), completedAt?, downloadAvailableUntil? }`.                                                                                                                                                                                                                                                       |
| `GET`  | `/api/exports/{id}/download` | Bearer | Admin | FR-13.5         | Downloads the completed export ZIP archive. Time-limited: valid for 24 hours from completion (FR-13.5). Returns `410 Gone` after expiry. No secrets, audit log entries, or credential hashes are included in the export (FR-13.9).                                                                                                                                             |

---

## 26. Patient Portal — Authentication

> **Phase:** 6A  
> **Controller:** `PortalAuthController`  
> **Base path:** `/api/portal/auth`  
> **JWT Audience:** `portal` (separate from clinic `Bearer` tokens — NFR-S03, BR-07.2)

| Method | Path                               | Auth   | Roles   | BRD                 | Description                                                                                                                                                                                                                                                                               |
| ------ | ---------------------------------- | ------ | ------- | ------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `POST` | `/api/portal/auth/register`        | Public | —       | FR-12.1.1–FR-12.1.5 | Patient self-registration. Body: `{ firstName, lastName, dateOfBirth, sex, mobileNumber, email, password, privacyConsentAcknowledged: true }`. Sends verification email (FR-12.1.4). Creates portal account in unverified state. Clinic staff receive in-app notification. Returns `201`. |
| `GET`  | `/api/portal/auth/verify-email`    | Public | —       | FR-12.1.4           | Email verification link target. Query: `?token=` (time-limited token, expires 24 h). Activates the portal account. Returns `200` or `422` if token expired.                                                                                                                               |
| `POST` | `/api/portal/auth/login`           | Public | —       | FR-12.2.1–FR-12.2.2 | Portal login. Body: `{ email, password }`. Returns `{ accessToken, refreshToken, expiresAt, patientId, fullName }`. Token has `portal` audience claim. After 5 failed attempts, account locks for 30 minutes (FR-12.2.3).                                                                 |
| `POST` | `/api/portal/auth/logout`          | Portal | Patient | FR-12.2             | Invalidates the current portal access token via Valkey revocation. Body: `{ refreshToken }`. Returns `204`.                                                                                                                                                                               |
| `POST` | `/api/portal/auth/forgot-password` | Public | —       | FR-12.2.3           | Sends a password reset email to the registered address. Body: `{ email }`. Always returns `200` regardless of whether the email exists (prevents enumeration).                                                                                                                            |
| `POST` | `/api/portal/auth/reset-password`  | Public | —       | FR-12.2.3           | Resets the portal account password using a time-limited token. Body: `{ token, newPassword }`. Password complexity: minimum 8 chars, at least one letter and one number (FR-12.1.3). Returns `204`.                                                                                       |

---

## 27. Patient Portal — Account & Profile

> **Phase:** 6A  
> **Controller:** `PortalAccountsController`  
> **Base path:** `/api/portal/accounts`  
> **JWT Audience:** `portal`

| Method | Path                                               | Auth   | Roles   | BRD              | Description                                                                                                                                                                                                                                 |
| ------ | -------------------------------------------------- | ------ | ------- | ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`  | `/api/portal/accounts/me`                          | Portal | Patient | FR-12.1, FR-12.3 | Returns the authenticated patient's portal account and linked Patient record: personal info, contact info (editable), dental/medical history (read-only after staff confirmation, with `Pending Staff Review` flags on unconfirmed fields). |
| `PUT`  | `/api/portal/accounts/me`                          | Portal | Patient | FR-12.3.5        | Updates the patient's own contact information (phone, address, email). Medical/dental history fields are **read-only** in the portal after initial staff confirmation (FR-12.3.5). Returns `422` if attempting to update locked fields.     |
| `GET`  | `/api/portal/accounts/me/notification-preferences` | Portal | Patient | FR-10.9          | Returns the patient's notification preferences: SMS/email opt-in per event type.                                                                                                                                                            |
| `PUT`  | `/api/portal/accounts/me/notification-preferences` | Portal | Patient | FR-10.9          | Updates notification preferences. Body: `{ preferences: [{ eventType, smsEnabled, emailEnabled }] }`.                                                                                                                                       |

---

## 28. Patient Portal — Dependents

> **Phase:** 6A  
> **Controller:** `PortalDependentsController`  
> **Base path:** `/api/portal/accounts/me/dependents`  
> **JWT Audience:** `portal`

| Method   | Path                                                      | Auth   | Roles   | BRD                 | Description                                                                                                                                                                                                                                                        |
| -------- | --------------------------------------------------------- | ------ | ------- | ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `GET`    | `/api/portal/accounts/me/dependents`                      | Portal | Patient | FR-12.1.7–FR-12.1.8 | Returns list of dependent Patient records linked to the Primary Account Holder. Max 10 (configurable).                                                                                                                                                             |
| `POST`   | `/api/portal/accounts/me/dependents`                      | Portal | Patient | FR-12.1.7–FR-12.1.8 | Adds a dependent. Body: `{ firstName, lastName, dateOfBirth, sex, relationship }`. Creates a new Patient record flagged `Portal-Registered (Dependent)`. Returns `422` if max dependent limit would be exceeded. Staff receive in-app notification. Returns `201`. |
| `DELETE` | `/api/portal/accounts/me/dependents/{dependentPatientId}` | Portal | Patient | FR-12.1.7           | Removes the dependent link. The Patient record is retained but unlinked from the portal account.                                                                                                                                                                   |

**IDOR enforcement:** All portal endpoints validate that the `patientId` in any request belongs to either the authenticated patient or one of their dependents. Any attempt to access another account holder's patient returns `403` (FR-12.2.4, BR-07.2).

---

## 29. Patient Portal — Online Booking

> **Phase:** 6A (booking without deposit); 7A (deposit flow)  
> **Controller:** `PortalBookingController`  
> **Base path:** `/api/portal/booking`  
> **JWT Audience:** `portal`

| Method | Path                                    | Auth   | Roles   | BRD                 | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           |
| ------ | --------------------------------------- | ------ | ------- | ------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`  | `/api/portal/booking/appointment-types` | Portal | Patient | FR-02.16            | Returns the list of active portal appointment types (label, description, estimatedDurationMinutes, requiredSpecialization). At least one active type must exist for booking to be available.                                                                                                                                                                                                                                                                                                                          |
| `GET`  | `/api/portal/booking/calendar`          | Portal | Patient | FR-12.4.2           | Returns available time slots for a given date and appointment type. Query: `?date=`, `?appointmentType=`, `?dentistId=` (optional). Slots occupied by existing non-cancelled appointments are returned as unavailable — no details of other patients are disclosed.                                                                                                                                                                                                                                                   |
| `POST` | `/api/portal/booking/appointments`      | Portal | Patient | FR-12.4.3–FR-12.4.5 | Creates a portal booking. Body: `{ patientId, dentistId (or "any"), appointmentTypeId, scheduledAt, chiefComplaint (min 5 chars) }`. Performs same conflict check as FR-04.1.4. Status depends on `bookingApprovalMode` (FR-02.13): `Immediate → Pending`, `AwaitingApproval → AwaitingApproval`. If deposit is required (FR-02.14 amount > 0), response includes `{ appointmentId, depositRequired: true, depositAmount, depositLabel, forfeiturePolicy }`. Clinic staff receive in-app notification. Returns `201`. |

---

## 30. Patient Portal — My Appointments

> **Phase:** 6A  
> **Controller:** `PortalAppointmentsController`  
> **Base path:** `/api/portal/appointments`  
> **JWT Audience:** `portal`

| Method   | Path                                       | Auth   | Roles   | BRD                 | Description                                                                                                                                                                                                                                                                                                                                                                             |
| -------- | ------------------------------------------ | ------ | ------- | ------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`    | `/api/portal/appointments`                 | Portal | Patient | FR-12.5.1           | Returns the authenticated patient's (or specified dependent's) appointments. Query: `?patientId=` (defaults to own), `?status=`, `?upcoming=true`. Returns: date/time, dentist name, planned treatments (names only), status in patient-friendly terms.                                                                                                                                 |
| `GET`    | `/api/portal/appointments/{id}`            | Portal | Patient | FR-12.5.1           | Returns appointment detail. Returns `403` if the appointment does not belong to the authenticated patient or their dependents.                                                                                                                                                                                                                                                          |
| `DELETE` | `/api/portal/appointments/{id}`            | Portal | Patient | FR-12.5.2–FR-12.5.4 | Patient cancels an appointment. Accepted statuses: `Pending`, `Confirmed`, `AwaitingApproval`, `AwaitingDepositVerification` (FR-12.5.2). Body: `{ reason }` (minimum 10 characters). `AwaitingDepositVerification` cancellation releases the slot and discards the receipt. Cancellation notified to clinic staff and assigned dentist. Returns `403` for `InProgress` or `Completed`. |
| `POST`   | `/api/portal/appointments/{id}/reschedule` | Portal | Patient | FR-12.5.5           | Atomic reschedule: cancels the current appointment and creates a new booking in a single transaction. Body: `{ newScheduledAt, chiefComplaint? }`. Returns `409` if the new slot is taken (and rolls back the cancellation). Returns `201` with new appointment ID on success.                                                                                                          |

---

## 31. Patient Portal — Treatment History

> **Phase:** 6A  
> **Controller:** `PortalTreatmentHistoryController`  
> **JWT Audience:** `portal`

| Method | Path                            | Auth   | Roles   | BRD     | Description                                                                                                                                                                                                                                                                                  |
| ------ | ------------------------------- | ------ | ------- | ------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`  | `/api/portal/treatment-history` | Portal | Patient | FR-12.6 | Returns chronological completed appointment history with procedures performed. Query: `?patientId=`, `?startDate=`, `?endDate=`, `?dentistId=`. Each entry: appointment date, dentist name, treatment names (no FDI condition codes — FR-12.6.5), patient-visible clinical notes. Read-only. |

---

## 32. Patient Portal — Billing & Payments

> **Phase:** 6A  
> **Controller:** `PortalBillingController`  
> **Base path:** `/api/portal/billing-summary`  
> **JWT Audience:** `portal`

| Method | Path                                        | Auth   | Roles   | BRD       | Description                                                                                                                                                                                                                                                                                          |
| ------ | ------------------------------------------- | ------ | ------- | --------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`  | `/api/portal/billing-summary`               | Portal | Patient | FR-12.7   | Returns billing records for the authenticated patient (or dependent). Each record: appointment date, procedures (names only), totalAmount, totalPaid, outstandingBalance, patient-friendly status (Unpaid\|PartiallyPaid\|FullyPaid). Internal statuses (Draft, Voided) are not exposed (FR-12.7.2). |
| `GET`  | `/api/portal/billing/{id}/pdf/or`           | Portal | Patient | FR-12.7.3 | Returns the Official Receipt PDF for a specific payment. Returns `403` if the billing does not belong to the authenticated patient (IDOR enforcement — FR-12.7.5).                                                                                                                                   |
| `GET`  | `/api/portal/billing/{id}/pdf/soa`          | Portal | Patient | FR-12.7.4 | Returns the Statement of Account PDF for this billing. Returns `403` if the billing does not belong to the authenticated patient.                                                                                                                                                                    |
| `GET`  | `/api/portal/billing/{id}/installment-plan` | Portal | Patient | FR-07.5.5 | Returns the installment plan for a billing (if any): each entry's due date, expected amount, and status. Patient-facing view only — no internal notes.                                                                                                                                               |

---

## 33. Patient Portal — Ratings

> **Phase:** 6A  
> **Controller:** `PortalRatingsController`  
> **Base path:** `/api/portal/appointments/{id}/rating`  
> **JWT Audience:** `portal`

| Method | Path                                          | Auth   | Roles   | BRD                 | Description                                                                                                                                                                                                                      |
| ------ | --------------------------------------------- | ------ | ------- | ------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`  | `/api/portal/appointments/{id}/rating-status` | Portal | Patient | FR-12.8.7           | Returns `{ canEditRating: bool, ratedAt: ISO8601 \| null }`. The frontend **must** rely exclusively on this flag to show or hide the Edit Rating button. The 7-day window is **never computed client-side** (FR-12.8.7).         |
| `POST` | `/api/portal/appointments/{id}/rating`        | Portal | Patient | FR-12.8.1–FR-12.8.3 | Submits a dentist rating for a Completed appointment. Body: `{ score (1–5), comment? (max 500 chars) }`. One rating per appointment per patient (FR-12.8.3). Returns `422` for Cancelled or No-Show appointments. Returns `201`. |
| `PUT`  | `/api/portal/appointments/{id}/rating`        | Portal | Patient | FR-12.8.7           | Updates the rating. Only allowed within 7 days of original submission (server-side enforced — returns `403` after 7 days). All edits are recorded in the audit log (FR-12.8.7).                                                  |

**Privacy rules (FR-12.8.4):** Ratings are visible only to the rated Dentist (star score, comment, appointment date — patient name hidden) and the Administrator (full details). Never publicly displayed, never on printed documents.

---

## 34. Patient Portal — Feedback

> **Phase:** 6A  
> **Controller:** `PortalFeedbackController`  
> **Base path:** `/api/portal/feedback`  
> **JWT Audience:** `portal`

| Method | Path                   | Auth   | Roles   | BRD                 | Description                                                                                                                                                                                          |
| ------ | ---------------------- | ------ | ------- | ------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `POST` | `/api/portal/feedback` | Portal | Patient | FR-12.9.1–FR-12.9.2 | Submits a feedback item. Body: `{ type (Complaint\|Suggestion\|Recommendation\|General), subject (max 200 chars), message (max 2000 chars), appointmentId? }`. Recorded in audit log. Returns `201`. |
| `GET`  | `/api/portal/feedback` | Portal | Patient | FR-12.9.5           | Returns the authenticated patient's own feedback submissions: subject, type, current status (New\|UnderReview\|Resolved\|Closed). Patient cannot see internal staff notes (FR-12.9.5).               |

---

## 35. Online Booking Deposits

> **Phase:** 7A  
> **Controller:** `PortalDepositController` (deposit initiation) / `AdminDepositController` (staff verification)  
> **JWT Audience:** `portal` for patient-facing; `Bearer` for staff-facing

| Method | Path                                                    | Auth   | Roles   | BRD                  | Description                                                                                                                                                                                                                                                                                                                                                                                                                                     |
| ------ | ------------------------------------------------------- | ------ | ------- | -------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `POST` | `/api/portal/booking/appointments/{id}/deposit/gateway` | Portal | Patient | FR-12.4.6, FR-02.12  | Initiates gateway payment for the booking deposit. Creates a Valkey slot-lock key with ≤ 15-minute TTL. Sets appointment status to `AwaitingPayment`. Returns `{ redirectUrl }` pointing to the configured payment gateway (PayMongo / Maya / Paynamics). If payment does not complete within 15 min, `SlotReleaseJob` releases the slot and the appointment is not committed.                                                                  |
| `POST` | `/api/portal/booking/appointments/{id}/deposit/manual`  | Portal | Patient | FR-12.4.6, FR-02.12a | Records a manual deposit receipt upload. Request: `multipart/form-data` with `receiptImage` (JPEG or PNG) and optional `notes`. Sets appointment status to `AwaitingDepositVerification`. **No Valkey slot-lock is created** — the slot is held indefinitely in the DB until staff Verify or Reject (FR-12.4.6). Receipt image is stored on the appointment record and is not returned by patient-facing endpoints (FR-12.4.6b). Returns `200`. |

---

## 36. Admin — Portal Account Management

> **Phase:** 6A  
> **Controller:** `AdminPortalAccountsController`  
> **Base path:** `/api/admin/portal-accounts`  
> **Auth:** Bearer (clinic staff JWT); Admin or Staff roles only.

| Method | Path                                                     | Auth   | Roles        | BRD       | Description                                                                                                                                                                           |
| ------ | -------------------------------------------------------- | ------ | ------------ | --------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`  | `/api/admin/portal-accounts`                             | Bearer | Admin, Staff | FR-12.2.3 | Returns list of portal accounts with status (Active\|Locked\|Unverified), linked patient ID, registration date, last login.                                                           |
| `POST` | `/api/admin/portal-accounts/{id}/unlock`                 | Bearer | Admin, Staff | FR-12.2.3 | Manually unlocks a locked portal account before the lockout timer expires. Recorded in audit log. Returns `204`.                                                                      |
| `POST` | `/api/admin/portal-accounts/{id}/reset-password`         | Bearer | Admin, Staff | FR-12.2.3 | Triggers a password reset email to the patient's registered address. Staff do not set the new password directly. Recorded in audit log. Returns `204`.                                |
| `POST` | `/api/admin/patients/{patientId}/send-portal-invitation` | Bearer | Admin, Staff | FR-12.1.6 | Sends a portal invitation email to an existing patient's email address. Invitation link expires after 72 hours. Returns `422` if patient already has a portal account. Returns `204`. |

---

## 37. Admin — Feedback Inbox

> **Phase:** 6A  
> **Controller:** `AdminFeedbackController`  
> **Base path:** `/api/admin/feedback`  
> **Auth:** Bearer; Admin or Staff.

| Method | Path                              | Auth   | Roles        | BRD                  | Description                                                                                                                                                                                                       |
| ------ | --------------------------------- | ------ | ------------ | -------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`  | `/api/admin/feedback`             | Bearer | Admin, Staff | FR-12.9.3, FR-12.9.7 | Returns feedback inbox. Query: `?status=`, `?type=`, `?startDate=`, `?endDate=`. Complaint-type items are flagged with a priority indicator (FR-12.9.7).                                                          |
| `GET`  | `/api/admin/feedback/{id}`        | Bearer | Admin, Staff | FR-12.9.3            | Returns full feedback detail including patient info, message, submission time, status history, and internal staff notes.                                                                                          |
| `PUT`  | `/api/admin/feedback/{id}/status` | Bearer | Admin, Staff | FR-12.9.4            | Updates feedback status (`New → UnderReview → Resolved → Closed`) and adds an optional internal staff note. Patient is notified of the status change (in-portal notification — FR-12.9.6). Recorded in audit log. |

---

## 38. Admin — Ratings Management

> **Phase:** 6A  
> **Controller:** `AdminRatingsController`  
> **Base path:** `/api/admin/ratings`  
> **Auth:** Bearer; Admin only.

| Method   | Path                      | Auth   | Roles | BRD                  | Description                                                                                                                                                      |
| -------- | ------------------------- | ------ | ----- | -------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`    | `/api/admin/ratings`      | Bearer | Admin | FR-12.8.4, FR-12.8.6 | Returns all dentist ratings (admin view: full details including patient name). Query: `?dentistId=`, `?startDate=`, `?endDate=`.                                 |
| `DELETE` | `/api/admin/ratings/{id}` | Bearer | Admin | FR-12.8.8            | Deletes an abusive or erroneous rating. Body: `{ reason }` (required). Deleted rating is removed from aggregate calculations immediately. Recorded in audit log. |

---

## 39. Admin — Deposit Verifications

> **Phase:** 7A  
> **Controller:** `AdminDepositVerificationsController`  
> **Base path:** `/api/admin/deposit-verifications`  
> **Auth:** Bearer; Admin or Staff.

| Method | Path                                                      | Auth   | Roles        | BRD        | Description                                                                                                                                                                                                                                                                               |
| ------ | --------------------------------------------------------- | ------ | ------------ | ---------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GET`  | `/api/admin/deposit-verifications`                        | Bearer | Admin, Staff | FR-12.4.6a | Returns paginated queue of appointments in `AwaitingDepositVerification` status. Each item: patient name, appointment date/time, submitted-at, receipt image URL (clinic-staff-accessible only).                                                                                          |
| `POST` | `/api/admin/deposit-verifications/{appointmentId}/verify` | Bearer | Admin, Staff | FR-12.4.6a | Staff verifies the manual deposit receipt. Advances appointment status to `Pending` (or `AwaitingApproval` per FR-12.4.5 mode). Records deposit as a pre-payment. Generates an Acknowledgment Receipt (AR) PDF — auto-saved to patient attachments and emailed to patient. Returns `200`. |
| `POST` | `/api/admin/deposit-verifications/{appointmentId}/reject` | Bearer | Admin, Staff | FR-12.4.6a | Staff rejects the deposit receipt. Body: `{ reason }`. Slot is released (appointment not committed). Patient is notified of rejection with the reason. Returns `200`.                                                                                                                     |

---

## 40. Webhooks

> **Phase:** 7A  
> **Controller:** `WebhooksController`  
> **Base path:** `/api/webhooks`  
> **Auth:** Webhook signature validation (not JWT)

| Method | Path                            | Auth      | BRD                | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            |
| ------ | ------------------------------- | --------- | ------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `POST` | `/api/webhooks/payment-gateway` | Signature | NFR-S04, FR-12.4.6 | Receives payment result notifications from the configured payment gateway (PayMongo / Maya / Paynamics). **HMAC signature is validated before any payload is trusted** (NFR-S04). A tampered or invalid signature returns `400` and is logged as a security warning — no state change occurs. On valid successful-payment payload: releases Valkey slot lock, sets appointment to `Pending`, records deposit pre-payment, sends email confirmation to patient. On failed/expired payment: releases slot, appointment is not committed. |

**Security note (NFR-S04):** The webhook receiver is the single most security-critical endpoint in the system. Signature validation uses the gateway's published signing key / HMAC secret. All processing is idempotent (duplicate delivery safe). The raw key is never logged (NFR-S08).

---

## 41. SignalR Hubs

> **Phase:** 2A (hub scaffolded); 4A (inventory alert events added)  
> **Hub:** `DashboardHub`  
> **Connection URL:** `https://{host}/hubs/dashboard`  
> **Auth:** Bearer JWT (sent as `access_token` query parameter or `Authorization` header)

### Connection

**WebSocket JWT Authentication via Query String:**

Browsers cannot send standard HTTP `Authorization: Bearer` headers on WebSocket `ws://` or `wss://` upgrade requests. Instead, the JWT must be passed as a query parameter:

```
wss://{host}/hubs/dashboard?access_token={jwt}
```

**Backend Configuration (.NET):**

In `Startup.cs` or `Program.cs`, configure the JWT bearer options to extract the token from the query string for hub connections:

```csharp
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters { ... };

        // Allow JWT via query string for WebSocket connections (SignalR)
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken) && context.Request.Path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
```

**Frontend Configuration (Vue 3 + SignalR client):**

```javascript
import * as signalR from "@signalr/client";

const connection = new signalR.HubConnectionBuilder()
  .withUrl(`https://{host}/hubs/dashboard?access_token=${jwtToken}`)
  .withAutomaticReconnect()
  .build();

await connection.start();
```

**Portal patients cannot connect** to `DashboardHub`. A `portal` audience JWT is rejected by the hub's `[Authorize]` policy (NFR-S03, BR-07.2).

### Server → Client Events

| Event Name                   | Payload                                                                | Triggered By                                                 | BRD         |
| ---------------------------- | ---------------------------------------------------------------------- | ------------------------------------------------------------ | ----------- |
| `AppointmentStatusChanged`   | `{ appointmentId, patientName, status, updatedAt }`                    | Any appointment status transition                            | FR-09.1.A.2 |
| `DashboardWidgetUpdated`     | `{ widgetKey, value }` — e.g., `{ widgetKey: "countToday", value: 7 }` | Appointment create/update, payment recorded                  | FR-09.1.A.2 |
| `LowStockAlert`              | `{ lowStockCount, nearExpiryCount }`                                   | `LowStockCheckJob` (every 6 h), `NearExpiryCheckJob` (daily) | FR-09.1.F   |
| `PortalBookingReceived`      | `{ appointmentId, patientName, appointmentType, scheduledAt }`         | New portal booking created                                   | FR-12.4.9   |
| `DepositVerificationPending` | `{ appointmentId, patientName }`                                       | Manual deposit receipt uploaded                              | FR-12.4.6a  |

### Client → Server Methods

| Method                        | Description                                                                                                 |
| ----------------------------- | ----------------------------------------------------------------------------------------------------------- |
| `JoinGroup(string groupName)` | Clients join role-based groups (e.g., `"Administrators"`, `"Dentist:{userId}"`) to receive targeted pushes. |

---

## Phase Summary

The table below maps each endpoint group to its implementation phase for planning and sprint allocation.

| Phase  | Scope                                                                                                                                                                                                      | Controllers / Areas                                                                                                                                              |
| ------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **1A** | Auth, Users, Clinic Settings (core), Audit Log, Health                                                                                                                                                     | `AuthController`, `UsersController`, `ClinicSettingsController`, `AuditLogsController`                                                                           |
| **2A** | Patients, Appointments (CRUD + status), Calendar, Vitals, Dashboard (base), RPT-01, SignalR Hub scaffold                                                                                                   | `PatientsController`, `AppointmentsController`, `DashboardController`, `ReportsController`                                                                       |
| **3A** | Dental Chart, Treatments, Procedures, Billing, Payments, Providers/LOA, Attachments, RPT-04–08/13/14, OR Register, PDFs                                                                                    | `DentalChartController`, `TreatmentsController`, `BillingsController`, `PaymentsController`, `ProvidersController`, `AttachmentsController`, `ReportsController` |
| **4A** | Inventory, Suppliers, Clinic Settings (SMS/SMTP), Notifications, RPT-02/03/09–12, Hangfire jobs                                                                                                            | `InventoryController`, `SuppliersController`, `NotificationsController`, `ReportsController`                                                                     |
| **5A** | Ortho Notes, Installment Plans, Prescriptions, Certificates, Recall/Installment Hangfire jobs                                                                                                              | `OrthoNotesController`, `InstallmentPlansController`, `PrescriptionsController`, `CertificatesController`                                                        |
| **6A** | Portal Auth, Portal Account, Dependents, Booking, Portal Appointments, Treatment History, Portal Billing, Ratings, Feedback, Admin Portal Mgmt, Admin Feedback/Ratings, Exports, Clinic Settings (booking) | All `PortalXxx` and `AdminXxx` controllers, `ExportsController`, `ClinicSettingsController` (booking section)                                                    |
| **7A** | Deposit (gateway + manual), Webhook, Admin Deposit Verifications, Clinic Settings (gateway), Valkey slot-lock, Hangfire `SlotReleaseJob`                                                                   | `PortalDepositController`, `WebhooksController`, `AdminDepositVerificationsController`, `ClinicSettingsController` (gateway section)                             |

---

## Appendix: Appointment Status Transition Reference

```
                   ┌──────────────────────────────────┐
                   │  APPOINTMENT STATUS MACHINE       │
                   └──────────────────────────────────┘

 Clinic-created:   [Pending] ──────────────────► [Confirmed]
                       │                              │
                       │                              │
                       ▼                              ▼
                  [Cancelled]               [In Progress]
                  [Cancelled]                     │
                                                  ▼
                   [No Show] ◄──────────    [Completed] (terminal)
                       │
                       ▼
              (Reschedule → new [Pending])

 Portal (Immediate mode):
                   [Pending] ────────────────────────►  (same as above)

 Portal (Awaiting Approval mode):
          [Awaiting Approval] ──► [Pending] ──────────► (same as above)
                   │
                   ▼
              [Rejected] (terminal — BR-01.8)

 Portal with deposit (gateway):
          [Pending/AwaitingApproval] → [Awaiting Payment]
                   │ (payment success)    │ (15-min timeout)
                   ▼                      ▼
              [Pending]            (slot released, record not committed)

 Portal with deposit (manual):
          [Pending/AwaitingApproval] → [Awaiting Deposit Verification]
                   │ (staff Verify)       │ (staff Reject)
                   ▼                      ▼
              [Pending]            (slot released, patient notified)
```

---

## Appendix: Key Validation Rules Quick Reference

| Rule                                                  | Enforcement                                                                | BRD                  |
| ----------------------------------------------------- | -------------------------------------------------------------------------- | -------------------- |
| Cancellation reason ≥ 10 characters                   | `422` via FluentValidation                                                 | FR-04.2.2            |
| Portal booking chief complaint ≥ 5 characters         | `422`                                                                      | FR-12.4.3            |
| Payment amount ≤ outstanding balance                  | `422`                                                                      | BR-02.4              |
| OR number unique (non-voided)                         | DB UNIQUE constraint → `422 "OR number is already recorded in the system"` | FR-02.7, BR-02.6     |
| Void reason ≥ 10 characters                           | `422`                                                                      | FR-07.4.2            |
| Appointment conflict detection                        | `409 Conflict`                                                             | BR-01.1              |
| Archive last active Administrator                     | `422`                                                                      | BR-04.1              |
| Admin archiving own account                           | `403`                                                                      | FR-01.1.8            |
| Delete `IsSystemGenerated` attachment                 | `403 "System-generated attachments cannot be deleted directly..."`         | FR-15.9, FR-16.7     |
| Anesthetic/Medication receipt without expiry/lot      | `422`                                                                      | FR-08.2.3            |
| Portal rating after Completed appointment only        | `422` for non-Completed                                                    | FR-12.8.3            |
| Rating edit after 7 days                              | `403` (server-side check, `canEditRating: false`)                          | FR-12.8.7            |
| Installment plan: booking deposit ≤ configured amount | `422`                                                                      | FR-02.14             |
| Webhook signature invalid                             | `400` + security warning log                                               | NFR-S04              |
| Export download after 24 h                            | `410 Gone`                                                                 | FR-13.5              |
| File upload > 20 MB                                   | `422`                                                                      | FR-03.7.2            |
| Patient total storage > 500 MB                        | `422`                                                                      | FR-03.7.2            |
| Password complexity failure                           | `422`                                                                      | FR-01.3.3, FR-12.1.3 |
| Discount resulting in TotalAmount < 0                 | `422`                                                                      | BR-02.10             |
| Appointment with clinical records cannot be deleted   | `422`                                                                      | FR-04.2.4            |
