namespace HMS.Common.Enums;

public enum TenantIsolationStrategy
{
    /// <summary>
    /// Shared Database and Shared Schema with TenantId row-level filtering.
    /// Used for Small/Normal Hospitals.
    /// </summary>
    SharedDatabase = 0,

    /// <summary>
    /// Dedicated Database per Tenant (Hospital).
    /// Used for Large/Enterprise Hospitals.
    /// </summary>
    DedicatedDatabase = 1
}

