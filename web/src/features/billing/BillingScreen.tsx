import React from 'react';
import { Receipt, Plus, DollarSign, CreditCard } from 'lucide-react';
import { Card } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { useApp } from '../../context/AppContext';

export const BillingScreen: React.FC = () => {
  const { openModal, showToast } = useApp();

  const handleCollect = (invNo: string, amount: number) => {
    const method = prompt(
      `Collect payment of $${amount} for ${invNo}.\nChoose method: Cash / Card / UPI / Insurance:`,
      'Card'
    );
    if (method) {
      showToast(`Payment of $${amount} recorded for ${invNo} via ${method}!`);
    }
  };

  const handleReceipt = (invNo: string, patient: string, amount: number) => {
    alert(
      `HOSPITAL INVOICE RECEIPT\n--------------------------------\nInvoice No: ${invNo}\nPatient: ${patient}\nFacility: ABC Healthcare Ltd - Jaipur Branch\nTotal Amount: $${amount}\nStatus: Paid\nTax Registration: GSTIN-08AAACH2026Z1Z\nThank you for choosing HealthCore.`
    );
  };

  return (
    <div className="space-y-6">
      {/* Title */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-xl font-bold text-slate-900 tracking-tight">
            Billing, POS Cashier & Invoices
          </h1>
          <p className="text-xs text-slate-500">
            Manage patient invoices, cash/card/UPI receipts, insurance claims, and outstanding
            balances.
          </p>
        </div>
        <Button
          variant="primary"
          icon={<Plus className="w-4 h-4" />}
          onClick={() => openModal('createInvoice')}
        >
          + Create New Invoice
        </Button>
      </div>

      {/* Invoices Table */}
      <Card className="overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-xs">
            <thead className="bg-slate-50 text-slate-500 font-semibold border-b border-slate-200 text-[11px] uppercase tracking-wider">
              <tr>
                <th className="px-4 py-3">Invoice #</th>
                <th className="px-4 py-3">Patient</th>
                <th className="px-4 py-3">Date</th>
                <th className="px-4 py-3">Subtotal</th>
                <th className="px-4 py-3">Discount</th>
                <th className="px-4 py-3">Total Payable</th>
                <th className="px-4 py-3">Paid Amount</th>
                <th className="px-4 py-3">Status</th>
                <th className="px-4 py-3 text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              <tr className="hover:bg-slate-50/80 transition">
                <td className="px-4 py-3 font-mono font-semibold text-teal-700">INV-2026-00001</td>
                <td className="px-4 py-3 font-semibold text-slate-900">John Doe</td>
                <td className="px-4 py-3 text-slate-500">04 Oct 2026</td>
                <td className="px-4 py-3">$500.00</td>
                <td className="px-4 py-3">$0.00</td>
                <td className="px-4 py-3 font-bold text-slate-900">$500.00</td>
                <td className="px-4 py-3 font-bold text-emerald-600">$500.00</td>
                <td className="px-4 py-3">
                  <Badge variant="success">Paid</Badge>
                </td>
                <td className="px-4 py-3 text-right space-x-1.5">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => handleReceipt('INV-2026-00001', 'John Doe', 500)}
                  >
                    Receipt
                  </Button>
                </td>
              </tr>

              <tr className="hover:bg-slate-50/80 transition">
                <td className="px-4 py-3 font-mono font-semibold text-teal-700">INV-2026-00002</td>
                <td className="px-4 py-3 font-semibold text-slate-900">Jane Smith</td>
                <td className="px-4 py-3 text-slate-500">04 Oct 2026</td>
                <td className="px-4 py-3">$800.00</td>
                <td className="px-4 py-3 text-emerald-600">-$50.00</td>
                <td className="px-4 py-3 font-bold text-slate-900">$750.00</td>
                <td className="px-4 py-3 font-bold text-slate-400">$0.00</td>
                <td className="px-4 py-3">
                  <Badge variant="danger">Unpaid</Badge>
                </td>
                <td className="px-4 py-3 text-right space-x-1.5">
                  <Button
                    variant="primary"
                    size="sm"
                    icon={<CreditCard className="w-3.5 h-3.5" />}
                    onClick={() => handleCollect('INV-2026-00002', 750)}
                  >
                    Collect $750
                  </Button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </Card>
    </div>
  );
};

