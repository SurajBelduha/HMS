import React, { useState } from 'react';
import { Settings, ShieldCheck, KeyRound, Save } from 'lucide-react';
import { Card, CardContent } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { useApp } from '../../context/AppContext';

export const SettingsScreen: React.FC = () => {
  const { showToast } = useApp();

  const [invPrefix, setInvPrefix] = useState('ABC-INV-');
  const [patPrefix, setPatPrefix] = useState('ABC-PAT-');

  const handleSave = () => {
    showToast('Tenant configuration saved successfully in database!');
  };

  return (
    <div className="space-y-6">
      {/* Title */}
      <div>
        <h1 className="text-xl font-bold text-slate-900 tracking-tight">
          Tenant Settings & Role-Based Access Control (RBAC)
        </h1>
        <p className="text-xs text-slate-500">
          Organization profile, prefix configurations, multi-tenancy parameters, and system roles.
        </p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Tenant Configuration */}
        <Card>
          <CardContent className="p-5 space-y-4">
            <h3 className="text-sm font-bold text-slate-900 border-b border-slate-100 pb-2">
              Tenant Master Configuration
            </h3>

            <div className="grid grid-cols-2 gap-3 text-xs">
              <div>
                <label className="text-slate-500 block mb-1 font-semibold">Tenant Code</label>
                <input
                  type="text"
                  value="ABC-HC"
                  disabled
                  className="w-full p-2 bg-slate-100 border border-slate-200 rounded-lg font-mono font-bold text-slate-700"
                />
              </div>
              <div>
                <label className="text-slate-500 block mb-1 font-semibold">Isolation Strategy</label>
                <input
                  type="text"
                  value="SharedDatabase"
                  disabled
                  className="w-full p-2 bg-slate-100 border border-slate-200 rounded-lg text-slate-700 font-semibold"
                />
              </div>
              <div>
                <Input
                  label="Invoice Number Prefix"
                  value={invPrefix}
                  onChange={(e) => setInvPrefix(e.target.value)}
                />
              </div>
              <div>
                <Input
                  label="Patient MRN Prefix"
                  value={patPrefix}
                  onChange={(e) => setPatPrefix(e.target.value)}
                />
              </div>
            </div>

            <Button
              variant="primary"
              size="md"
              icon={<Save className="w-3.5 h-3.5" />}
              onClick={handleSave}
            >
              Save Tenant Preferences
            </Button>
          </CardContent>
        </Card>

        {/* System Roles & Permissions */}
        <Card>
          <CardContent className="p-5 space-y-4">
            <h3 className="text-sm font-bold text-slate-900 border-b border-slate-100 pb-2">
              System Roles & Permission Matrix
            </h3>

            <div className="space-y-2 text-xs">
              <div className="flex items-center justify-between p-2.5 rounded-lg bg-slate-50 border border-slate-100">
                <span className="font-bold text-slate-800">HospitalAdmin</span>
                <span className="text-teal-700 font-semibold text-[11px]">
                  All Privileges (Tenant-scoped)
                </span>
              </div>
              <div className="flex items-center justify-between p-2.5 rounded-lg bg-slate-50 border border-slate-100">
                <span className="font-bold text-slate-800">Doctor</span>
                <span className="text-slate-600 text-[11px]">
                  Consultations, e-Rx, IPD Vitals, Lab Review
                </span>
              </div>
              <div className="flex items-center justify-between p-2.5 rounded-lg bg-slate-50 border border-slate-100">
                <span className="font-bold text-slate-800">Nurse</span>
                <span className="text-slate-600 text-[11px]">
                  Bed Management, Vitals Recording, Ward Transfers
                </span>
              </div>
              <div className="flex items-center justify-between p-2.5 rounded-lg bg-slate-50 border border-slate-100">
                <span className="font-bold text-slate-800">Pharmacist</span>
                <span className="text-slate-600 text-[11px]">
                  Drug Formulary, Stock In/Out, Dispense e-Rx
                </span>
              </div>
              <div className="flex items-center justify-between p-2.5 rounded-lg bg-slate-50 border border-slate-100">
                <span className="font-bold text-slate-800">Accountant</span>
                <span className="text-slate-600 text-[11px]">
                  Invoice Creation, Payments POS, Billing Reports
                </span>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
};

