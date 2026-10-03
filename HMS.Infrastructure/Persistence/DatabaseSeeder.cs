using System.Security.Cryptography;
using System.Text;
using HMS.Common.Enums;
using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(HmsDbContext context)
    {
        // 1. Seed Platform Super Admin Tenant & User if not existing
        var platformTenant = await context.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.TenantCode == "PLATFORM-HOST");
        if (platformTenant == null)
        {
            platformTenant = new Tenant
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                TenantCode = "PLATFORM-HOST",
                Name = "HMS SaaS Platform Host",
                LegalName = "HMS Global SaaS Corp",
                Email = "host@hmssaas.com",
                Phone = "1800-HMS-HOST",
                Status = TenantStatusEnum.Active.ToString(),
                IsolationStrategy = TenantIsolationStrategy.SharedDatabase
            };
            context.Tenants.Add(platformTenant);
            await context.SaveChangesAsync();
        }

        var superAdminRole = await context.Roles.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Code == UserRoleEnum.PlatformSuperAdmin.ToString());
        if (superAdminRole == null)
        {
            superAdminRole = new Role
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                TenantId = platformTenant.Id,
                Name = "Platform Super Admin",
                Code = UserRoleEnum.PlatformSuperAdmin.ToString(),
                IsSystemRole = true
            };
            context.Roles.Add(superAdminRole);
            await context.SaveChangesAsync();
        }

        var superAdminUser = await context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Email == "superadmin@hmssaas.com");
        if (superAdminUser == null)
        {
            superAdminUser = new User
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                TenantId = platformTenant.Id,
                Email = "superadmin@hmssaas.com",
                FirstName = "Platform",
                LastName = "SuperAdmin",
                PasswordHash = HashPassword("SuperAdmin@Pass2026!"),
                Status = UserStatusEnum.Active.ToString(),
                EmailVerified = true
            };
            context.Users.Add(superAdminUser);
            context.UserRoles.Add(new UserRole { UserId = superAdminUser.Id, RoleId = superAdminRole.Id });
            await context.SaveChangesAsync();
        }

        // 2. Seed 2 Sample Hospital Tenants (ABC Healthcare & City Care) if database has <= 1 tenant
        var tenantCount = await context.Tenants.IgnoreQueryFilters().CountAsync(t => t.TenantCode != "PLATFORM-HOST");
        if (tenantCount < 2)
        {
            var tenant1Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var tenant2Id = Guid.Parse("55555555-5555-5555-5555-555555555555");

            var t1 = new Tenant { Id = tenant1Id, TenantCode = "ABC-HC", Name = "ABC Healthcare", LegalName = "ABC Healthcare Ltd", Email = "contact@abchealthcare.com", Phone = "9876543210", Status = "Active" };
            var t2 = new Tenant { Id = tenant2Id, TenantCode = "CITY-CARE", Name = "City Care Hospital", LegalName = "City Care Hospital Pvt Ltd", Email = "info@citycare.com", Phone = "9876543211", Status = "Active" };

            context.Tenants.AddRange(t1, t2);
            context.TenantSettings.AddRange(
                new TenantSetting { TenantId = tenant1Id, InvoicePrefix = "ABC-INV-", PatientPrefix = "ABC-PAT-" },
                new TenantSetting { TenantId = tenant2Id, InvoicePrefix = "CC-INV-", PatientPrefix = "CC-PAT-" }
            );

            // Seed 2 Branches per Tenant
            var b1 = new Branch { Id = Guid.NewGuid(), TenantId = tenant1Id, BranchCode = "JPR-01", Name = "Jaipur Branch", Address = "Jaipur Main Rd", City = "Jaipur", State = "Rajasthan", Country = "India", PinCode = "302001", Phone = "0141-111111", Email = "jaipur@abchealthcare.com" };
            var b2 = new Branch { Id = Guid.NewGuid(), TenantId = tenant2Id, BranchCode = "DEL-01", Name = "Delhi Branch", Address = "Connaught Place", City = "Delhi", State = "Delhi", Country = "India", PinCode = "110001", Phone = "011-222222", Email = "delhi@citycare.com" };
            context.Branches.AddRange(b1, b2);

            // Seed 2 Departments
            var d1 = new Department { Id = Guid.NewGuid(), TenantId = tenant1Id, BranchId = b1.Id, Code = "CARD", Name = "Cardiology" };
            var d2 = new Department { Id = Guid.NewGuid(), TenantId = tenant2Id, BranchId = b2.Id, Code = "NEURO", Name = "Neurology" };
            context.Departments.AddRange(d1, d2);

            // Seed 2 Patients per Tenant
            var p1 = new Patient { Id = Guid.NewGuid(), TenantId = tenant1Id, BranchId = b1.Id, MRN = "PAT-2026-00001", FirstName = "John", LastName = "Doe", Gender = "Male", DOB = new DateTime(1990, 1, 1), Phone = "9876543210", Email = "john@example.com" };
            var p2 = new Patient { Id = Guid.NewGuid(), TenantId = tenant2Id, BranchId = b2.Id, MRN = "PAT-2026-00002", FirstName = "Jane", LastName = "Smith", Gender = "Female", DOB = new DateTime(1992, 2, 2), Phone = "9876543211", Email = "jane@example.com" };
            context.Patients.AddRange(p1, p2);

            // Seed 2 Doctors per Tenant
            var doc1 = new Doctor { Id = Guid.NewGuid(), TenantId = tenant1Id, BranchId = b1.Id, DoctorNo = "DOC-001", FirstName = "Alice", LastName = "Smith", Specialization = "Cardiology", Qualification = "MD", RegistrationNumber = "REG-101", ConsultationFee = 500, Phone = "111", Email = "dr.alice@abchealthcare.com" };
            var doc2 = new Doctor { Id = Guid.NewGuid(), TenantId = tenant2Id, BranchId = b2.Id, DoctorNo = "DOC-002", FirstName = "Robert", LastName = "Brown", Specialization = "Neurology", Qualification = "DM", RegistrationNumber = "REG-102", ConsultationFee = 800, Phone = "222", Email = "dr.robert@citycare.com" };
            context.Doctors.AddRange(doc1, doc2);

            // Seed 2 Doctor Schedules
            context.DoctorSchedules.AddRange(
                new DoctorSchedule { TenantId = tenant1Id, BranchId = b1.Id, DoctorId = doc1.Id, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(13, 0, 0), MaxPatients = 20 },
                new DoctorSchedule { TenantId = tenant2Id, BranchId = b2.Id, DoctorId = doc2.Id, DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(14, 0, 0), MaxPatients = 20 }
            );

            // Seed 2 Appointments
            var apt1 = new Appointment { Id = Guid.NewGuid(), TenantId = tenant1Id, BranchId = b1.Id, AppointmentNo = "APT-2026-00001", PatientId = p1.Id, DoctorId = doc1.Id, DepartmentId = d1.Id, AppointmentDate = DateTime.UtcNow.Date, SlotStartTime = new TimeSpan(9, 0, 0), SlotEndTime = new TimeSpan(9, 15, 0), TokenNumber = 1, Status = "Scheduled", ConsultationFee = 500 };
            var apt2 = new Appointment { Id = Guid.NewGuid(), TenantId = tenant2Id, BranchId = b2.Id, AppointmentNo = "APT-2026-00002", PatientId = p2.Id, DoctorId = doc2.Id, DepartmentId = d2.Id, AppointmentDate = DateTime.UtcNow.Date, SlotStartTime = new TimeSpan(10, 0, 0), SlotEndTime = new TimeSpan(10, 15, 0), TokenNumber = 1, Status = "Scheduled", ConsultationFee = 800 };
            context.Appointments.AddRange(apt1, apt2);

            // Seed 2 Wards, Rooms, Beds
            var w1 = new Ward { Id = Guid.NewGuid(), TenantId = tenant1Id, BranchId = b1.Id, WardName = "General Ward 1", WardType = "General", Floor = "1st Floor" };
            var w2 = new Ward { Id = Guid.NewGuid(), TenantId = tenant2Id, BranchId = b2.Id, WardName = "ICU Ward 1", WardType = "ICU", Floor = "2nd Floor" };
            context.Wards.AddRange(w1, w2);

            var r1 = new Room { Id = Guid.NewGuid(), WardId = w1.Id, RoomNumber = "101", RoomType = "General" };
            var r2 = new Room { Id = Guid.NewGuid(), WardId = w2.Id, RoomNumber = "201", RoomType = "ICU Room" };
            context.Rooms.AddRange(r1, r2);

            var bed1 = new Bed { Id = Guid.NewGuid(), RoomId = r1.Id, BedNumber = "B-101", DailyCharge = 1000, Status = "Available" };
            var bed2 = new Bed { Id = Guid.NewGuid(), RoomId = r2.Id, BedNumber = "ICU-01", DailyCharge = 5000, Status = "Available" };
            context.Beds.AddRange(bed1, bed2);

            // Seed 2 Medicines
            context.Medicines.AddRange(
                new Medicine { TenantId = tenant1Id, MedicineCode = "MED-001", Name = "Paracetamol 500mg", GenericName = "Acetaminophen", Category = "Analgesic", UnitPrice = 5, StockQuantity = 500, ExpiryDate = DateTime.UtcNow.AddYears(2) },
                new Medicine { TenantId = tenant2Id, MedicineCode = "MED-002", Name = "Amoxicillin 250mg", GenericName = "Amoxicillin", Category = "Antibiotic", UnitPrice = 12, StockQuantity = 300, ExpiryDate = DateTime.UtcNow.AddYears(1) }
            );

            // Seed 2 Invoices
            var inv1 = new Invoice { Id = Guid.NewGuid(), TenantId = tenant1Id, BranchId = b1.Id, InvoiceNumber = "INV-2026-00001", PatientId = p1.Id, SubTotal = 500, DiscountAmount = 0, TaxAmount = 0, TotalAmount = 500, PaidAmount = 500, Status = "Paid" };
            var inv2 = new Invoice { Id = Guid.NewGuid(), TenantId = tenant2Id, BranchId = b2.Id, InvoiceNumber = "INV-2026-00002", PatientId = p2.Id, SubTotal = 800, DiscountAmount = 50, TaxAmount = 0, TotalAmount = 750, PaidAmount = 0, Status = "Unpaid" };
            context.Invoices.AddRange(inv1, inv2);

            await context.SaveChangesAsync();
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
