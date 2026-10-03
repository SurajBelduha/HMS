namespace HMS.Domain.Contracts;

/// <summary>
/// Indicates that an entity belongs to a specific Hospital Branch.
/// </summary>
public interface IBranchScoped
{
    public Guid? BranchId { get; set; }
}

