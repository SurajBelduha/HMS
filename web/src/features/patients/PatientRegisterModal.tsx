import React, { useState } from 'react';
import { Modal } from '../../components/ui/Modal';
import { Input } from '../../components/ui/Input';
import { Button } from '../../components/ui/Button';
import { useApp } from '../../context/AppContext';

export const PatientRegisterModal: React.FC = () => {
  const { activeModal, closeModal, showToast } = useApp();
  const isOpen = activeModal === 'registerPatient';

  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    dob: '1995-05-15',
    gender: 'Male',
    bloodGroup: 'O+',
    phone: '',
    email: '',
    address: '',
    allergies: 'Penicillin',
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    closeModal();
    showToast(`Registered patient: ${formData.firstName} ${formData.lastName} (MRN: PAT-2026-00004)`);
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={closeModal}
      title="Register New Patient (Auto-MRN: PAT-2026-00004)"
      maxWidth="lg"
    >
      <form onSubmit={handleSubmit} className="space-y-4 text-xs">
        <div className="grid grid-cols-2 gap-3">
          <Input
            label="First Name *"
            required
            placeholder="e.g. David"
            value={formData.firstName}
            onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
          />
          <Input
            label="Last Name *"
            required
            placeholder="e.g. Miller"
            value={formData.lastName}
            onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
          />
        </div>

        <div className="grid grid-cols-3 gap-3">
          <Input
            label="Date of Birth *"
            type="date"
            required
            value={formData.dob}
            onChange={(e) => setFormData({ ...formData, dob: e.target.value })}
          />
          <div>
            <label className="block text-xs font-semibold text-slate-700 mb-1">Gender *</label>
            <select
              value={formData.gender}
              onChange={(e) => setFormData({ ...formData, gender: e.target.value })}
              className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-lg text-xs"
            >
              <option>Male</option>
              <option>Female</option>
              <option>Other</option>
            </select>
          </div>
          <div>
            <label className="block text-xs font-semibold text-slate-700 mb-1">Blood Group</label>
            <select
              value={formData.bloodGroup}
              onChange={(e) => setFormData({ ...formData, bloodGroup: e.target.value })}
              className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-lg text-xs"
            >
              <option>O+</option>
              <option>O-</option>
              <option>A+</option>
              <option>A-</option>
              <option>B+</option>
              <option>B-</option>
              <option>AB+</option>
            </select>
          </div>
        </div>

        <div className="grid grid-cols-2 gap-3">
          <Input
            label="Phone Number *"
            type="tel"
            required
            placeholder="+91 99999 88888"
            value={formData.phone}
            onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
          />
          <Input
            label="Email Address"
            type="email"
            placeholder="patient@example.com"
            value={formData.email}
            onChange={(e) => setFormData({ ...formData, email: e.target.value })}
          />
        </div>

        <Input
          label="Residential Address"
          placeholder="House #, Street, City, State"
          value={formData.address}
          onChange={(e) => setFormData({ ...formData, address: e.target.value })}
        />

        <Input
          label="Known Allergies (Comma separated)"
          placeholder="e.g. Penicillin, Sulfa drugs, Peanuts"
          value={formData.allergies}
          onChange={(e) => setFormData({ ...formData, allergies: e.target.value })}
        />

        <div className="pt-3 border-t border-slate-100 flex justify-end gap-2">
          <Button variant="outline" type="button" onClick={closeModal}>
            Cancel
          </Button>
          <Button variant="primary" type="submit">
            Register & Create EMR
          </Button>
        </div>
      </form>
    </Modal>
  );
};

