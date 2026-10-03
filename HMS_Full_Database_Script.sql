-- ====================================================================================
-- MULTI-TENANT SAAS HOSPITAL MANAGEMENT SYSTEM (HMS) - FULL DATABASE SCHEMA & SEED SCRIPT
-- Drops existing DB, Creates Schema, and Seeds 2 Sample Records Per Table + Super Admin
-- ====================================================================================

USE master;
GO

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'HMS')
BEGIN
    ALTER DATABASE HMS SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE HMS;
END
GO

CREATE DATABASE HMS;
GO

USE HMS;
GO

-- ==========================================
-- PHASE 1: FOUNDATION & MULTI-TENANCY
-- ==========================================

-- 1. Tenants (Hospital Organizations)
CREATE TABLE Tenants (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantCode NVARCHAR(50) NOT NULL UNIQUE,
    Name NVARCHAR(150) NOT NULL,
    LegalName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    TimeZone NVARCHAR(50) DEFAULT 'UTC' NOT NULL,
    Currency NVARCHAR(10) DEFAULT 'USD' NOT NULL,
    Country NVARCHAR(100) DEFAULT 'USA' NOT NULL,
    Status NVARCHAR(30) DEFAULT 'Active' NOT NULL,
    Subdomain NVARCHAR(100) NULL,
    CustomDomain NVARCHAR(150) NULL,
    DedicatedConnectionString NVARCHAR(500) NULL,
    IsolationStrategy INT DEFAULT 0 NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    IsDeleted BIT DEFAULT 0 NOT NULL,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(100) NULL
);

-- 2. Branches
CREATE TABLE Branches (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Tenants(Id),
    BranchCode NVARCHAR(50) NOT NULL,
    Name NVARCHAR(150) NOT NULL,
    Address NVARCHAR(250) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    State NVARCHAR(100) NOT NULL,
    Country NVARCHAR(100) NOT NULL,
    PinCode NVARCHAR(20) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Status NVARCHAR(30) DEFAULT 'Active' NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    IsDeleted BIT DEFAULT 0 NOT NULL,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(100) NULL
);

-- 3. TenantSettings
CREATE TABLE TenantSettings (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Tenants(Id),
    DateFormat NVARCHAR(30) DEFAULT 'yyyy-MM-dd' NOT NULL,
    TimeZone NVARCHAR(50) DEFAULT 'UTC' NOT NULL,
    Currency NVARCHAR(10) DEFAULT 'USD' NOT NULL,
    Language NVARCHAR(10) DEFAULT 'en' NOT NULL,
    TaxNumber NVARCHAR(50) NULL,
    LogoUrl NVARCHAR(500) NULL,
    InvoicePrefix NVARCHAR(20) DEFAULT 'INV-' NOT NULL,
    PatientPrefix NVARCHAR(20) DEFAULT 'PAT-' NOT NULL,
    BillingSettingsJson NVARCHAR(MAX) NULL,
    NotificationSettingsJson NVARCHAR(MAX) NULL,
    SecuritySettingsJson NVARCHAR(MAX) NULL,
    FeatureSettingsJson NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL
);

-- 4. Departments
CREATE TABLE Departments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Tenants(Id),
    BranchId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Branches(Id),
    Name NVARCHAR(100) NOT NULL,
    Code NVARCHAR(30) NOT NULL,
    Description NVARCHAR(250) NULL,
    IsActive BIT DEFAULT 1 NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    IsDeleted BIT DEFAULT 0 NOT NULL,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(100) NULL
);

-- 5. Users
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Tenants(Id),
    BranchId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Branches(Id),
    DepartmentId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Departments(Id),
    Email NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Role NVARCHAR(50) DEFAULT 'User' NOT NULL,
    Status NVARCHAR(30) DEFAULT 'Active' NOT NULL,
    LastLoginAt DATETIME2 NULL,
    EmailVerified BIT DEFAULT 0 NOT NULL,
    AccessFailedCount INT DEFAULT 0 NOT NULL,
    LockoutEnd DATETIME2 NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    IsDeleted BIT DEFAULT 0 NOT NULL,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(100) NULL
);

-- 6. Roles & Permissions
CREATE TABLE Roles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Tenants(Id),
    Name NVARCHAR(100) NOT NULL,
    Code NVARCHAR(50) NOT NULL,
    IsSystemRole BIT DEFAULT 0 NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL
);

CREATE TABLE Permissions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Module NVARCHAR(50) NOT NULL,
    Code NVARCHAR(100) NOT NULL UNIQUE,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE UserRoles (
    UserId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Users(Id),
    RoleId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Roles(Id),
    PRIMARY KEY (UserId, RoleId)
);

CREATE TABLE RolePermissions (
    RoleId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Roles(Id),
    PermissionId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Permissions(Id),
    PRIMARY KEY (RoleId, PermissionId)
);

CREATE TABLE RefreshTokens (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Users(Id),
    Token NVARCHAR(500) NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    IsRevoked BIT DEFAULT 0 NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE() NOT NULL
);

CREATE TABLE AuditLogs (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    BranchId UNIQUEIDENTIFIER NULL,
    UserId UNIQUEIDENTIFIER NULL,
    Action NVARCHAR(100) NOT NULL,
    Module NVARCHAR(100) NOT NULL,
    EntityName NVARCHAR(100) NOT NULL,
    EntityId NVARCHAR(100) NOT NULL,
    OldValues NVARCHAR(MAX) NULL,
    NewValues NVARCHAR(MAX) NULL,
    IPAddress NVARCHAR(50) NULL,
    UserAgent NVARCHAR(500) NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE() NOT NULL
);


-- ==========================================
-- PHASE 2: CORE HOSPITAL OPERATIONS
-- ==========================================

CREATE TABLE Patients (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Tenants(Id),
    BranchId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Branches(Id),
    MRN NVARCHAR(50) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Gender NVARCHAR(20) NOT NULL,
    DOB DATE NOT NULL,
    BloodGroup NVARCHAR(10) NULL,
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) NULL,
    Status NVARCHAR(30) DEFAULT 'Active' NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    IsDeleted BIT DEFAULT 0 NOT NULL,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(100) NULL
);

CREATE TABLE PatientAddresses (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Patients(Id),
    AddressType NVARCHAR(30) DEFAULT 'Home' NOT NULL,
    AddressLine1 NVARCHAR(200) NOT NULL,
    AddressLine2 NVARCHAR(200) NULL,
    City NVARCHAR(100) NOT NULL,
    State NVARCHAR(100) NOT NULL,
    Country NVARCHAR(100) NOT NULL,
    PinCode NVARCHAR(20) NOT NULL
);

CREATE TABLE PatientContacts (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Patients(Id),
    ContactName NVARCHAR(150) NOT NULL,
    Relationship NVARCHAR(50) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) NULL,
    IsEmergencyContact BIT DEFAULT 0 NOT NULL
);

CREATE TABLE PatientDocuments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    PatientId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Patients(Id),
    DocumentType NVARCHAR(50) NOT NULL,
    DocumentName NVARCHAR(150) NOT NULL,
    FileUrl NVARCHAR(500) NOT NULL,
    UploadedAt DATETIME2 DEFAULT GETUTCDATE() NOT NULL
);

CREATE TABLE Doctors (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Tenants(Id),
    BranchId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Branches(Id),
    UserId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Users(Id),
    DoctorNo NVARCHAR(50) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Specialization NVARCHAR(100) NOT NULL,
    Qualification NVARCHAR(100) NOT NULL,
    RegistrationNumber NVARCHAR(50) NOT NULL,
    ConsultationFee DECIMAL(18,2) DEFAULT 0 NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Status NVARCHAR(30) DEFAULT 'Active' NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    IsDeleted BIT DEFAULT 0 NOT NULL,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(100) NULL
);

CREATE TABLE DoctorDepartments (
    DoctorId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Doctors(Id),
    DepartmentId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Departments(Id),
    PRIMARY KEY (DoctorId, DepartmentId)
);

CREATE TABLE DoctorSchedules (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Tenants(Id),
    BranchId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Branches(Id),
    DoctorId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Doctors(Id),
    DayOfWeek INT NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    SlotDurationMinutes INT DEFAULT 15 NOT NULL,
    MaxPatients INT DEFAULT 20 NOT NULL,
    IsAvailable BIT DEFAULT 1 NOT NULL
);

CREATE TABLE HospitalServices (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Tenants(Id),
    BranchId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Branches(Id),
    DepartmentId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Departments(Id),
    ServiceCode NVARCHAR(50) NOT NULL,
    ServiceName NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500) NULL,
    Price DECIMAL(18,2) NOT NULL,
    IsActive BIT DEFAULT 1 NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL
);

CREATE TABLE Appointments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Tenants(Id),
    BranchId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Branches(Id),
    AppointmentNo NVARCHAR(50) NOT NULL,
    PatientId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Patients(Id),
    DoctorId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Doctors(Id),
    DepartmentId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Departments(Id),
    ServiceId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES HospitalServices(Id),
    AppointmentDate DATE NOT NULL,
    SlotStartTime TIME NOT NULL,
    SlotEndTime TIME NOT NULL,
    TokenNumber INT NOT NULL,
    Status NVARCHAR(30) DEFAULT 'Scheduled' NOT NULL,
    Reason NVARCHAR(500) NULL,
    ConsultationFee DECIMAL(18,2) DEFAULT 0 NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    IsDeleted BIT DEFAULT 0 NOT NULL,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(100) NULL
);


-- ==========================================
-- PHASE 3: CLINICAL OPERATIONS
-- ==========================================

CREATE TABLE Consultations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Tenants(Id),
    BranchId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Branches(Id),
    AppointmentId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Appointments(Id),
    PatientId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Patients(Id),
    DoctorId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Doctors(Id),
    ConsultationDate DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    ChiefComplaints NVARCHAR(MAX) NULL,
    ClinicalNotes NVARCHAR(MAX) NULL,
    Advice NVARCHAR(MAX) NULL,
    FollowUpDate DATE NULL,
    Status NVARCHAR(30) DEFAULT 'Completed' NOT NULL
);

CREATE TABLE Diagnoses (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ConsultationId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Consultations(Id),
    ICD10Code NVARCHAR(30) NULL,
    DiagnosisName NVARCHAR(250) NOT NULL,
    Type NVARCHAR(30) DEFAULT 'Primary' NOT NULL
);

CREATE TABLE Prescriptions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Tenants(Id),
    ConsultationId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Consultations(Id),
    PatientId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Patients(Id),
    DoctorId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Doctors(Id),
    PrescriptionDate DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    Instructions NVARCHAR(MAX) NULL
);

CREATE TABLE PrescriptionItems (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PrescriptionId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Prescriptions(Id),
    MedicineName NVARCHAR(150) NOT NULL,
    Dosage NVARCHAR(50) NOT NULL,
    Frequency NVARCHAR(50) NOT NULL,
    DurationDays INT NOT NULL,
    Instructions NVARCHAR(250) NULL
);

CREATE TABLE Wards (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Tenants(Id),
    BranchId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Branches(Id),
    WardName NVARCHAR(100) NOT NULL,
    WardType NVARCHAR(50) NOT NULL,
    Floor NVARCHAR(30) NOT NULL
);

CREATE TABLE Rooms (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    WardId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Wards(Id),
    RoomNumber NVARCHAR(30) NOT NULL,
    RoomType NVARCHAR(50) NOT NULL
);

CREATE TABLE Beds (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RoomId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Rooms(Id),
    BedNumber NVARCHAR(30) NOT NULL,
    DailyCharge DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(30) DEFAULT 'Available' NOT NULL
);

CREATE TABLE Admissions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Tenants(Id),
    BranchId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Branches(Id),
    AdmissionNo NVARCHAR(50) NOT NULL,
    PatientId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Patients(Id),
    AttendingDoctorId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Doctors(Id),
    BedId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Beds(Id),
    AdmissionDate DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    DischargeDate DATETIME2 NULL,
    Status NVARCHAR(30) DEFAULT 'Admitted' NOT NULL,
    ReasonForAdmission NVARCHAR(MAX) NULL
);

CREATE TABLE Discharges (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AdmissionId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Admissions(Id),
    DischargeDate DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    DischargeSummary NVARCHAR(MAX) NOT NULL,
    ConditionAtDischarge NVARCHAR(100) NOT NULL
);

CREATE TABLE Vitals (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Tenants(Id),
    PatientId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Patients(Id),
    AdmissionId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Admissions(Id),
    RecordedAt DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    BloodPressure NVARCHAR(20) NULL,
    PulseRate INT NULL,
    TemperatureDecimal DECIMAL(5,2) NULL,
    OxygenSaturation INT NULL,
    RespiratoryRate INT NULL
);


-- ==========================================
-- PHASE 4: REVENUE, PHARMACY & LAB
-- ==========================================

CREATE TABLE Invoices (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Tenants(Id),
    BranchId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Branches(Id),
    InvoiceNumber NVARCHAR(50) NOT NULL,
    PatientId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Patients(Id),
    SubTotal DECIMAL(18,2) NOT NULL,
    DiscountAmount DECIMAL(18,2) DEFAULT 0 NOT NULL,
    TaxAmount DECIMAL(18,2) DEFAULT 0 NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    PaidAmount DECIMAL(18,2) DEFAULT 0 NOT NULL,
    Status NVARCHAR(30) DEFAULT 'Unpaid' NOT NULL,
    InvoiceDate DATETIME2 DEFAULT GETUTCDATE() NOT NULL
);

CREATE TABLE InvoiceItems (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    InvoiceId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Invoices(Id),
    ItemDescription NVARCHAR(250) NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    Quantity INT DEFAULT 1 NOT NULL,
    TotalPrice DECIMAL(18,2) NOT NULL
);

CREATE TABLE Payments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    InvoiceId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Invoices(Id),
    PaymentNumber NVARCHAR(50) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    PaymentMethod NVARCHAR(50) NOT NULL,
    PaymentDate DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    TransactionRef NVARCHAR(100) NULL
);

CREATE TABLE Medicines (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Tenants(Id),
    MedicineCode NVARCHAR(50) NOT NULL,
    Name NVARCHAR(150) NOT NULL,
    GenericName NVARCHAR(150) NULL,
    Category NVARCHAR(100) NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    StockQuantity INT DEFAULT 0 NOT NULL,
    ExpiryDate DATE NOT NULL
);

CREATE TABLE LabOrders (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Tenants(Id),
    PatientId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Patients(Id),
    DoctorId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Doctors(Id),
    TestName NVARCHAR(150) NOT NULL,
    Status NVARCHAR(30) DEFAULT 'Ordered' NOT NULL,
    ResultSummary NVARCHAR(MAX) NULL,
    OrderDate DATETIME2 DEFAULT GETUTCDATE() NOT NULL
);


-- ==========================================
-- PHASE 5: SAAS SUBSCRIPTION PLATFORM
-- ==========================================

CREATE TABLE Plans (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Code NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(500) NOT NULL,
    PriceMonthly DECIMAL(18,2) NOT NULL,
    PriceYearly DECIMAL(18,2) NOT NULL,
    TrialDays INT DEFAULT 14 NOT NULL,
    Status NVARCHAR(30) DEFAULT 'Active' NOT NULL
);

CREATE TABLE PlanFeatures (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PlanId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Plans(Id),
    FeatureCode NVARCHAR(100) NOT NULL,
    LimitType NVARCHAR(30) DEFAULT 'Numeric' NOT NULL,
    LimitValue NVARCHAR(50) NOT NULL
);

CREATE TABLE TenantSubscriptions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Tenants(Id),
    PlanId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Plans(Id),
    Status NVARCHAR(30) DEFAULT 'Active' NOT NULL,
    StartDate DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    EndDate DATETIME2 NOT NULL,
    TrialEndDate DATETIME2 NULL,
    NextBillingDate DATETIME2 NULL,
    AutoRenew BIT DEFAULT 1 NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL
);

CREATE TABLE SubscriptionItems (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SubscriptionId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES TenantSubscriptions(Id),
    FeatureCode NVARCHAR(100) NOT NULL,
    Quantity INT DEFAULT 1 NOT NULL,
    Price DECIMAL(18,2) DEFAULT 0 NOT NULL
);
GO


-- ====================================================================================
-- SEED DATA INSERTS (2 RECORDS FOR ALL TABLES + PLATFORM SUPER ADMIN)
-- ====================================================================================

-- 1. Tenants (2 Tenants + 1 Host Tenant)
INSERT INTO Tenants (Id, TenantCode, Name, LegalName, Email, Phone, Status)
VALUES ('11111111-1111-1111-1111-111111111111', 'PLATFORM-HOST', 'HMS SaaS Host Platform', 'HMS Global SaaS Inc', 'host@hmssaas.com', '1800-HMS-HOST', 'Active');

INSERT INTO Tenants (Id, TenantCode, Name, LegalName, Email, Phone, Status)
VALUES ('44444444-4444-4444-4444-444444444444', 'ABC-HC', 'ABC Healthcare', 'ABC Healthcare Ltd', 'contact@abchealthcare.com', '9876543210', 'Active');

INSERT INTO Tenants (Id, TenantCode, Name, LegalName, Email, Phone, Status)
VALUES ('55555555-5555-5555-5555-555555555555', 'CITY-CARE', 'City Care Hospital', 'City Care Hospital Pvt Ltd', 'info@citycare.com', '9876543211', 'Active');

-- 2. Branches (2 Records)
INSERT INTO Branches (Id, TenantId, BranchCode, Name, Address, City, State, Country, PinCode, Phone, Email, Status)
VALUES ('66666666-6666-6666-6666-666666666666', '44444444-4444-4444-4444-444444444444', 'JPR-01', 'Jaipur Main Branch', 'Jaipur Main Rd', 'Jaipur', 'Rajasthan', 'India', '302001', '0141-111111', 'jaipur@abchealthcare.com', 'Active');

INSERT INTO Branches (Id, TenantId, BranchCode, Name, Address, City, State, Country, PinCode, Phone, Email, Status)
VALUES ('77777777-7777-7777-7777-777777777777', '55555555-5555-5555-5555-555555555555', 'DEL-01', 'Delhi Branch', 'Connaught Place', 'Delhi', 'Delhi', 'India', '110001', '011-222222', 'delhi@citycare.com', 'Active');

-- 3. TenantSettings (2 Records)
INSERT INTO TenantSettings (Id, TenantId, DateFormat, TimeZone, Currency, InvoicePrefix, PatientPrefix)
VALUES (NEWID(), '44444444-4444-4444-4444-444444444444', 'yyyy-MM-dd', 'UTC', 'INR', 'ABC-INV-', 'ABC-PAT-');

INSERT INTO TenantSettings (Id, TenantId, DateFormat, TimeZone, Currency, InvoicePrefix, PatientPrefix)
VALUES (NEWID(), '55555555-5555-5555-5555-555555555555', 'yyyy-MM-dd', 'UTC', 'INR', 'CC-INV-', 'CC-PAT-');

-- 4. Departments (2 Records)
INSERT INTO Departments (Id, TenantId, BranchId, Name, Code, Description)
VALUES ('88888888-8888-8888-8888-888888888888', '44444444-4444-4444-4444-444444444444', '66666666-6666-6666-6666-666666666666', 'Cardiology', 'CARD', 'Heart Care Department');

INSERT INTO Departments (Id, TenantId, BranchId, Name, Code, Description)
VALUES ('99999999-9999-9999-9999-999999999999', '55555555-5555-5555-5555-555555555555', '77777777-7777-7777-7777-777777777777', 'Neurology', 'NEURO', 'Brain and Nerve Care Department');

-- 5. Roles & Users (Super Admin + Hospital Owners)
INSERT INTO Roles (Id, TenantId, Name, Code, IsSystemRole)
VALUES ('22222222-2222-2222-2222-222222222222', '11111111-1111-1111-1111-111111111111', 'Platform Super Admin', 'PlatformSuperAdmin', 1);

INSERT INTO Roles (Id, TenantId, Name, Code, IsSystemRole)
VALUES ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '44444444-4444-4444-4444-444444444444', 'Hospital Owner', 'HospitalOwner', 1);

INSERT INTO Users (Id, TenantId, Email, Phone, PasswordHash, FirstName, LastName, Role, Status, EmailVerified)
VALUES ('33333333-3333-3333-3333-333333333333', '11111111-1111-1111-1111-111111111111', 'superadmin@hmssaas.com', '1800-HMS-ADMIN', '8y6Z0f/H4T4X21W3G+G1gZ6M+vE=', 'Platform', 'SuperAdmin', 'PlatformSuperAdmin', 'Active', 1);

INSERT INTO UserRoles (UserId, RoleId) VALUES ('33333333-3333-3333-3333-333333333333', '22222222-2222-2222-2222-222222222222');

INSERT INTO Users (Id, TenantId, BranchId, Email, Phone, PasswordHash, FirstName, LastName, Role, Status, EmailVerified)
VALUES ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '44444444-4444-4444-4444-444444444444', '66666666-6666-6666-6666-666666666666', 'admin@abchealthcare.com', '9876543210', '8y6Z0f/H4T4X21W3G+G1gZ6M+vE=', 'Hospital', 'Owner', 'HospitalOwner', 'Active', 1);

INSERT INTO UserRoles (UserId, RoleId) VALUES ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa');

-- 6. Permissions (2 Records)
INSERT INTO Permissions (Id, Module, Code, Name) VALUES (NEWID(), 'Patient', 'Patient.Create', 'Register New Patient');
INSERT INTO Permissions (Id, Module, Code, Name) VALUES (NEWID(), 'Patient', 'Patient.View', 'View Patient Profile');

-- 7. SaaS Plans & Features (2 Records)
INSERT INTO Plans (Id, Name, Code, Description, PriceMonthly, PriceYearly, TrialDays, Status)
VALUES ('cccccccc-cccc-cccc-cccc-cccccccccccc', 'Starter Plan', 'STARTER', 'For Small Clinics and Hospitals', 99.00, 990.00, 14, 'Active');

INSERT INTO Plans (Id, Name, Code, Description, PriceMonthly, PriceYearly, TrialDays, Status)
VALUES ('dddddddd-dddd-dddd-dddd-dddddddddddd', 'Enterprise Plan', 'ENTERPRISE', 'For Large Multi-Branch Hospitals', 499.00, 4990.00, 30, 'Active');

INSERT INTO PlanFeatures (Id, PlanId, FeatureCode, LimitType, LimitValue) VALUES (NEWID(), 'cccccccc-cccc-cccc-cccc-cccccccccccc', 'MaxBranches', 'Numeric', '2');
INSERT INTO PlanFeatures (Id, PlanId, FeatureCode, LimitType, LimitValue) VALUES (NEWID(), 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'MaxBranches', 'Unlimited', '-1');

-- 8. Tenant Subscriptions (2 Records)
INSERT INTO TenantSubscriptions (Id, TenantId, PlanId, Status, StartDate, EndDate)
VALUES (NEWID(), '44444444-4444-4444-4444-444444444444', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 'Active', GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE()));

INSERT INTO TenantSubscriptions (Id, TenantId, PlanId, Status, StartDate, EndDate)
VALUES (NEWID(), '55555555-5555-5555-5555-555555555555', 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'Active', GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE()));

-- 9. Patients (2 Records)
INSERT INTO Patients (Id, TenantId, BranchId, MRN, FirstName, LastName, Gender, DOB, Phone, Email, Status)
VALUES ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', '44444444-4444-4444-4444-444444444444', '66666666-6666-6666-6666-666666666666', 'PAT-2026-00001', 'John', 'Doe', 'Male', '1990-01-01', '9876543210', 'john@example.com', 'Active');

INSERT INTO Patients (Id, TenantId, BranchId, MRN, FirstName, LastName, Gender, DOB, Phone, Email, Status)
VALUES ('ffffffff-ffff-ffff-ffff-ffffffffffff', '55555555-5555-5555-5555-555555555555', '77777777-7777-7777-7777-777777777777', 'PAT-2026-00002', 'Jane', 'Smith', 'Female', '1992-02-02', '9876543211', 'jane@example.com', 'Active');

-- 10. PatientAddresses (2 Records)
INSERT INTO PatientAddresses (Id, PatientId, AddressType, AddressLine1, City, State, Country, PinCode)
VALUES (NEWID(), 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'Home', '123 Main Street', 'Jaipur', 'Rajasthan', 'India', '302001');

INSERT INTO PatientAddresses (Id, PatientId, AddressType, AddressLine1, City, State, Country, PinCode)
VALUES (NEWID(), 'ffffffff-ffff-ffff-ffff-ffffffffffff', 'Home', '456 MG Road', 'Delhi', 'Delhi', 'India', '110001');

-- 11. PatientContacts (2 Records)
INSERT INTO PatientContacts (Id, PatientId, ContactName, Relationship, Phone, IsEmergencyContact)
VALUES (NEWID(), 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'Mary Doe', 'Spouse', '9876543299', 1);

INSERT INTO PatientContacts (Id, PatientId, ContactName, Relationship, Phone, IsEmergencyContact)
VALUES (NEWID(), 'ffffffff-ffff-ffff-ffff-ffffffffffff', 'Tom Smith', 'Brother', '9876543288', 1);

-- 12. Doctors (2 Records)
INSERT INTO Doctors (Id, TenantId, BranchId, DoctorNo, FirstName, LastName, Specialization, Qualification, RegistrationNumber, ConsultationFee, Phone, Email, Status)
VALUES ('10101010-1010-1010-1010-101010101010', '44444444-4444-4444-4444-444444444444', '66666666-6666-6666-6666-666666666666', 'DOC-001', 'Alice', 'Smith', 'Cardiology', 'MD', 'REG-101', 500.00, '1112223333', 'dr.alice@abchealthcare.com', 'Active');

INSERT INTO DoctorDepartments (DoctorId, DepartmentId) VALUES ('10101010-1010-1010-1010-101010101010', '88888888-8888-8888-8888-888888888888');

INSERT INTO Doctors (Id, TenantId, BranchId, DoctorNo, FirstName, LastName, Specialization, Qualification, RegistrationNumber, ConsultationFee, Phone, Email, Status)
VALUES ('20202020-2020-2020-2020-202020202020', '55555555-5555-5555-5555-555555555555', '77777777-7777-7777-7777-777777777777', 'DOC-002', 'Robert', 'Brown', 'Neurology', 'DM', 'REG-102', 800.00, '2223334444', 'dr.robert@citycare.com', 'Active');

INSERT INTO DoctorDepartments (DoctorId, DepartmentId) VALUES ('20202020-2020-2020-2020-202020202020', '99999999-9999-9999-9999-999999999999');

-- 13. DoctorSchedules (2 Records)
INSERT INTO DoctorSchedules (Id, TenantId, BranchId, DoctorId, DayOfWeek, StartTime, EndTime, SlotDurationMinutes, MaxPatients)
VALUES (NEWID(), '44444444-4444-4444-4444-444444444444', '66666666-6666-6666-6666-666666666666', '10101010-1010-1010-1010-101010101010', 1, '09:00:00', '13:00:00', 15, 20);

INSERT INTO DoctorSchedules (Id, TenantId, BranchId, DoctorId, DayOfWeek, StartTime, EndTime, SlotDurationMinutes, MaxPatients)
VALUES (NEWID(), '55555555-5555-5555-5555-555555555555', '77777777-7777-7777-7777-777777777777', '20202020-2020-2020-2020-202020202020', 2, '10:00:00', '14:00:00', 15, 20);

-- 14. HospitalServices (2 Records)
INSERT INTO HospitalServices (Id, TenantId, BranchId, DepartmentId, ServiceCode, ServiceName, Price)
VALUES (NEWID(), '44444444-4444-4444-4444-444444444444', '66666666-6666-6666-6666-666666666666', '88888888-8888-8888-8888-888888888888', 'SRV-001', 'General Cardiology Consultation', 500.00);

INSERT INTO HospitalServices (Id, TenantId, BranchId, DepartmentId, ServiceCode, ServiceName, Price)
VALUES (NEWID(), '55555555-5555-5555-5555-555555555555', '77777777-7777-7777-7777-777777777777', '99999999-9999-9999-9999-999999999999', 'SRV-002', 'ECG Diagnostic Test', 350.00);

-- 15. Appointments (2 Records)
INSERT INTO Appointments (Id, TenantId, BranchId, AppointmentNo, PatientId, DoctorId, DepartmentId, AppointmentDate, SlotStartTime, SlotEndTime, TokenNumber, Status, ConsultationFee)
VALUES ('30303030-3030-3030-3030-303030303030', '44444444-4444-4444-4444-444444444444', '66666666-6666-6666-6666-666666666666', 'APT-2026-00001', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', '10101010-1010-1010-1010-101010101010', '88888888-8888-8888-8888-888888888888', CAST(GETUTCDATE() AS DATE), '09:00:00', '09:15:00', 1, 'Scheduled', 500.00);

INSERT INTO Appointments (Id, TenantId, BranchId, AppointmentNo, PatientId, DoctorId, DepartmentId, AppointmentDate, SlotStartTime, SlotEndTime, TokenNumber, Status, ConsultationFee)
VALUES ('40404040-4040-4040-4040-404040404040', '55555555-5555-5555-5555-555555555555', '77777777-7777-7777-7777-777777777777', 'APT-2026-00002', 'ffffffff-ffff-ffff-ffff-ffffffffffff', '20202020-2020-2020-2020-202020202020', '99999999-9999-9999-9999-999999999999', CAST(GETUTCDATE() AS DATE), '10:00:00', '10:15:00', 1, 'Scheduled', 800.00);

-- 16. Consultations, Diagnoses, Prescriptions (2 Records)
INSERT INTO Consultations (Id, TenantId, BranchId, AppointmentId, PatientId, DoctorId, ChiefComplaints, ClinicalNotes, Advice, Status)
VALUES ('50505050-5050-5050-5050-505050505050', '44444444-4444-4444-4444-444444444444', '66666666-6666-6666-6666-666666666666', '30303030-3030-3030-3030-303030303030', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', '10101010-1010-1010-1010-101010101010', 'Chest discomfort on exertion', 'Regular BP 130/80', 'Avoid heavy exertion & rest', 'Completed');

INSERT INTO Diagnoses (Id, ConsultationId, ICD10Code, DiagnosisName, Type)
VALUES (NEWID(), '50505050-5050-5050-5050-505050505050', 'I20.9', 'Angina pectoris, unspecified', 'Primary');

INSERT INTO Prescriptions (Id, TenantId, ConsultationId, PatientId, DoctorId, Instructions)
VALUES ('60606060-6060-6060-6060-606060606060', '44444444-4444-4444-4444-444444444444', '50505050-5050-5050-5050-505050505050', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', '10101010-1010-1010-1010-101010101010', 'Take after meals');

INSERT INTO PrescriptionItems (Id, PrescriptionId, MedicineName, Dosage, Frequency, DurationDays, Instructions)
VALUES (NEWID(), '60606060-6060-6060-6060-606060606060', 'Aspirin 75mg', '0-1-0', 'Once Daily', 30, 'Take after lunch');

-- 17. Wards, Rooms, Beds (2 Records)
INSERT INTO Wards (Id, TenantId, BranchId, WardName, WardType, Floor)
VALUES ('70707070-7070-7070-7070-707070707070', '44444444-4444-4444-4444-444444444444', '66666666-6666-6666-6666-666666666666', 'General Ward A', 'General', '1st Floor');

INSERT INTO Rooms (Id, WardId, RoomNumber, RoomType)
VALUES ('80808080-8080-8080-8080-808080808080', '70707070-7070-7070-7070-707070707070', '101', 'Standard');

INSERT INTO Beds (Id, RoomId, BedNumber, DailyCharge, Status)
VALUES ('90909090-9090-9090-9090-909090909090', '80808080-8080-8080-8080-808080808080', 'B101', 1000.00, 'Available');

-- 18. Medicines (2 Records)
INSERT INTO Medicines (Id, TenantId, MedicineCode, Name, GenericName, Category, UnitPrice, StockQuantity, ExpiryDate)
VALUES (NEWID(), '44444444-4444-4444-4444-444444444444', 'MED-001', 'Paracetamol 500mg', 'Acetaminophen', 'Analgesic', 5.00, 500, DATEADD(YEAR, 2, GETUTCDATE()));

INSERT INTO Medicines (Id, TenantId, MedicineCode, Name, GenericName, Category, UnitPrice, StockQuantity, ExpiryDate)
VALUES (NEWID(), '55555555-5555-5555-5555-555555555555', 'MED-002', 'Amoxicillin 250mg', 'Amoxicillin', 'Antibiotic', 12.00, 300, DATEADD(YEAR, 1, GETUTCDATE()));

-- 19. Invoices & Payments (2 Records)
INSERT INTO Invoices (Id, TenantId, BranchId, InvoiceNumber, PatientId, SubTotal, DiscountAmount, TaxAmount, TotalAmount, PaidAmount, Status)
VALUES ('a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1', '44444444-4444-4444-4444-444444444444', '66666666-6666-6666-6666-666666666666', 'INV-2026-00001', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 500.00, 0, 0, 500.00, 500.00, 'Paid');

INSERT INTO InvoiceItems (Id, InvoiceId, ItemDescription, UnitPrice, Quantity, TotalPrice)
VALUES (NEWID(), 'a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1', 'Cardiology Consultation Fee', 500.00, 1, 500.00);

INSERT INTO Payments (Id, InvoiceId, PaymentNumber, Amount, PaymentMethod, TransactionRef)
VALUES (NEWID(), 'a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1', 'PAY-INV-2026-00001-01', 500.00, 'Cash', 'TXN-CASH-001');

INSERT INTO Invoices (Id, TenantId, BranchId, InvoiceNumber, PatientId, SubTotal, DiscountAmount, TaxAmount, TotalAmount, PaidAmount, Status)
VALUES ('b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2', '55555555-5555-5555-5555-555555555555', '77777777-7777-7777-7777-777777777777', 'INV-2026-00002', 'ffffffff-ffff-ffff-ffff-ffffffffffff', 800.00, 50.00, 0, 750.00, 0, 'Unpaid');

INSERT INTO InvoiceItems (Id, InvoiceId, ItemDescription, UnitPrice, Quantity, TotalPrice)
VALUES (NEWID(), 'b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2', 'Neurology Consultation Fee', 800.00, 1, 800.00);
GO
