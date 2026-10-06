// Domain models matching .NET 8 Backend Entities

export type UserRole =
  | 'PlatformSuperAdmin'
  | 'HospitalAdmin'
  | 'BranchAdmin'
  | 'Doctor'
  | 'Nurse'
  | 'Receptionist'
  | 'Pharmacist'
  | 'LabTechnician'
  | 'Accountant';

export interface AuthUser {
  id: string;
  email: string;
  fullName: string;
  role: UserRole;
  tenantId: string;
  tenantName: string;
  branchId: string;
  branchName: string;
  token: string;
}

export interface Tenant {
  id: string;
  tenantCode: string;
  name: string;
  legalName: string;
  email: string;
  phone: string;
  status: 'Active' | 'Suspended' | 'Inactive';
}

export interface Branch {
  id: string;
  tenantId: string;
  branchCode: string;
  name: string;
  address: string;
  city: string;
  state: string;
  country: string;
  pinCode: string;
  phone: string;
  email: string;
}

export interface Department {
  id: string;
  branchId: string;
  code: string;
  name: string;
}

export interface Doctor {
  id: string;
  doctorNo: string;
  firstName: string;
  lastName: string;
  specialization: string;
  qualification: string;
  consultationFee: number;
  phone: string;
  email: string;
}

export interface Patient {
  id: string;
  mrn: string;
  firstName: string;
  lastName: string;
  gender: 'Male' | 'Female' | 'Other';
  dob: string;
  age: number;
  phone: string;
  email: string;
  bloodGroup: string;
  address?: string;
  allergies?: string[];
  status?: string;
  lastVisit?: string;
}

export interface Appointment {
  id: string;
  appointmentNo: string;
  patientId: string;
  patientName: string;
  patientMrn: string;
  doctorId: string;
  doctorName: string;
  departmentName: string;
  appointmentDate: string;
  slotStartTime: string;
  slotEndTime: string;
  tokenNumber: number;
  status: 'Scheduled' | 'CheckedIn' | 'InProgress' | 'Completed' | 'Cancelled';
  consultationFee: number;
  reason?: string;
}

export interface Vitals {
  bloodPressure: string;
  pulseRate: number;
  temperatureDecimal: number;
  oxygenSaturation: number;
  respiratoryRate: number;
}

export interface PrescriptionItem {
  id: string;
  medicineCode: string;
  medicineName: string;
  dosage: string;
  frequency: string;
  durationDays: number;
  instructions: string;
}

export interface Consultation {
  id: string;
  patientId: string;
  doctorId: string;
  chiefComplaints: string;
  clinicalNotes: string;
  advice: string;
  followUpDate?: string;
  diagnoses: { icd10Code: string; name: string; isPrimary: boolean }[];
  prescriptions: PrescriptionItem[];
}

export type BedStatus = 'Available' | 'Occupied' | 'Maintenance';

export interface Bed {
  id: string;
  roomId: string;
  roomNumber: string;
  wardName: string;
  floor: string;
  bedNumber: string;
  dailyCharge: number;
  status: BedStatus;
  patientName?: string;
  patientMrn?: string;
  attendingDoctor?: string;
  admittedDate?: string;
}

export interface Medicine {
  id: string;
  medicineCode: string;
  name: string;
  genericName: string;
  category: string;
  unitPrice: number;
  stockQuantity: number;
  expiryDate: string;
}

export interface LabOrder {
  id: string;
  orderNumber: string;
  patientId: string;
  patientName: string;
  patientMrn: string;
  doctorName: string;
  testName: string;
  orderDate: string;
  status: 'Ordered' | 'SampleCollected' | 'Completed';
  resultSummary?: string;
}

export interface InvoiceItem {
  id: string;
  description: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
}

export interface Invoice {
  id: string;
  invoiceNumber: string;
  patientId: string;
  patientName: string;
  invoiceDate: string;
  subTotal: number;
  discountAmount: number;
  taxAmount: number;
  totalAmount: number;
  paidAmount: number;
  status: 'Unpaid' | 'PartiallyPaid' | 'Paid';
  items: InvoiceItem[];
}

