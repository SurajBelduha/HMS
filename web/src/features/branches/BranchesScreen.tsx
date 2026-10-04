import React from 'react';
import { Building, MapPin, Phone, Users, Stethoscope } from 'lucide-react';
import { Card, CardContent } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { useApp } from '../../context/AppContext';

export const BranchesScreen: React.FC = () => {
  const { setActiveBranch, showToast } = useApp();

  return (
    <div className="space-y-6">
      {/* Title */}
      <div>
        <h1 className="text-xl font-bold text-slate-900 tracking-tight">
          Hospital Branches & Clinical Departments
        </h1>
        <p className="text-xs text-slate-500">
          Manage multiple hospital units, branch codes, contact coordinates, and clinical
          specialties.
        </p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {/* Jaipur Branch */}
        <Card className="hover:border-teal-300 transition">
          <CardContent className="p-5 space-y-4">
            <div className="flex items-center justify-between">
              <div>
                <span className="px-2 py-0.5 bg-teal-100 text-teal-800 font-mono text-[11px] font-bold rounded">
                  JPR-01
                </span>
                <h3 className="text-base font-bold text-slate-900 mt-1">Jaipur Main Hospital</h3>
              </div>
              <Badge variant="success">Active Unit</Badge>
            </div>

            <p className="text-xs text-slate-500 flex items-center gap-1.5">
              <MapPin className="w-3.5 h-3.5 text-slate-400 shrink-0" />
              <span>Jaipur Main Road, Jaipur, Rajasthan 302001, India</span>
            </p>

            <div className="pt-4 border-t border-slate-100 grid grid-cols-2 gap-2 text-xs">
              <div>
                Phone: <strong className="text-slate-700">0141-111111</strong>
              </div>
              <div>
                Departments: <strong className="text-slate-700">8 Specialties</strong>
              </div>
              <div>
                Total Beds: <strong className="text-slate-700">40 Beds</strong>
              </div>
              <div>
                Doctors: <strong className="text-slate-700">12 Active</strong>
              </div>
            </div>

            <Button
              variant="outline"
              size="sm"
              className="w-full"
              onClick={() => {
                setActiveBranch('JPR-01');
                showToast('Switched active branch to Jaipur Main');
              }}
            >
              Set as Active Branch
            </Button>
          </CardContent>
        </Card>

        {/* Delhi Branch */}
        <Card className="hover:border-indigo-300 transition">
          <CardContent className="p-5 space-y-4">
            <div className="flex items-center justify-between">
              <div>
                <span className="px-2 py-0.5 bg-indigo-100 text-indigo-800 font-mono text-[11px] font-bold rounded">
                  DEL-01
                </span>
                <h3 className="text-base font-bold text-slate-900 mt-1">
                  Delhi Connaught Place Branch
                </h3>
              </div>
              <Badge variant="success">Active Unit</Badge>
            </div>

            <p className="text-xs text-slate-500 flex items-center gap-1.5">
              <MapPin className="w-3.5 h-3.5 text-slate-400 shrink-0" />
              <span>Inner Circle, Connaught Place, New Delhi 110001, India</span>
            </p>

            <div className="pt-4 border-t border-slate-100 grid grid-cols-2 gap-2 text-xs">
              <div>
                Phone: <strong className="text-slate-700">011-222222</strong>
              </div>
              <div>
                Departments: <strong className="text-slate-700">6 Specialties</strong>
              </div>
              <div>
                Total Beds: <strong className="text-slate-700">25 Beds</strong>
              </div>
              <div>
                Doctors: <strong className="text-slate-700">8 Active</strong>
              </div>
            </div>

            <Button
              variant="outline"
              size="sm"
              className="w-full"
              onClick={() => {
                setActiveBranch('DEL-01');
                showToast('Switched active branch to Delhi CP');
              }}
            >
              Set as Active Branch
            </Button>
          </CardContent>
        </Card>
      </div>
    </div>
  );
};

