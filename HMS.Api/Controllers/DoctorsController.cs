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
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorsController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Doctor>>> CreateDoctor([FromBody] CreateDoctorRequest request, CancellationToken cancellationToken)
    {
        var result = await _doctorService.CreateDoctorAsync(request, cancellationToken);
        if (result.IsFailure)
            return BadRequest(ApiResponse<Doctor>.FailureResponse(result.Error!));

        return Ok(ApiResponse<Doctor>.SuccessResponse(result.Value!, "Doctor created successfully."));
    }

    [HttpPost("schedule")]
    public async Task<ActionResult<ApiResponse<DoctorSchedule>>> AddSchedule([FromBody] CreateDoctorScheduleRequest request, CancellationToken cancellationToken)
    {
        var result = await _doctorService.AddDoctorScheduleAsync(request, cancellationToken);
        if (result.IsFailure)
            return BadRequest(ApiResponse<DoctorSchedule>.FailureResponse(result.Error!));

        return Ok(ApiResponse<DoctorSchedule>.SuccessResponse(result.Value!, "Schedule added successfully."));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Doctor>>>> GetDoctors(CancellationToken cancellationToken)
    {
        var result = await _doctorService.GetDoctorsAsync(cancellationToken);
        return Ok(ApiResponse<List<Doctor>>.SuccessResponse(result.Value!));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<Doctor>>> GetDoctorById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _doctorService.GetDoctorByIdAsync(id, cancellationToken);
        if (result.IsFailure)
            return NotFound(ApiResponse<Doctor>.FailureResponse(result.Error!));

        return Ok(ApiResponse<Doctor>.SuccessResponse(result.Value!));
    }
}

