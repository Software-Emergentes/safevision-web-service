namespace SafeVisionPlatform.FatigueMonitoring.Domain.Model.ValueObjects;

/// <summary>
/// Valor que mide la severidad del evento de fatiga con base en los datos de sensores.
/// Rango de 0.0 (sin fatiga) a 1.0 (fatiga crítica).
/// </summary>
public class SeverityScore
{
    public double Value { get; private set; }
    public string Level { get; private set; } = string.Empty;

    private SeverityScore() { }

    public SeverityScore(double value)
    {
        if (value < 0 || value > 1)
            throw new ArgumentException("El valor de severidad debe estar entre 0 y 1.", nameof(value));

        Value = value;
        Level = DetermineLevel(value);
    }

    private static string DetermineLevel(double value)
    {
        return value switch
        {
            >= 0.85 => "Critical",
            >= 0.70 => "High",
            >= 0.50 => "Medium",
            >= 0.30 => "Low",
            _ => "Minimal"
        };
    }

    public bool IsCritical() => Value >= 0.85;
    public bool IsHigh() => Value >= 0.70;
    public bool RequiresAlert() => Value >= 0.50;

    public override string ToString() => $"{Value:F2} ({Level})";
}
