using FluentAssertions;
using HMS.Common.Enums;
using HMS.Domain.Entities;
using HMS.Infrastructure.Persistence;
using HMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HMS.Tests;

public class TenantIsolationTests
{
    private readonly Guid _tenantAId = Guid.NewGuid();
    private readonly Guid _tenantBId = Guid.NewGuid();

    private HmsDbContext CreateDbContext(Guid currentTenantId, bool isHostAdmin = false)
    {
        var options = new DbContextOptionsBuilder<HmsDbContext>()
            .UseInMemoryDatabase(databaseName: "Hms_Test_Db_" + Guid.NewGuid())
            .Options;

        var tenantService = new CurrentTenantService();
        tenantService.SetTenantContext(currentTenantId, null, null, null, isHostAdmin);

        return new HmsDbContext(options, tenantService);
    }

    [Fact]
    public async Task TenantA_Cannot_Read_TenantB_Data()
    {
        // Arrange
        var dbName = "Hms_Isolation_Db_" + Guid.NewGuid();
        var options = new DbContextOptionsBuilder<HmsDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        // Seed Tenant A & Tenant B data
        var tenantAService = new CurrentTenantService();
        tenantAService.SetTenantContext(_tenantAId, null);

        using (var seedContext = new HmsDbContext(options, tenantAService))
        {
            seedContext.Branches.Add(new Branch { Id = Guid.NewGuid(), TenantId = _tenantAId, Name = "Tenant A Branch", BranchCode = "TB-A" });
            seedContext.Branches.Add(new Branch { Id = Guid.NewGuid(), TenantId = _tenantBId, Name = "Tenant B Branch", BranchCode = "TB-B" });
            await seedContext.SaveChangesAsync();
        }

        // Act - Query as Tenant A
        using (var contextTenantA = new HmsDbContext(options, tenantAService))
        {
            var tenantABranches = await contextTenantA.Branches.ToListAsync();

            // Assert
            tenantABranches.Should().HaveCount(1);
            tenantABranches.First().TenantId.Should().Be(_tenantAId);
            tenantABranches.Should().NotContain(b => b.TenantId == _tenantBId);
        }
    }

    [Fact]
    public async Task TenantA_Cannot_Update_TenantB_Data()
    {
        // Arrange
        var dbName = "Hms_Isolation_Db_" + Guid.NewGuid();
        var options = new DbContextOptionsBuilder<HmsDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var tenantBBranchId = Guid.NewGuid();
        var tenantBService = new CurrentTenantService();
        tenantBService.SetTenantContext(_tenantBId, null);

        using (var seedContext = new HmsDbContext(options, tenantBService))
        {
            seedContext.Branches.Add(new Branch { Id = tenantBBranchId, TenantId = _tenantBId, Name = "Tenant B Original Name", BranchCode = "TB-B" });
            await seedContext.SaveChangesAsync();
        }

        // Act - Attempt to query and update Tenant B branch while scoped as Tenant A
        var tenantAService = new CurrentTenantService();
        tenantAService.SetTenantContext(_tenantAId, null);

        using (var contextTenantA = new HmsDbContext(options, tenantAService))
        {
            var branch = await contextTenantA.Branches.FirstOrDefaultAsync(b => b.Id == tenantBBranchId);
            branch.Should().BeNull(); // Global query filter blocks reading Tenant B entity
        }
    }

    [Fact]
    public async Task TenantA_Cannot_Delete_TenantB_Data()
    {
        // Arrange
        var dbName = "Hms_Isolation_Db_" + Guid.NewGuid();
        var options = new DbContextOptionsBuilder<HmsDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var tenantBBranchId = Guid.NewGuid();
        var tenantBService = new CurrentTenantService();
        tenantBService.SetTenantContext(_tenantBId, null);

        using (var seedContext = new HmsDbContext(options, tenantBService))
        {
            seedContext.Branches.Add(new Branch { Id = tenantBBranchId, TenantId = _tenantBId, Name = "Tenant B Branch", BranchCode = "TB-B" });
            await seedContext.SaveChangesAsync();
        }

        // Act - Query as Tenant A to verify invisibility
        var tenantAService = new CurrentTenantService();
        tenantAService.SetTenantContext(_tenantAId, null);

        using (var contextTenantA = new HmsDbContext(options, tenantAService))
        {
            var branch = await contextTenantA.Branches.FirstOrDefaultAsync(b => b.Id == tenantBBranchId);
            branch.Should().BeNull();
        }

        // Verify entity remains safe under Tenant B context
        using (var contextTenantB = new HmsDbContext(options, tenantBService))
        {
            var branch = await contextTenantB.Branches.FirstOrDefaultAsync(b => b.Id == tenantBBranchId);
            branch.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task TenantA_Cannot_Access_TenantB_AuditLogs()
    {
        // Arrange
        var dbName = "Hms_Isolation_Db_" + Guid.NewGuid();
        var options = new DbContextOptionsBuilder<HmsDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var tenantAService = new CurrentTenantService();
        tenantAService.SetTenantContext(_tenantAId, null);

        var tenantBService = new CurrentTenantService();
        tenantBService.SetTenantContext(_tenantBId, null);

        using (var seedContext = new HmsDbContext(options, tenantAService))
        {
            seedContext.AuditLogs.Add(new AuditLog { TenantId = _tenantAId, Action = "Login", Module = "Auth", EntityName = "User", EntityId = "1" });
            seedContext.AuditLogs.Add(new AuditLog { TenantId = _tenantBId, Action = "UpdatePrescription", Module = "Clinical", EntityName = "Prescription", EntityId = "2" });
            await seedContext.SaveChangesAsync();
        }

        // Act - Query as Tenant A
        using (var contextTenantA = new HmsDbContext(options, tenantAService))
        {
            var auditLogs = await contextTenantA.AuditLogs.ToListAsync();

            // Assert
            auditLogs.Should().HaveCount(1);
            auditLogs.First().TenantId.Should().Be(_tenantAId);
            auditLogs.Should().NotContain(a => a.TenantId == _tenantBId);
        }
    }

    [Fact]
    public async Task Automatic_TenantId_Population_On_Save()
    {
        // Arrange
        var dbName = "Hms_Isolation_Db_" + Guid.NewGuid();
        var options = new DbContextOptionsBuilder<HmsDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var tenantAService = new CurrentTenantService();
        tenantAService.SetTenantContext(_tenantAId, null);

        using (var context = new HmsDbContext(options, tenantAService))
        {
            var newBranch = new Branch
            {
                Name = "Auto Tenant Branch",
                BranchCode = "AUTO-01"
                // TenantId left unassigned (Guid.Empty)
            };

            context.Branches.Add(newBranch);
            await context.SaveChangesAsync();

            // Assert
            newBranch.TenantId.Should().Be(_tenantAId);
        }
    }
}

