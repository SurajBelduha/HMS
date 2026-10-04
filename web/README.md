# HMS HealthCore — Web Frontend (React + TypeScript + Vite + Tailwind CSS)

Welcome to the modern, component-based frontend for the **Hospital Management System (HMS)**.

---

## 🚀 Quick Start

```bash
# 1. Navigate to the web folder
cd web

# 2. Run the development server
npm run dev

# 3. Build for production
npm run build
```

---

## 🏛️ Project Architecture

```
web/src/
  ├── components/
  │   ├── layout/            # Layout shell: Header, Sidebar, AppLayout
  │   └── ui/                # Base design system: Button, Card, Badge, Modal, Input
  ├── context/
  │   └── AppContext.tsx     # Global tenant, branch, role, and navigation state
  ├── features/              # Modular clinical & operational domain features
  │   ├── dashboard/         # Operations analytics, live queue, bed census
  │   ├── patients/          # Directory, EMR 360, registration modal
  │   ├── appointments/      # Doctor schedule, 15-min token slots
  │   ├── clinical/          # Doctor's OPD desk, vitals strip, e-Prescription pad
  │   ├── ipd/               # Visual bed matrix (Available, Occupied, Maintenance)
  │   ├── pharmacy/          # Drug formulary, stock tracker, dispensing
  │   ├── laboratory/        # LIS diagnostic orders and results sign-off
  │   ├── billing/           # Invoices ledger, checkout POS, payment capture
  │   ├── branches/          # Multi-branch master & clinical departments
  │   └── settings/          # Tenant master config & RBAC role permissions
  ├── services/
  │   └── api.ts             # API client with automatic multi-tenant & branch headers
  ├── types/
  │   └── index.ts           # TypeScript interfaces matching .NET 8 backend models
  ├── App.tsx                # App routing shell & global modals
  ├── main.tsx               # React 19 application entry point
  └── index.css              # Tailwind CSS styles & design tokens
```

---

## 🔌 .NET 8 API Integration & Multi-Tenancy

Every API request dispatched through `src/services/api.ts` automatically attaches:
- `X-Tenant-Id`: Active hospital tenant ID
- `X-Branch-Id`: Active branch code (`JPR-01`, `DEL-01`)
- `Authorization`: Bearer JWT token

