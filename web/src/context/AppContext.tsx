import React, { createContext, useContext, useState } from 'react';
import type { UserRole, Patient } from '../types';
import { api } from '../services/api';

export type ScreenType =
  | 'dashboard'
  | 'patients'
  | 'appointments'
  | 'clinical'
  | 'ipd'
  | 'pharmacy'
  | 'laboratory'
  | 'billing'
  | 'branches'
  | 'settings';

interface AppContextType {
  currentScreen: ScreenType;
  setCurrentScreen: (screen: ScreenType) => void;
  activeTenant: string;
  setActiveTenant: (tenant: string) => void;
  activeBranch: string;
  setActiveBranch: (branch: string) => void;
  userRole: UserRole;
  setUserRole: (role: UserRole) => void;
  selectedPatient: Patient | null;
  setSelectedPatient: (patient: Patient | null) => void;
  activeModal: string | null;
  openModal: (modalName: string) => void;
  closeModal: () => void;
  toast: string | null;
  showToast: (msg: string) => void;
}

const AppContext = createContext<AppContextType | undefined>(undefined);

export const AppProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [currentScreen, setCurrentScreen] = useState<ScreenType>('dashboard');
  const [activeTenant, setActiveTenantState] = useState<string>('ABC-HC');
  const [activeBranch, setActiveBranchState] = useState<string>('JPR-01');
  const [userRole, setUserRole] = useState<UserRole>('Doctor');
  const [selectedPatient, setSelectedPatient] = useState<Patient | null>(null);
  const [activeModal, setActiveModal] = useState<string | null>(null);
  const [toast, setToast] = useState<string | null>(null);

  const setActiveTenant = (tenant: string) => {
    setActiveTenantState(tenant);
    api.setTenant(tenant);
    showToast(`Switched active tenant to ${tenant}`);
  };

  const setActiveBranch = (branch: string) => {
    setActiveBranchState(branch);
    api.setBranch(branch);
    showToast(`Switched active branch to ${branch}`);
  };

  const showToast = (msg: string) => {
    setToast(msg);
    setTimeout(() => {
      setToast(null);
    }, 3500);
  };

  const openModal = (modalName: string) => setActiveModal(modalName);
  const closeModal = () => setActiveModal(null);

  return (
    <AppContext.Provider
      value={{
        currentScreen,
        setCurrentScreen,
        activeTenant,
        setActiveTenant,
        activeBranch,
        setActiveBranch,
        userRole,
        setUserRole,
        selectedPatient,
        setSelectedPatient,
        activeModal,
        openModal,
        closeModal,
        toast,
        showToast,
      }}
    >
      {children}
    </AppContext.Provider>
  );
};

export const useApp = () => {
  const context = useContext(AppContext);
  if (!context) {
    throw new Error('useApp must be used within an AppProvider');
  }
  return context;
};

