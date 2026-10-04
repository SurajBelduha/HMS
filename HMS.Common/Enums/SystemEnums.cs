namespace HMS.Common.Enums;

public enum UserRoleEnum
{
    PlatformSuperAdmin = 0,
    PlatformSupport = 1,
    HospitalOwner = 2,
    HospitalAdmin = 3,
    BranchAdmin = 4,
    Doctor = 5,
    Nurse = 6,
    Receptionist = 7,
    Pharmacist = 8,
    LabTechnician = 9,
    Accountant = 10
}

public enum TenantStatusEnum
{
    Active = 0,
    Suspended = 1,
    Inactive = 2
}

public enum UserStatusEnum
{
    Active = 0,
    Suspended = 1,
    Inactive = 2,
    LockedOut = 3
}

public enum AppointmentStatusEnum
{
    Scheduled = 0,
    Completed = 1,
    Cancelled = 2,
    Rescheduled = 3,
    NoShow = 4
}

public enum ConsultationStatusEnum
{
    InProgress = 0,
    Completed = 1,
    Cancelled = 2
}

public enum AdmissionStatusEnum
{
    Admitted = 0,
    Discharged = 1,
    Transferred = 2
}

public enum BedStatusEnum
{
    Available = 0,
    Occupied = 1,
    Maintenance = 2
}

public enum InvoiceStatusEnum
{
    Unpaid = 0,
    PartiallyPaid = 1,
    Paid = 2,
    Cancelled = 3
}

public enum LabOrderStatusEnum
{
    Ordered = 0,
    SampleCollected = 1,
    Completed = 2,
    Cancelled = 3
}

public enum SubscriptionStatusEnum
{
    Trialing = 0,
    Active = 1,
    PastDue = 2,
    GracePeriod = 3,
    Suspended = 4,
    Cancelled = 5
}

