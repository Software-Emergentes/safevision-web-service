namespace SafeVisionPlatform.FatigueMonitoring.Domain.Model.ValueObjects;

/// <summary>
/// Contiene los valores recogidos por los sensores de detección de fatiga,
/// como el índice de parpadeo, apertura bucal y posición de la cabeza.
/// </summary>
public class SensorData
{
    /// <summary>
    /// Índice de parpadeo (frecuencia por minuto).
    /// </summary>
    public double BlinkRate { get; private set; }

    /// <summary>
    /// Porcentaje de apertura de ojos (0-100). Valores bajos indican ojos cerrados.
    /// </summary>
    public double EyeOpenness { get; private set; }

    /// <summary>
    /// Apertura bucal detectada (indicador de bostezo).
    /// </summary>
    public double MouthOpenness { get; private set; }

    /// <summary>
    /// Inclinación de la cabeza en grados.
    /// </summary>
    public double HeadTilt { get; private set; }

    /// <summary>
    /// Duración de ojos cerrados en segundos.
    /// </summary>
    public double EyeClosureDuration { get; private set; }

    /// <summary>
    /// Timestamp de la captura del sensor.
    /// </summary>
    public DateTime CapturedAt { get; private set; }

    private SensorData() { }

    public SensorData(
        double blinkRate,
        double eyeOpenness,
        double mouthOpenness,
        double headTilt,
        double eyeClosureDuration)
    {
        BlinkRate = blinkRate;
        EyeOpenness = Math.Clamp(eyeOpenness, 0, 100);
        MouthOpenness = Math.Clamp(mouthOpenness, 0, 100);
        HeadTilt = headTilt;
        EyeClosureDuration = Math.Max(0, eyeClosureDuration);
        CapturedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Detecta si hay indicios de somnolencia basado en los datos del sensor.
    /// </summary>
    public bool IndicatesDrowsiness()
    {
        return EyeOpenness < 50 || BlinkRate > 25 || EyeClosureDuration > 2;
    }

    /// <summary>
    /// Detecta si hay indicios de bostezo.
    /// </summary>
    public bool IndicatesYawning()
    {
        return MouthOpenness > 60;
    }

    /// <summary>
    /// Detecta si hay indicios de micro-sueño.
    /// </summary>
    public bool IndicatesMicroSleep()
    {
        return EyeClosureDuration >= 3 || (EyeOpenness < 20 && HeadTilt > 15);
    }
}
