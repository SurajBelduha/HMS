using HMS.Application.Contracts;
using HMS.Application.DTOs.Phase3And4;
using HMS.Common.Models;
using HMS.Domain.Entities;
using HMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Services;

public class BillingService : IBillingService
{
    private readonly HmsDbContext _dbContext;
    private readonly ICurrentTenantService _tenantService;
    private readonly ISequenceGeneratorService _sequenceGenerator;
    private readonly IAuditService _auditService;

    public BillingService(
        HmsDbContext dbContext,
        ICurrentTenantService tenantService,
        ISequenceGeneratorService sequenceGenerator,
        IAuditService auditService)
    {
        _dbContext = dbContext;
        _tenantService = tenantService;
        _sequenceGenerator = sequenceGenerator;
        _auditService = auditService;
    }

    public async Task<Result<Invoice>> GenerateInvoiceAsync(GenerateInvoiceRequest request, CancellationToken cancellationToken = default)
    {
        var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.Id == request.PatientId, cancellationToken);
        if (patient == null)
            return Result.Failure<Invoice>("Patient not found.");

        var invoiceNo = await _sequenceGenerator.GenerateSequenceNumberAsync<Invoice>(
            "INV",
            q => q.Where(i => i.TenantId == _tenantService.TenantId),
            padLeft: 5,
            includeYear: true,
            cancellationToken);

        decimal subTotal = 0;
        var invoiceItems = new List<InvoiceItem>();

        foreach (var item in request.Items)
        {
            var total = item.UnitPrice * item.Quantity;
            subTotal += total;

            invoiceItems.Add(new InvoiceItem
            {
                ItemDescription = item.ItemDescription,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                TotalPrice = total
            });
        }

        var totalAmount = (subTotal - request.DiscountAmount) + request.TaxAmount;

        var invoice = new Invoice
        {
            TenantId = _tenantService.TenantId,
            BranchId = _tenantService.BranchId,
            InvoiceNumber = invoiceNo,
            PatientId = request.PatientId,
            SubTotal = subTotal,
            DiscountAmount = request.DiscountAmount,
            TaxAmount = request.TaxAmount,
            TotalAmount = totalAmount,
            PaidAmount = 0,
            Status = "Unpaid",
            InvoiceDate = DateTime.UtcNow,
            Items = invoiceItems
        };

        _dbContext.Invoices.Add(invoice);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync("GenerateInvoice", "Revenue", "Invoice", invoice.Id.ToString(), null, new { invoice.InvoiceNumber, invoice.TotalAmount }, cancellationToken);

        return Result.Success(invoice);
    }

    public async Task<Result<Payment>> RecordPaymentAsync(RecordPaymentRequest request, CancellationToken cancellationToken = default)
    {
        var invoice = await _dbContext.Invoices
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

        if (invoice == null)
            return Result.Failure<Payment>("Invoice not found.");

        var paymentCount = invoice.Payments.Count + 1;
        var paymentNo = $"PAY-{invoice.InvoiceNumber}-{paymentCount:D2}";

        var payment = new Payment
        {
            InvoiceId = request.InvoiceId,
            PaymentNumber = paymentNo,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            PaymentDate = DateTime.UtcNow,
            TransactionRef = request.TransactionRef
        };

        invoice.PaidAmount += request.Amount;
        if (invoice.PaidAmount >= invoice.TotalAmount)
        {
            invoice.Status = "Paid";
        }
        else if (invoice.PaidAmount > 0)
        {
            invoice.Status = "PartiallyPaid";
        }

        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync("RecordPayment", "Revenue", "Payment", payment.Id.ToString(), null, new { payment.PaymentNumber, payment.Amount, invoice.Status }, cancellationToken);

        return Result.Success(payment);
    }

    public async Task<Result<Invoice>> GetInvoiceByIdAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        var invoice = await _dbContext.Invoices
            .Include(i => i.Patient)
            .Include(i => i.Items)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == invoiceId, cancellationToken);

        if (invoice == null)
            return Result.Failure<Invoice>("Invoice not found.");

        return Result.Success(invoice);
    }

    public async Task<Result<List<Invoice>>> GetPatientInvoicesAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        var invoices = await _dbContext.Invoices
            .Include(i => i.Items)
            .Include(i => i.Payments)
            .Where(i => i.PatientId == patientId)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync(cancellationToken);

        return Result.Success(invoices);
    }
}

