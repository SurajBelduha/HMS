import React from 'react';
import {
  LayoutDashboard,
  Users,
  Calendar,
  Stethoscope,
  BedDouble,
  Pill,
  FlaskConical,
  Receipt,
  Building,
  Settings,
  Circle,
  LogOut,
  Shield,
} from 'lucide-react';
import { useApp, type ScreenType } from '../../context/AppContext';

interface NavItem {
  id: ScreenType;
  label: string;
  icon: React.ReactNode;
  badge?: string;
  badgeColor?: string;
  pulse?: boolean;
}

export const Sidebar: React.FC = () => {
  const { currentScreen, setCurrentScreen, allowedScreens, currentUser, logoutUser, userRole } =
    useApp();

  const clinicalNav: NavItem[] = [
    {
      id: 'dashboard',
      label: 'Operations Dashboard',
      icon: <LayoutDashboard className="w-4 h-4" />,
    },
    {
      id: 'patients',
      label: 'Patients & EMR',
      icon: <Users className="w-4 h-4" />,
      badge: '1,248',
    },
    {
      id: 'appointments',
      label: 'Doctor Appointments',
      icon: <Calendar className="w-4 h-4" />,
      badge: '48 Today',
      badgeColor: 'bg-teal-50 text-teal-700',
    },
    {
      id: 'clinical',
      label: 'OPD Desk & e-Rx',
      icon: <Stethoscope className="w-4 h-4" />,
      pulse: true,
    },
    {
      id: 'ipd',
      label: 'IPD Wards & Beds',
      icon: <BedDouble className="w-4 h-4" />,
      badge: '80% Full',
      badgeColor: 'bg-amber-50 text-amber-700',
    },
  ];

  const servicesNav: NavItem[] = [
    {
      id: 'pharmacy',
      label: 'Pharmacy & Stock',
      icon: <Pill className="w-4 h-4" />,
      badge: '2 Low',
      badgeColor: 'bg-rose-50 text-rose-600',
    },
    {
      id: 'laboratory',
      label: 'Lab & Diagnostics',
      icon: <FlaskConical className="w-4 h-4" />,
      badge: '14 Pending',
    },
    {
      id: 'billing',
      label: 'Billing & Invoices',
      icon: <Receipt className="w-4 h-4" />,
    },
  ];

  const adminNav: NavItem[] = [
    {
      id: 'branches',
      label: 'Branches & Doctors',
      icon: <Building className="w-4 h-4" />,
    },
    {
      id: 'settings',
      label: 'Tenant Settings & RBAC',
      icon: <Settings className="w-4 h-4" />,
    },
  ];

  // RBAC: Filter items so only permitted screens are shown to this role
  const visibleClinical = clinicalNav.filter((item) => allowedScreens.includes(item.id));
  const visibleServices = servicesNav.filter((item) => allowedScreens.includes(item.id));
  const visibleAdmin = adminNav.filter((item) => allowedScreens.includes(item.id));

  const renderNavGroup = (items: NavItem[]) => {
    return items.map((item) => {
      const isActive = currentScreen === item.id;
      return (
        <button
          key={item.id}
          onClick={() => setCurrentScreen(item.id)}
          className={`w-full flex items-center gap-3 px-3 py-2 rounded-lg text-xs font-medium transition cursor-pointer ${
            isActive
              ? 'bg-teal-50 text-teal-900 border-r-3 border-teal-600 font-semibold'
              : 'text-slate-600 hover:bg-slate-50 hover:text-slate-900'
          }`}
        >
          <span className={isActive ? 'text-teal-600' : 'text-slate-400'}>{item.icon}</span>
          <span className="truncate">{item.label}</span>

          {item.pulse && (
            <span className="ml-auto w-2 h-2 rounded-full bg-emerald-500 animate-pulse" />
          )}

          {item.badge && (
            <span
              className={`ml-auto text-[11px] px-1.5 py-0.5 rounded font-mono font-medium ${
                item.badgeColor || 'bg-slate-100 text-slate-600'
              }`}
            >
              {item.badge}
            </span>
          )}
        </button>
      );
    });
  };

  return (
    <aside className="w-64 bg-white border-r border-slate-200 flex flex-col shrink-0 select-none">
      {/* Active Role Indicator */}
      <div className="p-3 border-b border-slate-100 bg-slate-50/50">
        <div className="flex items-center justify-between text-xs">
          <span className="text-slate-500 font-medium flex items-center gap-1.5">
            <Shield className="w-3.5 h-3.5 text-teal-600" />
            <span>Assigned Role</span>
          </span>
          <span className="font-bold text-teal-800 bg-teal-100 px-2 py-0.5 rounded-full text-[11px]">
            {userRole}
          </span>
        </div>
      </div>

      <div className="p-3 overflow-y-auto flex-1 space-y-4">
        {/* Clinical Operations */}
        {visibleClinical.length > 0 && (
          <div>
            <div className="px-3 py-1.5 text-[11px] font-bold text-slate-400 uppercase tracking-wider">
              Clinical & Operations
            </div>
            <nav className="space-y-0.5 mt-1">{renderNavGroup(visibleClinical)}</nav>
          </div>
        )}

        {/* Hospital Services */}
        {visibleServices.length > 0 && (
          <div>
            <div className="px-3 pt-2 pb-1.5 text-[11px] font-bold text-slate-400 uppercase tracking-wider">
              Hospital Services
            </div>
            <nav className="space-y-0.5 mt-1">{renderNavGroup(visibleServices)}</nav>
          </div>
        )}

        {/* Administration */}
        {visibleAdmin.length > 0 && (
          <div>
            <div className="px-3 pt-2 pb-1.5 text-[11px] font-bold text-slate-400 uppercase tracking-wider">
              Administration
            </div>
            <nav className="space-y-0.5 mt-1">{renderNavGroup(visibleAdmin)}</nav>
          </div>
        )}
      </div>

      {/* Footer Info & Logout */}
      <div className="p-3 border-t border-slate-200 bg-slate-50/60 space-y-2.5">
        <div className="flex items-center justify-between text-[11px]">
          <span className="text-slate-500 font-medium">Tenant Isolation</span>
          <span className="font-mono text-teal-700 bg-teal-100 px-1.5 py-0.2 rounded font-semibold">
            Shared DB
          </span>
        </div>

        {/* User logout action */}
        {currentUser && (
          <div className="pt-2 border-t border-slate-200 flex items-center justify-between">
            <div className="truncate pr-2">
              <div className="text-xs font-bold text-slate-800 truncate">{currentUser.fullName}</div>
              <div className="text-[11px] text-slate-400 truncate">{currentUser.email}</div>
            </div>
            <button
              onClick={logoutUser}
              title="Sign Out"
              className="p-1.5 text-slate-400 hover:text-rose-600 hover:bg-rose-50 rounded-lg transition cursor-pointer"
            >
              <LogOut className="w-4 h-4" />
            </button>
          </div>
        )}
      </div>
    </aside>
  );
};

