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
  const { currentScreen, setCurrentScreen } = useApp();

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
      <div className="p-3 overflow-y-auto flex-1">
        {/* Clinical Operations */}
        <div className="px-3 py-1.5 text-[11px] font-bold text-slate-400 uppercase tracking-wider">
          Clinical & Operations
        </div>
        <nav className="space-y-0.5 mt-1">{renderNavGroup(clinicalNav)}</nav>

        {/* Hospital Services */}
        <div className="px-3 pt-4 pb-1.5 text-[11px] font-bold text-slate-400 uppercase tracking-wider">
          Hospital Services
        </div>
        <nav className="space-y-0.5 mt-1">{renderNavGroup(servicesNav)}</nav>

        {/* Administration */}
        <div className="px-3 pt-4 pb-1.5 text-[11px] font-bold text-slate-400 uppercase tracking-wider">
          Administration
        </div>
        <nav className="space-y-0.5 mt-1">{renderNavGroup(adminNav)}</nav>
      </div>

      {/* Footer Info */}
      <div className="p-4 border-t border-slate-200 bg-slate-50/50">
        <div className="flex items-center justify-between text-[11px] mb-1.5">
          <span className="text-slate-500 font-medium">Tenant Isolation</span>
          <span className="font-mono text-teal-700 bg-teal-100 px-1.5 py-0.5 rounded font-semibold">
            Shared DB
          </span>
        </div>
        <div className="flex items-center justify-between text-[11px]">
          <span className="text-slate-500">API Health (.NET 8)</span>
          <span className="flex items-center gap-1 text-emerald-600 font-medium">
            <Circle className="w-2 h-2 fill-emerald-500 text-emerald-500" /> Online
          </span>
        </div>
      </div>
    </aside>
  );
};

