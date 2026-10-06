import React, { useState } from 'react';
import {
  Activity,
  Lock,
  Mail,
  Eye,
  EyeOff,
  ShieldCheck,
  ArrowRight,
  AlertCircle,
  Loader2,
  CheckCircle2,
} from 'lucide-react';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Card, CardContent } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { useApp } from '../../context/AppContext';
import { authService } from '../../services/authService';
import { DEMO_PRESETS, type DemoUserPreset } from '../../config/rbac';

export const LoginScreen: React.FC = () => {
  const { loginUser, showToast } = useApp();

  const [email, setEmail] = useState('dr.alice@abchealthcare.com');
  const [password, setPassword] = useState('Doctor@Pass2026!');
  const [showPassword, setShowPassword] = useState(false);
  const [rememberMe, setRememberMe] = useState(true);
  const [loading, setLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  // Field validation errors
  const [emailError, setEmailError] = useState<string | null>(null);
  const [passwordError, setPasswordError] = useState<string | null>(null);

  const validateEmail = (val: string) => {
    if (!val.trim()) {
      return 'Email address is required.';
    }
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!regex.test(val.trim())) {
      return 'Please enter a valid email address (e.g. name@hospital.com).';
    }
    return null;
  };

  const validatePassword = (val: string) => {
    if (!val) {
      return 'Password is required.';
    }
    if (val.length < 6) {
      return 'Password must be at least 6 characters.';
    }
    return null;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setErrorMessage(null);

    const emailErr = validateEmail(email);
    const passErr = validatePassword(password);
    setEmailError(emailErr);
    setPasswordError(passErr);

    if (emailErr || passErr) {
      return;
    }

    setLoading(true);
    try {
      const user = await authService.login(email, password);
      loginUser(user);
    } catch (err: unknown) {
      const errorMsg =
        err instanceof Error ? err.message : 'Invalid credentials. Please verify your email and password.';
      setErrorMessage(errorMsg);
    } finally {
      setLoading(false);
    }
  };

  const handleSelectPreset = async (preset: DemoUserPreset) => {
    setEmail(preset.email);
    setPassword('DemoPass@2026!');
    setEmailError(null);
    setPasswordError(null);
    setErrorMessage(null);

    // Auto-login on click for instant testing
    setLoading(true);
    try {
      const user = await authService.login(preset.email, 'DemoPass@2026!');
      loginUser(user);
    } catch {
      setErrorMessage('Failed to sign in with demo preset.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-teal-950 to-slate-900 text-slate-100 flex flex-col justify-center items-center p-4 selection:bg-teal-500 selection:text-white">
      {/* Background ambient lighting */}
      <div className="absolute top-1/4 left-1/4 w-96 h-96 bg-teal-500/10 rounded-full blur-3xl pointer-events-none" />
      <div className="absolute bottom-1/4 right-1/4 w-96 h-96 bg-indigo-500/10 rounded-full blur-3xl pointer-events-none" />

      <div className="w-full max-w-4xl grid grid-cols-1 lg:grid-cols-12 gap-8 items-center z-10">
        {/* Left Side: Brand Story & Security Highlights */}
        <div className="lg:col-span-5 space-y-6 text-center lg:text-left">
          <div className="flex items-center justify-center lg:justify-start gap-3">
            <div className="w-12 h-12 rounded-2xl bg-gradient-to-tr from-teal-500 to-teal-400 flex items-center justify-center text-slate-900 shadow-xl shadow-teal-500/25">
              <Activity className="w-7 h-7 stroke-[2.5]" />
            </div>
            <div>
              <div className="flex items-center gap-2">
                <span className="text-2xl font-black tracking-tight text-white">HealthCore</span>
                <span className="px-2 py-0.5 text-xs font-bold bg-teal-400/20 text-teal-300 rounded border border-teal-400/30">
                  HMS v1.0
                </span>
              </div>
              <p className="text-xs text-slate-400">Enterprise Healthcare SaaS</p>
            </div>
          </div>

          <div className="space-y-3">
            <h1 className="text-2xl sm:text-3xl font-extrabold text-white tracking-tight leading-tight">
              Hospital Operations & Clinical EMR Portal
            </h1>
            <p className="text-xs text-slate-300 leading-relaxed">
              Secure multi-tenant hospital workspace. Access is dynamically restricted based on
              assigned clinical, administrative, or financial roles.
            </p>
          </div>

          <div className="pt-2 space-y-2.5 text-xs text-slate-300 hidden sm:block">
            <div className="flex items-center gap-2.5">
              <ShieldCheck className="w-4 h-4 text-teal-400 shrink-0" />
              <span>Role-Based Access Control (RBAC) Enforced</span>
            </div>
            <div className="flex items-center gap-2.5">
              <CheckCircle2 className="w-4 h-4 text-teal-400 shrink-0" />
              <span>Multi-Tenant Header Isolation (`X-Tenant-Id`)</span>
            </div>
            <div className="flex items-center gap-2.5">
              <Lock className="w-4 h-4 text-teal-400 shrink-0" />
              <span>256-Bit Encrypted JWT Authentication</span>
            </div>
          </div>
        </div>

        {/* Right Side: Login Card & Demo Role Selectors */}
        <div className="lg:col-span-7 space-y-4">
          <div className="bg-white text-slate-900 rounded-2xl shadow-2xl border border-slate-100 p-6 sm:p-8">
            <div className="mb-6">
              <h2 className="text-lg font-bold text-slate-900 tracking-tight">
                Sign in to your account
              </h2>
              <p className="text-xs text-slate-500 mt-1">
                Enter your hospital credentials to access your assigned workstation.
              </p>
            </div>

            {/* Error Alert Banner */}
            {errorMessage && (
              <div className="mb-4 p-3 bg-rose-50 border border-rose-200 rounded-xl text-xs text-rose-800 flex items-start gap-2.5">
                <AlertCircle className="w-4 h-4 text-rose-600 shrink-0 mt-0.5" />
                <span>{errorMessage}</span>
              </div>
            )}

            <form onSubmit={handleSubmit} className="space-y-4 text-xs">
              {/* Email Input */}
              <div className="space-y-1">
                <label className="block text-xs font-semibold text-slate-700">
                  Email Address *
                </label>
                <div className="relative">
                  <Mail className="w-4 h-4 text-slate-400 absolute left-3 top-2.5" />
                  <input
                    type="email"
                    required
                    placeholder="doctor@hospital.com"
                    value={email}
                    onChange={(e) => {
                      setEmail(e.target.value);
                      if (emailError) setEmailError(null);
                    }}
                    className={`w-full pl-9 pr-3 py-2 bg-slate-50 border rounded-lg text-xs transition focus:bg-white focus:outline-hidden focus:ring-2 ${
                      emailError
                        ? 'border-rose-400 focus:ring-rose-200 bg-rose-50/40'
                        : 'border-slate-200 focus:border-teal-500 focus:ring-teal-100'
                    }`}
                  />
                </div>
                {emailError && <p className="text-[11px] text-rose-600 font-medium">{emailError}</p>}
              </div>

              {/* Password Input */}
              <div className="space-y-1">
                <div className="flex items-center justify-between">
                  <label className="block text-xs font-semibold text-slate-700">Password *</label>
                  <button
                    type="button"
                    onClick={() => showToast('Password reset link dispatched to registered email.')}
                    className="text-[11px] text-teal-600 hover:text-teal-800 font-semibold cursor-pointer"
                  >
                    Forgot password?
                  </button>
                </div>
                <div className="relative">
                  <Lock className="w-4 h-4 text-slate-400 absolute left-3 top-2.5" />
                  <input
                    type={showPassword ? 'text' : 'password'}
                    required
                    placeholder="••••••••••••"
                    value={password}
                    onChange={(e) => {
                      setPassword(e.target.value);
                      if (passwordError) setPasswordError(null);
                    }}
                    className={`w-full pl-9 pr-10 py-2 bg-slate-50 border rounded-lg text-xs transition focus:bg-white focus:outline-hidden focus:ring-2 ${
                      passwordError
                        ? 'border-rose-400 focus:ring-rose-200 bg-rose-50/40'
                        : 'border-slate-200 focus:border-teal-500 focus:ring-teal-100'
                    }`}
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="absolute right-3 top-2.5 text-slate-400 hover:text-slate-600 cursor-pointer"
                  >
                    {showPassword ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                  </button>
                </div>
                {passwordError && (
                  <p className="text-[11px] text-rose-600 font-medium">{passwordError}</p>
                )}
              </div>

              {/* Remember Me */}
              <div className="flex items-center justify-between text-xs text-slate-600">
                <label className="flex items-center gap-2 cursor-pointer select-none">
                  <input
                    type="checkbox"
                    checked={rememberMe}
                    onChange={(e) => setRememberMe(e.target.checked)}
                    className="rounded text-teal-600 focus:ring-teal-500"
                  />
                  <span>Keep me logged in for 30 days</span>
                </label>
              </div>

              {/* Submit Button */}
              <Button
                variant="primary"
                size="lg"
                type="submit"
                disabled={loading}
                className="w-full text-xs font-bold py-2.5 shadow-md shadow-teal-600/20"
                icon={
                  loading ? (
                    <Loader2 className="w-4 h-4 animate-spin" />
                  ) : (
                    <ArrowRight className="w-4 h-4" />
                  )
                }
              >
                {loading ? 'Authenticating & Loading Workstation...' : 'Sign In to Medical Portal'}
              </Button>
            </form>
          </div>

          {/* Quick 1-Click Role Login Bar */}
          <div className="bg-slate-800/80 backdrop-blur-md rounded-2xl border border-slate-700/60 p-4 text-xs">
            <div className="flex items-center justify-between mb-2.5">
              <span className="font-bold text-teal-300 flex items-center gap-1.5">
                <span>⚡ Quick Demo Login (Role-Based Menu Test)</span>
              </span>
              <span className="text-[11px] text-slate-400">Click any role to test menus</span>
            </div>

            <div className="grid grid-cols-2 sm:grid-cols-4 gap-2">
              {DEMO_PRESETS.map((preset) => (
                <button
                  key={preset.role}
                  type="button"
                  disabled={loading}
                  onClick={() => handleSelectPreset(preset)}
                  className="p-2 bg-slate-900/80 hover:bg-teal-900/50 border border-slate-700 hover:border-teal-500/60 rounded-xl text-left transition cursor-pointer group"
                >
                  <div className="flex items-center gap-1.5 text-xs font-semibold text-slate-200 group-hover:text-teal-300">
                    <span>{preset.icon}</span>
                    <span className="truncate">{preset.label}</span>
                  </div>
                  <div className="text-[10px] text-slate-400 truncate mt-0.5">
                    {preset.role === 'Doctor'
                      ? 'OPD, e-Rx, EMR'
                      : preset.role === 'Pharmacist'
                      ? 'Pharmacy Only'
                      : preset.role === 'Accountant'
                      ? 'Billing Only'
                      : preset.role === 'Nurse'
                      ? 'IPD Beds, Queue'
                      : 'Assigned Menus'}
                  </div>
                </button>
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

