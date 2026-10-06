import React, { createContext, useContext, useState, useEffect } from 'react';
import type { UserRole, Patient, AuthUser } from '../types';
import { api } from '../services/api';
import { authService } from '../services/authService';
import {
  isScreenAllowedForRole,
  DEFAULT_SCREEN_FOR_ROLE,
  ROLE_ALLOWED_SCREENS,
} from '../config/rbac';

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
  currentUser: AuthUser | null;
  isAuthenticated: boolean;
  loginUser: (user: AuthUser) => void;
  logoutUser: () => void;
  currentScreen: ScreenType;
  setCurrentScreen: (screen: ScreenType) => void;
  allowedScreens: ScreenType[];
  activeTenant: string;
  setActiveTenant: (tenant: string) => void;
  activeBranch: string;
  setActiveBranch: (branch: string) => void;
  userRole: UserRole;
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
  const [currentUser, setCurrentUser] = useState<AuthUser | null>(() =>
    authService.getCurrentUser()
  );

  const userRole: UserRole = currentUser ? currentUser.role : 'Doctor';
  const allowedScreens: ScreenType[] = ROLE_ALLOWED_SCREENS[userRole] || ['dashboard'];

  const [currentScreen, setCurrentScreenState] = useState<ScreenType>(() => {
    if (currentUser) {
      return DEFAULT_SCREEN_FOR_ROLE[currentUser.role] || 'dashboard';
    }
    return 'dashboard';
  });

  const [activeTenant, setActiveTenantState] = useState<string>(
    currentUser ? currentUser.tenantId : '44444444-4444-4444-4444-444444444444'
  );
  const [activeBranch, setActiveBranchState] = useState<string>(
    currentUser?.branchId || 'JPR-01'
  );
  const [selectedPatient, setSelectedPatient] = useState<Patient | null>(null);
  const [activeModal, setActiveModal] = useState<string | null>(null);
  const [toast, setToast] = useState<string | null>(null);

  // Sync API headers on mount
  useEffect(() => {
    if (currentUser) {
      api.setToken(currentUser.token);
      api.setTenant(currentUser.tenantId);
      api.setBranch(currentUser.branchId);
    }
  }, [currentUser]);

  const loginUser = (user: AuthUser) => {
    setCurrentUser(user);
    authService.saveSession(user);
    setActiveTenantState(user.tenantId);
    setActiveBranchState(user.branchId);

    // Navigate to default screen allowed for this role
    const defaultScreen = DEFAULT_SCREEN_FOR_ROLE[user.role] || 'dashboard';
    setCurrentScreenState(defaultScreen);
    showToast(`Welcome back, ${user.fullName}! Logged in as ${user.role}`);
  };

  const logoutUser = () => {
    authService.logout();
    setCurrentUser(null);
    setCurrentScreenState('dashboard');
    showToast('Logged out successfully.');
  };

  // Safe navigation with Role-Based Guard (RBAC)
  const setCurrentScreen = (screen: ScreenType) => {
    if (!currentUser) {
      setCurrentScreenState(screen);
      return;
    }

    if (!isScreenAllowedForRole(currentUser.role, screen)) {
      showToast(
        `⛔ Access Denied: Your role (${currentUser.role}) does not have permission to access "${screen}".`
      );
      return;
    }

    setCurrentScreenState(screen);
  };

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
    }, 4000);
  };

  const openModal = (modalName: string) => setActiveModal(modalName);
  const closeModal = () => setActiveModal(null);

  return (
    <AppContext.Provider
      value={{
        currentUser,
        isAuthenticated: !!currentUser,
        loginUser,
        logoutUser,
        currentScreen,
        setCurrentScreen,
        allowedScreens,
        activeTenant,
        setActiveTenant,
        activeBranch,
        setActiveBranch,
        userRole,
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

