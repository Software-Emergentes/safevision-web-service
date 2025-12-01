using SafeVisionPlatform.Shared.Domain.Repositories;
using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Domain.Model.Entities;
using SafeVisionPlatform.Trip.Domain.Repositories;
using SafeVisionPlatform.Trip.Domain.Services;

namespace SafeVisionPlatform.Trip.Application.Internal.Services;

/// <summary>
/// Implementación del servicio de configuración de parámetros de seguridad.
/// </summary>
public class SecurityConfigurationService : ISecurityConfigurationService
{
    private readonly ISecurityConfigurationRepository _configRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SecurityConfigurationService> _logger;

    public SecurityConfigurationService(
        ISecurityConfigurationRepository configRepository,
        IUnitOfWork unitOfWork,
        ILogger<SecurityConfigurationService> logger)
    {
        _configRepository = configRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ConfigurationOperationResponseDTO> CreateConfigurationAsync(CreateSecurityConfigurationDTO configDto)
    {
        try
        {
            var configuration = new SecurityConfiguration(
                configDto.Name,
                configDto.Description,
                configDto.ManagerId,
                configDto.FleetId,
                configDto.CreatedBy
            );

            await _configRepository.AddAsync(configuration);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Configuración de seguridad '{configuration.Name}' creada con ID {configuration.Id}");

            return new ConfigurationOperationResponseDTO
            {
                Success = true,
                Message = "Configuración de seguridad creada exitosamente",
                ConfigurationId = configuration.Id,
                OperationTimestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al crear configuración: {ex.Message}");
            return new ConfigurationOperationResponseDTO
            {
                Success = false,
                Message = $"Error al crear configuración: {ex.Message}",
                OperationTimestamp = DateTime.UtcNow
            };
        }
    }

    public async Task<SecurityConfigurationDTO?> GetConfigurationByIdAsync(int id)
    {
        var config = await _configRepository.FindByIdAsync(id);
        return config != null ? MapToDTO(config) : null;
    }

    public async Task<SecurityConfigurationDTO?> GetActiveConfigurationForManagerAsync(int managerId)
    {
        var config = await _configRepository.GetActiveConfigurationByManagerIdAsync(managerId);

        // Si no existe configuración para el gerente, retornar la predeterminada
        if (config == null)
        {
            config = await _configRepository.GetDefaultConfigurationAsync();
        }

        return config != null ? MapToDTO(config) : null;
    }

    public async Task<SecurityConfigurationDTO?> GetActiveConfigurationForFleetAsync(int fleetId, int? managerId = null)
    {
        // Prioridad 1: Configuración específica de la flota
        var config = await _configRepository.GetActiveConfigurationByFleetIdAsync(fleetId);

        // Prioridad 2: Configuración del gerente
        if (config == null && managerId.HasValue)
        {
            config = await _configRepository.GetActiveConfigurationByManagerIdAsync(managerId.Value);
        }

        // Prioridad 3: Configuración predeterminada
        if (config == null)
        {
            config = await _configRepository.GetDefaultConfigurationAsync();
        }

        return config != null ? MapToDTO(config) : null;
    }

    public async Task<IEnumerable<SecurityConfigurationSummaryDTO>> GetAllConfigurationsByManagerAsync(int managerId)
    {
        var configs = await _configRepository.GetAllByManagerIdAsync(managerId);
        return configs.Select(MapToSummaryDTO);
    }

    public async Task<IEnumerable<SecurityConfigurationSummaryDTO>> GetAllActiveConfigurationsAsync()
    {
        var configs = await _configRepository.GetAllActiveConfigurationsAsync();
        return configs.Select(MapToSummaryDTO);
    }

    public async Task<SecurityConfigurationDTO?> GetDefaultConfigurationAsync()
    {
        var config = await _configRepository.GetDefaultConfigurationAsync();
        return config != null ? MapToDTO(config) : null;
    }

    public async Task<ConfigurationOperationResponseDTO> UpdateDrowsinessThresholdsAsync(UpdateDrowsinessThresholdsDTO dto)
    {
        try
        {
            var config = await _configRepository.FindByIdAsync(dto.ConfigurationId);
            if (config == null)
            {
                return new ConfigurationOperationResponseDTO
                {
                    Success = false,
                    Message = "Configuración no encontrada",
                    OperationTimestamp = DateTime.UtcNow
                };
            }

            config.UpdateDrowsinessThresholds(
                dto.EarThreshold,
                dto.ConsecutiveFrames,
                dto.MinDurationSeconds,
                dto.UpdatedBy
            );

            await _configRepository.UpdateAsync(config);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Umbrales de somnolencia actualizados en configuración {dto.ConfigurationId}");

            return new ConfigurationOperationResponseDTO
            {
                Success = true,
                Message = "Umbrales de somnolencia actualizados exitosamente",
                ConfigurationId = config.Id,
                OperationTimestamp = DateTime.UtcNow
            };
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning($"Validación fallida al actualizar umbrales: {ex.Message}");
            return new ConfigurationOperationResponseDTO
            {
                Success = false,
                Message = ex.Message,
                OperationTimestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al actualizar umbrales de somnolencia: {ex.Message}");
            return new ConfigurationOperationResponseDTO
            {
                Success = false,
                Message = $"Error al actualizar: {ex.Message}",
                OperationTimestamp = DateTime.UtcNow
            };
        }
    }

    public async Task<ConfigurationOperationResponseDTO> UpdateMicroSleepThresholdsAsync(UpdateMicroSleepThresholdsDTO dto)
    {
        try
        {
            var config = await _configRepository.FindByIdAsync(dto.ConfigurationId);
            if (config == null)
            {
                return new ConfigurationOperationResponseDTO
                {
                    Success = false,
                    Message = "Configuración no encontrada",
                    OperationTimestamp = DateTime.UtcNow
                };
            }

            config.UpdateMicroSleepThresholds(
                dto.EarThreshold,
                dto.MinDurationSeconds,
                dto.MaxOccurrencesPerWindow,
                dto.WindowMinutes,
                dto.UpdatedBy
            );

            await _configRepository.UpdateAsync(config);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Umbrales de microsueño actualizados en configuración {dto.ConfigurationId}");

            return new ConfigurationOperationResponseDTO
            {
                Success = true,
                Message = "Umbrales de microsueño actualizados exitosamente",
                ConfigurationId = config.Id,
                OperationTimestamp = DateTime.UtcNow
            };
        }
        catch (ArgumentException ex)
        {
            return new ConfigurationOperationResponseDTO
            {
                Success = false,
                Message = ex.Message,
                OperationTimestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al actualizar umbrales de microsueño: {ex.Message}");
            return new ConfigurationOperationResponseDTO
            {
                Success = false,
                Message = $"Error al actualizar: {ex.Message}",
                OperationTimestamp = DateTime.UtcNow
            };
        }
    }

    public async Task<ConfigurationOperationResponseDTO> UpdateDistractionThresholdsAsync(UpdateDistractionThresholdsDTO dto)
    {
        try
        {
            var config = await _configRepository.FindByIdAsync(dto.ConfigurationId);
            if (config == null)
            {
                return new ConfigurationOperationResponseDTO
                {
                    Success = false,
                    Message = "Configuración no encontrada",
                    OperationTimestamp = DateTime.UtcNow
                };
            }

            config.UpdateDistractionThresholds(
                dto.YawThresholdDegrees,
                dto.PitchThresholdDegrees,
                dto.MinDurationSeconds,
                dto.UpdatedBy
            );

            await _configRepository.UpdateAsync(config);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Umbrales de distracción actualizados en configuración {dto.ConfigurationId}");

            return new ConfigurationOperationResponseDTO
            {
                Success = true,
                Message = "Umbrales de distracción actualizados exitosamente",
                ConfigurationId = config.Id,
                OperationTimestamp = DateTime.UtcNow
            };
        }
        catch (ArgumentException ex)
        {
            return new ConfigurationOperationResponseDTO
            {
                Success = false,
                Message = ex.Message,
                OperationTimestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al actualizar umbrales de distracción: {ex.Message}");
            return new ConfigurationOperationResponseDTO
            {
                Success = false,
                Message = $"Error al actualizar: {ex.Message}",
                OperationTimestamp = DateTime.UtcNow
            };
        }
    }

    public async Task<ConfigurationOperationResponseDTO> UpdateCriticalAlertsThresholdsAsync(UpdateCriticalAlertsThresholdsDTO dto)
    {
        try
        {
            var config = await _configRepository.FindByIdAsync(dto.ConfigurationId);
            if (config == null)
            {
                return new ConfigurationOperationResponseDTO
                {
                    Success = false,
                    Message = "Configuración no encontrada",
                    OperationTimestamp = DateTime.UtcNow
                };
            }

            config.UpdateCriticalAlertsThresholds(
                dto.CriticalAlertsThreshold,
                dto.CriticalAlertsWindowMinutes,
                dto.NotificationCooldownMinutes,
                dto.UpdatedBy
            );

            await _configRepository.UpdateAsync(config);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Umbrales de alertas críticas actualizados en configuración {dto.ConfigurationId}");

            return new ConfigurationOperationResponseDTO
            {
                Success = true,
                Message = "Umbrales de alertas críticas actualizados exitosamente",
                ConfigurationId = config.Id,
                OperationTimestamp = DateTime.UtcNow
            };
        }
        catch (ArgumentException ex)
        {
            return new ConfigurationOperationResponseDTO
            {
                Success = false,
                Message = ex.Message,
                OperationTimestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al actualizar umbrales de alertas críticas: {ex.Message}");
            return new ConfigurationOperationResponseDTO
            {
                Success = false,
                Message = $"Error al actualizar: {ex.Message}",
                OperationTimestamp = DateTime.UtcNow
            };
        }
    }

    public async Task<ConfigurationOperationResponseDTO> UpdateSafetyScoreConfigurationAsync(UpdateSafetyScoreConfigurationDTO dto)
    {
        try
        {
            var config = await _configRepository.FindByIdAsync(dto.ConfigurationId);
            if (config == null)
            {
                return new ConfigurationOperationResponseDTO
                {
                    Success = false,
                    Message = "Configuración no encontrada",
                    OperationTimestamp = DateTime.UtcNow
                };
            }

            config.UpdateSafetyScoreConfiguration(
                dto.DrowsinessPenalty,
                dto.MicroSleepPenalty,
                dto.DistractionPenalty,
                dto.SafeTripBonus,
                dto.UpdatedBy
            );

            await _configRepository.UpdateAsync(config);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Configuración de puntuación de seguridad actualizada en configuración {dto.ConfigurationId}");

            return new ConfigurationOperationResponseDTO
            {
                Success = true,
                Message = "Configuración de puntuación de seguridad actualizada exitosamente",
                ConfigurationId = config.Id,
                OperationTimestamp = DateTime.UtcNow
            };
        }
        catch (ArgumentException ex)
        {
            return new ConfigurationOperationResponseDTO
            {
                Success = false,
                Message = ex.Message,
                OperationTimestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al actualizar configuración de puntuación: {ex.Message}");
            return new ConfigurationOperationResponseDTO
            {
                Success = false,
                Message = $"Error al actualizar: {ex.Message}",
                OperationTimestamp = DateTime.UtcNow
            };
        }
    }

    public async Task<ConfigurationOperationResponseDTO> ActivateConfigurationAsync(int configurationId, int userId)
    {
        try
        {
            var config = await _configRepository.FindByIdAsync(configurationId);
            if (config == null)
            {
                return new ConfigurationOperationResponseDTO
                {
                    Success = false,
                    Message = "Configuración no encontrada",
                    OperationTimestamp = DateTime.UtcNow
                };
            }

            config.Activate(userId);
            await _configRepository.UpdateAsync(config);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Configuración {configurationId} activada");

            return new ConfigurationOperationResponseDTO
            {
                Success = true,
                Message = "Configuración activada exitosamente",
                ConfigurationId = config.Id,
                OperationTimestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al activar configuración: {ex.Message}");
            return new ConfigurationOperationResponseDTO
            {
                Success = false,
                Message = $"Error al activar: {ex.Message}",
                OperationTimestamp = DateTime.UtcNow
            };
        }
    }

    public async Task<ConfigurationOperationResponseDTO> DeactivateConfigurationAsync(int configurationId, int userId)
    {
        try
        {
            var config = await _configRepository.FindByIdAsync(configurationId);
            if (config == null)
            {
                return new ConfigurationOperationResponseDTO
                {
                    Success = false,
                    Message = "Configuración no encontrada",
                    OperationTimestamp = DateTime.UtcNow
                };
            }

            if (config.IsDefault)
            {
                return new ConfigurationOperationResponseDTO
                {
                    Success = false,
                    Message = "No se puede desactivar la configuración predeterminada del sistema",
                    OperationTimestamp = DateTime.UtcNow
                };
            }

            config.Deactivate(userId);
            await _configRepository.UpdateAsync(config);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Configuración {configurationId} desactivada");

            return new ConfigurationOperationResponseDTO
            {
                Success = true,
                Message = "Configuración desactivada exitosamente",
                ConfigurationId = config.Id,
                OperationTimestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al desactivar configuración: {ex.Message}");
            return new ConfigurationOperationResponseDTO
            {
                Success = false,
                Message = $"Error al desactivar: {ex.Message}",
                OperationTimestamp = DateTime.UtcNow
            };
        }
    }

    public async Task<ConfigurationOperationResponseDTO> DeleteConfigurationAsync(int configurationId)
    {
        try
        {
            var config = await _configRepository.FindByIdAsync(configurationId);
            if (config == null)
            {
                return new ConfigurationOperationResponseDTO
                {
                    Success = false,
                    Message = "Configuración no encontrada",
                    OperationTimestamp = DateTime.UtcNow
                };
            }

            if (config.IsDefault)
            {
                return new ConfigurationOperationResponseDTO
                {
                    Success = false,
                    Message = "No se puede eliminar la configuración predeterminada del sistema",
                    OperationTimestamp = DateTime.UtcNow
                };
            }

            await _configRepository.DeleteAsync(config);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Configuración {configurationId} eliminada");

            return new ConfigurationOperationResponseDTO
            {
                Success = true,
                Message = "Configuración eliminada exitosamente",
                ConfigurationId = configurationId,
                OperationTimestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al eliminar configuración: {ex.Message}");
            return new ConfigurationOperationResponseDTO
            {
                Success = false,
                Message = $"Error al eliminar: {ex.Message}",
                OperationTimestamp = DateTime.UtcNow
            };
        }
    }

    // ==================== MÉTODOS PRIVADOS DE MAPEO ====================

    private SecurityConfigurationDTO MapToDTO(SecurityConfiguration config)
    {
        return new SecurityConfigurationDTO
        {
            Id = config.Id,
            ManagerId = config.ManagerId,
            FleetId = config.FleetId,
            Name = config.Name,
            Description = config.Description,
            DrowsinessEarThreshold = config.DrowsinessEarThreshold,
            DrowsinessConsecutiveFrames = config.DrowsinessConsecutiveFrames,
            DrowsinessMinDurationSeconds = config.DrowsinessMinDurationSeconds,
            MicroSleepEarThreshold = config.MicroSleepEarThreshold,
            MicroSleepMinDurationSeconds = config.MicroSleepMinDurationSeconds,
            MicroSleepMaxOccurrencesPerWindow = config.MicroSleepMaxOccurrencesPerWindow,
            MicroSleepWindowMinutes = config.MicroSleepWindowMinutes,
            DistractionYawThresholdDegrees = config.DistractionYawThresholdDegrees,
            DistractionPitchThresholdDegrees = config.DistractionPitchThresholdDegrees,
            DistractionMinDurationSeconds = config.DistractionMinDurationSeconds,
            CriticalAlertsThreshold = config.CriticalAlertsThreshold,
            CriticalAlertsWindowMinutes = config.CriticalAlertsWindowMinutes,
            NotificationCooldownMinutes = config.NotificationCooldownMinutes,
            SafetyScoreDrowsinessPenalty = config.SafetyScoreDrowsinessPenalty,
            SafetyScoreMicroSleepPenalty = config.SafetyScoreMicroSleepPenalty,
            SafetyScoreDistractionPenalty = config.SafetyScoreDistractionPenalty,
            SafetyScoreSafeTripBonus = config.SafetyScoreSafeTripBonus,
            IsActive = config.IsActive,
            IsDefault = config.IsDefault,
            CreatedAt = config.CreatedAt,
            UpdatedAt = config.UpdatedAt,
            CreatedBy = config.CreatedBy,
            UpdatedBy = config.UpdatedBy
        };
    }

    private SecurityConfigurationSummaryDTO MapToSummaryDTO(SecurityConfiguration config)
    {
        return new SecurityConfigurationSummaryDTO
        {
            Id = config.Id,
            Name = config.Name,
            Description = config.Description,
            ManagerId = config.ManagerId,
            FleetId = config.FleetId,
            IsActive = config.IsActive,
            IsDefault = config.IsDefault,
            CreatedAt = config.CreatedAt,
            UpdatedAt = config.UpdatedAt
        };
    }
}
