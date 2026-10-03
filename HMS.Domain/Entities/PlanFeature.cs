namespace HMS.Domain.Entities;

public class PlanFeature
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PlanId { get; set; }
    public string FeatureCode { get; set; } = string.Empty; // MaxBranches, MaxUsers, MaxPatients, Pharmacy, Lab, Storage, SMS, etc.
    public string LimitType { get; set; } = "Numeric";      // Numeric, Boolean, Unlimited
    public string LimitValue { get; set; } = "0";

    public Plan Plan { get; set; } = null!;
}

