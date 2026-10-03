namespace HMS.Application.Contracts;

public interface ISequenceGeneratorService
{
    Task<string> GenerateSequenceNumberAsync<TEntity>(
        string prefix,
        Func<IQueryable<TEntity>, IQueryable<TEntity>> filter,
        int padLeft = 5,
        bool includeYear = true,
        CancellationToken cancellationToken = default) where TEntity : class;
}

