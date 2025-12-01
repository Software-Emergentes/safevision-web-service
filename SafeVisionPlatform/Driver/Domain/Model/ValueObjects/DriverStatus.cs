namespace SafeVisionPlatform.Driver.Domain.Model.ValueObjects;

/// <summary>
/// Value Object que define los estados válidos del conductor.
/// Asegura la consistencia en las transiciones de estado.
/// </summary>
public class DriverStatus
{
    public enum Status
    {
        Active = 1,
        Inactive = 2,
        Suspended = 3,
        InTrip = 4
    }

    public Status Value { get; private set; }

    // Constructor para EF Core - debe tener parámetro que coincida con propiedad mapeada
    public DriverStatus(Status value)
    {
        Value = value;
    }

    // Constructor privado para los métodos factory
    private DriverStatus() : this(Status.Inactive)
    {
    }

    public static DriverStatus Active() => new(Status.Active);
    public static DriverStatus Inactive() => new(Status.Inactive);
    public static DriverStatus Suspended() => new(Status.Suspended);
    public static DriverStatus InTrip() => new(Status.InTrip);

    public static DriverStatus FromValue(int value)
    {
        return value switch
        {
            1 => Active(),
            2 => Inactive(),
            3 => Suspended(),
            4 => InTrip(),
            _ => throw new ArgumentException($"Status value {value} is not valid")
        };
    }

    public bool IsActive() => Value == Status.Active;
    public bool IsInactive() => Value == Status.Inactive;
    public bool IsSuspended() => Value == Status.Suspended;
    public bool IsInTrip() => Value == Status.InTrip;

    public override bool Equals(object? obj)
    {
        if (obj is not DriverStatus other) return false;
        return Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
