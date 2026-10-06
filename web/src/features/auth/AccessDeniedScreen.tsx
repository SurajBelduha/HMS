import React from 'react';
import { ShieldAlert, ArrowLeft, Lock } from 'lucide-react';
import { Card, CardContent } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import { useApp } from '../../context/AppContext';
import { DEFAULT_SCREEN_FOR_ROLE } from '../../config/rbac';

export const AccessDeniedScreen: React.FC<{ screenName: string }> = ({ screenName }) => {
  const { userRole, setCurrentScreen } = useApp();

  const handleReturn = () => {
    const defaultScreen = DEFAULT_SCREEN_FOR_ROLE[userRole] || 'dashboard';
    setCurrentScreen(defaultScreen);
  };

  return (
    <div className="min-h-[70vh] flex items-center justify-center p-4">
      <Card className="max-w-md w-full border-rose-200 shadow-xl overflow-hidden">
        <div className="bg-gradient-to-r from-rose-600 to-rose-700 p-6 text-white text-center">
          <div className="w-14 h-14 bg-white/10 rounded-2xl border border-white/20 flex items-center justify-center mx-auto mb-3">
            <ShieldAlert className="w-8 h-8 text-white" />
          </div>
          <h2 className="text-lg font-bold">Access Restricted (403 Forbidden)</h2>
          <p className="text-xs text-rose-100 mt-1">
            Clinical Governance & Role-Based Access Control (RBAC)
          </p>
        </div>

        <CardContent className="p-6 text-center space-y-4 text-xs">
          <p className="text-slate-600 leading-relaxed">
            Your current account role does not have authorization to view the{' '}
            <strong className="text-slate-900 font-mono font-bold capitalize">
              "{screenName}"
            </strong>{' '}
            module.
          </p>

          <div className="p-3 bg-slate-50 rounded-xl border border-slate-200 text-left space-y-1.5">
            <div className="flex items-center justify-between">
              <span className="text-slate-500">Your Assigned Role:</span>
              <Badge variant="primary">{userRole}</Badge>
            </div>
            <div className="flex items-center justify-between">
              <span className="text-slate-500">Policy:</span>
              <span className="font-semibold text-slate-700">Strict Least-Privilege</span>
            </div>
          </div>

          <Button
            variant="primary"
            size="md"
            icon={<ArrowLeft className="w-4 h-4" />}
            onClick={handleReturn}
            className="w-full"
          >
            Back to Authorized Workstation
          </Button>
        </CardContent>
      </Card>
    </div>
  );
};

