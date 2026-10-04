import React, { useState } from 'react';
import { BedDouble, Plus, User, AlertTriangle } from 'lucide-react';
import { Card } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { useApp } from '../../context/AppContext';
import type { Bed } from '../../types';

export const BedMatrixScreen: React.FC = () => {
  const { openModal, showToast } = useApp();
  const [selectedWard, setSelectedWard] = useState('General');

  const beds: Bed[] = [
    {
      id: '1',
      roomId: '101',
      roomNumber: '101',
      wardName: 'General Ward 1',
      floor: 'Floor 1',
      bedNumber: 'B-101',
      dailyCharge: 1000,
      status: 'Available',
    },
    {
      id: '2',
      roomId: '101',
      roomNumber: '101',
      wardName: 'General Ward 1',
      floor: 'Floor 1',
      bedNumber: 'B-102',
      dailyCharge: 1000,
      status: 'Occupied',
      patientName: 'Robert Brown',
      patientMrn: 'PAT-2026-00003',
      attendingDoctor: 'Dr. Robert Brown',
      admittedDate: '02 Oct 2026',
    },
    {
      id: '3',
      roomId: '102',
      roomNumber: '102',
      wardName: 'General Ward 1',
      floor: 'Floor 1',
      bedNumber: 'B-103',
      dailyCharge: 1000,
      status: 'Available',
    },
    {
      id: '4',
      roomId: '102',
      roomNumber: '102',
      wardName: 'General Ward 1',
      floor: 'Floor 1',
      bedNumber: 'B-104',
      dailyCharge: 1000,
      status: 'Maintenance',
    },
  ];

  return (
    <div className="space-y-6">
      {/* Title */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-xl font-bold text-slate-900 tracking-tight">
            Inpatient Wards & Visual Bed Matrix
          </h1>
          <p className="text-xs text-slate-500">
            Live floor-wise bed allocation, patient admissions, and transfer matrix.
          </p>
        </div>
        <Button
          variant="primary"
          icon={<Plus className="w-4 h-4" />}
          onClick={() => openModal('admitPatient')}
        >
          + New Patient Admission
        </Button>
      </div>

      {/* Ward Switcher Tabs & Legend */}
      <div className="bg-white p-3 rounded-xl border border-slate-200 shadow-xs flex flex-wrap items-center justify-between gap-4">
        <div className="flex items-center gap-2">
          <button
            onClick={() => setSelectedWard('General')}
            className={`px-3 py-1.5 rounded-lg text-xs font-semibold cursor-pointer transition ${
              selectedWard === 'General'
                ? 'bg-teal-600 text-white'
                : 'text-slate-600 hover:bg-slate-100'
            }`}
          >
            General Ward 1 (Floor 1)
          </button>
          <button
            onClick={() => setSelectedWard('ICU')}
            className={`px-3 py-1.5 rounded-lg text-xs font-semibold cursor-pointer transition ${
              selectedWard === 'ICU'
                ? 'bg-teal-600 text-white'
                : 'text-slate-600 hover:bg-slate-100'
            }`}
          >
            ICU Ward 1 (Floor 2)
          </button>
          <button
            onClick={() => setSelectedWard('Private')}
            className={`px-3 py-1.5 rounded-lg text-xs font-semibold cursor-pointer transition ${
              selectedWard === 'Private'
                ? 'bg-teal-600 text-white'
                : 'text-slate-600 hover:bg-slate-100'
            }`}
          >
            Private Deluxe Suites (Floor 3)
          </button>
        </div>

        {/* Status Legend */}
        <div className="flex items-center gap-4 text-xs">
          <span className="flex items-center gap-1.5">
            <span className="w-2.5 h-2.5 rounded-full bg-emerald-500" /> Available (Green)
          </span>
          <span className="flex items-center gap-1.5">
            <span className="w-2.5 h-2.5 rounded-full bg-blue-500" /> Occupied (Blue)
          </span>
          <span className="flex items-center gap-1.5">
            <span className="w-2.5 h-2.5 rounded-full bg-amber-500" /> Maintenance (Amber)
          </span>
        </div>
      </div>

      {/* Bed Grid Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        {beds.map((bed) => {
          if (bed.status === 'Available') {
            return (
              <div
                key={bed.id}
                className="bg-white rounded-xl border-2 border-emerald-500/40 p-4 shadow-xs hover:border-emerald-500 transition relative flex flex-col justify-between"
              >
                <div>
                  <div className="flex items-center justify-between mb-2">
                    <span className="font-mono font-bold text-sm text-slate-900">
                      Bed {bed.bedNumber}
                    </span>
                    <Badge variant="success">Available</Badge>
                  </div>
                  <p className="text-xs text-slate-500">
                    Room {bed.roomNumber} • {bed.wardName}
                  </p>
                  <div className="mt-3 flex items-baseline justify-between text-xs">
                    <span className="text-slate-400">Daily Charge:</span>
                    <span className="font-bold text-slate-800">${bed.dailyCharge} / day</span>
                  </div>
                </div>
                <Button
                  variant="outline"
                  size="sm"
                  className="mt-4 w-full bg-emerald-50/50 hover:bg-emerald-100 text-emerald-800 border-emerald-200"
                  onClick={() => openModal('admitPatient')}
                >
                  + Assign Patient
                </Button>
              </div>
            );
          }

          if (bed.status === 'Occupied') {
            return (
              <div
                key={bed.id}
                className="bg-white rounded-xl border-2 border-blue-500/50 p-4 shadow-xs hover:border-blue-600 transition flex flex-col justify-between"
              >
                <div>
                  <div className="flex items-center justify-between mb-2">
                    <span className="font-mono font-bold text-sm text-slate-900">
                      Bed {bed.bedNumber}
                    </span>
                    <Badge variant="info">Occupied</Badge>
                  </div>
                  <div className="text-xs">
                    <div className="font-semibold text-slate-900 flex items-center gap-1.5">
                      <User className="w-3.5 h-3.5 text-blue-600" />
                      <span>{bed.patientName}</span>
                    </div>
                    <div className="text-[11px] text-slate-500 font-mono mt-0.5">
                      {bed.patientMrn}
                    </div>
                    <div className="text-[11px] text-slate-500 mt-1">
                      {bed.attendingDoctor} • Admitted {bed.admittedDate}
                    </div>
                  </div>
                </div>

                <div className="mt-4 pt-2.5 border-t border-slate-100 flex items-center justify-between">
                  <button
                    onClick={() => showToast(`Opened clinical records for ${bed.patientName}`)}
                    className="text-xs font-semibold text-blue-600 hover:text-blue-800 cursor-pointer"
                  >
                    Manage &rarr;
                  </button>
                  <button
                    onClick={() =>
                      showToast(`Discharge requested for ${bed.patientName}. Transferred to Billing.`)
                    }
                    className="text-[11px] text-rose-600 hover:text-rose-800 font-semibold cursor-pointer"
                  >
                    Discharge
                  </button>
                </div>
              </div>
            );
          }

          return (
            <div
              key={bed.id}
              className="bg-white rounded-xl border-2 border-amber-500/40 p-4 shadow-xs flex flex-col justify-between"
            >
              <div>
                <div className="flex items-center justify-between mb-2">
                  <span className="font-mono font-bold text-sm text-slate-900">
                    Bed {bed.bedNumber}
                  </span>
                  <Badge variant="warning">Maintenance</Badge>
                </div>
                <p className="text-xs text-slate-500">
                  Room {bed.roomNumber} • UV Sterilization & Deep Cleaning
                </p>
                <div className="mt-3 text-xs text-amber-800 bg-amber-50 p-2.5 rounded-lg flex items-center gap-2 border border-amber-200">
                  <AlertTriangle className="w-4 h-4 shrink-0 text-amber-600" />
                  <span>Sanitation complete in ~35 mins</span>
                </div>
              </div>
              <Button
                variant="outline"
                size="sm"
                className="mt-4 w-full"
                onClick={() => showToast(`Bed ${bed.bedNumber} marked available`)}
              >
                Mark Ready
              </Button>
            </div>
          );
        })}
      </div>
    </div>
  );
};

