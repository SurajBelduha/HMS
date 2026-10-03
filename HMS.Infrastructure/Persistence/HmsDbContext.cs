using HMS.Application.Contracts;
using HMS.Domain.Contracts;
using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Persistence;

public class HmsDbContext : DbContext
{
    private readonly ICurrentTenantService? _tenantService;

    public HmsDbContext(DbContextOptions<HmsDbContext> options, ICurrentTenantService? tenantService = null)
        : base(options)
    {
        _tenantService = tenantService;
    }

    // Phase 1 Foundation DbSets
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<TenantSetting> TenantSettings => Set<TenantSetting>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<PlanFeature> PlanFeatures => Set<PlanFeature>();
    public DbSet<TenantSubscription> TenantSubscriptions => Set<TenantSubscription>();
    public DbSet<SubscriptionItem> SubscriptionItems => Set<SubscriptionItem>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // Phase 2 Core Hospital Operations DbSets
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<PatientAddress> PatientAddresses => Set<PatientAddress>();
    public DbSet<PatientDocument> PatientDocuments => Set<PatientDocument>();
    public DbSet<PatientContact> PatientContacts => Set<PatientContact>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<DoctorDepartment> DoctorDepartments => Set<DoctorDepartment>();
    public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>();
    public DbSet<HospitalService> HospitalServices => Set<HospitalService>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    // Phase 3 Clinical Operations DbSets
    public DbSet<Consultation> Consultations => Set<Consultation>();
    public DbSet<Diagnosis> Diagnoses => Set<Diagnosis>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
    public DbSet<Ward> Wards => Set<Ward>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Bed> Beds => Set<Bed>();
    public DbSet<Admission> Admissions => Set<Admission>();
    public DbSet<Vital> Vitals => Set<Vital>();

    // Phase 4 Revenue, Pharmacy & Lab DbSets
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Medicine> Medicines => Set<Medicine>();
    public DbSet<LabOrder> LabOrders => Set<LabOrder>();

    public Guid CurrentTenantId => _tenantService?.TenantId ?? Guid.Empty;
    public bool CurrentTenantIsHostAdmin => _tenantService?.IsHostAdmin ?? false;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply Entity Type Configurations from assembly (SOLID Open/Closed Principle)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HmsDbContext).Assembly);

        // Global Query Filters (Soft Delete & Multi-Tenancy)
        modelBuilder.Entity<Tenant>()
            .HasQueryFilter(t => !t.IsDeleted);

        modelBuilder.Entity<Branch>()
            .HasQueryFilter(b => !b.IsDeleted && (CurrentTenantIsHostAdmin || b.TenantId == CurrentTenantId));

        modelBuilder.Entity<Department>()
            .HasQueryFilter(d => !d.IsDeleted && (CurrentTenantIsHostAdmin || d.TenantId == CurrentTenantId));

        modelBuilder.Entity<User>()
            .HasQueryFilter(u => !u.IsDeleted && (CurrentTenantIsHostAdmin || u.TenantId == CurrentTenantId));

        modelBuilder.Entity<TenantSetting>()
            .HasQueryFilter(s => CurrentTenantIsHostAdmin || s.TenantId == CurrentTenantId);

        modelBuilder.Entity<Role>()
            .HasQueryFilter(r => CurrentTenantIsHostAdmin || r.TenantId == CurrentTenantId);

        modelBuilder.Entity<TenantSubscription>()
            .HasQueryFilter(s => CurrentTenantIsHostAdmin || s.TenantId == CurrentTenantId);

        modelBuilder.Entity<AuditLog>()
            .HasQueryFilter(a => CurrentTenantIsHostAdmin || a.TenantId == CurrentTenantId);

        modelBuilder.Entity<Patient>()
            .HasQueryFilter(p => !p.IsDeleted && (CurrentTenantIsHostAdmin || p.TenantId == CurrentTenantId));

        modelBuilder.Entity<PatientDocument>()
            .HasQueryFilter(pd => CurrentTenantIsHostAdmin || pd.TenantId == CurrentTenantId);

        modelBuilder.Entity<Doctor>()
            .HasQueryFilter(doc => !doc.IsDeleted && (CurrentTenantIsHostAdmin || doc.TenantId == CurrentTenantId));

        modelBuilder.Entity<DoctorSchedule>()
            .HasQueryFilter(ds => CurrentTenantIsHostAdmin || ds.TenantId == CurrentTenantId);

        modelBuilder.Entity<HospitalService>()
            .HasQueryFilter(hs => CurrentTenantIsHostAdmin || hs.TenantId == CurrentTenantId);

        modelBuilder.Entity<Appointment>()
            .HasQueryFilter(apt => !apt.IsDeleted && (CurrentTenantIsHostAdmin || apt.TenantId == CurrentTenantId));

        modelBuilder.Entity<Consultation>()
            .HasQueryFilter(c => CurrentTenantIsHostAdmin || c.TenantId == CurrentTenantId);

        modelBuilder.Entity<Prescription>()
            .HasQueryFilter(pr => CurrentTenantIsHostAdmin || pr.TenantId == CurrentTenantId);

        modelBuilder.Entity<Ward>()
            .HasQueryFilter(w => CurrentTenantIsHostAdmin || w.TenantId == CurrentTenantId);

        modelBuilder.Entity<Admission>()
            .HasQueryFilter(adm => CurrentTenantIsHostAdmin || adm.TenantId == CurrentTenantId);

        modelBuilder.Entity<Vital>()
            .HasQueryFilter(v => CurrentTenantIsHostAdmin || v.TenantId == CurrentTenantId);

        modelBuilder.Entity<Invoice>()
            .HasQueryFilter(inv => CurrentTenantIsHostAdmin || inv.TenantId == CurrentTenantId);

        modelBuilder.Entity<Medicine>()
            .HasQueryFilter(med => CurrentTenantIsHostAdmin || med.TenantId == CurrentTenantId);

        modelBuilder.Entity<LabOrder>()
            .HasQueryFilter(lab => CurrentTenantIsHostAdmin || lab.TenantId == CurrentTenantId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSaveChanges();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        OnBeforeSaveChanges();
        return base.SaveChanges();
    }

    private void OnBeforeSaveChanges()
    {
        var currentUserId = _tenantService?.UserId?.ToString() ?? "System";
        var currentTenantId = _tenantService?.TenantId ?? Guid.Empty;
        var currentBranchId = _tenantService?.BranchId;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is ITenantScoped tenantScoped && entry.State == EntityState.Added)
            {
                if (tenantScoped.TenantId == Guid.Empty && currentTenantId != Guid.Empty)
                {
                    tenantScoped.TenantId = currentTenantId;
                }
            }

            if (entry.Entity is IBranchScoped branchScoped && entry.State == EntityState.Added)
            {
                if (branchScoped.BranchId == null && currentBranchId.HasValue)
                {
                    branchScoped.BranchId = currentBranchId.Value;
                }
            }

            if (entry.Entity is IAuditableEntity auditable)
            {
                if (entry.State == EntityState.Added)
                {
                    auditable.CreatedAt = DateTime.UtcNow;
                    auditable.CreatedBy = currentUserId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    auditable.UpdatedAt = DateTime.UtcNow;
                    auditable.UpdatedBy = currentUserId;
                }
            }

            if (entry.Entity is ISoftDelete softDelete && entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                softDelete.IsDeleted = true;
                softDelete.DeletedAt = DateTime.UtcNow;
                softDelete.DeletedBy = currentUserId;
            }
        }
    }
}

