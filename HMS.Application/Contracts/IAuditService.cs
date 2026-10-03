namespace HMS.Application.Contracts;

public interface IAuditService
{
    Task LogAsync(string action, string module, string entityName, string entityId, object? oldValues = null, object? newValues = null, CancellationToken cancellationToken = default);
}

