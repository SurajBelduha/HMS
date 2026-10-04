import React from 'react';
import { FlaskConical, Plus, CheckCircle, Clock } from 'lucide-react';
import { Card } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { useApp } from '../../context/AppContext';
import type { LabOrder } from '../../types';

export const LaboratoryScreen: React.FC = () => {
  const { showToast } = useApp();

  const orders: LabOrder[] = [
    {
      id: '1',
      orderNumber: 'LAB-2026-081',
      patientId: '1',
      patientName: 'John Doe',
      patientMrn: 'PAT-2026-00001',
      doctorName: 'Dr. Alice Smith',
      testName: 'Complete Blood Count (CBC) with Differential',
      orderDate: 'Today, 09:10 AM',
      status: 'SampleCollected',
    },
    {
      id: '2',
      orderNumber: 'LAB-2026-082',
      patientId: '2',
      patientName: 'Jane Smith',
      patientMrn: 'PAT-2026-00002',
      doctorName: 'Dr. Alice Smith',
      testName: 'Lipid Profile & Glucose Fasting',
      orderDate: 'Today, 09:20 AM',
      status: 'Ordered',
    },
    {
      id: '3',
      orderNumber: 'LAB-2026-080',
      patientId: '3',
      patientName: 'Robert Brown',
      patientMrn: 'PAT-2026-00003',
      doctorName: 'Dr. Robert Brown',
      testName: 'Cardiac Troponin-I (STAT)',
      orderDate: '02 Oct 2026',
      status: 'Completed',
      resultSummary: 'Troponin-I: 0.02 ng/mL (Normal < 0.04)',
    },
  ];

  return (
    <div className="space-y-6">
      {/* Title */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-xl font-bold text-slate-900 tracking-tight">
            Laboratory Information System (LIS)
          </h1>
          <p className="text-xs text-slate-500">
            Manage diagnostic orders, sample collection status, and enter lab test results.
          </p>
        </div>
        <Button
          variant="primary"
          icon={<Plus className="w-4 h-4" />}
          onClick={() => showToast('Order New Test Modal')}
        >
          + Order New Test
        </Button>
      </div>

      {/* Lab Orders Table */}
      <Card className="overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-xs">
            <thead className="bg-slate-50 text-slate-500 font-semibold border-b border-slate-200 text-[11px] uppercase tracking-wider">
              <tr>
                <th className="px-4 py-3">Order ID</th>
                <th className="px-4 py-3">Patient</th>
                <th className="px-4 py-3">Prescribing Doctor</th>
                <th className="px-4 py-3">Test Title</th>
                <th className="px-4 py-3">Date</th>
                <th className="px-4 py-3">Status</th>
                <th className="px-4 py-3 text-right">Action</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {orders.map((order) => (
                <tr key={order.id} className="hover:bg-slate-50/80 transition">
                  <td className="px-4 py-3 font-mono font-semibold text-teal-700">
                    {order.orderNumber}
                  </td>
                  <td className="px-4 py-3">
                    <div className="font-semibold text-slate-900">{order.patientName}</div>
                    <div className="text-[11px] text-slate-400 font-mono">{order.patientMrn}</div>
                  </td>
                  <td className="px-4 py-3 text-slate-700">{order.doctorName}</td>
                  <td className="px-4 py-3 font-medium text-slate-900">{order.testName}</td>
                  <td className="px-4 py-3 text-slate-500">{order.orderDate}</td>
                  <td className="px-4 py-3">
                    <Badge
                      variant={
                        order.status === 'Completed'
                          ? 'success'
                          : order.status === 'SampleCollected'
                          ? 'warning'
                          : 'neutral'
                      }
                    >
                      {order.status}
                    </Badge>
                  </td>
                  <td className="px-4 py-3 text-right">
                    {order.status === 'SampleCollected' ? (
                      <Button
                        variant="primary"
                        size="sm"
                        onClick={() =>
                          showToast(`Results signed off for ${order.orderNumber}: Hb 14.2, WBC 7,200`)
                        }
                      >
                        Enter Results
                      </Button>
                    ) : order.status === 'Ordered' ? (
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => showToast(`Sample collected for ${order.orderNumber}`)}
                      >
                        Collect Sample
                      </Button>
                    ) : (
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => alert(`LAB REPORT:\n${order.resultSummary}`)}
                      >
                        View Report
                      </Button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </Card>
    </div>
  );
};

