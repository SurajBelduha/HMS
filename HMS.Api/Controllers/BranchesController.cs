using HMS.Application.Contracts;
using HMS.Common.Models;
using HMS.Domain.Entities;
using HMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMS.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class BranchesController : ControllerBase
{
    private readonly HmsDbContext _dbContext;
    private readonly IAuditService _auditService;

    public BranchesController(HmsDbContext dbContext, IAuditService auditService)
    {
        _dbContext = dbContext;
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<ActionResult<Result<List<Branch>>>> GetBranches(CancellationToken cancellationToken)
    {
        var branches = await _dbContext.Branches.ToListAsync(cancellationToken);
        return Ok(Result.Success(branches));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Result<Branch>>> GetBranchById(Guid id, CancellationToken cancellationToken)
    {
        var branch = await _dbContext.Branches.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        if (branch == null)
            return NotFound(Result.Failure<Branch>("Branch not found."));

        return Ok(Result.Success(branch));
    }

    [HttpPost]
    public async Task<ActionResult<Result<Guid>>> CreateBranch([FromBody] Branch branch, CancellationToken cancellationToken)
    {
        _dbContext.Branches.Add(branch);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync("CreateBranch", "System", "Branch", branch.Id.ToString(), null, branch, cancellationToken);

        return Ok(Result.Success(branch.Id));
    }
}

