namespace SafeVisionPlatform.Trip.Application.Internal.DTO;

/// <summary>
/// DTO para crear una nueva configuración de seguridad.
/// </summary>
public class CreateSecurityConfigurationDTO
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ManagerId { get; set; }
    public int? FleetId { get; set; }
    public int? CreatedBy { get; set; }
}

/// <summary>
/// DTO para respuesta de configuración de seguridad completa.
/// </summary>
public class SecurityConfigurationDTO
{
    public int Id { get; set; }
    public int? ManagerId { get; set; }
    public int? FleetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Umbrales de Drowsiness
    public double DrowsinessEarThreshold { get; set; }
    public int DrowsinessConsecutiveFrames { get; set; }
    public double DrowsinessMinDurationSeconds { get; set; }

    // Umbrales de MicroSleep
    public double MicroSleepEarThreshold { get; set; }
    public double MicroSleepMinDurationSeconds { get; set; }
    public int MicroSleepMaxOccurrencesPerWindow { get; set; }
    public int MicroSleepWindowMinutes { get; set; }

    // Umbrales de Distraction
    public double DistractionYawThresholdDegrees { get; set; }
    public double DistractionPitchThresholdDegrees { get; set; }
    public double DistractionMinDurationSeconds { get; set; }

    // Umbrales de Alertas Críticas
    public int CriticalAlertsThreshold { get; set; }
    public int CriticalAlertsWindowMinutes { get; set; }
    public int NotificationCooldownMinutes { get; set; }

    // Configuración de Safety Score
    public int SafetyScoreDrowsinessPenalty { get; set; }
    public int SafetyScoreMicroSleepPenalty { get; set; }
    public int SafetyScoreDistractionPenalty { get; set; }
    public int SafetyScoreSafeTripBonus { get; set; }

    // Metadatos
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
}

/// <summary>
/// DTO para actualizar umbrales de detección de somnolencia.
/// </summary>
public class UpdateDrowsinessThresholdsDTO
{
    public int ConfigurationId { get; set; }
    public double EarThreshold { get; set; }
    public int ConsecutiveFrames { get; set; }
    public double MinDurationSeconds { get; set; }
    public int UpdatedBy { get; set; }
}

/// <summary>
/// DTO para actualizar umbrales de detección de microsueño.
/// </summary>
public class UpdateMicroSleepThresholdsDTO
{
    public int ConfigurationId { get; set; }
    public double EarThreshold { get; set; }
    public double MinDurationSeconds { get; set; }
    public int MaxOccurrencesPerWindow { get; set; }
    public int WindowMinutes { get; set; }
    public int UpdatedBy { get; set; }
}

/// <summary>
/// DTO para actualizar umbrales de detección de distracción.
/// </summary>
public class UpdateDistractionThresholdsDTO
{
    public int ConfigurationId { get; set; }
    public double YawThresholdDegrees { get; set; }
    public double PitchThresholdDegrees { get; set; }
    public double MinDurationSeconds { get; set; }
    public int UpdatedBy { get; set; }
}

/// <summary>
/// DTO para actualizar umbrales de alertas críticas.
/// </summary>
public class UpdateCriticalAlertsThresholdsDTO
{
    public int ConfigurationId { get; set; }
    public int CriticalAlertsThreshold { get; set; }
    public int CriticalAlertsWindowMinutes { get; set; }
    public int NotificationCooldownMinutes { get; set; }
    public int UpdatedBy { get; set; }
}

/// <summary>
/// DTO para actualizar configuración de puntuación de seguridad.
/// </summary>
public class UpdateSafetyScoreConfigurationDTO
{
    public int ConfigurationId { get; set; }
    public int DrowsinessPenalty { get; set; }
    public int MicroSleepPenalty { get; set; }
    public int DistractionPenalty { get; set; }
    public int SafeTripBonus { get; set; }
    public int UpdatedBy { get; set; }
}

/// <summary>
/// DTO para respuesta de operación sobre configuración.
/// </summary>
public class ConfigurationOperationResponseDTO
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? ConfigurationId { get; set; }
    public DateTime OperationTimestamp { get; set; }
}

/// <summary>
/// DTO resumido para listado de configuraciones.
/// </summary>
public class SecurityConfigurationSummaryDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ManagerId { get; set; }
    public int? FleetId { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
