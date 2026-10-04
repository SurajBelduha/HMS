import React, { useState } from 'react';
import { Modal } from '../../components/ui/Modal';
import { Input } from '../../components/ui/Input';
import { Button } from '../../components/ui/Button';
import { useApp } from '../../context/AppContext';

export const BookAppointmentModal: React.FC = () => {
  const { activeModal, closeModal, showToast, setCurrentScreen } = useApp();
  const isOpen = activeModal === 'bookAppointment';

  const [patient, setPatient] = useState('John Doe (PAT-2026-00001)');
  const [doctor, setDoctor] = useState('Dr. Alice Smith — Cardiology ($500 fee)');
  const [date, setDate] = useState('2026-10-04');
  const [reason, setReason] = useState('Routine Cardiac Consultation');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    closeModal();
    showToast('Appointment booked! Token #04 allocated for Dr. Alice Smith');
    setCurrentScreen('appointments');
  };

  return (
    <Modal isOpen={isOpen} onClose={closeModal} title="Book Doctor Appointment" maxWidth="md">
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

        <div>
          <label className="block text-xs font-semibold text-slate-700 mb-1">
            Consulting Doctor *
          </label>
          <select
            value={doctor}
            onChange={(e) => setDoctor(e.target.value)}
            className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-lg text-xs"
          >
            <option>Dr. Alice Smith — Cardiology ($500 fee)</option>
            <option>Dr. Robert Brown — Neurology ($800 fee)</option>
          </select>
        </div>

        <div className="grid grid-cols-2 gap-3">
          <Input
            label="Appointment Date"
            type="date"
            value={date}
            onChange={(e) => setDate(e.target.value)}
          />
          <div>
            <label className="block text-xs font-semibold text-slate-700 mb-1">Next Free Slot</label>
            <input
              type="text"
              value="09:45 AM (Token #04)"
              disabled
              className="w-full px-3 py-2 bg-slate-100 font-bold text-teal-800 border border-slate-200 rounded-lg text-xs"
            />
          </div>
        </div>

        <Input
          label="Chief Complaint / Purpose of Visit"
          value={reason}
          onChange={(e) => setReason(e.target.value)}
          placeholder="e.g. Follow-up ECG, Chest ache"
        />

        <div className="p-3 bg-teal-50 rounded-xl border border-teal-100 flex items-center justify-between">
          <span className="text-teal-800 font-medium">Consultation Fee</span>
          <span className="text-base font-bold text-teal-900">$500.00</span>
        </div>

        <div className="pt-3 border-t border-slate-100 flex justify-end gap-2">
          <Button variant="outline" type="button" onClick={closeModal}>
            Cancel
          </Button>
          <Button variant="primary" type="submit">
            Confirm & Issue Token
          </Button>
        </div>
      </form>
    </Modal>
  );
};

