import React from 'react';
import { AppProvider, useApp } from './context/AppContext';
import { AppLayout } from './components/layout/AppLayout';
import { DashboardScreen } from './features/dashboard/DashboardScreen';
import { PatientListScreen } from './features/patients/PatientListScreen';
import { PatientRegisterModal } from './features/patients/PatientRegisterModal';
import { AppointmentScreen } from './features/appointments/AppointmentScreen';
import { BookAppointmentModal } from './features/appointments/BookAppointmentModal';
import { ClinicalDeskScreen } from './features/clinical/ClinicalDeskScreen';
import { BedMatrixScreen } from './features/ipd/BedMatrixScreen';
import { AdmitPatientModal } from './features/ipd/AdmitPatientModal';
import { PharmacyScreen } from './features/pharmacy/PharmacyScreen';
import { LaboratoryScreen } from './features/laboratory/LaboratoryScreen';
import { BillingScreen } from './features/billing/BillingScreen';
import { CreateInvoiceModal } from './features/billing/CreateInvoiceModal';
import { BranchesScreen } from './features/branches/BranchesScreen';
import { SettingsScreen } from './features/settings/SettingsScreen';

const MainScreenRouter: React.FC = () => {
  const { currentScreen } = useApp();

  switch (currentScreen) {
    case 'dashboard':
      return <DashboardScreen />;
    case 'patients':
      return <PatientListScreen />;
    case 'appointments':
      return <AppointmentScreen />;
    case 'clinical':
      return <ClinicalDeskScreen />;
    case 'ipd':
      return <BedMatrixScreen />;
    case 'pharmacy':
      return <PharmacyScreen />;
    case 'laboratory':
      return <LaboratoryScreen />;
    case 'billing':
      return <BillingScreen />;
    case 'branches':
      return <BranchesScreen />;
    case 'settings':
      return <SettingsScreen />;
    default:
      return <DashboardScreen />;
  }
};

export const AppContent: React.FC = () => {
  return (
    <AppLayout>
      <MainScreenRouter />

      {/* Global Application Modals */}
      <PatientRegisterModal />
      <BookAppointmentModal />
      <AdmitPatientModal />
      <CreateInvoiceModal />
    </AppLayout>
  );
};

export default function App() {
  return (
    <AppProvider>
      <AppContent />
    </AppProvider>
  );
}

