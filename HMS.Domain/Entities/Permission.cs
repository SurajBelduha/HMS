namespace HMS.Domain.Entities;

public class Permission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Module { get; set; } = string.Empty; // Patient, Billing, Pharmacy, System, etc.
    public string Code { get; set; } = string.Empty;   // Patient.View, Billing.Create, etc.
    public string Name { get; set; } = string.Empty;

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

