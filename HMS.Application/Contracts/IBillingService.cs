using HMS.Application.DTOs.Phase3And4;
using HMS.Common.Models;
using HMS.Domain.Entities;

namespace HMS.Application.Contracts;

public interface IBillingService
{
    Task<Result<Invoice>> GenerateInvoiceAsync(GenerateInvoiceRequest request, CancellationToken cancellationToken = default);
    Task<Result<Payment>> RecordPaymentAsync(RecordPaymentRequest request, CancellationToken cancellationToken = default);
    Task<Result<Invoice>> GetInvoiceByIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task<Result<List<Invoice>>> GetPatientInvoicesAsync(Guid patientId, CancellationToken cancellationToken = default);
}

