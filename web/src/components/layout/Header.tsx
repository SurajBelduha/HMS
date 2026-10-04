import React, { useState } from 'react';
import {
  Activity,
  Building2,
  MapPin,
  Search,
  Plus,
  Bell,
  ChevronDown,
} from 'lucide-react';
import { useApp } from '../../context/AppContext';
import { Button } from '../ui/Button';
import type { UserRole } from '../../types';

export const Header: React.FC = () => {
  const {
    activeTenant,
    setActiveTenant,
    activeBranch,
    setActiveBranch,
    userRole,
    setUserRole,
    openModal,
    showToast,
  } = useApp();

  const [showQuickMenu, setShowQuickMenu] = useState(false);

  return (
    <header className="bg-white border-b border-slate-200 sticky top-0 z-40 shadow-xs">
      <div className="px-5 py-3 flex items-center justify-between gap-4">
        {/* Brand & Organization */}
        <div className="flex items-center gap-4">
          <div className="flex items-center gap-2.5">
            <div className="w-10 h-10 rounded-xl bg-gradient-to-tr from-teal-700 to-teal-500 flex items-center justify-center text-white shadow-md shadow-teal-500/20">
              <Activity className="w-6 h-6 stroke-[2.5]" />
            </div>
            <div>
              <div className="flex items-center gap-1.5">
                <span className="font-bold text-slate-900 tracking-tight text-base">HealthCore</span>
                <span className="px-1.5 py-0.5 text-xs font-semibold bg-teal-100 text-teal-800 rounded">
                  HMS
                </span>
              </div>
              <p className="text-xs text-slate-500">Enterprise SaaS Platform</p>
            </div>
          </div>

          <div className="h-6 w-px bg-slate-200 hidden sm:block" />

          {/* Tenant Selector */}
          <div className="hidden md:flex items-center gap-2">
            <div className="flex items-center gap-1.5 bg-slate-100/80 px-2.5 py-1.5 rounded-lg border border-slate-200 text-xs">
              <Building2 className="w-3.5 h-3.5 text-slate-500" />
              <span className="text-slate-500">Tenant:</span>
              <select
                value={activeTenant}
                onChange={(e) => setActiveTenant(e.target.value)}
                className="bg-transparent font-medium text-slate-700 focus:outline-hidden cursor-pointer"
              >
                <option value="ABC-HC">ABC Healthcare Ltd</option>
                <option value="CITY-CARE">City Care Hospital</option>
                <option value="PLATFORM">Platform Host (SaaS)</option>
              </select>
            </div>

            {/* Branch Selector */}
            <div className="flex items-center gap-1.5 bg-slate-100/80 px-2.5 py-1.5 rounded-lg border border-slate-200 text-xs">
              <MapPin className="w-3.5 h-3.5 text-slate-500" />
              <span className="text-slate-500">Branch:</span>
              <select
                value={activeBranch}
                onChange={(e) => setActiveBranch(e.target.value)}
                className="bg-transparent font-medium text-slate-700 focus:outline-hidden cursor-pointer"
              >
                <option value="JPR-01">Jaipur Main (JPR-01)</option>
                <option value="DEL-01">Delhi CP (DEL-01)</option>
              </select>
            </div>
          </div>
        </div>

        {/* Global Search */}
        <div className="flex-1 max-w-md hidden lg:block">
          <div className="relative">
            <Search className="w-4 h-4 text-slate-400 absolute left-3 top-2.5" />
            <input
              type="text"
              placeholder="Search patient by MRN, doctor, bed, invoice #... (Press /)"
              className="w-full pl-9 pr-12 py-1.5 bg-slate-100/90 border border-slate-200 rounded-lg text-xs placeholder-slate-400 focus:outline-hidden focus:bg-white focus:border-teal-500 focus:ring-2 focus:ring-teal-100 transition-all"
            />
            <kbd className="absolute right-2.5 top-2 text-[10px] uppercase font-mono px-1.5 py-0.5 bg-slate-200/80 text-slate-500 rounded">
              ⌘K
            </kbd>
          </div>
        </div>

        {/* Actions & Role Selector */}
        <div className="flex items-center gap-3">
          {/* Quick Action Button */}
          <div className="relative">
            <Button
              variant="primary"
              size="md"
              icon={<Plus className="w-3.5 h-3.5 stroke-[2.5]" />}
              onClick={() => setShowQuickMenu(!showQuickMenu)}
            >
              <span>+ Quick Action</span>
              <ChevronDown className="w-3 h-3 ml-0.5" />
            </Button>

            {showQuickMenu && (
              <div className="absolute right-0 mt-1.5 w-52 bg-white border border-slate-200 rounded-xl shadow-xl py-1 text-xs z-50 animate-in fade-in zoom-in-95 duration-150">
                <button
                  onClick={() => {
                    openModal('registerPatient');
                    setShowQuickMenu(false);
                  }}
                  className="w-full text-left px-3.5 py-2.5 hover:bg-slate-50 flex items-center gap-2 text-slate-700 font-medium cursor-pointer"
                >
                  <span className="w-2 h-2 rounded-full bg-teal-500" />
                  <span>Register New Patient</span>
                </button>
                <button
                  onClick={() => {
                    openModal('bookAppointment');
                    setShowQuickMenu(false);
                  }}
                  className="w-full text-left px-3.5 py-2.5 hover:bg-slate-50 flex items-center gap-2 text-slate-700 font-medium cursor-pointer"
                >
                  <span className="w-2 h-2 rounded-full bg-blue-500" />
                  <span>Book Appointment</span>
                </button>
                <button
                  onClick={() => {
                    openModal('admitPatient');
                    setShowQuickMenu(false);
                  }}
                  className="w-full text-left px-3.5 py-2.5 hover:bg-slate-50 flex items-center gap-2 text-slate-700 font-medium cursor-pointer"
                >
                  <span className="w-2 h-2 rounded-full bg-indigo-500" />
                  <span>Admit to IPD Bed</span>
                </button>
                <button
                  onClick={() => {
                    openModal('createInvoice');
                    setShowQuickMenu(false);
                  }}
                  className="w-full text-left px-3.5 py-2.5 hover:bg-slate-50 flex items-center gap-2 text-slate-700 font-medium cursor-pointer"
                >
                  <span className="w-2 h-2 rounded-full bg-emerald-500" />
                  <span>Generate Invoice</span>
                </button>
              </div>
            )}
          </div>

          {/* Role Simulator */}
          <div className="hidden xl:flex items-center gap-1.5 bg-slate-100 px-2.5 py-1.5 rounded-lg border border-slate-200 text-xs">
            <span className="text-slate-400">Role:</span>
            <select
              value={userRole}
              onChange={(e) => {
                setUserRole(e.target.value as UserRole);
                showToast(`Simulating user role: ${e.target.value}`);
              }}
              className="bg-transparent font-medium text-slate-700 focus:outline-hidden cursor-pointer"
            >
              <option value="HospitalAdmin">Hospital Admin</option>
              <option value="Doctor">Doctor (Dr. Alice)</option>
              <option value="Nurse">Staff Nurse</option>
              <option value="Receptionist">Receptionist</option>
              <option value="Pharmacist">Pharmacist</option>
              <option value="LabTechnician">Lab Technician</option>
              <option value="Accountant">Accountant</option>
              <option value="PlatformSuperAdmin">Platform SuperAdmin</option>
            </select>
          </div>

          {/* Notifications */}
          <button
            onClick={() => showToast('No unread emergency alerts')}
            className="p-2 text-slate-500 hover:text-slate-700 rounded-lg hover:bg-slate-100 transition relative cursor-pointer"
          >
            <Bell className="w-4 h-4" />
            <span className="w-2 h-2 bg-rose-500 rounded-full absolute top-1.5 right-1.5" />
          </button>

          {/* User Profile Pill */}
          <div className="flex items-center gap-2.5 pl-2 border-l border-slate-200">
            <div className="w-8 h-8 rounded-full bg-teal-100 border border-teal-200 text-teal-800 flex items-center justify-center font-bold text-xs">
              AS
            </div>
            <div className="hidden 2xl:block text-left">
              <div className="text-xs font-semibold text-slate-800 leading-tight">
                Dr. Alice Smith
              </div>
              <div className="text-[11px] text-slate-400">Cardiology Lead</div>
            </div>
          </div>
        </div>
      </div>
    </header>
  );
};

