namespace HMS.Application.Contracts;

public interface ITenantDbContextFactory<TContext> where TContext : class
{
    TContext CreateDbContext();
}

