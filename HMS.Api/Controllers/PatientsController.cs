using HMS.Api.Attributes;
using HMS.Application.Contracts;
using HMS.Application.DTOs.Phase2;
using HMS.Common.Models;
using HMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpPost]
    [HasPermission("Patient.Create")]
    public async Task<ActionResult<ApiResponse<Patient>>> RegisterPatient([FromBody] RegisterPatientRequest request, CancellationToken cancellationToken)
    {
        var result = await _patientService.RegisterPatientAsync(request, cancellationToken);
        if (result.IsFailure)
            return BadRequest(ApiResponse<Patient>.FailureResponse(result.Error!));

        return Ok(ApiResponse<Patient>.SuccessResponse(result.Value!, "Patient registered successfully."));
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Patient.View")]
    public async Task<ActionResult<ApiResponse<Patient>>> GetPatientById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _patientService.GetPatientByIdAsync(id, cancellationToken);
        if (result.IsFailure)
            return NotFound(ApiResponse<Patient>.FailureResponse(result.Error!));

        return Ok(ApiResponse<Patient>.SuccessResponse(result.Value!));
    }

    [HttpGet]
    [HasPermission("Patient.View")]
    public async Task<ActionResult<ApiResponse<PagedResult<Patient>>>> SearchPatients([FromQuery] PagedRequest request, CancellationToken cancellationToken)
    {
        var result = await _patientService.SearchPatientsAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<Patient>>.SuccessResponse(result.Value!));
    }
}

