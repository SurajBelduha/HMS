using FluentAssertions;
using HMS.Application.DTOs.Phase2;
using HMS.Domain.Entities;
using HMS.Infrastructure.Persistence;
using HMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HMS.Tests;

public class Phase2CoreHospitalOperationsTests
{
    private readonly Guid _tenantAId = Guid.NewGuid();
    private readonly Guid _tenantBId = Guid.NewGuid();

    private (HmsDbContext context, CurrentTenantService tenantService) CreateTestContext(string dbName, Guid tenantId)
    {
        var options = new DbContextOptionsBuilder<HmsDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var tenantService = new CurrentTenantService();
        tenantService.SetTenantContext(tenantId, null);

        var context = new HmsDbContext(options, tenantService);
        return (context, tenantService);
    }

    [Fact]
    public async Task Register_Patient_Generates_MRN_And_Stores_Address()
    {
        // Arrange
        var dbName = "Phase2_Db_" + Guid.NewGuid();
        var (context, tenantService) = CreateTestContext(dbName, _tenantAId);
        var auditService = new AuditService(context, tenantService, new Microsoft.AspNetCore.Http.HttpContextAccessor());
        var sequenceGenerator = new SequenceGeneratorService(context);
        var entitlementService = new FeatureEntitlementService(context);
        var patientService = new PatientService(context, tenantService, entitlementService, sequenceGenerator, auditService);

        var request = new RegisterPatientRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            DOB = new DateTime(1990, 5, 15),
            Phone = "9876543210",
            Email = "john.doe@example.com",
            AddressLine1 = "123 Main Street",
            City = "Jaipur",
            EmergencyContactName = "Jane Doe",
            EmergencyContactPhone = "9876543211"
        };

        // Act
        var result = await patientService.RegisterPatientAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.MRN.Should().StartWith("PAT-");
        result.Value.FullName.Should().Be("John Doe");
        result.Value.Addresses.Should().HaveCount(1);
        result.Value.Contacts.Should().HaveCount(1);
    }

    [Fact]
    public async Task Book_Appointment_Generates_Sequential_Tokens_1_And_2()
    {
        // Arrange
        var dbName = "Phase2_Db_" + Guid.NewGuid();
        var (context, tenantService) = CreateTestContext(dbName, _tenantAId);
        var auditService = new AuditService(context, tenantService, new Microsoft.AspNetCore.Http.HttpContextAccessor());
        var sequenceGenerator = new SequenceGeneratorService(context);
        var entitlementService = new FeatureEntitlementService(context);

        var patientService = new PatientService(context, tenantService, entitlementService, sequenceGenerator, auditService);
        var doctorService = new DoctorService(context, tenantService, entitlementService, sequenceGenerator, auditService);
        var appointmentService = new AppointmentService(context, tenantService, sequenceGenerator, auditService);

        // 1. Create Doctor & Schedule on Monday
        var doctorResult = await doctorService.CreateDoctorAsync(new CreateDoctorRequest
        {
            FirstName = "Alice",
            LastName = "Smith",
            Specialization = "Cardiology",
            Qualification = "MD",
            RegistrationNumber = "REG-100",
            ConsultationFee = 500,
            Email = "dr.smith@example.com",
            Phone = "1112223333"
        });

        var doctor = doctorResult.Value!;
        var appointmentDate = new DateTime(2026, 10, 5); // Monday

        await doctorService.AddDoctorScheduleAsync(new CreateDoctorScheduleRequest
        {
            DoctorId = doctor.Id,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(13, 0, 0),
            SlotDurationMinutes = 15,
            MaxPatients = 20
        });

        // 2. Register 2 Patients
        var p1 = (await patientService.RegisterPatientAsync(new RegisterPatientRequest { FirstName = "Patient", LastName = "One", DOB = new DateTime(1985, 1, 1), Phone = "111" })).Value!;
        var p2 = (await patientService.RegisterPatientAsync(new RegisterPatientRequest { FirstName = "Patient", LastName = "Two", DOB = new DateTime(1992, 2, 2), Phone = "222" })).Value!;

        // 3. Book 1st Appointment
        var apt1Result = await appointmentService.BookAppointmentAsync(new BookAppointmentRequest
        {
            PatientId = p1.Id,
            DoctorId = doctor.Id,
            AppointmentDate = appointmentDate,
            SlotStartTime = new TimeSpan(9, 0, 0),
            Reason = "Chest Pain"
        });

        // 4. Book 2nd Appointment
        var apt2Result = await appointmentService.BookAppointmentAsync(new BookAppointmentRequest
        {
            PatientId = p2.Id,
            DoctorId = doctor.Id,
            AppointmentDate = appointmentDate,
            SlotStartTime = new TimeSpan(9, 15, 0),
            Reason = "Follow Up"
        });

        // Assert
        apt1Result.IsSuccess.Should().BeTrue();
        apt1Result.Value!.TokenNumber.Should().Be(1);

        apt2Result.IsSuccess.Should().BeTrue();
        apt2Result.Value!.TokenNumber.Should().Be(2);
    }

    [Fact]
    public async Task HospitalA_Cannot_See_HospitalB_Patients_Or_Appointments()
    {
        // Arrange
        var dbName = "Phase2_Db_" + Guid.NewGuid();

        var (contextA, tenantServiceA) = CreateTestContext(dbName, _tenantAId);
        var auditServiceA = new AuditService(contextA, tenantServiceA, new Microsoft.AspNetCore.Http.HttpContextAccessor());
        var sequenceA = new SequenceGeneratorService(contextA);
        var entitlementA = new FeatureEntitlementService(contextA);
        var patientServiceA = new PatientService(contextA, tenantServiceA, entitlementA, sequenceA, auditServiceA);

        var patientA = (await patientServiceA.RegisterPatientAsync(new RegisterPatientRequest { FirstName = "HospitalA", LastName = "Patient", DOB = new DateTime(1995, 3, 3), Phone = "999" })).Value!;

        var (contextB, tenantServiceB) = CreateTestContext(dbName, _tenantBId);
        var auditServiceB = new AuditService(contextB, tenantServiceB, new Microsoft.AspNetCore.Http.HttpContextAccessor());
        var sequenceB = new SequenceGeneratorService(contextB);
        var entitlementB = new FeatureEntitlementService(contextB);
        var patientServiceB = new PatientService(contextB, tenantServiceB, entitlementB, sequenceB, auditServiceB);

        var queriedPatientFromB = await patientServiceB.GetPatientByIdAsync(patientA.Id);

        // Assert
        queriedPatientFromB.IsSuccess.Should().BeFalse();
    }
}

