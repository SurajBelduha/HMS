import React, { useState } from 'react';
import { Modal } from '../../components/ui/Modal';
import { Input } from '../../components/ui/Input';
import { Button } from '../../components/ui/Button';
import { useApp } from '../../context/AppContext';

export const AdmitPatientModal: React.FC = () => {
  const { activeModal, closeModal, showToast, setCurrentScreen } = useApp();
  const isOpen = activeModal === 'admitPatient';

  const [patient, setPatient] = useState('John Doe (PAT-2026-00001)');
  const [doctor, setDoctor] = useState('Dr. Alice Smith');
  const [bed, setBed] = useState('Bed B-101 (General - $1,000/day)');
  const [reason, setReason] = useState('Unstable Angina monitoring & observation');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    closeModal();
    showToast(`Admitted patient to ${bed} (Admission No: ADM-2026-00002)`);
    setCurrentScreen('ipd');
  };

  return (
    <Modal isOpen={isOpen} onClose={closeModal} title="Admit Patient to IPD Ward" maxWidth="md">
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
          <div>
            <label className="block text-xs font-semibold text-slate-700 mb-1">
              Attending Doctor *
            </label>
            <select
              value={doctor}
              onChange={(e) => setDoctor(e.target.value)}
              className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-lg text-xs"
            >
              <option>Dr. Alice Smith (Cardiology)</option>
              <option>Dr. Robert Brown (Neurology)</option>
            </select>
          </div>

          <div>
            <label className="block text-xs font-semibold text-slate-700 mb-1">
              Allocated Bed *
            </label>
            <select
              value={bed}
              onChange={(e) => setBed(e.target.value)}
              className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-lg text-xs font-semibold text-teal-800"
            >
              <option>Bed B-101 (General - $1,000/day)</option>
              <option>Bed B-103 (General - $1,000/day)</option>
              <option>ICU-02 (ICU - $5,000/day)</option>
            </select>
          </div>
        </div>

        <div>
          <label className="block text-xs font-semibold text-slate-700 mb-1">
            Reason for Admission
          </label>
          <textarea
            rows={2}
            value={reason}
            onChange={(e) => setReason(e.target.value)}
            className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-lg text-xs"
          />
        </div>

        <div className="pt-3 border-t border-slate-100 flex justify-end gap-2">
          <Button variant="outline" type="button" onClick={closeModal}>
            Cancel
          </Button>
          <Button variant="secondary" type="submit">
            Confirm Admission & Allocate Bed
          </Button>
        </div>
      </form>
    </Modal>
  );
};

