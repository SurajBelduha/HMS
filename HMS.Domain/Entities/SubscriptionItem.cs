namespace HMS.Domain.Entities;

public class SubscriptionItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SubscriptionId { get; set; }
    public string FeatureCode { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal Price { get; set; }

    public TenantSubscription Subscription { get; set; } = null!;
}

