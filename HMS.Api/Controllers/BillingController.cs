using HMS.Application.Contracts;
using HMS.Application.DTOs.Phase3And4;
using HMS.Common.Models;
using HMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class BillingController : ControllerBase
{
    private readonly IBillingService _billingService;

    public BillingController(IBillingService billingService)
    {
        _billingService = billingService;
    }

    [HttpPost("invoices")]
    public async Task<ActionResult<ApiResponse<Invoice>>> GenerateInvoice([FromBody] GenerateInvoiceRequest request, CancellationToken cancellationToken)
    {
        var result = await _billingService.GenerateInvoiceAsync(request, cancellationToken);
        if (result.IsFailure)
            return BadRequest(ApiResponse<Invoice>.FailureResponse(result.Error!));

        return Ok(ApiResponse<Invoice>.SuccessResponse(result.Value!, "Invoice generated successfully."));
    }

    [HttpPost("payments")]
    public async Task<ActionResult<ApiResponse<Payment>>> RecordPayment([FromBody] RecordPaymentRequest request, CancellationToken cancellationToken)
    {
        var result = await _billingService.RecordPaymentAsync(request, cancellationToken);
        if (result.IsFailure)
            return BadRequest(ApiResponse<Payment>.FailureResponse(result.Error!));

        return Ok(ApiResponse<Payment>.SuccessResponse(result.Value!, "Payment recorded successfully."));
    }

    [HttpGet("invoices/{id:guid}")]
    public async Task<ActionResult<ApiResponse<Invoice>>> GetInvoiceById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _billingService.GetInvoiceByIdAsync(id, cancellationToken);
        if (result.IsFailure)
            return NotFound(ApiResponse<Invoice>.FailureResponse(result.Error!));

        return Ok(ApiResponse<Invoice>.SuccessResponse(result.Value!));
    }

    [HttpGet("patients/{patientId:guid}/invoices")]
    public async Task<ActionResult<ApiResponse<List<Invoice>>>> GetPatientInvoices(Guid patientId, CancellationToken cancellationToken)
    {
        var result = await _billingService.GetPatientInvoicesAsync(patientId, cancellationToken);
        return Ok(ApiResponse<List<Invoice>>.SuccessResponse(result.Value!));
    }
}

