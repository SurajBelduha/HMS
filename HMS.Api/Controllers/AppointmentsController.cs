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
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Appointment>>> BookAppointment([FromBody] BookAppointmentRequest request, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.BookAppointmentAsync(request, cancellationToken);
        if (result.IsFailure)
            return BadRequest(ApiResponse<Appointment>.FailureResponse(result.Error!));

        return Ok(ApiResponse<Appointment>.SuccessResponse(result.Value!, "Appointment booked successfully with OPD Token #" + result.Value!.TokenNumber));
    }

    [HttpPost("reschedule")]
    public async Task<ActionResult<ApiResponse<Appointment>>> RescheduleAppointment([FromBody] RescheduleAppointmentRequest request, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.RescheduleAppointmentAsync(request, cancellationToken);
        if (result.IsFailure)
            return BadRequest(ApiResponse<Appointment>.FailureResponse(result.Error!));

        return Ok(ApiResponse<Appointment>.SuccessResponse(result.Value!, "Appointment rescheduled successfully."));
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<ApiResponse<object>>> CancelAppointment(Guid id, [FromQuery] string? reason, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.CancelAppointmentAsync(id, reason, cancellationToken);
        if (result.IsFailure)
            return BadRequest(ApiResponse<object>.FailureResponse(result.Error!));

        return Ok(ApiResponse<object>.SuccessResponse(null!, "Appointment cancelled successfully."));
    }

    [HttpGet("doctor/{doctorId:guid}")]
    public async Task<ActionResult<ApiResponse<List<Appointment>>>> GetDoctorAppointments(Guid doctorId, [FromQuery] DateTime? date, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.GetDoctorAppointmentsAsync(doctorId, date, cancellationToken);
        return Ok(ApiResponse<List<Appointment>>.SuccessResponse(result.Value!));
    }
}

