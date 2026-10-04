import React, { useState } from 'react';
import { Plus, Trash2, CheckCircle2, Bed, AlertCircle, FileSpreadsheet } from 'lucide-react';
import { Card, CardContent } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { useApp } from '../../context/AppContext';

interface RxItem {
  id: string;
  name: string;
  dosage: string;
  frequency: string;
  duration: string;
  instructions: string;
}

export const ClinicalDeskScreen: React.FC = () => {
  const { openModal, showToast, setCurrentScreen } = useApp();

  const [diagnoses, setDiagnoses] = useState([
    'I10 — Essential (Primary) Hypertension',
    'R07.9 — Chest Pain, Unspecified',
  ]);

  const [prescriptionItems, setPrescriptionItems] = useState<RxItem[]>([
    {
      id: '1',
      name: 'Paracetamol 500mg (MED-001)',
      dosage: '1 tablet',
      frequency: '1-0-1 (Twice Daily)',
      duration: '5 Days',
      instructions: 'After food',
    },
    {
      id: '2',
      name: 'Amlodipine 5mg (MED-003)',
      dosage: '1 tablet',
      frequency: '0-0-1 (Night)',
      duration: '30 Days',
      instructions: 'Regular BP monitoring',
    },
  ]);

  const addMedicine = () => {
    const newItem: RxItem = {
      id: Date.now().toString(),
      name: 'Atorvastatin 10mg (MED-004)',
      dosage: '1 tablet',
      frequency: '0-0-1 (Night)',
      duration: '30 Days',
      instructions: 'After dinner',
    };
    setPrescriptionItems([...prescriptionItems, newItem]);
    showToast('Added Atorvastatin 10mg to prescription');
  };

  const removeMedicine = (id: string) => {
    setPrescriptionItems(prescriptionItems.filter((i) => i.id !== id));
    showToast('Removed item from prescription');
  };

  const handleFinalize = () => {
    showToast('Consultation finalized! e-Prescription generated & dispatched to Pharmacy.');
  };

  return (
    <div className="space-y-6">
      {/* Patient Banner */}
      <div className="bg-gradient-to-r from-teal-800 to-slate-800 text-white p-4 rounded-xl shadow-md flex flex-wrap items-center justify-between gap-4">
        <div className="flex items-center gap-4">
          <div className="w-12 h-12 rounded-xl bg-white/10 border border-white/20 flex items-center justify-center font-bold text-lg">
            JD
          </div>
          <div>
            <div className="flex items-center gap-2">
              <h2 className="text-base font-bold">John Doe</h2>
              <span className="font-mono text-xs px-2 py-0.5 bg-teal-500/30 border border-teal-400/30 rounded">
                PAT-2026-00001
              </span>
              <span className="text-xs bg-rose-500/40 text-rose-200 px-2 py-0.5 rounded font-bold flex items-center gap-1">
                <AlertCircle className="w-3 h-3" /> Allergy: Penicillin
              </span>
            </div>
            <p className="text-xs text-teal-100/80 mt-0.5">
              36 Yrs, Male • Blood Group: O+ • Attending: Dr. Alice Smith (Cardiology)
            </p>
          </div>
        </div>

        {/* Live Vitals Context */}
        <div className="flex items-center gap-3 bg-white/10 px-3 py-1.5 rounded-lg border border-white/10 text-xs">
          <div>
            BP: <strong className="text-teal-300">120/80 mmHg</strong>
          </div>
          <div className="w-px h-3 bg-white/20" />
          <div>
            Pulse: <strong className="text-teal-300">72 bpm</strong>
          </div>
          <div className="w-px h-3 bg-white/20" />
          <div>
            Temp: <strong className="text-teal-300">98.6 °F</strong>
          </div>
          <div className="w-px h-3 bg-white/20" />
          <div>
            SpO2: <strong className="text-teal-300">99%</strong>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Left: Notes & Prescription (2 Cols) */}
        <div className="lg:col-span-2 space-y-5">
          {/* Complaints & Diagnosis */}
          <Card>
            <CardContent className="p-4 space-y-3">
              <h3 className="text-xs font-bold text-slate-800 uppercase tracking-wider">
                Chief Complaints & Clinical Observations
              </h3>
              <textarea
                rows={3}
                defaultValue="Patient reports intermittent retrosternal chest discomfort radiating to left shoulder on exertion since 3 days. No associated syncope or dyspnea."
                className="w-full p-2.5 bg-slate-50 border border-slate-200 rounded-lg text-xs focus:bg-white focus:outline-hidden focus:border-teal-500"
              />

              {/* ICD-10 Diagnoses */}
              <div>
                <div className="flex items-center justify-between mb-1.5">
                  <label className="text-xs font-semibold text-slate-700">ICD-10 Diagnoses</label>
                  <span className="text-[11px] text-slate-400">Primary + Secondary</span>
                </div>
                <div className="flex flex-wrap gap-2 mb-2">
                  {diagnoses.map((d, idx) => (
                    <span
                      key={idx}
                      className="inline-flex items-center gap-1.5 px-2.5 py-1 bg-teal-50 text-teal-800 rounded-md font-semibold text-xs border border-teal-200"
                    >
                      <span>{d}</span>
                      <button
                        onClick={() => setDiagnoses(diagnoses.filter((_, i) => i !== idx))}
                        className="text-teal-600 hover:text-teal-900 cursor-pointer"
                      >
                        &times;
                      </button>
                    </span>
                  ))}
                </div>
                <input
                  type="text"
                  placeholder="Type ICD-10 code or diagnosis to add..."
                  className="w-full px-3 py-1.5 bg-slate-50 border border-slate-200 rounded-lg text-xs focus:bg-white focus:outline-hidden"
                />
              </div>
            </CardContent>
          </Card>

          {/* e-Prescription Pad */}
          <Card>
            <CardContent className="p-4 space-y-3">
              <div className="flex items-center justify-between">
                <h3 className="text-xs font-bold text-slate-800 uppercase tracking-wider flex items-center gap-2">
                  <span className="text-teal-600 font-serif text-base font-bold">℞</span>
                  <span>Prescription & Medication Orders</span>
                </h3>
                <Button
                  variant="outline"
                  size="sm"
                  icon={<Plus className="w-3.5 h-3.5" />}
                  onClick={addMedicine}
                >
                  Add Medicine
                </Button>
              </div>

              <div className="overflow-x-auto">
                <table className="w-full text-left text-xs">
                  <thead className="bg-slate-50 text-slate-500 border-b border-slate-200 text-[11px]">
                    <tr>
                      <th className="px-3 py-2">Medicine (Stock)</th>
                      <th className="px-3 py-2">Dosage</th>
                      <th className="px-3 py-2">Frequency</th>
                      <th className="px-3 py-2">Duration</th>
                      <th className="px-3 py-2">Instructions</th>
                      <th className="px-2 py-2" />
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-100">
                    {prescriptionItems.map((item) => (
                      <tr key={item.id}>
                        <td className="px-3 py-2.5 font-semibold text-slate-900">{item.name}</td>
                        <td className="px-3 py-2.5">
                          <input
                            type="text"
                            defaultValue={item.dosage}
                            className="w-20 p-1 bg-slate-50 border border-slate-200 rounded text-xs"
                          />
                        </td>
                        <td className="px-3 py-2.5">
                          <select
                            defaultValue={item.frequency}
                            className="p-1 bg-slate-50 border border-slate-200 rounded text-xs"
                          >
                            <option>1-0-1 (Twice Daily)</option>
                            <option>1-1-1 (Thrice Daily)</option>
                            <option>1-0-0 (Morning)</option>
                            <option>0-0-1 (Night)</option>
                          </select>
                        </td>
                        <td className="px-3 py-2.5">
                          <input
                            type="text"
                            defaultValue={item.duration}
                            className="w-16 p-1 bg-slate-50 border border-slate-200 rounded text-xs"
                          />
                        </td>
                        <td className="px-3 py-2.5">
                          <input
                            type="text"
                            defaultValue={item.instructions}
                            className="w-28 p-1 bg-slate-50 border border-slate-200 rounded text-xs"
                          />
                        </td>
                        <td className="px-2 py-2.5 text-right">
                          <button
                            onClick={() => removeMedicine(item.id)}
                            className="text-rose-500 hover:text-rose-700 p-1 cursor-pointer"
                          >
                            <Trash2 className="w-3.5 h-3.5" />
                          </button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Right: Lab Orders & Follow-up */}
        <div className="space-y-4">
          {/* Order Diagnostics */}
          <Card>
            <CardContent className="p-4 space-y-3">
              <h3 className="text-xs font-bold text-slate-800 uppercase tracking-wider">
                Order Diagnostic Tests
              </h3>
              <div className="space-y-2 text-xs">
                <label className="flex items-center gap-2 text-slate-700 cursor-pointer">
                  <input
                    type="checkbox"
                    defaultChecked
                    className="rounded text-teal-600 focus:ring-teal-500"
                  />
                  <span>12-Lead Electrocardiogram (ECG)</span>
                </label>
                <label className="flex items-center gap-2 text-slate-700 cursor-pointer">
                  <input
                    type="checkbox"
                    defaultChecked
                    className="rounded text-teal-600 focus:ring-teal-500"
                  />
                  <span>Lipid Profile Panel</span>
                </label>
                <label className="flex items-center gap-2 text-slate-700 cursor-pointer">
                  <input type="checkbox" className="rounded text-teal-600 focus:ring-teal-500" />
                  <span>Cardiac Troponin-I (STAT)</span>
                </label>
                <label className="flex items-center gap-2 text-slate-700 cursor-pointer">
                  <input type="checkbox" className="rounded text-teal-600 focus:ring-teal-500" />
                  <span>Echocardiography (2D Echo)</span>
                </label>
              </div>
            </CardContent>
          </Card>

          {/* Follow-up & Advice */}
          <Card>
            <CardContent className="p-4 space-y-3 text-xs">
              <h3 className="text-xs font-bold text-slate-800 uppercase tracking-wider">
                Follow-Up & Dietary Advice
              </h3>
              <div>
                <label className="text-[11px] text-slate-500 font-medium">Follow-Up Date</label>
                <input
                  type="date"
                  defaultValue="2026-10-11"
                  className="w-full mt-1 p-2 bg-slate-50 border border-slate-200 rounded-lg text-xs"
                />
              </div>
              <div>
                <label className="text-[11px] text-slate-500 font-medium">Dietary Guidelines</label>
                <textarea
                  rows={2}
                  defaultValue="Low sodium diet, avoid sudden physical exertion. Return immediately if pain worsens."
                  className="w-full mt-1 p-2 bg-slate-50 border border-slate-200 rounded-lg text-xs"
                />
              </div>
            </CardContent>
          </Card>

          {/* Finalize Button */}
          <div className="space-y-2">
            <Button
              variant="primary"
              size="lg"
              className="w-full"
              icon={<CheckCircle2 className="w-4 h-4" />}
              onClick={handleFinalize}
            >
              Finalize & Issue Prescription
            </Button>
            <Button
              variant="secondary"
              size="md"
              className="w-full"
              icon={<Bed className="w-4 h-4" />}
              onClick={() => openModal('admitPatient')}
            >
              Admit to Inpatient Bed (IPD)
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
};

