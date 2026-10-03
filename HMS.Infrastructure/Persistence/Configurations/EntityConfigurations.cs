using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Infrastructure.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        builder.HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);

        builder.HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);
    }
}

public class DoctorDepartmentConfiguration : IEntityTypeConfiguration<DoctorDepartment>
{
    public void Configure(EntityTypeBuilder<DoctorDepartment> builder)
    {
        builder.HasKey(dd => new { dd.DoctorId, dd.DepartmentId });

        builder.HasOne(dd => dd.Doctor)
            .WithMany(d => d.DoctorDepartments)
            .HasForeignKey(dd => dd.DoctorId);

        builder.HasOne(dd => dd.Department)
            .WithMany()
            .HasForeignKey(dd => dd.DepartmentId);
    }
}

