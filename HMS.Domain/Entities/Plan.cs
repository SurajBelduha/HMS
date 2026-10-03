namespace HMS.Domain.Entities;

public class Plan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty; // Starter, Professional, Enterprise
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PriceMonthly { get; set; }
    public decimal PriceYearly { get; set; }
    public int TrialDays { get; set; } = 14;
    public string Status { get; set; } = "Active"; // Active, Inactive

    public ICollection<PlanFeature> Features { get; set; } = new List<PlanFeature>();
}

