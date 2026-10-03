using FluentAssertions;
using HMS.Application.DTOs.Phase2;
using HMS.Application.DTOs.Phase3And4;
using HMS.Domain.Entities;
using HMS.Infrastructure.Persistence;
using HMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HMS.Tests;

public class EndToEndWorkflowTests
{
    private readonly Guid _tenantId = Guid.NewGuid();

    private (HmsDbContext context, CurrentTenantService tenantService) CreateTestContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<HmsDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var tenantService = new CurrentTenantService();
        tenantService.SetTenantContext(_tenantId, null);

        var context = new HmsDbContext(options, tenantService);
        return (context, tenantService);
    }

    [Fact]
    public async Task Complete_Hospital_Patient_Doctor_Consultation_IPD_Billing_Workflow()
    {
        // Arrange
        var dbName = "E2E_Db_" + Guid.NewGuid();
        var (context, tenantService) = CreateTestContext(dbName);
        var auditService = new AuditService(context, tenantService, new Microsoft.AspNetCore.Http.HttpContextAccessor());
        var sequenceGenerator = new SequenceGeneratorService(context);
        var entitlementService = new FeatureEntitlementService(context);

        var patientService = new PatientService(context, tenantService, entitlementService, sequenceGenerator, auditService);
        var doctorService = new DoctorService(context, tenantService, entitlementService, sequenceGenerator, auditService);
        var appointmentService = new AppointmentService(context, tenantService, sequenceGenerator, auditService);
        var clinicalService = new ClinicalService(context, tenantService, sequenceGenerator, auditService);
        var billingService = new BillingService(context, tenantService, sequenceGenerator, auditService);

        // 1. Register Patient
        var patientResult = await patientService.RegisterPatientAsync(new RegisterPatientRequest
        {
            FirstName = "Robert",
            LastName = "Johnson",
            Gender = "Male",
            DOB = new DateTime(1988, 12, 10),
            Phone = "9998887770",
            Email = "robert@example.com"
        });
        patientResult.IsSuccess.Should().BeTrue();
        var patient = patientResult.Value!;

        // 2. Create Doctor & Schedule
        var doctorResult = await doctorService.CreateDoctorAsync(new CreateDoctorRequest
        {
            FirstName = "Sarah",
            LastName = "Connor",
            Specialization = "Neurology",
            Qualification = "MD, DM",
            RegistrationNumber = "DOC-REG-999",
            ConsultationFee = 800,
            Email = "dr.sarah@example.com",
            Phone = "5554443333"
        });
        doctorResult.IsSuccess.Should().BeTrue();
        var doctor = doctorResult.Value!;

        await doctorService.AddDoctorScheduleAsync(new CreateDoctorScheduleRequest
        {
            DoctorId = doctor.Id,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(17, 0, 0),
            SlotDurationMinutes = 15,
            MaxPatients = 30
        });

        // 3. Book OPD Appointment
        var appointmentResult = await appointmentService.BookAppointmentAsync(new BookAppointmentRequest
        {
            PatientId = patient.Id,
            DoctorId = doctor.Id,
            AppointmentDate = new DateTime(2026, 10, 5),
            SlotStartTime = new TimeSpan(10, 0, 0),
            Reason = "Severe Headache"
        });
        appointmentResult.IsSuccess.Should().BeTrue();
        var appointment = appointmentResult.Value!;
        appointment.TokenNumber.Should().Be(1);

        // 4. Clinical Consultation & Prescription
        var consultationResult = await clinicalService.CreateConsultationAsync(new CreateConsultationRequest
        {
            PatientId = patient.Id,
            DoctorId = doctor.Id,
            AppointmentId = appointment.Id,
            ChiefComplaints = "Migraine & dizziness",
            ClinicalNotes = "Patient exhibits signs of severe migraine",
            Advice = "Rest & hydration",
            Diagnoses = new List<CreateDiagnosisDto>
            {
                new() { DiagnosisName = "Migraine without aura", ICD10Code = "G43.0" }
            },
            Prescriptions = new List<CreatePrescriptionItemDto>
            {
                new() { MedicineName = "Sumatriptan 50mg", Dosage = "1-0-0", Frequency = "As needed", DurationDays = 5 }
            }
        });
        consultationResult.IsSuccess.Should().BeTrue();

        // 5. Setup Ward, Room, Bed & Admit Patient to IPD
        var ward = new Ward { TenantId = _tenantId, WardName = "General Ward A", WardType = "General", Floor = "2nd Floor" };
        var room = new Room { WardId = ward.Id, RoomNumber = "201", RoomType = "Standard" };
        var bed = new Bed { RoomId = room.Id, BedNumber = "B101", DailyCharge = 1500, Status = "Available" };

        context.Wards.Add(ward);
        context.Rooms.Add(room);
        context.Beds.Add(bed);
        await context.SaveChangesAsync();

        var admissionResult = await clinicalService.AdmitPatientAsync(new AdmitPatientRequest
        {
            PatientId = patient.Id,
            AttendingDoctorId = doctor.Id,
            BedId = bed.Id,
            ReasonForAdmission = "Observation for acute migraine symptoms"
        });
        admissionResult.IsSuccess.Should().BeTrue();
        admissionResult.Value!.Status.Should().Be("Admitted");

        var occupiedBed = await context.Beds.FindAsync(bed.Id);
        occupiedBed!.Status.Should().Be("Occupied");

        // 6. Generate Billing Invoice
        var invoiceResult = await billingService.GenerateInvoiceAsync(new GenerateInvoiceRequest
        {
            PatientId = patient.Id,
            DiscountAmount = 100,
            TaxAmount = 50,
            Items = new List<CreateInvoiceItemDto>
            {
                new() { ItemDescription = "Neurology OPD Consultation", UnitPrice = 800, Quantity = 1 },
                new() { ItemDescription = "General Ward Bed Charge (1 Day)", UnitPrice = 1500, Quantity = 1 }
            }
        });
        invoiceResult.IsSuccess.Should().BeTrue();
        var invoice = invoiceResult.Value!;
        invoice.TotalAmount.Should().Be((800 + 1500 - 100) + 50); // 2250

        // 7. Record Payment
        var paymentResult = await billingService.RecordPaymentAsync(new RecordPaymentRequest
        {
            InvoiceId = invoice.Id,
            Amount = 2250,
            PaymentMethod = "Credit Card",
            TransactionRef = "TXN-998877"
        });
        paymentResult.IsSuccess.Should().BeTrue();

        var paidInvoice = (await billingService.GetInvoiceByIdAsync(invoice.Id)).Value!;
        paidInvoice.Status.Should().Be("Paid");
        paidInvoice.PaidAmount.Should().Be(2250);
    }
}

