namespace SafeVisionPlatform.Trip.Domain.Model.Entities;

/// <summary>
/// Entidad que representa la configuración de parámetros de seguridad para detección de fatiga.
/// </summary>
public class SecurityConfiguration
{
    /// <summary>
    /// Identificador único de la configuración.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// ID del gerente o organización que aplica esta configuración.
    /// Null indica configuración global predeterminada.
    /// </summary>
    public int? ManagerId { get; private set; }

    /// <summary>
    /// ID de la flota a la que aplica esta configuración.
    /// Null indica que aplica a todas las flotas del gerente.
    /// </summary>
    public int? FleetId { get; private set; }

    /// <summary>
    /// Nombre descriptivo de la configuración.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Descripción de la configuración y su propósito.
    /// </summary>
    public string? Description { get; private set; }

    // ==================== UMBRALES DE DETECCIÓN DE DROWSINESS ====================

    /// <summary>
    /// Umbral de Eye Aspect Ratio (EAR) para detectar somnolencia.
    /// Valor típico: 0.2 - 0.3 (valores más bajos = ojos más cerrados)
    /// </summary>
    public double DrowsinessEarThreshold { get; private set; }

    /// <summary>
    /// Número de frames consecutivos con EAR bajo para confirmar somnolencia.
    /// Valor típico: 15-30 frames
    /// </summary>
    public int DrowsinessConsecutiveFrames { get; private set; }

    /// <summary>
    /// Duración mínima en segundos de ojos cerrados para generar alerta de somnolencia.
    /// Valor típico: 2-5 segundos
    /// </summary>
    public double DrowsinessMinDurationSeconds { get; private set; }

    // ==================== UMBRALES DE DETECCIÓN DE MICROSLEEP ====================

    /// <summary>
    /// Umbral de EAR para detectar microsueños (generalmente más bajo que drowsiness).
    /// Valor típico: 0.15 - 0.25
    /// </summary>
    public double MicroSleepEarThreshold { get; private set; }

    /// <summary>
    /// Duración mínima en segundos de cierre de ojos para considerar microsueño.
    /// Valor típico: 0.5-2 segundos
    /// </summary>
    public double MicroSleepMinDurationSeconds { get; private set; }

    /// <summary>
    /// Número máximo de microsueños permitidos en una ventana de tiempo antes de generar alerta crítica.
    /// </summary>
    public int MicroSleepMaxOccurrencesPerWindow { get; private set; }

    /// <summary>
    /// Ventana de tiempo en minutos para contar microsueños.
    /// Valor típico: 10-30 minutos
    /// </summary>
    public int MicroSleepWindowMinutes { get; private set; }

    // ==================== UMBRALES DE DETECCIÓN DE DISTRACTION ====================

    /// <summary>
    /// Ángulo máximo de desviación de la mirada (yaw) en grados para considerar distracción.
    /// Valor típico: 20-40 grados
    /// </summary>
    public double DistractionYawThresholdDegrees { get; private set; }

    /// <summary>
    /// Ángulo máximo de desviación de la cabeza (pitch) en grados para considerar distracción.
    /// Valor típico: 15-30 grados
    /// </summary>
    public double DistractionPitchThresholdDegrees { get; private set; }

    /// <summary>
    /// Duración mínima en segundos de mirada desviada para generar alerta de distracción.
    /// Valor típico: 2-5 segundos
    /// </summary>
    public double DistractionMinDurationSeconds { get; private set; }

    // ==================== UMBRALES DE ALERTAS Y NOTIFICACIONES ====================

    /// <summary>
    /// Número de alertas críticas que disparan notificación al gerente.
    /// Valor típico: 3-5 alertas
    /// </summary>
    public int CriticalAlertsThreshold { get; private set; }

    /// <summary>
    /// Ventana de tiempo en minutos para contar alertas críticas.
    /// Valor típico: 10-30 minutos
    /// </summary>
    public int CriticalAlertsWindowMinutes { get; private set; }

    /// <summary>
    /// Tiempo mínimo en minutos entre notificaciones para el mismo conductor.
    /// Evita spam de notificaciones.
    /// Valor típico: 5-15 minutos
    /// </summary>
    public int NotificationCooldownMinutes { get; private set; }

    // ==================== CONFIGURACIÓN DE PUNTUACIÓN DE SEGURIDAD ====================

    /// <summary>
    /// Penalización en puntos por cada alerta de somnolencia.
    /// </summary>
    public int SafetyScoreDrowsinessPenalty { get; private set; }

    /// <summary>
    /// Penalización en puntos por cada alerta de microsueño.
    /// </summary>
    public int SafetyScoreMicroSleepPenalty { get; private set; }

    /// <summary>
    /// Penalización en puntos por cada alerta de distracción.
    /// </summary>
    public int SafetyScoreDistractionPenalty { get; private set; }

    /// <summary>
    /// Bonificación en puntos por viaje completado sin alertas.
    /// </summary>
    public int SafetyScoreSafeTripBonus { get; private set; }

    // ==================== METADATOS ====================

    /// <summary>
    /// Indica si esta configuración está activa.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Indica si esta es la configuración predeterminada del sistema.
    /// </summary>
    public bool IsDefault { get; private set; }

    /// <summary>
    /// Fecha de creación de la configuración.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Fecha de última modificación.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// ID del usuario que creó esta configuración.
    /// </summary>
    public int? CreatedBy { get; private set; }

    /// <summary>
    /// ID del usuario que modificó esta configuración por última vez.
    /// </summary>
    public int? UpdatedBy { get; private set; }

    // Constructor sin parámetros para EF
    protected SecurityConfiguration() { }

    /// <summary>
    /// Constructor para crear una nueva configuración de seguridad.
    /// </summary>
    public SecurityConfiguration(
        string name,
        string? description,
        int? managerId,
        int? fleetId,
        int? createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre de la configuración no puede estar vacío", nameof(name));

        Name = name;
        Description = description;
        ManagerId = managerId;
        FleetId = fleetId;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
        IsDefault = false;

        // Establecer valores predeterminados seguros
        SetDefaultThresholds();
    }

    /// <summary>
    /// Establece los umbrales predeterminados basados en mejores prácticas de seguridad.
    /// </summary>
    private void SetDefaultThresholds()
    {
        // Drowsiness defaults
        DrowsinessEarThreshold = 0.25;
        DrowsinessConsecutiveFrames = 20;
        DrowsinessMinDurationSeconds = 3.0;

        // MicroSleep defaults
        MicroSleepEarThreshold = 0.20;
        MicroSleepMinDurationSeconds = 1.0;
        MicroSleepMaxOccurrencesPerWindow = 3;
        MicroSleepWindowMinutes = 15;

        // Distraction defaults
        DistractionYawThresholdDegrees = 30.0;
        DistractionPitchThresholdDegrees = 25.0;
        DistractionMinDurationSeconds = 3.0;

        // Critical alerts defaults
        CriticalAlertsThreshold = 3;
        CriticalAlertsWindowMinutes = 15;
        NotificationCooldownMinutes = 10;

        // Safety score defaults
        SafetyScoreDrowsinessPenalty = 10;
        SafetyScoreMicroSleepPenalty = 15;
        SafetyScoreDistractionPenalty = 5;
        SafetyScoreSafeTripBonus = 10;
    }

    /// <summary>
    /// Actualiza los umbrales de detección de somnolencia.
    /// </summary>
    public void UpdateDrowsinessThresholds(
        double earThreshold,
        int consecutiveFrames,
        double minDurationSeconds,
        int updatedBy)
    {
        ValidateEarThreshold(earThreshold, nameof(earThreshold));
        ValidatePositiveValue(consecutiveFrames, nameof(consecutiveFrames));
        ValidatePositiveValue(minDurationSeconds, nameof(minDurationSeconds));

        DrowsinessEarThreshold = earThreshold;
        DrowsinessConsecutiveFrames = consecutiveFrames;
        DrowsinessMinDurationSeconds = minDurationSeconds;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Actualiza los umbrales de detección de microsueño.
    /// </summary>
    public void UpdateMicroSleepThresholds(
        double earThreshold,
        double minDurationSeconds,
        int maxOccurrencesPerWindow,
        int windowMinutes,
        int updatedBy)
    {
        ValidateEarThreshold(earThreshold, nameof(earThreshold));
        ValidatePositiveValue(minDurationSeconds, nameof(minDurationSeconds));
        ValidatePositiveValue(maxOccurrencesPerWindow, nameof(maxOccurrencesPerWindow));
        ValidatePositiveValue(windowMinutes, nameof(windowMinutes));

        MicroSleepEarThreshold = earThreshold;
        MicroSleepMinDurationSeconds = minDurationSeconds;
        MicroSleepMaxOccurrencesPerWindow = maxOccurrencesPerWindow;
        MicroSleepWindowMinutes = windowMinutes;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Actualiza los umbrales de detección de distracción.
    /// </summary>
    public void UpdateDistractionThresholds(
        double yawThresholdDegrees,
        double pitchThresholdDegrees,
        double minDurationSeconds,
        int updatedBy)
    {
        ValidateAngleThreshold(yawThresholdDegrees, nameof(yawThresholdDegrees));
        ValidateAngleThreshold(pitchThresholdDegrees, nameof(pitchThresholdDegrees));
        ValidatePositiveValue(minDurationSeconds, nameof(minDurationSeconds));

        DistractionYawThresholdDegrees = yawThresholdDegrees;
        DistractionPitchThresholdDegrees = pitchThresholdDegrees;
        DistractionMinDurationSeconds = minDurationSeconds;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Actualiza los umbrales de alertas críticas.
    /// </summary>
    public void UpdateCriticalAlertsThresholds(
        int criticalAlertsThreshold,
        int criticalAlertsWindowMinutes,
        int notificationCooldownMinutes,
        int updatedBy)
    {
        ValidatePositiveValue(criticalAlertsThreshold, nameof(criticalAlertsThreshold));
        ValidatePositiveValue(criticalAlertsWindowMinutes, nameof(criticalAlertsWindowMinutes));
        ValidatePositiveValue(notificationCooldownMinutes, nameof(notificationCooldownMinutes));

        CriticalAlertsThreshold = criticalAlertsThreshold;
        CriticalAlertsWindowMinutes = criticalAlertsWindowMinutes;
        NotificationCooldownMinutes = notificationCooldownMinutes;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Actualiza la configuración de puntuación de seguridad.
    /// </summary>
    public void UpdateSafetyScoreConfiguration(
        int drowsinessPenalty,
        int microSleepPenalty,
        int distractionPenalty,
        int safeTripBonus,
        int updatedBy)
    {
        ValidatePositiveValue(drowsinessPenalty, nameof(drowsinessPenalty));
        ValidatePositiveValue(microSleepPenalty, nameof(microSleepPenalty));
        ValidatePositiveValue(distractionPenalty, nameof(distractionPenalty));
        ValidatePositiveValue(safeTripBonus, nameof(safeTripBonus));

        SafetyScoreDrowsinessPenalty = drowsinessPenalty;
        SafetyScoreMicroSleepPenalty = microSleepPenalty;
        SafetyScoreDistractionPenalty = distractionPenalty;
        SafetyScoreSafeTripBonus = safeTripBonus;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Marca esta configuración como predeterminada del sistema.
    /// </summary>
    public void MarkAsDefault()
    {
        IsDefault = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activa la configuración.
    /// </summary>
    public void Activate(int updatedBy)
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Desactiva la configuración.
    /// </summary>
    public void Deactivate(int updatedBy)
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    // ==================== MÉTODOS DE VALIDACIÓN ====================

    private void ValidateEarThreshold(double value, string paramName)
    {
        if (value < 0.1 || value > 0.5)
            throw new ArgumentException($"El umbral EAR debe estar entre 0.1 y 0.5", paramName);
    }

    private void ValidateAngleThreshold(double value, string paramName)
    {
        if (value < 0 || value > 90)
            throw new ArgumentException($"El ángulo debe estar entre 0 y 90 grados", paramName);
    }

    private void ValidatePositiveValue(double value, string paramName)
    {
        if (value <= 0)
            throw new ArgumentException($"El valor debe ser positivo", paramName);
    }

    private void ValidatePositiveValue(int value, string paramName)
    {
        if (value <= 0)
            throw new ArgumentException($"El valor debe ser positivo", paramName);
    }
}
