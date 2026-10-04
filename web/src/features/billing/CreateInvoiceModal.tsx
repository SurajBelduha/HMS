import React, { useState } from 'react';
import { Modal } from '../../components/ui/Modal';
import { Input } from '../../components/ui/Input';
import { Button } from '../../components/ui/Button';
import { useApp } from '../../context/AppContext';

export const CreateInvoiceModal: React.FC = () => {
  const { activeModal, closeModal, showToast, setCurrentScreen } = useApp();
  const isOpen = activeModal === 'createInvoice';

  const [patient, setPatient] = useState('John Doe (PAT-2026-00001)');
  const [subtotal, setSubtotal] = useState(500);
  const [discount, setDiscount] = useState(0);

  const total = Math.max(0, subtotal - discount);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    closeModal();
    showToast(`Invoice generated: INV-2026-00003 for $${total.toFixed(2)}`);
    setCurrentScreen('billing');
  };

  return (
    <Modal isOpen={isOpen} onClose={closeModal} title="Generate Patient Invoice" maxWidth="md">
      <form onSubmit={handleSubmit} className="space-y-4 text-xs">
        <div>
          <label className="block text-xs font-semibold text-slate-700 mb-1">
            Select Patient *
          </label>
          <select
            value={patient}
            onChange={(e) => setPatient(e.target.value)}
            className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-lg text-xs"
          >
            <option>John Doe (PAT-2026-00001)</option>
            <option>Jane Smith (PAT-2026-00002)</option>
            <option>Robert Brown (PAT-2026-00003)</option>
          </select>
        </div>

        <div className="grid grid-cols-2 gap-3">
          <Input
            label="Subtotal ($)"
            type="number"
            value={subtotal}
            onChange={(e) => setSubtotal(Number(e.target.value))}
          />
          <Input
            label="Discount ($)"
            type="number"
            value={discount}
            onChange={(e) => setDiscount(Number(e.target.value))}
          />
        </div>

        <div className="p-3 bg-teal-50 rounded-xl border border-teal-100 flex items-center justify-between">
          <span className="text-teal-800 font-medium">Total Payable Amount</span>
          <span className="text-base font-bold text-teal-900">${total.toFixed(2)}</span>
        </div>

        <div className="pt-3 border-t border-slate-100 flex justify-end gap-2">
          <Button variant="outline" type="button" onClick={closeModal}>
            Cancel
          </Button>
          <Button variant="primary" type="submit">
            Create Invoice
          </Button>
        </div>
      </form>
    </Modal>
  );
};

