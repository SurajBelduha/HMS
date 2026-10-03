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
public class ClinicalController : ControllerBase
{
    private readonly IClinicalService _clinicalService;

    public ClinicalController(IClinicalService clinicalService)
    {
        _clinicalService = clinicalService;
    }

    [HttpPost("consultations")]
    public async Task<ActionResult<ApiResponse<Consultation>>> CreateConsultation([FromBody] CreateConsultationRequest request, CancellationToken cancellationToken)
    {
        var result = await _clinicalService.CreateConsultationAsync(request, cancellationToken);
        if (result.IsFailure)
            return BadRequest(ApiResponse<Consultation>.FailureResponse(result.Error!));

        return Ok(ApiResponse<Consultation>.SuccessResponse(result.Value!, "Consultation completed successfully."));
    }

    [HttpPost("admissions")]
    public async Task<ActionResult<ApiResponse<Admission>>> AdmitPatient([FromBody] AdmitPatientRequest request, CancellationToken cancellationToken)
    {
        var result = await _clinicalService.AdmitPatientAsync(request, cancellationToken);
        if (result.IsFailure)
            return BadRequest(ApiResponse<Admission>.FailureResponse(result.Error!));

        return Ok(ApiResponse<Admission>.SuccessResponse(result.Value!, "Patient admitted successfully."));
    }

    [HttpPost("vitals")]
    public async Task<ActionResult<ApiResponse<Vital>>> RecordVitals([FromBody] Vital vital, CancellationToken cancellationToken)
    {
        var result = await _clinicalService.RecordVitalsAsync(vital, cancellationToken);
        return Ok(ApiResponse<Vital>.SuccessResponse(result.Value!, "Vitals recorded successfully."));
    }

    [HttpGet("patients/{patientId:guid}/consultations")]
    public async Task<ActionResult<ApiResponse<List<Consultation>>>> GetPatientConsultations(Guid patientId, CancellationToken cancellationToken)
    {
        var result = await _clinicalService.GetPatientConsultationsAsync(patientId, cancellationToken);
        return Ok(ApiResponse<List<Consultation>>.SuccessResponse(result.Value!));
    }
}

