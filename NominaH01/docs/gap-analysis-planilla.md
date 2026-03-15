# Gap analysis vs PlanillaPanama.com quotation

## Offered modules (quotation)
- Core: Recruitment/Selection, HR, Attendance control (cloud clock + Android app), Payroll (regular, vacations, 13th-month, terminations, claims, extraordinary, other rubrics via Excel), Employee portal, Multi-company.
- Processes: Commercial discounts, employee receivables, overtime calc, holidays, rotating shifts, auto income tax, occupational risks/licenses, provisions/reserves, construction concepts, payments ACH/cheque/cash, multiple pay slips, accounting entry export, prior-period payments.
- Compliance: SIPE (file + reconciliation), e-Tax planilla 03, income tax return module, severance funds (PROFUTURO/PROGRESO), expiring docs control.
- HR mgmt: Digital file, related persons, salary raise history, contracts, letters (work/income), notifications (contracts, vacations, docs, birthdays, leaves), KPIs (rotation, absenteeism, extras, retention), reports (vacation status, medical certificates, ISR status, labor liabilities).
- Support/Hosting: per-employee fee, first payroll accompaniment, training.

## Modules already present (from controllers)
- Payroll/catalogs: PayrollHeaders, PayrollTmpHeaders, PaymentConcepts, PaymentFrequencies, PaymentGroups, FixedPayments, OvertimeCodes, OverTimeFactors, LegalDeductions, Liabilities.
- HR base: Employees, EmployeeTypes, EmployeeTransfer, EmployeeObservations, ObservationTypes, Positions, Departments, Divisions, Projects, CostCenters.
- Banking/accounting: Banks, BankDepositStructures, CostCenterPaymentConceptAccounts.
- Attendance/shifts (partial): Shifts, ShiftAssignments, TypeOfDays, TypeOfWorkSchedules.
- Catalogs/docs: DocumentTypes, DocumentTemplates, IdentityDocumentTypes, Countries, Branches, Companies.
- Other: Holidays, PieceworkUnitTypes, DestajoDocuments, SuperAdminManagement.

## Gaps (not evident in codebase)
- Recruitment/Selection and candidate portal.
- Employee portal (receipts, certificates, requests, notifications).
- Clock-in/out (cloud/mobile) and attendance reconciliation; only employee `CodOfClock` found.
- SIPE integration (initial load and monthly file) and reconciliation.
- e-Tax planilla 03 integration.
- KPIs dashboards (rotation, absenteeism, extras, retention).
- Contract lifecycle, letters (work/income), digital dossier, expiring-docs control and alerts.
- Special processes: construction concepts, severance funds (PROFUTURO/PROGRESO), extraordinary/claims payrolls, employee receivables with amortization.
- Notification automation (contracts/vacations/licenses/birthdays).
- Income tax return module.
- App/mobile clock or hardware clock integration APIs.

## Proposed roadmap
- Sprint 0: Confirm current scope, data sources for attendance and construction concepts; prioritize by business.
- Sprint 1: Employee portal + notifications (receipts/certificates/vacation-medical status, alerts for contracts/docs/licences/birthdays); auth and roles for employees.
- Sprint 2: Attendance API + reconciliation (4 punches/shift, nocturnal 8th-hour rules), batch imports, mobile/clock endpoints.
- Sprint 3: Compliance (SIPE export + reconciliation, e-Tax 03 export, income tax return export).
- Sprint 4: Contracts/expediente/docs (versions, renewables, letters), alerts.
- Sprint 5: KPIs and dashboards; labor liability and provisions views.
- Transversal: construction concept parametrization; employee receivables/discounts with caps; ACH/cheque/cash layouts.

## Sprint 1 — Employee portal & notifications (user stories)
- Como empleado, quiero autenticarme en el portal con mis credenciales actuales para acceder a mi información de nómina.
- Como empleado, quiero ver y descargar mis recibos de pago desde el portal para consultar mis pagos históricos.
- Como empleado, quiero generar/descargar certificados de trabajo e ingresos parametrizados para trámites externos.
- Como empleado, quiero ver mi saldo de vacaciones y certificados médicos disponibles para planificar solicitudes.
- Como empleado, quiero recibir notificaciones sobre vencimiento de contratos, documentos renovables, licencias y cumpleaños.
- Como administrador, quiero configurar plantillas de notificaciones y destinatarios para automatizar avisos.
- Como administrador, quiero definir roles/permisos mínimos de empleado para limitar el alcance del portal.

## Sprint 1 — Tareas técnicas sugeridas
- Backend: endpoints seguros (JWT/cookies) para recibos, certificados, saldos de vacaciones/certificados médicos; emisión de certificados con plantillas.
- Backend: scheduler de notificaciones (contratos, documentos renovables, licencias, cumpleaños) con cola de envíos y logs.
- Backend: servicio de almacenamiento/serving de recibos (PDF/HTML) y de plantillas de certificados; control de acceso por empleado.
- Frontend portal empleado: vista responsive con listado de recibos, descarga, sección de certificados, saldos de vacaciones/médicos, centro de alertas.
- AuthZ: rol “Empleado” y políticas; si aplica, SSO con el esquema actual (Identity) y sesión cookie.
- Datos: revisar fuentes actuales de recibos (PayrollHeaders/PayrollTmpHeaders) y de saldos de vacaciones/médicos; definir DTOs.
- DevOps: feature toggle para portal; logging y métricas básicas de acceso/descarga.
