# Wireframe Specification

## All About Teeth Dental Clinic Management System (DCMS)

---

| Field            | Value                      |
| ---------------- | -------------------------- |
| Document Version | 1.0                        |
| Date             | May 14, 2026               |
| Reference BRD    | BRD v4.3                   |
| Reference Phases | DEVELOPMENT_PHASES.md v2.0 |
| Prepared By      | Frontend Design Team       |

---

## Overview

This document lists every wireframe required across both the **Clinic SPA** (`frontend/allaboutteeth-web/`) and the **Patient Portal SPA** (`frontend/allaboutteeth-portal/`). Wireframes are grouped by module following the BRD functional requirement structure and ordered by development phase priority. Each wireframe entry includes the screen purpose, primary actors, key UI elements, and the BRD/phase reference.

---

## Table of Contents

1. [Global / Shell Wireframes](#1-global--shell-wireframes)
2. [Authentication & Password Recovery](#2-authentication--password-recovery)
3. [Dashboard](#3-dashboard)
4. [User & Access Management](#4-user--access-management)
5. [Clinic Settings](#5-clinic-settings)
6. [Patient Management](#6-patient-management)
7. [Appointment Scheduling](#7-appointment-scheduling)
8. [Dental Charting](#8-dental-charting)
9. [Treatment Records & Procedures](#9-treatment-records--procedures)
10. [Billing & Payments](#10-billing--payments)
11. [Inventory & Supply Management](#11-inventory--supply-management)
12. [Reports & Analytics](#12-reports--analytics)
13. [Audit Log](#13-audit-log)
14. [Specialized Clinical Modules](#14-specialized-clinical-modules)
15. [Patient Portal](#15-patient-portal)
16. [Online Booking Deposits](#16-online-booking-deposits)
17. [Shared / Reusable Components](#17-shared--reusable-components)

---

## Wireframe Conventions

| Notation | Meaning                                                       |
| -------- | ------------------------------------------------------------- |
| `[M]`    | Modal / Dialog — overlays the current page                    |
| `[D]`    | Drawer / Side panel — slides in from the right or left        |
| `[P]`    | Full page — occupies the main content area                    |
| `[T]`    | Tab panel — a tab within a larger page                        |
| `[W]`    | Widget — embedded within a larger page (e.g., dashboard card) |
| `[B]`    | Banner / Inline alert — inline element, not a separate screen |

---

## 1. Global / Shell Wireframes

These wireframes define the persistent layout wrapping every authenticated page.

### WF-SH-01 — App Shell (Clinic SPA) `[P]`

**Phase:** 1B  
**Actor:** All authenticated clinic staff  
**BRD Ref:** FR-01.4 (role-gated access), FR-09.1.A.6 (role-sensitive dashboard), NFR-U01

**Description:** The persistent outer container for the entire clinic application. Present on every authenticated page.

**Key UI Elements:**

- **Left Sidebar Navigation** — vertical nav rail with icons and labels
  - Logo / clinic name at top
  - Nav items (role-gated, hidden if user lacks access — not greyed):
    - Dashboard
    - Appointments (Dentist, Staff, Admin)
    - Patients (Dentist, Staff, Admin)
    - Dental Chart (Dentist, Admin only)
    - Billing & Payments (Admin, Staff)
    - Inventory (Admin, Staff)
    - Treatments (Admin, Dentist)
    - Prescriptions (Admin, Dentist)
    - Reports (Admin, Staff, Dentist)
    - Clinic Settings (Admin only)
    - Users (Admin only)
    - Audit Log (Admin only)
  - Collapsed / expanded toggle at bottom
- **Top Bar** — displays current user name, role badge, notification bell icon with badge count, logout button
- **Notification Bell Dropdown** — quick-access list: low-stock alerts, near-expiry alerts, today's appointment count, portal booking inbox count (clickable to navigate to record)
- **Main Content Area** — `<router-view>` outlet for page content
- **Toast Notification Container** — top-right, PrimeVue Toast, stacked up to 3

---

### WF-SH-02 — Empty / Not Found State `[P]`

**Phase:** 1B  
**Actor:** All users  
**BRD Ref:** NFR-U01

**Description:** Shown when a route does not exist or a resource is not found.

**Key UI Elements:**

- Clinic logo
- "Page Not Found" or "You don't have access to this page" message
- Navigation back to Dashboard button

---

### WF-SH-03 — Global Confirmation Dialog `[M]`

**Phase:** 1B  
**Actor:** All users  
**BRD Ref:** NFR-U04

**Description:** Reusable confirmation modal shown before any destructive or irreversible action (archive patient, void payment, cancel appointment, delete rating).

**Key UI Elements:**

- Title (e.g., "Archive Patient?")
- Description text explaining the consequences
- Optional reason text input (required for actions that mandate a reason — minimum character count shown)
- "Confirm" (danger-colored) and "Cancel" buttons
- Disabled state on "Confirm" if reason field is required but empty

---

## 2. Authentication & Password Recovery

### WF-AU-01 — Login Page `[P]`

**Phase:** 1B  
**Actor:** All clinic staff (Admin, Dentist, Staff)  
**BRD Ref:** FR-01.2

**Description:** The public entry point for the clinic SPA. Unauthenticated users are redirected here by the route guard.

**Key UI Elements:**

- Clinic logo and name
- **Username** field (text input)
- **Password** field (masked input, show/hide toggle)
- "Login" primary button
- Inline error states:
  - Invalid credentials → "Incorrect username or password."
  - Account locked → "Your account is locked. Try again in [X] minutes." (countdown or static text)
- "Forgot password?" link → navigates to WF-AU-02
- No registration link (clinic staff accounts are admin-created only)

---

### WF-AU-02 — Password Recovery — Step 1: Enter Username `[P]`

**Phase:** 1B  
**Actor:** All clinic staff  
**BRD Ref:** FR-01.3.1–FR-01.3.5

**Description:** First step of the three-step password recovery flow.

**Key UI Elements:**

- "Recover Your Password" heading
- **Username** field
- "Continue" button → proceeds to WF-AU-03 if user exists
- Back to login link
- If email recovery is configured: "Send recovery link to my email" option shown after username validation

---

### WF-AU-03 — Password Recovery — Step 2: Security Questions `[P]`

**Phase:** 1B  
**Actor:** All clinic staff  
**BRD Ref:** FR-01.3.2

**Description:** Security question verification step.

**Key UI Elements:**

- Security Question 1 (read-only label from server) + answer input
- Security Question 2 (read-only label from server) + answer input
- "Verify Answers" button
- Inline error on failed verification: "One or more answers are incorrect."
- Back button

---

### WF-AU-04 — Password Recovery — Step 3: Set New Password `[P]`

**Phase:** 1B  
**Actor:** All clinic staff  
**BRD Ref:** FR-01.3.3

**Description:** Final step after security question verification passes.

**Key UI Elements:**

- **New Password** field (masked, show/hide toggle)
- **Confirm New Password** field
- Password strength / requirement hint: "Minimum 8 characters, at least 1 letter and 1 number"
- Inline validation errors per rule
- "Set Password" button
- Success state → auto-redirect to login with success toast

---

## 3. Dashboard

### WF-DB-01 — Dashboard Page `[P]`

**Phase:** 2B  
**Actor:** All authenticated clinic users (role-sensitive layout)  
**BRD Ref:** FR-09.1.A through FR-09.1.F

**Description:** The default landing page after login. Widget composition is determined by the user's role. Layout uses a responsive CSS grid.

---

#### WF-DB-W01 — Today's Appointments Widget `[W]`

**BRD Ref:** FR-09.1.B.1 | All users

**Key UI Elements:**

- Widget card header: "Today's Appointments" + count badge + Refresh button
- Scrollable list/table:
  - Time slot | Patient Name | Dentist | Planned Treatments | Status badge (color-coded per FR-04.3.3)
- Each row is clickable → opens appointment detail (WF-AP-03)
- Empty state: "No appointments scheduled for today"

---

#### WF-DB-W02 — Appointment Count Widgets `[W]`

**BRD Ref:** FR-09.1.B.2–B.5 | All users

**Key UI Elements (per widget):**

- 4 stat cards in a row:
  1. "Today's Appointments" — integer
  2. "This Month's Appointments" — integer
  3. "Active Patients" — integer
  4. "Pending Appointments" — integer
- Each card has a subtle icon, the metric value in large type, and a small label
- Values update via SignalR WebSocket push (no full-page reload)

---

#### WF-DB-W03 — Revenue Summary Widgets `[W]`

**BRD Ref:** FR-09.1.C.1–C.4 | Administrator only

**Key UI Elements:**

- 4 stat cards:
  1. "Revenue Today" (₱ formatted)
  2. "Revenue This Week" (₱ formatted)
  3. "Revenue This Month" (₱ formatted)
  4. "Outstanding Receivables" (₱ formatted)
- Values update via SignalR push
- "Outstanding Receivables" card links to Accounts Receivable report (RPT-06)

---

#### WF-DB-W04 — New Patients This Month `[W]`

**BRD Ref:** FR-09.1.C.6 | Administrator only

**Key UI Elements:**

- Stat card with count and a "View Masterlist" link

---

#### WF-DB-W05 — Revenue Trend Chart `[W]`

**BRD Ref:** FR-09.1.C.5 | Administrator only

**Key UI Elements:**

- Bar chart (vue-echarts) — 30-day daily revenue
- X-axis: dates | Y-axis: ₱ amount
- Hover tooltip showing exact date and amount
- Chart title with date range label

---

#### WF-DB-W06 — Monthly Appointment Status Breakdown `[W]`

**BRD Ref:** FR-09.1.D.1 | Administrator, Dentist

**Key UI Elements:**

- Donut chart (vue-echarts) — segments by status with FR-04.3.3 colors
- Legend with status label + count
- Hover tooltip: count and percentage

---

#### WF-DB-W07 — Top 5 Treatments This Month `[W]`

**BRD Ref:** FR-09.1.D.2 | Administrator, Dentist

**Key UI Elements:**

- Horizontal bar chart (vue-echarts) — treatment name (Y) vs. count (X)
- Top 5 bars labeled with procedure count
- Chart title: "Top Treatments This Month"

---

#### WF-DB-W08 — My Appointments Widgets (Dentist) `[W]`

**BRD Ref:** FR-09.1.E.1–E.3 | Dentist only

**Key UI Elements:**

- "My Appointments Today" — filtered list table (same columns as WF-DB-W01)
- "My Appointments This Month" — integer stat card
- "My Completed Procedures This Month" — integer stat card

---

#### WF-DB-W09 — Low-Stock Alerts Widget `[W]`

**BRD Ref:** FR-09.1.F.1 | Administrator, Staff

**Key UI Elements:**

- Widget header: "Low-Stock Alerts" + red badge count
- List: Item Name | Current Stock | Critical Threshold | Unit | link icon to item
- Empty state: "All items are above critical threshold"

---

#### WF-DB-W10 — Near-Expiry Alerts Widget `[W]`

**BRD Ref:** FR-09.1.F.2 | Administrator, Staff

**Key UI Elements:**

- Widget header: "Near-Expiry Items" + amber badge count
- List: Item Name | Lot Number | Expiry Date | Days Remaining | link icon
- Empty state: "No items expiring within the warning window"

---

## 4. User & Access Management

### WF-US-01 — User List Page `[P]`

**Phase:** 1B  
**Actor:** Administrator only  
**BRD Ref:** FR-01.1, FR-01.4

**Key UI Elements:**

- Page title: "User Accounts"
- "Create User" button (top-right)
- Filter bar: Status (Active / Archived), Role (All / Administrator / Dentist / Staff), search by name
- Paginated DataTable columns: Photo | Full Name | Username | Role badge | Status badge | Last Login | Actions
- Row actions: Edit (pencil icon) → WF-US-02, Archive (if Active, with confirmation WF-SH-03), Restore (if Archived), Unlock (if locked, shown as amber badge)
- Archived rows visually dimmed

---

### WF-US-02 — Create / Edit User Dialog `[M]`

**Phase:** 1B  
**Actor:** Administrator only  
**BRD Ref:** FR-01.1.3–FR-01.1.4, FR-01.3.1

**Key UI Elements:**

- Dialog title: "Create User" or "Edit User — [Name]"
- **General tab:**
  - First Name*, Middle Name, Last Name*, Username* (unique check), Role* (dropdown), Sex*, Birthdate* (date picker), Contact Number, Email, Home Address
  - Profile Photo — webcam capture (WF-GL-CAM) or file upload
  - Status toggle (Active / Archived) — only shown in edit mode
- **Dentist Tab** (shown only if Role = Dentist or Administrator):
  - Specialization (multi-select from predefined list), PRC License Number*, PRC License Expiry Date* (date picker), PTR Number, S2 License Number (optional)
- **Security Tab** (shown only in Create mode):
  - Temporary Password*, Confirm Password*
  - Security Question 1* (dropdown) + Answer 1*
  - Security Question 2* (dropdown) + Answer 2*
- Inline validation errors per field
- "Save" and "Cancel" buttons; Save disabled until all required fields pass validation

---

### WF-US-03 — Reset / Force-Change Password Dialog `[M]`

**Phase:** 1B  
**Actor:** Administrator only  
**BRD Ref:** BR-04.3

**Key UI Elements:**

- Target user name (read-only)
- New Password* field + Confirm New Password* field
- Password requirement hint text
- "Reset Password" and "Cancel" buttons

---

## 5. Clinic Settings

### WF-CS-01 — Clinic Settings Page `[P]`

**Phase:** 1B  
**Actor:** Administrator only  
**BRD Ref:** FR-02.1–FR-02.16

**Description:** A tabbed settings page. Each tab maps to a settings category. Saving on any tab applies changes for that tab only.

---

#### Tab 1 — Clinic Identity `[T]`

**BRD Ref:** FR-02.2

**Key UI Elements:**

- Clinic Name*, Address*, Contact Number(s), Email, Website
- Clinic Logo — upload control with preview thumbnail; current logo shown if set
- "Save Identity" button

---

#### Tab 2 — Compliance & Regulatory `[T]`

**BRD Ref:** FR-02.3

**Key UI Elements:**

- BIR TIN\*, BIR COR Number
- DOH Health Facility License Number, DOH License Expiry Date (date picker)
- PhilHealth Accreditation Number, PhilHealth Accreditation Expiry
- "Save Compliance Info" button

---

#### Tab 3 — Operating Hours `[T]`

**BRD Ref:** FR-02.4–FR-02.5

**Key UI Elements:**

- 7-row table (Monday–Sunday):
  - Day | Is Working Day (toggle) | Opening Time (time picker) | Closing Time (time picker)
- Default Appointment Duration (number input, minutes)
- "Save Schedule" button

---

#### Tab 4 — Financial Settings `[T]`

**BRD Ref:** FR-02.6–FR-02.7

**Key UI Elements:**

- VAT Status toggle: "Non-VAT" ↔ "VAT-Registered (12%)"
- OR Number — informational note explaining manual ATP booklet entry process (no auto-generation field)
- "Save Financial Settings" button

---

#### Tab 5 — Security Settings `[T]`

**BRD Ref:** FR-02.8

**Key UI Elements:**

- Max Failed Login Attempts (number input, default 5)
- Lockout Duration (number input, minutes, default 30)
- JWT Token Expiry (number input, hours, default 8)
- "Save Security Settings" button

---

#### Tab 6 — Notifications `[T]`

**BRD Ref:** FR-02.9–FR-02.11, FR-02.15

**Key UI Elements:**

- **Inventory** sub-section: Expiry Warning Threshold (days, default 30)
- **SMS Gateway** sub-section: Provider Name, API Base URL, API Key/Token (masked), Sender Name / Mobile, "Send Test SMS" button (with phone number input for test)
- **Email (SMTP)** sub-section: SMTP Host, Port, Encryption (dropdown: None / STARTTLS / SSL), Sender Email, Sender Name, Username, Password (masked), "Send Test Email" button
- **Installment Reminders**: Reminder Lead-Time (days before due date, default 3) _(stub field only in Phase 4B; not functional until Phase 5B when FR-07.5 Installment Plans backend is introduced)_
- "Save Notification Settings" button

---

#### Tab 7 — Payment Gateway `[T]`

**BRD Ref:** FR-02.12–FR-02.12a  
**Phase:** 7B _(payment gateway configuration is deferred to Phase 7A/7B; scaffold as a locked placeholder tab in earlier phases)_

**Key UI Elements:**

- Gateway sub-section: Provider dropdown (PayMongo / Maya / Paynamics / Other), API Public Key, API Secret Key (masked), Environment (Sandbox / Production), "Test Connection" button
- Manual Deposit sub-section: Enable toggle, Instruction Text (rich text / textarea), "Save Gateway Settings" button
- Both methods can be active simultaneously (note explaining this)

---

#### Tab 8 — Online Booking `[T]`

**BRD Ref:** FR-02.13–FR-02.16  
**Phase:** 6B _(online booking settings are deferred to Phase 6A/6B; scaffold as a locked placeholder tab in earlier phases)_

**Key UI Elements:**

- **Approval Mode**: radio buttons — "Immediate (auto-Pending)" / "Awaiting Approval (staff approval required)"
- **Booking Deposit**: Deposit Amount (₱ input, 0 = disabled), Deposit Label (text), Forfeiture Policy Description (textarea)
- **Recall Reminders**: Recall Threshold (months, default 6), Re-send Interval (days, default 30)
- **Appointment Types** sub-section:
  - Sortable list of configured appointment types (label, duration, specialization tag, status)
  - "Add Appointment Type" button → WF-CS-02
  - Edit / Archive actions per row
- "Save Booking Settings" button

---

### WF-CS-02 — Appointment Type Dialog `[M]`

**Phase:** 6B  
**Actor:** Administrator  
**BRD Ref:** FR-02.16

**Key UI Elements:**

- Label\* (text input)
- Patient-Facing Description (textarea)
- Estimated Duration (minutes, number input)\*
- Required Specialization Tag (dropdown, optional — links to treatment specializations)
- Status (Active / Archived)
- "Save" and "Cancel"

---

## 6. Patient Management

### WF-PT-01 — Patient List Page `[P]`

**Phase:** 2B  
**Actor:** Admin, Dentist, Staff  
**BRD Ref:** FR-03.5

**Key UI Elements:**

- Page title: "Patients"
- "Register New Patient" button (top-right)
- **Search bar**: real-time search by last name, first name, contact number, or patient ID
- **Filter bar**: Status (Active / Archived)
- Paginated DataTable:
  - Photo thumbnail | Patient ID | Full Name | Age | Sex | Contact | Status badge | Last Visit | Actions
- Row actions: View Profile → WF-PT-02, Archive (with confirmation), Restore (if archived)
- Archived rows visually dimmed

---

### WF-PT-02 — Patient Profile Page `[P]`

**Phase:** 2B (Tabs 1–7 only); Tabs 11–12 in Phase 3B; Tabs 8–10 in Phase 5B; Tab 13 in Phase 6B  
**Actor:** Admin, Dentist, Staff  
**BRD Ref:** FR-03.1–FR-03.7

**Description:** The central patient record page. Uses a tab layout to organize the patient's comprehensive record. The header area with patient identity and quick-stats is always visible.

> **Phase note:** Tabs are injected progressively as their backend modules become available:
>
> - **Phase 3B** — Tab 11 (LOA History, FR-14.3) and Tab 12 (Attachments, FR-03.7) are introduced alongside HMO Claim Recording and Patient File Attachments. Scaffold as placeholders in Phase 2B.
> - **Phase 5B** — Tabs 8 (Prescriptions, FR-15), 9 (Medical Certificates, FR-16), and 10 (Orthodontic History, FR-06.4) are activated when their respective clinical module APIs are available. Scaffold as placeholders in Phase 2B.
> - **Phase 6B** — Tab 13 (Portal Account, FR-12.2.3) is wired when Patient Portal APIs are introduced. Scaffold as a disabled placeholder in earlier phases.

**Header:**

- Patient photo (large), Patient ID, Full Name, Age, Sex, Status badge
- Quick stats: Total Appointments, Outstanding Balance (₱), Last Visit date
- Action buttons: "Edit Patient", "New Appointment", "Archive Patient" (with confirmation), "Send Portal Invitation" (if no portal account linked)

---

#### Tab 1 — Personal & Contact `[T]`

**BRD Ref:** FR-03.2

**Key UI Elements:**

- Two-column read-only display of all FR-03.2 fields
- Emergency Contact / Guardian section (shown based on age)
- Referral Source
- Privacy Consent status (read-only: "Consented on MM/DD/YYYY")
- "Edit" button → opens edit mode inline

---

#### Tab 2 — Dental History `[T]`

**BRD Ref:** FR-03.3

**Key UI Elements:**

- Previous dentist name and clinic
- Last dental visit date
- Chief complaint / reason for initial visit
- HMO/Provider linked (badge showing provider name + member ID)
- PhilHealth member number
- "Edit" button

---

#### Tab 3 — Medical History `[T]`

**BRD Ref:** FR-03.4

**Key UI Elements:**

- Attending physician details
- General health status
- Previous illnesses / hospitalizations
- Blood type, blood pressure, bleeding time
- **Allergies** — read-only list with severity badges (Mild / Moderate / Severe in green/amber/red)
- **Current Medications** — read-only list
- **Medical Conditions** — read-only checklist with active/inactive/resolved flags
- Lifestyle section (tobacco, alcohol, substances)
- Female-specific fields (shown only if sex = Female)
- Last reviewed date + reviewed-by user
- "Edit Medical History" button → WF-PT-04
- "Pending Staff Review" banner (shown if portal-submitted data awaits confirmation) → WF-PT-05

---

#### Tab 4 — Dental Chart `[T]`

**BRD Ref:** FR-05

**Key UI Elements:**

- Navigates to / renders WF-DC-01 within the tab context
- Read-only when no appointment is In Progress; edit mode only during an active In Progress appointment

---

#### Tab 5 — Appointments `[T]`

**BRD Ref:** FR-04.4

**Key UI Elements:**

- Filter: date range, status
- Paginated list: Date | Dentist | Treatments | Status badge | Actions
- Row action: "View Appointment" → WF-AP-03

---

#### Tab 6 — Treatment History `[T]`

**BRD Ref:** FR-06.3.3–FR-06.3.4

**Key UI Elements:**

- Filter: date range, tooth, treatment category
- Chronological list: Date | Appointment | Dentist | Tooth (FDI) | Surface | Treatment | Completion badge | Notes
- Click-through to appointment record

---

#### Tab 7 — Billing `[T]`

**BRD Ref:** FR-07

**Key UI Elements:**

- List: Date | Appointment | Total | Patient Share | Paid | Balance | Status badge | Actions
- Row actions: "View Billing" → WF-BL-01, "Download SOA" (PDF)

---

#### Tab 8 — Prescriptions `[T]`

**BRD Ref:** FR-15.11

**Key UI Elements:**

- "New Prescription" button (Dentist/Admin only)
- List: Date | Rx No. | Prescribing Dentist | Items summary | Actions
- Row actions: "View/Print" → WF-RX-01, "Void" (Admin only)

---

#### Tab 9 — Medical Certificates `[T]`

**BRD Ref:** FR-16.9

**Key UI Elements:**

- "Issue Certificate" button (Dentist/Admin only)
- List: Date | Cert No. | Issuing Dentist | Diagnosis summary | Actions
- Row actions: "View/Print" → WF-MC-01, "Void" (Admin only)

---

#### Tab 10 — Orthodontic History `[T]`

**BRD Ref:** FR-06.4.4

**Key UI Elements:**

- Chronological list of Ortho Progress Notes
- Each entry: Date | Appointment | Upper Wire | Lower Wire | Appliance Status | View button
- Clicking "View" expands full note detail inline

---

#### Tab 11 — LOA History `[T]`

**BRD Ref:** FR-14.3

**Key UI Elements:**

- HMO disclaimer banner (FR-14.4 text)
- List: LOA Number | Provider | Authorized Amount | Visit Date | Applied Amount | Billing link
- Filter by provider

---

#### Tab 12 — Attachments `[T]`

**BRD Ref:** FR-03.7

**Key UI Elements:**

- "Upload File" button
- Reverse-chronological file list:
  - Date | Label | Uploader | File Type icon (PDF/image) | Notes | Download | Delete (if uploader or Admin)
- Upload dialog (WF-PT-06) opens on "Upload File"
- Quota bar showing used vs. allowed storage (500 MB default)
- System-generated attachments (prescription/cert PDFs) shown with a lock icon — delete disabled

---

#### Tab 13 — Portal Account `[T]`

**BRD Ref:** FR-12.2.3

**Key UI Elements:**

- Shows: "No portal account" or portal email, account status (Active / Locked / Pending Verification)
- If locked: "Unlock Account" button
- "Send Password Reset Email" button
- "Send Portal Invitation" button (if no account yet)
- Linked Dependents list (if applicable)

---

### WF-PT-03 — Patient Registration / Edit Form `[P]` or `[M]`

**Phase:** 2B  
**Actor:** Admin, Dentist, Staff  
**BRD Ref:** FR-03.1–FR-03.4

**Description:** Multi-step wizard for new registration; single-page form for edits. The new patient flow uses steps to avoid form overwhelm.

**Steps (New Registration):**

1. **Step 1 — Personal Information**: First Name*, Last Name*, Middle Name, Nickname, Birthdate* (date picker → age auto-computed), Sex*, Nationality, Religion, Civil Status
2. **Step 2 — Contact Information**: Mobile Number\* (at least one contact), Home Phone, Email, Home Address, Office Address, Office Phone, Fax, Occupation, Emergency Contact / Guardian section (conditional on age)
3. **Step 3 — Dental History**: Previous dentist name and clinic, last dental visit date, chief complaint, HMO/Provider linked (searchable dropdown), insurance member ID, insurance effective date, PhilHealth number, referral source (dropdown + free text)
4. **Step 4 — Medical History**: Full FR-03.4 intake form — physician, general health, previous illnesses, hospitalizations, blood type, BP, bleeding time, allergies (dynamic multi-entry), medications (dynamic multi-entry), conditions (checklist), lifestyle, female fields (conditional)
5. **Step 5 — Privacy Consent**: Full privacy notice text displayed, "I acknowledge and consent" checkbox\*, consent date (auto-filled to today), patient photo capture (WF-GL-CAM or file upload, optional)

**Footer:** Step indicator (1 of 5), "Back" / "Next" / "Register Patient" (final step) buttons, inline validation errors

---

### WF-PT-04 — Edit Medical History Form `[M]`

**Phase:** 2B  
**Actor:** Admin, Dentist, Staff  
**BRD Ref:** FR-03.4

**Key UI Elements:**

- Full medical history form (same fields as Step 4 above) pre-filled with current values
- Allergies and medications use dynamic add/remove rows
- Medical conditions use checkbox grid with status selector per condition
- "Last Reviewed By / Date" fields (auto-filled to current user/today on save)
- "Save" and "Cancel"

---

### WF-PT-05 — Portal-Submitted Data Review `[M]`

**Phase:** 2B (Patient portal data review shown in 6B)  
**Actor:** Admin, Staff  
**BRD Ref:** FR-12.3.3–FR-12.3.4

**Key UI Elements:**

- Side-by-side comparison: "Patient-Submitted" (left, amber background) vs. "Current Record" (right)
- Per-field accept/reject toggle or "Accept All" button
- Text fields showing diff highlights (changed value in amber)
- "Confirm Reviewed Fields" and "Dismiss" buttons

---

### WF-PT-06 — Upload Attachment Dialog `[M]`

**Phase:** 5B  
**Actor:** Admin, Dentist, Staff  
**BRD Ref:** FR-03.7.2–FR-03.7.3

**Key UI Elements:**

- Drag-and-drop or browse file upload area (JPEG, PNG, PDF, max 20 MB)
- **Label\*** (text input, required)
- Notes (textarea, optional)
- File preview thumbnail (for images) or PDF icon
- "Upload" and "Cancel" buttons
- File size and type validation feedback inline

---

## 7. Appointment Scheduling

### WF-AP-01 — Appointment Calendar View `[P]`

**Phase:** 2B  
**Actor:** Admin, Dentist, Staff  
**BRD Ref:** FR-04.3

**Key UI Elements:**

- Page title: "Appointments"
- View switcher: Day / Week / Month (FullCalendar)
- "New Appointment" button (top-right)
- **Filter bar**: Dentist dropdown (All / specific dentist)
- FullCalendar grid with appointment blocks:
  - Block content: Patient Name, Dentist Name, Planned Treatments (truncated), Status badge
  - Block color: mapped to FR-04.3.3 status colors
  - Clicking a block → opens WF-AP-03
- Today navigation button, forward/back arrows
- Portal-sourced appointments flagged with a "Portal" icon badge on the block
- "Awaiting Approval" count badge on the page header if any portal bookings need review → links to WF-AP-08

---

### WF-AP-02 — Appointment List View `[P]`

**Phase:** 2B  
**Actor:** Admin, Dentist, Staff  
**BRD Ref:** FR-04.4

**Key UI Elements:**

- Tab alongside or toggle from Calendar view
- Filter bar: Date Range (start/end date pickers), Status (multi-select), Patient (search), Dentist (dropdown)
- Paginated DataTable: Date | Time | Patient | Dentist | Treatments | Status badge | Source (Portal / Clinic) | Actions
- Row actions: "View" → WF-AP-03, quick status change dropdown for allowed transitions

---

### WF-AP-03 — Appointment Detail Page `[P]`

**Phase:** 2B  
**Actor:** Admin, Dentist, Staff  
**BRD Ref:** FR-04.1–FR-04.5

**Description:** The full read/action view for a single appointment.

**Key UI Elements:**

- **Header section**: Patient name (link to WF-PT-02), Dentist, Date & Time, Duration, Source badge (Portal / Clinic), Status badge (color-coded), Created by
- **Status Action Bar**: Available transition buttons based on current status:
  - Pending: "Confirm", "Cancel"
  - Confirmed: "Mark In Progress", "No Show", "Cancel"
  - In Progress: "Complete Appointment"
  - Awaiting Approval: "Approve", "Reject"
  - Awaiting Deposit Verification: "Verify Deposit", "Reject Deposit" → WF-DP-02
- **Planned Treatments section**: list of treatments with prices and optional negotiated price; HMO coverage per treatment (if applicable); LOA Number field
- **Chief Complaint** (read-only)
- **Vital Signs panel** (FR-04.5): shown for Confirmed or In Progress status — fields: BP Systolic / Diastolic, Pulse Rate, Respiratory Rate, Temperature, SpO2, Clinical Notes. "Record Vitals" button. High-BP warning banner shown inline if systolic ≥ 180 or diastolic ≥ 110. X-Ray + pregnancy safety warning banner (FR-04.5.6)
- **Procedures panel** (In Progress only): link/button → WF-PR-01 (procedure recording), list of already-recorded procedures
- **Billing panel** (Completed only): link to billing record → WF-BL-01
- **Status History** (collapsible): timeline of all status transitions with actor and timestamp
- **Cancellation Reason** (shown if Cancelled or No Show)
- **Reschedule link** (if No Show or Cancelled by staff): "Create Follow-up Appointment"

---

### WF-AP-04 — Create / Edit Appointment Form `[M]` or `[P]`

**Phase:** 2B  
**Actor:** Admin, Dentist, Staff  
**BRD Ref:** FR-04.1

**Key UI Elements:**

- **Patient** — search-as-you-type (active patients only, per BR-01.2)
- **Dentist** — dropdown (Users with role Dentist or Administrator)
- **Date** — date picker
- **Time** — time picker (within clinic operating hours)
- **Duration** — number input (minutes, defaults to clinic setting)
- **Conflict Detection** — triggered on date/time/dentist change; if conflict detected: red inline alert showing conflicting appointment (patient name, time) + "Save" button disabled
- **Planned Treatments** — searchable multi-select tag input; each selected treatment shows base price and a negotiated price override field. HMO coverage info shown if patient has a linked provider.
- **Chief Complaint** (textarea)
- **LOA Number** (text input, shown if patient has HMO provider linked)
- **Walk-in toggle** — when enabled, sets initial status to "In Progress"
- **Appointment Type** (for portal-context appointments — shown when creating from portal staff side)
- "Save Appointment" and "Cancel"

---

### WF-AP-05 — Cancel Appointment Dialog `[M]`

**Phase:** 2B  
**Actor:** Admin, Dentist, Staff  
**BRD Ref:** FR-04.2, FR-04.2.2

**Key UI Elements:**

- "Cancel Appointment" title
- Read-only: Patient name, Date/Time
- **Cancellation Reason\*** text area (min 10 characters, character counter shown)
- "Confirm Cancellation" (danger button) and "Go Back"

---

### WF-AP-06 — Reschedule Appointment `[M]`

**Phase:** 2B  
**Actor:** Admin, Dentist, Staff  
**BRD Ref:** FR-04.2 (No Show → Reschedule), FR-10.4

**Key UI Elements:**

- "Reschedule Appointment" title
- New Date*, New Time*, New Duration
- Dentist (pre-filled, editable)
- Conflict detection inline
- Notification notice: "An SMS and email will be sent to the patient with the original and new appointment details."
- "Confirm Reschedule" and "Cancel"

---

### WF-AP-07 — Vital Signs Recording Panel `[D]` or inline `[T]`

**Phase:** 2B  
**Actor:** Admin, Dentist, Staff  
**BRD Ref:** FR-04.5

**Key UI Elements:**

- BP Systolic (mmHg), BP Diastolic (mmHg) — side by side
- Pulse Rate (bpm), Respiratory Rate (breaths/min)
- Temperature (°C, optional), SpO2 (%, optional)
- Clinical Notes (textarea)
- High-BP warning banner (inline, shown dynamically when value ≥ threshold)
- X-Ray + pregnancy safety banner (if applicable)
- "Save Vitals" and "Cancel"

---

### WF-AP-08 — Pending Approvals Inbox `[P]`

**Phase:** 2B (expanded in 6B)  
**Actor:** Admin, Staff  
**BRD Ref:** FR-04.2, FR-12.4.5–FR-12.4.6a, BR-01.7

**Description:** A dedicated view for portal bookings awaiting staff action.

**Key UI Elements:**

- Tab switcher: "Awaiting Approval" | "Awaiting Deposit Verification"
- DataTable: Date Submitted | Patient | Appointment Type | Requested Date/Time | Dentist | Actions
- For each row: "Approve" (→ transitions to Pending), "Reject" (→ opens reason dialog), "View Patient"
- For deposit verification rows: uploaded receipt image preview thumbnail, "Verify Deposit", "Reject" actions → WF-DP-02

---

## 8. Dental Charting

### WF-DC-01 — Dental Chart View / Edit `[P]` or `[T]`

**Phase:** 3B  
**Actor:** Admin, Dentist (view); Admin, Dentist + active In Progress appointment (edit)  
**BRD Ref:** FR-05

**Description:** The FDI ISO 3950:2016 interactive SVG dental chart showing all 52 teeth.

**Key UI Elements:**

- **Chart header**: Patient name, date of birth, current date, viewing dentist
- **SVG Chart Canvas**:
  - Upper jaw row: permanent Q1 (11–18) + Q2 (21–28), primary Q5 (51–55) + Q6 (61–65)
  - Lower jaw row: permanent Q3 (31–38) + Q4 (41–48), primary Q7 (71–75) + Q8 (81–85)
  - Each tooth rendered as an SVG shape with 5 surface areas (Buccal, Lingual, Mesial, Distal, Occlusal/Incisal)
  - Each surface colored/symboled according to active condition using PDA/FDI standard symbols
  - Teeth with an "In Progress" procedure (FR-06.2.7) flagged with a distinct border (e.g., amber dashed outline)
  - Hover on a tooth → tooltip showing tooth number and active condition per surface
  - Click on a tooth → opens WF-DC-02 (detail/condition update panel)
- **Chart Legend**: always visible; lists all 23 condition codes with their symbols and names
- **Quick Set Tool** (edit mode only, FR-03.1.6): "Select multiple teeth, then set condition" — multi-select mode toggle, condition picker, "Apply to Selected" button
- **View Mode vs. Edit Mode**: edit mode enabled only during an active In Progress appointment session; a banner shows "Editing chart for: [Patient] — Appointment on [date]" in edit mode
- **Quadrant labels**: Q1–Q8 quadrant captions above/below the rows

---

### WF-DC-02 — Tooth Detail Panel `[D]`

**Phase:** 3B  
**Actor:** Admin, Dentist  
**BRD Ref:** FR-05.2–FR-05.3

**Description:** Slides in from the right when a tooth is clicked on the chart.

**Key UI Elements:**

- Tooth number (FDI, e.g., "Tooth 36") and name (e.g., "Lower Left First Molar") as panel title
- **Active Conditions section**: grid showing each of the 5 surfaces with its current active condition code and name
- **Update Condition form** (edit mode only):
  - Surface selection (multi-checkbox: B / L / M / D / O)
  - Condition picker (dropdown of 23 standard conditions)
  - Remarks (optional textarea)
  - "Save Condition Update" button
- **Condition History section**: timeline list — Date | Surfaces | Condition | Recorded By | Linked Appointment | Status (Active / Historical)
- **Linked Treatment Records**: list of all procedures ever performed on this tooth — Date | Treatment | Dentist | Completion Status
- Close (X) button

---

### WF-DC-03 — Condition Update Confirmation `[B]` inline

**Phase:** 3B  
**BRD Ref:** FR-05.3.3

**Key UI Elements:**

- After saving a condition update: inline success message "Condition updated for Surface(s) [B, M] on Tooth 36. Previous condition archived."
- If updating a surface that already has an active condition: advisory "Previous [condition] for [surface] has been moved to history."

---

## 9. Treatment Records & Procedures

### WF-PR-01 — Procedure Recording Workspace `[P]`

**Phase:** 3B  
**Actor:** Admin, Dentist  
**BRD Ref:** FR-06.2

**Description:** The main clinical workspace active during an In Progress appointment. Combines the dental chart and procedure entry into a single workspace view.

**Key UI Elements:**

- **Left panel**: Dental Chart (WF-DC-01 in edit mode) — clicking a tooth populates the right panel
- **Right panel**: Procedure entry and recorded procedure list
  - **Add Tooth-Specific Procedure** (when a tooth is selected from chart):
    - Tooth (auto-filled from chart selection) and surface(s)
    - Before condition (auto-filled from active condition)
    - Treatment (searchable dropdown — applicable conditions prioritized per FR-05.4.1)
    - After condition (pre-filled from Resulting Condition, editable)
    - Completion flag: "Complete" / "In Progress" radio
    - If "In Progress" selected and no Interim Condition: advisory message (FR-06.2.5)
    - Amount charged (pre-filled from price, editable)
    - Clinical notes (textarea)
    - "Add Procedure" button
  - **Add Global Procedure**:
    - Treatment (searchable dropdown — Global-scoped only)
    - Amount charged
    - Clinical notes
    - "Add Global Procedure" button
  - **Procedure List**: recorded procedures for this appointment in a scrollable table — Type (Tooth/Global) | Tooth/Surface | Treatment | Completion | Amount | Notes | Remove
  - Consumed Supplies panel (WF-PR-02)
- **Footer bar**: "Mark Appointment as Completed" button (triggers billing creation) + confirmation dialog

---

### WF-PR-02 — Supply Consumption Panel `[D]` or inline section

**Phase:** 3B  
**Actor:** Admin, Dentist, Staff  
**BRD Ref:** FR-08.3.1–FR-08.3.4

**Key UI Elements:**

- Header: "Consumed Supplies"
- Auto-populated rows from Procedure Kit (if applicable) — item name, default quantity, editable quantity field
- "Add Supply Item" button: search for Tracked supply items, quantity input
- Each row: Item Name | Lot (optional) | Quantity | Remove
- Warning banner if requested quantity exceeds current stock: "Insufficient stock for [item]. Saving will require an explanation." + explanation text field

---

### WF-PR-03 — Ortho Progress Note Form `[M]` or inline panel

**Phase:** 5B  
**Actor:** Admin, Dentist  
**BRD Ref:** FR-06.4

**Key UI Elements:**

- Panel title: "Orthodontic Progress Note — [Appointment Date]"
- Upper arch wire (text input)
- Lower arch wire (text input)
- Upper arch elastics / elastic chain (text, optional)
- Lower arch elastics / elastic chain (text, optional)
- Bracket / attachment changes (textarea, optional)
- Appliance status (dropdown: Active / Adjusted / Completed / Retention Phase / Interrupted)
- Clinical notes (textarea)
- "Save Note" and "Cancel"

---

### WF-TR-01 — Treatment Catalog Page `[P]`

**Phase:** 3B  
**Actor:** Admin, Dentist (manage); Staff (read-only)  
**BRD Ref:** FR-06.1

**Key UI Elements:**

- Page title: "Treatment Catalog"
- "Add Treatment" button (Admin, Dentist)
- Filter bar: Category (multi-select), Status (Active / Archived), Scope (Tooth-Specific / Global), search by name
- DataTable: Name | Category badge | Scope badge | Base Price | Estimated Duration | Requires X-Ray | Status | Actions
- Row actions: Edit → WF-TR-02, Archive (with confirmation), Restore

---

### WF-TR-02 — Create / Edit Treatment Dialog `[M]`

**Phase:** 3B  
**Actor:** Admin, Dentist  
**BRD Ref:** FR-06.1.1–FR-06.1.9

**Key UI Elements:**

- Name\*, Description
- Category\* (dropdown: 8 categories from BRD section 4.3)
- Procedure Scope\* (radio: Tooth-Specific / Global)
- Base Price\* (₱ decimal input)
- Estimated Duration (minutes)
- Requires X-Ray (toggle)
- Status (Active / Archived)
- _(Tooth-Specific only)_ Applicable Conditions (multi-select from 23-condition enum)
- _(Tooth-Specific only)_ Resulting Condition (dropdown, nullable)
- _(Tooth-Specific only)_ Interim Condition (dropdown, nullable)
- _(Tooth-Specific only)_ Required Specialization (dropdown, optional)
- **Procedure Kit tab**: dynamic rows for supply items + default quantities (search supply catalog)
- **Dentist Pricing tab**: table of dentist-specific price overrides — Dentist name | Override Price | Add/Edit/Remove
- "Save" and "Cancel"

---

## 10. Billing & Payments

### WF-BL-01 — Billing Detail Page `[P]`

**Phase:** 3B  
**Actor:** Admin (full), Staff (create/finalize), Dentist (read-only)  
**BRD Ref:** FR-07.1–FR-07.5

**Key UI Elements:**

- **Header**: Patient name (link), Appointment Date, Billing Status badge, Created Date
- **Procedures section** (line items): Treatment | Tooth | Amount | HMO Coverage | Patient Share per line
- **Totals section**:
  - Subtotal, Discount (with reason), Tax (12% VAT or 0%), Total Amount
  - HMO Coverage Amount (editable if Draft/Final status), LOA reference
  - Patient Share, Total Paid, **Outstanding Balance** (computed, highlighted)
- **Discount field**: amount input + reason text (Admin/Staff only, Draft/Final status)
- **Status action bar**: "Finalize Billing" (Draft → Final, Staff/Admin), "Void Billing" (Admin only → WF-BL-03)
- **Installment Plan section** (FR-07.5): if plan active — shows plan details + schedule table (WF-BL-04). If no plan: "Create Installment Plan" button (Admin/Staff only)
- **Payments section**: list of recorded payments — Date | OR Number | Amount | Method | Received By | Status | Actions (Void if Admin, Print OR)
- "Record Payment" button → WF-BL-02
- "Download SOA" (PDF) button

---

### WF-BL-02 — Record Payment Dialog `[M]`

**Phase:** 3B  
**Actor:** Admin, Staff  
**BRD Ref:** FR-07.3

**Key UI Elements:**

- Outstanding Balance shown prominently (read-only)
- **Amount Paid\*** (decimal input — validated: cannot exceed outstanding balance)
- **Payment Method\*** (dropdown: Cash / GCash / Maya / Credit Card / Debit Card / Check / HMO Direct Pay / Bank Transfer)
- **OR Number\*** (text input — manually entered from ATP booklet; uniqueness validated)
- Date & Time (auto-filled to now, editable)
- Notes (textarea)
- "Record Payment" and "Cancel"
- After save: success toast + "Print Official Receipt" button

---

### WF-BL-03 — Void Billing Dialog `[M]`

**Phase:** 3B  
**Actor:** Administrator only  
**BRD Ref:** FR-07.1.7

**Key UI Elements:**

- Warning: "Voiding this billing is irreversible. All linked payments will also be voided."
- Void Reason\*\*\* text area (min 10 characters)
- "Confirm Void" (danger) and "Cancel"

---

### WF-BL-04 — Installment Plan Form `[M]`

**Phase:** 5B  
**Actor:** Admin, Staff  
**BRD Ref:** FR-07.5.1–FR-07.5.8

**Key UI Elements:**

- Total Amount shown (read-only, = Outstanding Balance)
- Number of Installments\* (number input)
- Amount Per Installment\* (auto-computed or manually set)
- Description / Notes
- **Installment Schedule table** (auto-generated, editable):
  - # | Due Date (editable date picker) | Expected Amount (editable) | Status
  - "Add row" / "Remove row" actions
- "Save Plan" and "Cancel"
- After creation: the installment schedule is shown in WF-BL-01 with individual Waive/Edit actions per row (Admin only)

---

### WF-BL-05 — Official Receipt View / Print `[P]` or `[M]`

**Phase:** 3B  
**Actor:** Admin, Dentist, Staff  
**BRD Ref:** FR-07.3.6

**Key UI Elements:**

- Formatted receipt layout (print-optimized CSS):
  - Clinic name, address, TIN, BIR COR number
  - OR Number (large, prominent), Date of Payment
  - Patient Name, Amount in Words, Amount in Figures
  - Services Rendered (treatments listed)
  - Payment Method
  - Cashier name
  - "VOID" watermark if payment is voided
- "Print" button, "Download PDF" button, "Close" button

---

### WF-BL-06 — OR Register Page `[P]`

**Phase:** 4B  
**Actor:** Administrator  
**BRD Ref:** FR-09.3

**Key UI Elements:**

- Filter: date range
- DataTable: OR Number | Date | Patient | Amount | Method | Issued By | Status (Valid / Voided)
- "Export PDF" and "Export CSV" buttons

---

## 11. Inventory & Supply Management

### WF-INV-01 — Supply Item List Page `[P]`

**Phase:** 4B  
**Actor:** Admin, Staff  
**BRD Ref:** FR-08.1

**Key UI Elements:**

- Page title: "Inventory"
- "Add Supply Item" button
- Filter bar: Category (multi-select), Tier (Tracked / Bulk-Managed), Status (Active / Archived), Low Stock (toggle), Near Expiry (toggle), search by name
- DataTable: Name | Category badge | Tier badge | Unit | Current Stock | Critical Threshold | Low-Stock flag | Nearest Expiry | Status | Actions
- Low-stock rows highlighted with amber/red indicator
- Near-expiry rows highlighted with amber/orange indicator
- Row actions: "Edit" → WF-INV-03, "Receive Stock" → WF-INV-04, "Adjust Stock" → WF-INV-05, "View Ledger" → WF-INV-02, Archive

---

### WF-INV-02 — Stock Ledger Page `[P]`

**Phase:** 4B  
**Actor:** Admin, Staff  
**BRD Ref:** FR-08.2

**Key UI Elements:**

- Sub-page title: "[Item Name] — Stock Ledger"
- Current stock total (computed) shown prominently
- Filter: date range, change type
- Chronological ledger table: Date | Change Type badge | Quantity Change (+/-) | Running Total | Lot | Expiry | Unit Cost | Reference | Recorded By | Remarks
- "Back to Inventory" breadcrumb

---

### WF-INV-03 — Create / Edit Supply Item Dialog `[M]`

**Phase:** 4B  
**Actor:** Admin, Staff  
**BRD Ref:** FR-08.1.1

**Key UI Elements:**

- Name*, Description, Category* (dropdown), Unit of Measure* (dropdown), Tier* (Tracked / Bulk-Managed), Preferred Supplier (searchable dropdown), Unit Cost (₱), Critical Quantity Threshold\* (integer), Status
- "Save" and "Cancel"

---

### WF-INV-04 — Receive Stock Dialog `[M]`

**Phase:** 4B  
**Actor:** Admin, Staff  
**BRD Ref:** FR-08.2.1–FR-08.2.3

**Key UI Elements:**

- Supply Item (read-only if opened from item row, searchable if standalone)
- Quantity Received\* (positive number)
- Lot Number\*
- Expiry Date (required for Anesthetic/Medication category items, optional others)
- Unit Cost at Receipt\*
- Supplier\* (searchable dropdown)
- Remarks
- "Receive" and "Cancel"

---

### WF-INV-05 — Stock Adjustment Dialog `[M]`

**Phase:** 4B  
**Actor:** Admin, Staff  
**BRD Ref:** FR-08.2.2 (Adjustment, Expired, Voided)

**Key UI Elements:**

- Supply Item (read-only or searchable)
- Change Type\* (dropdown: Adjustment / Expired / Voided)
- Quantity Change\* (signed number input: positive = stock in, negative = stock out)
- Lot Number (optional)
- Mandatory Reason\*\*\* (textarea, always required for adjustments)
- "Save" and "Cancel"

---

### WF-INV-06 — Physical Count Worksheet `[P]`

**Phase:** 4B  
**Actor:** Admin, Staff  
**BRD Ref:** FR-08.3.5

**Description:** Bulk-Managed items periodic count reconciliation.

**Key UI Elements:**

- List of all Bulk-Managed active items: Item Name | Current System Stock | Physical Count (editable input) | Variance (auto-computed)
- Date of Count (date picker)
- "Submit Physical Count" button — generates Adjustment entries for all items with non-zero variance
- "Cancel"

---

### WF-INV-07 — Supplier List Page `[P]`

**Phase:** 4B  
**Actor:** Admin, Staff  
**BRD Ref:** FR-08.4

**Key UI Elements:**

- Page title: "Suppliers"
- "Add Supplier" button
- DataTable: Name | Contact | Email | Delivery Days | Status | Actions
- Row actions: Edit → WF-INV-08, Archive

---

### WF-INV-08 — Create / Edit Supplier Dialog `[M]`

**Phase:** 4B  
**Actor:** Admin, Staff  
**BRD Ref:** FR-08.4.1

**Key UI Elements:**

- Name\*, Address, Contact Number, Email, Delivery Days (multi-checkbox: Mon–Sun), Notes, Status
- "Save" and "Cancel"

---

## 12. Reports & Analytics

### WF-RP-00 — Reports Hub Page `[P]`

**Phase:** 4B  
**Actor:** Admin (all 14), Staff (limited), Dentist (limited)  
**BRD Ref:** FR-09.2

**Key UI Elements:**

- Page title: "Reports"
- Grid or list of available reports (role-filtered — reports user cannot access are not shown):
  - Card per report: report name, description, access role badge, "Generate" button
- Reports list: RPT-01 through RPT-14

---

### WF-RP-01 — Report Viewer `[P]`

**Phase:** 4B  
**Actor:** Per report access rules  
**BRD Ref:** FR-09.2

**Description:** Generic report viewer shared by all 14 standard reports. Parameter section at top, results below.

**Key UI Elements:**

- Report title and description
- **Parameters section** (varies by report):
  - Date / date range pickers (most reports)
  - Patient search (RPT-03)
  - Dentist dropdown (RPT-07)
  - Provider dropdown (RPT-13)
- "Generate Report" button
- Loading spinner while fetching
- **Results area**: table or structured layout (report-specific columns per FR-09.2)
- "Print" button, "Export PDF" button, "Export CSV" button (where applicable per report)
- Empty state if no data matches parameters

---

### WF-RP-14 — Daily Collection Report (RPT-14) Viewer `[P]`

**Phase:** 4B  
**Actor:** Administrator  
**BRD Ref:** FR-09.2 (RPT-14)

**Description:** Specialized viewer for the BIR-aligned daily collection report.

**Key UI Elements:**

- Date picker (single date, defaults to today)
- "Generate" button
- Table: OR Number | Patient Name | Services Rendered | Amount | Payment Method | Cashier
- Grand Total row
- "Export PDF" (print-optimized single-page format), "Export CSV"

---

### WF-RP-EX — Bulk Data Export Page `[P]`

**Phase:** 6B  
**Actor:** Administrator only  
**BRD Ref:** FR-13

**Key UI Elements:**

- Page title: "Bulk Data Export"
- Export scope: "All Records" (radio) or "Date Range" (radio + start/end date pickers)
- Entity checkboxes: Patients, Appointments, Treatment Records, Billings, Payments, Installment Plans, Prescriptions, Medical Certificates, Ortho Progress Notes
- Format: CSV (default) / Excel (.xlsx) radio
- "Initiate Export" button
- Status panel: shows pending / processing / ready states; "Download (valid 24h)" link when ready
- Export history table: Date Initiated | Scope | Entities | Format | Status | Download
- Privacy notice: "Exported files contain sensitive personal information (RA 10173). Handle securely."

---

## 13. Audit Log

### WF-AL-01 — Audit Log Page `[P]`

**Phase:** 1B  
**Actor:** Administrator only  
**BRD Ref:** FR-11

**Key UI Elements:**

- Page title: "Audit Log"
- Filter bar: Date Range (start/end), Actor (user search), Action Type (dropdown: Create / Update / Archive / Restore / Delete / Login / Logout / Password Change), Entity Type (dropdown of all entity types)
- Paginated DataTable:
  - Timestamp | Actor (name + role) | Action Type badge | Entity Type | Entity ID | Description
- Row expandable: shows JSON diff of changed field values (old → new); sensitive fields show "[field changed]" only
- IP Address column (collapsible)
- "Export CSV" button (filtered results)
- Read-only — no edit or delete actions

---

## 14. Specialized Clinical Modules

### WF-RX-01 — Prescription Form / View `[P]`

**Phase:** 5B  
**Actor:** Admin, Dentist  
**BRD Ref:** FR-15

**Key UI Elements:**

- Page shows clinic letterhead header (clinic name, address)
- Patient name and age (auto-filled), Date\*, Rx No. (auto-assigned), linked appointment (optional dropdown)
- Prescribing dentist (auto-filled to logged-in dentist), PRC No., PTR No. (editable for annual renewal), S2 License No. (optional)
- S2 warning banner if S2 field is empty and controlled substance may be needed (FR-15.7, BR-10.4)
- **Prescription Items** — dynamic multi-row:
  - Drug Name\* (text with autocomplete suggestions: Amoxicillin, Mefenamic Acid, Ibuprofen, etc.)
  - Strength / Dosage Form*, Quantity*, Sig / Directions\*
  - "Add Item" button, remove per row
- Clinical Notes
- **Actions**: "Save Prescription", "Print / Download PDF", "Void" (Admin, read-only mode only)
- Wet signature line note: "Sign in ink before dispensing"
- View mode (after save): read-only display, "Print PDF" button visible

---

### WF-MC-01 — Medical / Dental Certificate Form / View `[P]`

**Phase:** 5B  
**Actor:** Admin, Dentist  
**BRD Ref:** FR-16

**Key UI Elements:**

- Certificate type selector: "Medical Certificate" / "Dental Certificate"
- Patient name, age, sex (auto-filled from patient record)
- Issuing dentist (auto-filled), PRC No., PTR No.
- Date of Consultation/Procedure\* (defaults to linked appointment date or today)
- Diagnosis / Finding\*\*\* (free text)
- Procedure Performed (free text, defaults to appointment procedures if linked)
- Recommended Rest Period (integer, days, optional)
- Fitness for Duty / Return-to-School Date (date picker, optional)
- Additional Remarks (textarea)
- Cert No. (auto-assigned)
- Wet signature line note: "Sign in ink before submission"
- **Actions**: "Save Certificate", "Print / Download PDF", "Void" (Admin only)
- View mode: read-only, "Print PDF" button

---

## 15. Patient Portal

> All wireframes in this section belong to the separate **Patient Portal SPA** (`frontend/allaboutteeth-portal/`). The portal uses a distinct layout shell from the clinic app, with a patient-focused navigation and no clinical tool access.

### WF-PO-00 — Portal Shell `[P]`

**Phase:** 6B  
**BRD Ref:** FR-12, BR-07.2

**Key UI Elements:**

- Clinic logo and name in top bar
- Top navigation: Home | My Appointments | My Health Records | Billing | Feedback | Profile | Logout
- For Primary Account Holders with Dependents: account switcher dropdown in top bar (switch active profile to self or a dependent)
- Notification bell (in-portal notifications: rating prompts, feedback status updates, appointment actions)
- "Book Appointment" CTA button (sticky)
- Footer: clinic contact, privacy notice link

---

### WF-PO-01 — Portal Registration Page `[P]`

**Phase:** 6B  
**Actor:** New patient (public, unauthenticated)  
**BRD Ref:** FR-12.1.1–FR-12.1.5

**Key UI Elements:**

- "Create Your Account" heading
- First Name*, Last Name*, Date of Birth*, Sex* (dropdown)
- Mobile Number*, Email Address* (used as login)
- Password* + Confirm Password* (strength hint)
- **Privacy Notice text** displayed in scrollable box
- "I acknowledge and consent to the collection and processing of my personal information" checkbox\*
- "Create Account" button
- Success state: "A verification email has been sent to [email]. Please check your inbox." with resend link
- Already have an account? Log in link

---

### WF-PO-02 — Portal Login Page `[P]`

**Phase:** 6B  
**Actor:** Registered patient  
**BRD Ref:** FR-12.2.1–FR-12.2.3

**Key UI Elements:**

- Email* and Password* fields
- "Login" button
- Inline error: invalid credentials / locked account message
- "Forgot your password?" link → WF-PO-03
- "Don't have an account? Register here" link

---

### WF-PO-03 — Portal Password Reset `[P]`

**Phase:** 6B  
**Actor:** Registered patient  
**BRD Ref:** FR-12.2.3

**Key UI Elements:**

- Email input
- "Send Reset Link" button
- Confirmation message: "If your email is registered, you will receive a reset link."
- Reset link page: new password + confirm password fields

---

### WF-PO-04 — Portal Medical / Dental History Intake (Multi-Step Form) `[P]`

**Phase:** 6B  
**Actor:** New portal patient  
**BRD Ref:** FR-12.3.1–FR-12.3.5

**Description:** Guided multi-step form presented after account verification. Optional but encouraged.

**Steps:**

1. Contact information (phone, address — FR-03.2)
2. Dental history (previous dentist, last visit, chief complaint, HMO — FR-03.3)
3. Medical history intake (full FR-03.4 form — physician, health, allergies, medications, conditions, lifestyle)
4. Review & Submit

**Key UI Elements:**

- Step progress indicator
- "Skip for now" option on each step
- "Pending Staff Review" notice displayed after submission: "Your information will be reviewed by our clinic staff at your first visit."
- "Back" / "Next" / "Submit" buttons

---

### WF-PO-05 — Book Appointment Page `[P]`

**Phase:** 6B  
**Actor:** Logged-in patient  
**BRD Ref:** FR-12.4

**Description:** Multi-step booking wizard.

**Step 1 — Select Appointment Type:**

- Grid of available appointment type cards (from FR-02.16 configuration): label, description, estimated duration
- Patient selects one type

**Step 2 — Select Dentist & Date:**

- "Any Available Dentist" or select specific dentist (filtered by specialization if appointment type has Required Specialization)
- Month calendar view showing available/unavailable dates
- Available dates are clickable

**Step 3 — Select Time Slot:**

- Time slot grid for selected date and dentist
- Available slots shown; booked slots shown as greyed "Unavailable" (no patient info disclosed)
- Patient selects a slot

**Step 4 — Chief Complaint:**

- "What brings you in?" textarea (required, min 5 characters)
- Summary of selected: Appointment Type, Date, Time, Dentist (or "First Available")

**Step 5 — Review & Confirm:**

- Full booking summary
- If deposit is configured (FR-02.14): deposit amount shown, forfeiture policy text, payment method selector → proceeds to WF-DP-03 or WF-DP-04
- If no deposit: "Confirm Booking" button
- Intake-pending banner if medical history not yet reviewed (FR-12.4.10)

**Confirmation page:**

- "Booking Confirmed!" (or "Awaiting Approval" if in that mode)
- Appointment summary, reference number
- "View My Appointments" button
- Email confirmation notice

---

### WF-PO-06 — My Appointments Page `[P]`

**Phase:** 6B  
**Actor:** Logged-in patient  
**BRD Ref:** FR-12.5

**Key UI Elements:**

- Tab: Upcoming | Past
- List for each: Date & Time | Dentist | Appointment Type | Status badge (patient-friendly labels)
- Row actions (upcoming, Pending/Confirmed status only):
  - "Cancel" → WF-PO-07
  - "Reschedule" → guided flow: cancels current + opens WF-PO-05
- Past appointments: "Rate Dentist" button if not yet rated (FR-12.8.1) → WF-PO-09
- Awaiting Approval status shown as "Pending Review" label
- Awaiting Deposit Verification shown as "Upload Confirmed — Awaiting Staff Verification"

---

### WF-PO-07 — Cancel Appointment Dialog (Portal) `[M]`

**Phase:** 6B  
**Actor:** Logged-in patient  
**BRD Ref:** FR-12.5.2, BR-07.4

**Key UI Elements:**

- Appointment summary (read-only)
- Cancellation reason\*\*\* (textarea, min 10 characters)
- If deposit paid: forfeiture policy notice displayed
- "Confirm Cancellation" and "Keep Appointment" buttons

---

### WF-PO-08 — My Health Records Page `[P]`

**Phase:** 6B  
**Actor:** Logged-in patient  
**BRD Ref:** FR-12.6

**Key UI Elements:**

- Chronological treatment history list:
  - Date | Dentist | Treatments (by name only — no FDI codes per FR-12.6.5) | Patient-visible Notes
- Filter: date range, dentist
- Read-only; no edit or delete actions available
- Note at top: "For full clinical records, please contact our clinic."

---

### WF-PO-09 — Billing & Payments Page `[P]`

**Phase:** 6B  
**Actor:** Logged-in patient  
**BRD Ref:** FR-12.7

**Key UI Elements:**

- List of billing records: Date | Treatments | Total Billed | Total Paid | Balance | Status badge (Unpaid / Partially Paid / Fully Paid)
- Row actions: "Download SOA" (PDF), "View Payments" (expand/collapse sub-list of payment records with OR download links)
- Draft and Voided billings are not shown

---

### WF-PO-10 — Installment Schedule View (Portal) `[P]` or `[T]`

**Phase:** 6B  
**Actor:** Logged-in patient  
**BRD Ref:** FR-07.5.5

**Key UI Elements:**

- Installment schedule table: # | Due Date | Expected Amount | Status badge (Upcoming / Due / Overdue / Paid / Waived)
- Total plan amount, amount paid to date, remaining balance
- Read-only

---

### WF-PO-11 — Rate Your Dentist Dialog `[M]`

**Phase:** 6B  
**Actor:** Logged-in patient  
**BRD Ref:** FR-12.8

**Key UI Elements:**

- Appointment details (date, dentist name — anonymously for the dentist, but shown to patient)
- 5-star interactive rating widget
- Optional comment (textarea, max 500 characters, character counter)
- "Submit Rating" and "Skip" buttons
- Edit state (within 7-day window): pre-filled with existing rating, "Update Rating" button
- Post-lock state: read-only rating display

---

### WF-PO-12 — Submit Feedback Page `[P]`

**Phase:** 6B  
**Actor:** Logged-in patient  
**BRD Ref:** FR-12.9

**Key UI Elements:**

- Type\* (radio or dropdown: Complaint / Suggestion / Recommendation / General)
- Subject\*\* (text input, max 200 characters)
- Message\*\*\* (textarea, max 2,000 characters with character counter)
- Link to specific appointment (optional — dropdown of past appointments)
- "Submit Feedback" button
- "My Feedback" list below or on a tab: Type | Subject | Date | Status badge (New / Under Review / Resolved / Closed). Complaint items shown with red priority indicator.

---

### WF-PO-13 — Patient Profile (Portal) `[P]`

**Phase:** 6B  
**Actor:** Logged-in patient  
**BRD Ref:** FR-12.3.5, FR-10.9

**Key UI Elements:**

- Read-only display of personal information
- **Editable fields** (contact info only — per FR-12.3.5): Phone Number, Address, Email (triggers re-verification if changed)
- "Save Contact Info" button
- **Notification Preferences section** (FR-10.9): per-event toggle for SMS and email — Appointment Confirmation, Reschedule, Cancellation, 24-hour Reminder, Recall Reminder
- **Dependents section**: list of dependent patient profiles with "Add Dependent" button → WF-PO-14
- **Account Security**: "Change Password" link (opens password change form)

---

### WF-PO-14 — Add Dependent Form `[M]`

**Phase:** 6B  
**Actor:** Logged-in patient (Primary Account Holder)  
**BRD Ref:** FR-12.1.7–FR-12.1.8

**Key UI Elements:**

- First Name*, Last Name*, Date of Birth*, Sex*, Relationship to Account Holder\*
- "Add Dependent" and "Cancel" buttons
- Limit notice: "You may add up to [N] dependents" with current count shown

---

## 16. Online Booking Deposits

### WF-DP-01 — Payment Gateway Redirect Flow `[P]`

**Phase:** 7B  
**Actor:** Logged-in patient  
**BRD Ref:** FR-12.4.6 (gateway method)

**Key UI Elements:**

- Deposit summary: Amount (₱), Appointment Details, Forfeiture Policy text
- "Proceed to Payment" button → redirects to gateway checkout page (PayMongo / Maya / Paynamics)
- Countdown timer: "Your slot is reserved for 15 minutes"
- Return states:
  - **Success**: "Deposit Paid! Your appointment is confirmed." → appointment summary
  - **Cancelled / Timeout**: "Your slot reservation has been released. Please rebook." → link to WF-PO-05

---

### WF-DP-02 — Manual Deposit Upload `[P]`

**Phase:** 7B  
**Actor:** Logged-in patient  
**BRD Ref:** FR-12.4.6 (manual deposit method), FR-02.12a

**Key UI Elements:**

- Clinic's manual deposit instructions text (GCash / bank details, configured in FR-02.12a)
- Deposit Amount displayed prominently
- File upload control: "Upload your payment receipt screenshot (JPEG, PNG, or PDF)"
- File preview thumbnail
- "Submit Deposit Receipt" button
- Post-submission: "Your booking is awaiting deposit verification. We will notify you once confirmed."

---

### WF-DP-03 — Staff Deposit Verification Inbox `[P]`

**Phase:** 7B  
**Actor:** Admin, Staff  
**BRD Ref:** FR-12.4.6a

**Description:** Part of WF-AP-08 or a dedicated sub-view. Accessed via the portal bookings inbox.

**Key UI Elements:**

- DataTable: Patient | Appointment Date | Submitted At | Receipt Image (thumbnail, click to enlarge)
- Row actions: "Verify Deposit" (green button) | "Reject" (opens reason dialog)
- After verification: appointment advances to Pending; deposit recorded as pre-payment
- After rejection: patient notified; slot released

---

### WF-DP-04 — Acknowledgment Receipt View `[P]`

**Phase:** 7B  
**Actor:** Logged-in patient  
**BRD Ref:** BR-02.12

**Key UI Elements:**

- Formatted AR (Acknowledgment Receipt) layout:
  - AR header note: "This is an Acknowledgment Receipt, not a BIR Official Receipt"
  - AR Number, Date/Time, Patient Name, Appointment Reference, Deposit Amount
  - Payment Gateway Transaction Reference
  - Clinic name and contact
- "Download PDF" button
- Note: "Your official BIR receipt will be issued at the clinic upon your visit."

---

## 17. Shared / Reusable Components

These are not standalone wireframes but component-level specs that appear across multiple screens.

### WF-GL-CAM — Webcam Capture Dialog `[M]`

**BRD Ref:** FR-03.1.4, FR-01.1.3  
**Screens Used:** Patient registration, User create/edit

**Key UI Elements:**

- Live webcam preview frame (square aspect ratio)
- "Capture Photo" button → shows preview of captured frame
- "Retake" and "Use This Photo" buttons
- "Upload from File" alternate tab (file picker, JPEG/PNG only)
- Crop tool (optional, center-crop to square)

---

### WF-GL-PDF — PDF Print / Download Panel `[M]` or inline

**BRD Ref:** FR-07.3.6, FR-07.2.3, FR-15.9, FR-16.7, FR-09.2

**Screens Used:** OR print, SOA download, Prescription print, Certificate print, Report export

**Key UI Elements:**

- Document preview (scaled-down render of the PDF layout)
- "Download PDF" button
- "Print" button (opens browser print dialog with print-optimized CSS)
- "Close"

---

### WF-GL-SEARCH — Patient Quick-Search Input

**BRD Ref:** FR-03.5.1  
**Screens Used:** Appointment create, Billing, Prescription creation, Certificate creation, Webcam capture

**Key UI Elements:**

- Text input with debounced search (real-time results after 2 characters)
- Dropdown results: photo thumbnail | Name | Patient ID | Age | Contact | Status badge
- Only Active patients shown (unless context requires archived)
- "No results found" empty state
- Keyboard navigation (arrow keys + Enter to select)

---

### WF-GL-STATUS — Status Badge Component

**BRD Ref:** FR-04.3.3  
**Screens Used:** Appointments throughout the app

| Status                        | Color    |
| ----------------------------- | -------- |
| Pending                       | Gray     |
| Awaiting Approval             | Teal     |
| Confirmed                     | Blue     |
| In Progress                   | Amber    |
| Completed                     | Green    |
| Cancelled                     | Red      |
| Rejected                      | Dark Red |
| No Show                       | Purple   |
| Awaiting Payment (Portal)     | Orange   |
| Awaiting Deposit Verification | Yellow   |

---

### WF-GL-NOTIF — In-App Notification Center `[D]`

**BRD Ref:** FR-10.1–FR-10.2  
**Accessed via:** Notification bell in WF-SH-01

**Key UI Elements:**

- Slide-in drawer: "Notifications" header + "Mark All Read" button
- Notification items by category:
  - Low-stock items (count badge, link to inventory item)
  - Near-expiry items (count badge, link)
  - Today's upcoming appointments (list)
  - Portal booking requests awaiting approval/verification (count badge)
  - New self-registered portal patients
- Each notification clickable → navigates to relevant page
- "View All" links per category

---

## Summary — Wireframe Count by Section

| Section                            | Wireframe Count  |
| ---------------------------------- | ---------------- |
| Global / Shell                     | 3                |
| Authentication & Password Recovery | 4                |
| Dashboard                          | 11 (1P + 10W)    |
| User & Access Management           | 3                |
| Clinic Settings                    | 9 (1P + 8T + 1M) |
| Patient Management                 | 6                |
| Appointment Scheduling             | 8                |
| Dental Charting                    | 3                |
| Treatment Records & Procedures     | 4                |
| Billing & Payments                 | 6                |
| Inventory & Supply Management      | 8                |
| Reports & Analytics                | 3                |
| Audit Log                          | 1                |
| Specialized Clinical Modules       | 2                |
| Patient Portal                     | 15               |
| Online Booking Deposits            | 4                |
| Shared / Reusable Components       | 5                |
| **Total**                          | **96**           |

---

## Wireframe Priority by Development Phase

| Phase | Wireframes                                                                                            |
| ----- | ----------------------------------------------------------------------------------------------------- |
| 1B    | WF-SH-01, WF-SH-02, WF-SH-03, WF-AU-01–04, WF-US-01–03, WF-CS-01 (tabs 1–5), WF-AL-01                 |
| 2B    | WF-DB-01–W10, WF-PT-01, WF-PT-02 (Tabs 1–7), WF-PT-03–05, WF-AP-01–08                                 |
| 3B    | WF-DC-01–03, WF-PR-01–02, WF-TR-01–02, WF-BL-01–05, WF-PT-02 (Tabs 11–12 injected)                    |
| 4B    | WF-INV-01–08, WF-RP-00–01, WF-RP-14, WF-BL-06, WF-CS-01 (Tab 6; stub only in 4B)                      |
| 5B    | WF-PR-03, WF-PT-06, WF-BL-04, WF-RX-01, WF-MC-01, WF-PT-02 (Tabs 8–10 injected)                       |
| 6B    | WF-PO-00–14 (all portal wireframes), WF-CS-01 (Tab 8), WF-CS-02, WF-RP-EX, WF-PT-02 (Tab 13 injected) |
| 7B    | WF-DP-01–04, WF-CS-01 (Tab 7)                                                                         |

---

_End of Wireframe Specification Document_
