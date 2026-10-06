import type { UserRole } from '../types';
import type { ScreenType } from '../context/AppContext';

// Mapping each role to its accessible screens
export const ROLE_ALLOWED_SCREENS: Record<UserRole, ScreenType[]> = {
  PlatformSuperAdmin: [
    'dashboard',
    'patients',
    'appointments',
    'clinical',
    'ipd',
    'pharmacy',
    'laboratory',
    'billing',
    'branches',
    'settings',
  ],
  HospitalAdmin: [
    'dashboard',
    'patients',
    'appointments',
    'clinical',
    'ipd',
    'pharmacy',
    'laboratory',
    'billing',
    'branches',
    'settings',
  ],
  BranchAdmin: [
    'dashboard',
    'patients',
    'appointments',
    'clinical',
    'ipd',
    'pharmacy',
    'laboratory',
    'billing',
    'branches',
  ],
  Doctor: [
    'dashboard',
    'patients',
    'appointments',
    'clinical',
    'ipd',
    'laboratory',
  ],
  Nurse: [
    'dashboard',
    'patients',
    'appointments',
    'ipd',
  ],
  Receptionist: [
    'dashboard',
    'patients',
    'appointments',
    'billing',
  ],
  Pharmacist: [
    'dashboard',
    'pharmacy',
  ],
  LabTechnician: [
    'dashboard',
    'laboratory',
  ],
  Accountant: [
    'dashboard',
    'billing',
  ],
};

// Default landing screen for each role upon login
export const DEFAULT_SCREEN_FOR_ROLE: Record<UserRole, ScreenType> = {
  PlatformSuperAdmin: 'dashboard',
  HospitalAdmin: 'dashboard',
  BranchAdmin: 'dashboard',
  Doctor: 'clinical',
  Nurse: 'ipd',
  Receptionist: 'appointments',
  Pharmacist: 'pharmacy',
  LabTechnician: 'laboratory',
  Accountant: 'billing',
};

// Helper function to check if screen is allowed
export const isScreenAllowedForRole = (role: UserRole, screen: ScreenType): boolean => {
  const allowed = ROLE_ALLOWED_SCREENS[role];
  if (!allowed) return false;
  return allowed.includes(screen);
};

// Demo quick-login accounts for instant testing
export interface DemoUserPreset {
  role: UserRole;
  label: string;
  email: string;
  fullName: string;
  avatarText: string;
  icon: string;
  description: string;
}

export const DEMO_PRESETS: DemoUserPreset[] = [
  {
    role: 'Doctor',
    label: 'Doctor (Dr. Alice)',
    email: 'dr.alice@abchealthcare.com',
    fullName: 'Dr. Alice Smith',
    avatarText: 'AS',
    icon: '🩺',
    description: 'OPD Desk, e-Rx, Patient EMR, Vitals & Diagnostics',
  },
  {
    role: 'HospitalAdmin',
    label: 'Hospital Admin',
    email: 'admin@abchealthcare.com',
    fullName: 'Robert Sterling',
    avatarText: 'RS',
    icon: '🏥',
    description: 'All 10 modules: Clinical, IPD, Billing, Branches & Settings',
  },
  {
    role: 'Nurse',
    label: 'Staff Nurse',
    email: 'nurse.sarah@abchealthcare.com',
    fullName: 'Sarah Jenkins, RN',
    avatarText: 'SJ',
    icon: '👩‍⚕️',
    description: 'IPD Bed Matrix, Ward admissions, Patient vitals & queue',
  },
  {
    role: 'Pharmacist',
    label: 'Pharmacist',
    email: 'pharma.rahul@abchealthcare.com',
    fullName: 'Rahul Verma, R.Ph',
    avatarText: 'RV',
    icon: '💊',
    description: 'Pharmacy Formulary, Stock Alerts & Medication Dispensing',
  },
  {
    role: 'LabTechnician',
    label: 'Lab Technician',
    email: 'lab.tech@abchealthcare.com',
    fullName: 'Elena Rostova',
    avatarText: 'ER',
    icon: '🔬',
    description: 'Diagnostic Orders, Specimen collection & Result Entry',
  },
  {
    role: 'Accountant',
    label: 'Accountant',
    email: 'accounts@abchealthcare.com',
    fullName: 'Vikram Mehta',
    avatarText: 'VM',
    icon: '💳',
    description: 'Invoices, POS Cashier, Payment Capture & Receipts',
  },
  {
    role: 'Receptionist',
    label: 'Reception Desk',
    email: 'frontdesk@abchealthcare.com',
    fullName: 'Pooja Sharma',
    avatarText: 'PS',
    icon: '📋',
    description: 'Patient Registration, Token Queue & OPD Booking',
  },
  {
    role: 'PlatformSuperAdmin',
    label: 'Platform SuperAdmin',
    email: 'superadmin@hmssaas.com',
    fullName: 'System SuperAdmin',
    avatarText: 'SA',
    icon: '👑',
    description: 'Multi-Tenant Host, Global Subscriptions & Audit Logs',
  },
];

