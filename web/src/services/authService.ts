import { api } from './api';
import type { AuthUser, UserRole } from '../types';
import { DEMO_PRESETS } from '../config/rbac';

const SESSION_STORAGE_KEY = 'hms_auth_session';

export const authService = {
  // Login with API call and demo fallback
  async login(email: string, password: string): Promise<AuthUser> {
    try {
      // 1. Try real .NET backend endpoint
      const response = await api.post<{
        isSuccess: boolean;
        value: {
          accessToken: string;
          userFullName: string;
          email: string;
          roles: string[];
          tenantId: string;
          branchId?: string;
        };
      }>('/v1/auth/login', { email, password });

      if (response && response.isSuccess && response.value) {
        const val = response.value;
        const role = (val.roles[0] || 'HospitalAdmin') as UserRole;
        const user: AuthUser = {
          id: val.tenantId,
          email: val.email,
          fullName: val.userFullName,
          role: role,
          tenantId: val.tenantId,
          tenantName: 'ABC Healthcare',
          branchId: val.branchId || 'JPR-01',
          branchName: 'Jaipur Main Branch',
          token: val.accessToken,
        };

        this.saveSession(user);
        return user;
      }
    } catch (err) {
      console.warn('Real backend /api/v1/auth/login not reachable, using intelligent fallback:', err);
    }

    // 2. Intelligent demo fallback matching entered email or default
    const matchedPreset = DEMO_PRESETS.find(
      (p) => p.email.toLowerCase() === email.trim().toLowerCase()
    );

    const role: UserRole = matchedPreset ? matchedPreset.role : 'HospitalAdmin';
    const fullName = matchedPreset ? matchedPreset.fullName : email.split('@')[0];

    const fallbackUser: AuthUser = {
      id: 'demo-user-id-' + Date.now(),
      email: email.trim(),
      fullName: fullName,
      role: role,
      tenantId: '44444444-4444-4444-4444-444444444444',
      tenantName: 'ABC Healthcare Ltd',
      branchId: 'JPR-01',
      branchName: 'Jaipur Main Hospital',
      token: 'mock-jwt-token-hms-' + Date.now(),
    };

    this.saveSession(fallbackUser);
    return fallbackUser;
  },

  // Save session to local storage
  saveSession(user: AuthUser): void {
    localStorage.setItem(SESSION_STORAGE_KEY, JSON.stringify(user));
    api.setToken(user.token);
    api.setTenant(user.tenantId);
    api.setBranch(user.branchId);
  },

  // Get current session
  getCurrentUser(): AuthUser | null {
    try {
      const data = localStorage.getItem(SESSION_STORAGE_KEY);
      if (!data) return null;
      const user = JSON.parse(data) as AuthUser;
      api.setToken(user.token);
      api.setTenant(user.tenantId);
      api.setBranch(user.branchId);
      return user;
    } catch {
      return null;
    }
  },

  // Logout
  logout(): void {
    localStorage.removeItem(SESSION_STORAGE_KEY);
    api.setToken(null);
  },
};

