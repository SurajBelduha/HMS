import React from 'react';
import { Calendar, Plus, Clock, ChevronLeft, ChevronRight, Stethoscope } from 'lucide-react';
import { Card, CardHeader, CardTitle, CardContent } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { useApp } from '../../context/AppContext';

export const AppointmentScreen: React.FC = () => {
  const { openModal, setCurrentScreen, showToast } = useApp();

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-xl font-bold text-slate-900 tracking-tight">
            Doctor Appointments & Daily Token Queue
          </h1>
          <p className="text-xs text-slate-500">
            Automated 15-minute slot allocation, token generation, and outpatient schedule.
          </p>
        </div>
        <Button
          variant="primary"
          icon={<Plus className="w-4 h-4" />}
          onClick={() => openModal('bookAppointment')}
        >
          + Book New Appointment
        </Button>
      </div>

      {/* Doctor & Date Header */}
      <Card>
        <CardContent className="p-4 flex flex-wrap items-center justify-between gap-4">
          <div className="flex items-center gap-3">
            <div className="w-11 h-11 rounded-full bg-teal-100 text-teal-800 font-bold flex items-center justify-center text-sm">
              AS
            </div>
            <div>
              <div className="text-xs font-bold text-slate-900">
                Dr. Alice Smith (MD, Chief Cardiologist)
              </div>
              <div className="text-[11px] text-slate-500">
                Shift: Mon - Fri (09:00 AM - 01:00 PM) • Max 20 Patients • Room 101 • Fee: $500
              </div>
            </div>
          </div>

          <div className="flex items-center gap-2">
            <button className="p-1.5 rounded-lg border border-slate-200 hover:bg-slate-100 text-slate-600 text-xs cursor-pointer">
              <ChevronLeft className="w-4 h-4" />
            </button>
            <span className="px-3 py-1.5 bg-slate-100 font-semibold text-xs text-slate-800 rounded-lg">
              Today, 04 Oct 2026
            </span>
            <button className="p-1.5 rounded-lg border border-slate-200 hover:bg-slate-100 text-slate-600 text-xs cursor-pointer">
              <ChevronRight className="w-4 h-4" />
            </button>
          </div>
        </CardContent>
      </Card>

      {/* Slots Queue */}
      <Card className="overflow-hidden">
        <CardHeader>
          <CardTitle
            title="Time Slots & Tokens (15-min Intervals)"
            subtitle="Today's Outpatient Appointment Register"
          />
          <Badge variant="primary">Booked: 2 / 20</Badge>
        </CardHeader>
        <CardContent className="p-0 divide-y divide-slate-100 text-xs">
          {/* Slot 1 */}
          <div className="p-4 flex items-center justify-between bg-teal-50/40">
            <div className="flex items-center gap-4">
              <span className="font-mono font-bold text-teal-800 bg-teal-200/60 px-2 py-1 rounded">
                Token #01
              </span>
              <div>
                <span className="font-semibold text-slate-900">John Doe (PAT-2026-00001)</span>
                <div className="text-[11px] text-slate-500">
                  09:00 AM - 09:15 AM • Reason: Chest Discomfort • Paid: $500.00
                </div>
              </div>
            </div>
            <div className="flex items-center gap-2">
              <Badge variant="success" dot={true}>
                In Consultation
              </Badge>
              <Button
                variant="primary"
                size="sm"
                icon={<Stethoscope className="w-3.5 h-3.5" />}
                onClick={() => setCurrentScreen('clinical')}
              >
                Start Consult
              </Button>
            </div>
          </div>

          {/* Slot 2 */}
          <div className="p-4 flex items-center justify-between">
            <div className="flex items-center gap-4">
              <span className="font-mono font-bold text-slate-700 bg-slate-100 px-2 py-1 rounded">
                Token #02
              </span>
              <div>
                <span className="font-semibold text-slate-900">Jane Smith (PAT-2026-00002)</span>
                <div className="text-[11px] text-slate-500">
                  09:15 AM - 09:30 AM • Reason: Routine ECG Review • Paid: $500.00
                </div>
              </div>
            </div>
            <div className="flex items-center gap-2">
              <Badge variant="warning">Checked-In (Waiting)</Badge>
              <Button
                variant="outline"
                size="sm"
                onClick={() => showToast('Reschedule modal opened')}
              >
                Reschedule
              </Button>
            </div>
          </div>

          {/* Slot 3 */}
          <div className="p-4 flex items-center justify-between bg-slate-50/40">
            <div className="flex items-center gap-4 text-slate-400">
              <span className="font-mono font-bold bg-slate-200/50 px-2 py-1 rounded">
                Token #03
              </span>
              <span>09:30 AM - 09:45 AM • Slot Available</span>
            </div>
            <Button variant="outline" size="sm" onClick={() => openModal('bookAppointment')}>
              + Book Slot
            </Button>
          </div>

          {/* Slot 4 */}
          <div className="p-4 flex items-center justify-between bg-slate-50/40">
            <div className="flex items-center gap-4 text-slate-400">
              <span className="font-mono font-bold bg-slate-200/50 px-2 py-1 rounded">
                Token #04
              </span>
              <span>09:45 AM - 10:00 AM • Slot Available</span>
            </div>
            <Button variant="outline" size="sm" onClick={() => openModal('bookAppointment')}>
              + Book Slot
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

