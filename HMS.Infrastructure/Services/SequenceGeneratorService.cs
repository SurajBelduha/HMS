using HMS.Application.Contracts;
using HMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Services;

public class SequenceGeneratorService : ISequenceGeneratorService
{
    private readonly HmsDbContext _dbContext;

    public SequenceGeneratorService(HmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> GenerateSequenceNumberAsync<TEntity>(
        string prefix,
        Func<IQueryable<TEntity>, IQueryable<TEntity>> filter,
        int padLeft = 5,
        bool includeYear = true,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        var query = _dbContext.Set<TEntity>().IgnoreQueryFilters();
        query = filter(query);

        var count = await query.CountAsync(cancellationToken) + 1;
        var formattedNumber = count.ToString().PadLeft(padLeft, '0');

        if (includeYear)
        {
            var year = DateTime.UtcNow.Year;
            return $"{prefix}-{year}-{formattedNumber}";
        }

        return $"{prefix}-{formattedNumber}";
    }
}

