import React from 'react';
import {
  Calendar,
  BedDouble,
  DollarSign,
  FlaskConical,
  Clock,
  ArrowRight,
  TrendingUp,
} from 'lucide-react';
import { Card, CardHeader, CardTitle, CardContent } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { useApp } from '../../context/AppContext';

export const DashboardScreen: React.FC = () => {
  const { setCurrentScreen, showToast } = useApp();

  return (
    <div className="space-y-6">
      {/* Title & Live Status */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
        <div>
          <h1 className="text-xl font-bold text-slate-900 tracking-tight">
            Executive Hospital Operations
          </h1>
          <p className="text-xs text-slate-500">
            Live operational feed for{' '}
            <span className="font-semibold text-slate-700">ABC Healthcare — Jaipur Branch</span> •
            Sunday, Oct 4, 2026
          </p>
        </div>
        <div>
          <Badge variant="success" dot={true}>
            Real-time Sync Active
          </Badge>
        </div>
      </div>

      {/* 4 Top KPI Stat Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        {/* Card 1 */}
        <Card className="hover:border-teal-300 transition">
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <span className="text-xs font-medium text-slate-500">Today's Appointments</span>
              <span className="p-2 rounded-lg bg-teal-50 text-teal-600">
                <Calendar className="w-4 h-4" />
              </span>
            </div>
            <div className="mt-2 flex items-baseline gap-2">
              <span className="text-2xl font-bold text-slate-900">48</span>
              <span className="text-xs font-semibold text-emerald-600 flex items-center gap-0.5">
                <TrendingUp className="w-3 h-3" /> +12%
              </span>
            </div>
            <p className="text-[11px] text-slate-400 mt-1">28 Completed • 12 In-Queue • 8 Upcoming</p>
          </CardContent>
        </Card>

        {/* Card 2 */}
        <Card className="hover:border-blue-300 transition">
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <span className="text-xs font-medium text-slate-500">Inpatient Bed Occupancy</span>
              <span className="p-2 rounded-lg bg-blue-50 text-blue-600">
                <BedDouble className="w-4 h-4" />
              </span>
            </div>
            <div className="mt-2 flex items-baseline gap-2">
              <span className="text-2xl font-bold text-slate-900">32 / 40</span>
              <span className="text-xs font-semibold text-blue-600 font-mono">80.0%</span>
            </div>
            <div className="w-full bg-slate-100 h-1.5 rounded-full mt-2 overflow-hidden">
              <div className="bg-blue-600 h-1.5 rounded-full" style={{ width: '80%' }} />
            </div>
            <p className="text-[11px] text-slate-400 mt-1">ICU: 5/6 Occupied • General: 18/20</p>
          </CardContent>
        </Card>

        {/* Card 3 */}
        <Card className="hover:border-emerald-300 transition">
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <span className="text-xs font-medium text-slate-500">Revenue Collected</span>
              <span className="p-2 rounded-lg bg-emerald-50 text-emerald-600">
                <DollarSign className="w-4 h-4" />
              </span>
            </div>
            <div className="mt-2 flex items-baseline gap-2">
              <span className="text-2xl font-bold text-slate-900">$14,850</span>
              <span className="text-xs font-semibold text-emerald-600">+8.4%</span>
            </div>
            <p className="text-[11px] text-slate-400 mt-1">
              OPD: $4,200 • IPD: $8,650 • Pharmacy: $2,000
            </p>
          </CardContent>
        </Card>

        {/* Card 4 */}
        <Card className="hover:border-purple-300 transition">
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <span className="text-xs font-medium text-slate-500">Diagnostic Orders</span>
              <span className="p-2 rounded-lg bg-purple-50 text-purple-600">
                <FlaskConical className="w-4 h-4" />
              </span>
            </div>
            <div className="mt-2 flex items-baseline gap-2">
              <span className="text-2xl font-bold text-slate-900">14 Orders</span>
              <span className="text-xs font-semibold text-amber-600">4 Urgent STAT</span>
            </div>
            <p className="text-[11px] text-slate-400 mt-1">8 Collected • 6 Processing</p>
          </CardContent>
        </Card>
      </div>

      {/* Middle Grid: OPD Live Queue & Ward Occupancy */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Live OPD Queue (2 Cols) */}
        <div className="lg:col-span-2">
          <Card>
            <CardHeader>
              <CardTitle
                title="Today's Live OPD Queue"
                subtitle="Live doctor consulting desk tokens & patient status"
              />
              <Button
                variant="ghost"
                size="sm"
                icon={<ArrowRight className="w-3.5 h-3.5" />}
                onClick={() => setCurrentScreen('appointments')}
              >
                View Schedule
              </Button>
            </CardHeader>
            <CardContent className="p-0 divide-y divide-slate-100">
              {/* Token 1 */}
              <div className="p-4 flex items-center justify-between gap-4 hover:bg-slate-50/50 transition">
                <div className="flex items-center gap-3">
                  <div className="w-9 h-9 rounded-lg bg-teal-600 text-white font-bold text-sm flex items-center justify-center font-mono">
                    #01
                  </div>
                  <div>
                    <div className="text-xs font-semibold text-slate-900 flex items-center gap-2">
                      <span>John Doe</span>
                      <span className="text-[11px] font-mono px-1.5 py-0.2 bg-slate-100 text-slate-600 rounded">
                        PAT-2026-00001
                      </span>
                    </div>
                    <div className="text-[11px] text-slate-500">
                      Dr. Alice Smith • Cardiology • 09:00 - 09:15 AM
                    </div>
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  <Badge variant="success" dot={true}>
                    In Consultation
                  </Badge>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setCurrentScreen('clinical')}
                  >
                    Open Desk
                  </Button>
                </div>
              </div>

              {/* Token 2 */}
              <div className="p-4 flex items-center justify-between gap-4 hover:bg-slate-50/50 transition">
                <div className="flex items-center gap-3">
                  <div className="w-9 h-9 rounded-lg bg-amber-500 text-white font-bold text-sm flex items-center justify-center font-mono">
                    #02
                  </div>
                  <div>
                    <div className="text-xs font-semibold text-slate-900 flex items-center gap-2">
                      <span>Jane Smith</span>
                      <span className="text-[11px] font-mono px-1.5 py-0.2 bg-slate-100 text-slate-600 rounded">
                        PAT-2026-00002
                      </span>
                    </div>
                    <div className="text-[11px] text-slate-500">
                      Dr. Alice Smith • Cardiology • 09:15 - 09:30 AM
                    </div>
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  <Badge variant="warning">Waiting (Vitals Done)</Badge>
                  <Button
                    variant="primary"
                    size="sm"
                    onClick={() => showToast('Calling Token #02 Jane Smith into Room 101')}
                  >
                    Call Next
                  </Button>
                </div>
              </div>

              {/* Token 3 */}
              <div className="p-4 flex items-center justify-between gap-4 hover:bg-slate-50/50 transition">
                <div className="flex items-center gap-3">
                  <div className="w-9 h-9 rounded-lg bg-slate-200 text-slate-700 font-bold text-sm flex items-center justify-center font-mono">
                    #03
                  </div>
                  <div>
                    <div className="text-xs font-semibold text-slate-900 flex items-center gap-2">
                      <span>Robert Brown</span>
                      <span className="text-[11px] font-mono px-1.5 py-0.2 bg-slate-100 text-slate-600 rounded">
                        PAT-2026-00003
                      </span>
                    </div>
                    <div className="text-[11px] text-slate-500">
                      Dr. Robert Brown • Neurology • 10:00 - 10:15 AM
                    </div>
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  <Badge variant="neutral">Scheduled</Badge>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => showToast('Checked in Robert Brown')}
                  >
                    Check-In
                  </Button>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Ward Bed Matrix Census */}
        <div>
          <Card className="h-full flex flex-col">
            <CardHeader>
              <CardTitle title="Bed Matrix Census" subtitle="Floor-wise Inpatient utilization" />
              <Button
                variant="ghost"
                size="sm"
                icon={<ArrowRight className="w-3.5 h-3.5" />}
                onClick={() => setCurrentScreen('ipd')}
              >
                Matrix
              </Button>
            </CardHeader>
            <CardContent className="space-y-4 flex-1">
              <div>
                <div className="flex justify-between text-xs mb-1">
                  <span className="font-medium text-slate-700">General Ward 1 (Floor 1)</span>
                  <span className="font-mono text-slate-500">18 / 20 Beds (90%)</span>
                </div>
                <div className="w-full bg-slate-100 h-2 rounded-full overflow-hidden">
                  <div className="bg-teal-600 h-2 rounded-full" style={{ width: '90%' }} />
                </div>
              </div>

              <div>
                <div className="flex justify-between text-xs mb-1">
                  <span className="font-medium text-slate-700">ICU Ward 1 (Floor 2)</span>
                  <span className="font-mono text-amber-600 font-semibold">5 / 6 Beds (83%)</span>
                </div>
                <div className="w-full bg-slate-100 h-2 rounded-full overflow-hidden">
                  <div className="bg-amber-500 h-2 rounded-full" style={{ width: '83%' }} />
                </div>
              </div>

              <div>
                <div className="flex justify-between text-xs mb-1">
                  <span className="font-medium text-slate-700">Private Deluxe Suites</span>
                  <span className="font-mono text-slate-500">9 / 14 Beds (64%)</span>
                </div>
                <div className="w-full bg-slate-100 h-2 rounded-full overflow-hidden">
                  <div className="bg-indigo-600 h-2 rounded-full" style={{ width: '64%' }} />
                </div>
              </div>

              <div className="pt-4 border-t border-slate-100 flex items-center justify-between text-xs text-slate-500">
                <span className="flex items-center gap-1.5">
                  <span className="w-2.5 h-2.5 rounded-full bg-emerald-500" /> 8 Free
                </span>
                <span className="flex items-center gap-1.5">
                  <span className="w-2.5 h-2.5 rounded-full bg-blue-500" /> 32 Occupied
                </span>
                <span className="flex items-center gap-1.5">
                  <span className="w-2.5 h-2.5 rounded-full bg-amber-500" /> 2 Maint
                </span>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
};

