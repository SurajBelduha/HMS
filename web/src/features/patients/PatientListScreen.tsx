import React, { useState } from 'react';
import { UserPlus, Search, FileText, Calendar, Bed } from 'lucide-react';
import { Card } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { useApp } from '../../context/AppContext';
import type { Patient } from '../../types';

export const PatientListScreen: React.FC = () => {
  const { openModal, setSelectedPatient, setCurrentScreen, showToast } = useApp();

  const [search, setSearch] = useState('');
  const [selectedGender, setSelectedGender] = useState('All');

  const patients: Patient[] = [
    {
      id: '1',
      mrn: 'PAT-2026-00001',
      firstName: 'John',
      lastName: 'Doe',
      gender: 'Male',
      dob: '1990-01-01',
      age: 36,
      phone: '+91 98765 43210',
      email: 'john@example.com',
      bloodGroup: 'O+',
      allergies: ['Penicillin'],
      status: 'OPD In-Consult',
      lastVisit: 'Today, 09:00 AM',
    },
    {
      id: '2',
      mrn: 'PAT-2026-00002',
      firstName: 'Jane',
      lastName: 'Smith',
      gender: 'Female',
      dob: '1992-02-02',
      age: 34,
      phone: '+91 98765 43211',
      email: 'jane@example.com',
      bloodGroup: 'A+',
      allergies: [],
      status: 'Waiting in Lobby',
      lastVisit: 'Today, 09:15 AM',
    },
    {
      id: '3',
      mrn: 'PAT-2026-00003',
      firstName: 'Robert',
      lastName: 'Brown',
      gender: 'Male',
      dob: '1968-06-14',
      age: 58,
      phone: '+91 98111 22334',
      email: 'robert.b@domain.com',
      bloodGroup: 'B+',
      allergies: ['Aspirin'],
      status: 'Admitted (Bed B-102)',
      lastVisit: '02 Oct 2026',
    },
  ];

  const filteredPatients = patients.filter((p) => {
    const matchesSearch =
      p.firstName.toLowerCase().includes(search.toLowerCase()) ||
      p.lastName.toLowerCase().includes(search.toLowerCase()) ||
      p.mrn.toLowerCase().includes(search.toLowerCase()) ||
      p.phone.includes(search);
    const matchesGender = selectedGender === 'All' || p.gender === selectedGender;
    return matchesSearch && matchesGender;
  });

  const handleOpenEMR = (p: Patient) => {
    setSelectedPatient(p);
    showToast(`Loaded EMR 360 chart for ${p.firstName} ${p.lastName}`);
    setCurrentScreen('clinical');
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-xl font-bold text-slate-900 tracking-tight">
            Patient Directory & Electronic Medical Records (EMR)
          </h1>
          <p className="text-xs text-slate-500">
            Comprehensive patient registry, visit history, diagnoses, and medical charts.
          </p>
        </div>
        <Button
          variant="primary"
          icon={<UserPlus className="w-4 h-4" />}
          onClick={() => openModal('registerPatient')}
        >
          + Register New Patient
        </Button>
      </div>

      {/* Filter Toolbar */}
      <div className="bg-white p-3 rounded-xl border border-slate-200 shadow-xs flex flex-wrap items-center justify-between gap-3">
        <div className="flex items-center gap-2 flex-1 min-w-[280px]">
          <div className="relative w-full">
            <Search className="w-4 h-4 text-slate-400 absolute left-3 top-2.5" />
            <input
              type="text"
              placeholder="Search by Name, MRN (PAT-2026-...), or Phone..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="w-full pl-9 pr-3 py-1.5 bg-slate-50 border border-slate-200 rounded-lg text-xs focus:bg-white focus:outline-hidden focus:border-teal-500"
            />
          </div>
        </div>

        <div className="flex items-center gap-2">
          <select
            value={selectedGender}
            onChange={(e) => setSelectedGender(e.target.value)}
            className="px-2.5 py-1.5 bg-slate-50 border border-slate-200 rounded-lg text-xs text-slate-600 focus:outline-hidden cursor-pointer"
          >
            <option value="All">All Genders</option>
            <option value="Male">Male</option>
            <option value="Female">Female</option>
          </select>
        </div>
      </div>

      {/* Data Table */}
      <Card className="overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-xs">
            <thead className="bg-slate-50/80 text-slate-500 font-semibold border-b border-slate-200 uppercase tracking-wider text-[11px]">
              <tr>
                <th className="px-4 py-3">Patient MRN</th>
                <th className="px-4 py-3">Full Name & Demographics</th>
                <th className="px-4 py-3">Phone & Email</th>
                <th className="px-4 py-3">Blood Group</th>
                <th className="px-4 py-3">Last Visit</th>
                <th className="px-4 py-3">Status</th>
                <th className="px-4 py-3 text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {filteredPatients.map((patient) => (
                <tr key={patient.id} className="hover:bg-slate-50/80 transition">
                  <td className="px-4 py-3 font-mono font-semibold text-teal-700">{patient.mrn}</td>
                  <td className="px-4 py-3">
                    <div className="font-semibold text-slate-900">
                      {patient.firstName} {patient.lastName}
                    </div>
                    <div className="text-[11px] text-slate-400">
                      {patient.age} Yrs • {patient.gender} • DOB: {patient.dob}
                    </div>
                  </td>
                  <td className="px-4 py-3 text-slate-600">
                    <div>{patient.phone}</div>
                    <div className="text-[11px] text-slate-400">{patient.email}</div>
                  </td>
                  <td className="px-4 py-3 font-medium">
                    <span className="px-2 py-0.5 bg-rose-50 text-rose-700 rounded-md font-bold text-xs border border-rose-100">
                      {patient.bloodGroup}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-slate-500">{patient.lastVisit}</td>
                  <td className="px-4 py-3">
                    <Badge
                      variant={
                        patient.status?.includes('In-Consult')
                          ? 'success'
                          : patient.status?.includes('Waiting')
                          ? 'warning'
                          : 'info'
                      }
                    >
                      {patient.status}
                    </Badge>
                  </td>
                  <td className="px-4 py-3 text-right space-x-1.5">
                    <Button
                      variant="primary"
                      size="sm"
                      icon={<FileText className="w-3 h-3" />}
                      onClick={() => handleOpenEMR(patient)}
                    >
                      View EMR
                    </Button>
                    <Button
                      variant="outline"
                      size="sm"
                      icon={<Calendar className="w-3 h-3" />}
                      onClick={() => openModal('bookAppointment')}
                    >
                      Book
                    </Button>
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

