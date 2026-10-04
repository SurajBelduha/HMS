import React, { useState } from 'react';
import { Pill, Plus, Search, AlertCircle, CheckCircle } from 'lucide-react';
import { Card } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { useApp } from '../../context/AppContext';
import type { Medicine } from '../../types';

export const PharmacyScreen: React.FC = () => {
  const { showToast } = useApp();
  const [search, setSearch] = useState('');

  const medicines: Medicine[] = [
    {
      id: '1',
      medicineCode: 'MED-001',
      name: 'Paracetamol 500mg',
      genericName: 'Acetaminophen',
      category: 'Analgesic',
      unitPrice: 5.0,
      stockQuantity: 500,
      expiryDate: 'Oct 2028',
    },
    {
      id: '2',
      medicineCode: 'MED-002',
      name: 'Amoxicillin 250mg',
      genericName: 'Amoxicillin Trihydrate',
      category: 'Antibiotic',
      unitPrice: 12.0,
      stockQuantity: 300,
      expiryDate: 'Oct 2027',
    },
    {
      id: '3',
      medicineCode: 'MED-003',
      name: 'Amlodipine 5mg',
      genericName: 'Amlodipine Besylate',
      category: 'Antihypertensive',
      unitPrice: 8.5,
      stockQuantity: 45,
      expiryDate: 'Dec 2026',
    },
    {
      id: '4',
      medicineCode: 'MED-004',
      name: 'Atorvastatin 10mg',
      genericName: 'Atorvastatin Calcium',
      category: 'Statin / Lipid Lowering',
      unitPrice: 15.0,
      stockQuantity: 180,
      expiryDate: 'Jan 2028',
    },
  ];

  const filtered = medicines.filter(
    (m) =>
      m.name.toLowerCase().includes(search.toLowerCase()) ||
      m.genericName.toLowerCase().includes(search.toLowerCase()) ||
      m.medicineCode.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-xl font-bold text-slate-900 tracking-tight">
            Pharmacy Formulary & Inventory Control
          </h1>
          <p className="text-xs text-slate-500">
            Track medication stock quantities, unit prices, expirations, and dispensing.
          </p>
        </div>
        <Button
          variant="primary"
          icon={<Plus className="w-4 h-4" />}
          onClick={() => showToast('Opened Add Medicine Modal')}
        >
          + Add Medicine Stock
        </Button>
      </div>

      {/* Search Toolbar */}
      <div className="bg-white p-3 rounded-xl border border-slate-200 shadow-xs flex items-center justify-between gap-3">
        <div className="relative w-full max-w-sm">
          <Search className="w-4 h-4 text-slate-400 absolute left-3 top-2.5" />
          <input
            type="text"
            placeholder="Search by Trade Name, Generic, or Code..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="w-full pl-9 pr-3 py-1.5 bg-slate-50 border border-slate-200 rounded-lg text-xs focus:bg-white focus:outline-hidden focus:border-teal-500"
          />
        </div>
      </div>

      {/* Inventory Table */}
      <Card className="overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-xs">
            <thead className="bg-slate-50 text-slate-500 font-semibold border-b border-slate-200 text-[11px] uppercase tracking-wider">
              <tr>
                <th className="px-4 py-3">Code</th>
                <th className="px-4 py-3">Trade Name & Generic</th>
                <th className="px-4 py-3">Category</th>
                <th className="px-4 py-3">Unit Price</th>
                <th className="px-4 py-3">Current Stock</th>
                <th className="px-4 py-3">Expiry Date</th>
                <th className="px-4 py-3 text-right">Action</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {filtered.map((m) => {
                const isLowStock = m.stockQuantity < 100;
                return (
                  <tr
                    key={m.id}
                    className={`hover:bg-slate-50/80 transition ${
                      isLowStock ? 'bg-rose-50/30' : ''
                    }`}
                  >
                    <td className="px-4 py-3 font-mono font-semibold text-slate-800">
                      {m.medicineCode}
                    </td>
                    <td className="px-4 py-3">
                      <div className="font-bold text-slate-900">{m.name}</div>
                      <div className="text-[11px] text-slate-400">{m.genericName}</div>
                    </td>
                    <td className="px-4 py-3">
                      <span className="px-2 py-0.5 bg-slate-100 rounded text-slate-700 font-medium">
                        {m.category}
                      </span>
                    </td>
                    <td className="px-4 py-3 font-semibold text-slate-900">
                      ${m.unitPrice.toFixed(2)}
                    </td>
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-2">
                        <span
                          className={`font-bold ${
                            isLowStock ? 'text-rose-600' : 'text-emerald-700'
                          }`}
                        >
                          {m.stockQuantity} units
                        </span>
                        {isLowStock ? (
                          <Badge variant="danger" size="sm">
                            Low Stock
                          </Badge>
                        ) : (
                          <Badge variant="success" size="sm">
                            Optimal
                          </Badge>
                        )}
                      </div>
                    </td>
                    <td className="px-4 py-3 text-slate-500">{m.expiryDate}</td>
                    <td className="px-4 py-3 text-right">
                      {isLowStock ? (
                        <Button
                          variant="danger"
                          size="sm"
                          onClick={() => showToast(`Restocked ${m.name} (+200 units)`)}
                        >
                          Restock
                        </Button>
                      ) : (
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => showToast(`Dispensed ${m.name} for e-Prescription`)}
                        >
                          Dispense
                        </Button>
                      )}
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      </Card>
    </div>
  );
};

