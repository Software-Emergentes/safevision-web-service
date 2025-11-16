namespace SafeVisionPlatform.Trip.Domain.Services;

/// <summary>
/// Servicio de dominio para exportar reportes de viajes en diferentes formatos.
/// Soporta exportación a PDF y Excel para auditoría y análisis.
/// </summary>
public interface IReportExportService
{
    /// <summary>
    /// Exporta un reporte individual a formato PDF.
    /// </summary>
    /// <param name="reportId">ID del reporte a exportar</param>
    /// <returns>Archivo PDF en bytes</returns>
    Task<byte[]> ExportReportToPdfAsync(int reportId);

    /// <summary>
    /// Exporta múltiples reportes a formato PDF.
    /// </summary>
    /// <param name="reportIds">Lista de IDs de reportes</param>
    /// <returns>Archivo PDF consolidado en bytes</returns>
    Task<byte[]> ExportMultipleReportsToPdfAsync(IEnumerable<int> reportIds);

    /// <summary>
    /// Exporta un reporte individual a formato Excel.
    /// </summary>
    /// <param name="reportId">ID del reporte a exportar</param>
    /// <returns>Archivo Excel en bytes</returns>
    Task<byte[]> ExportReportToExcelAsync(int reportId);

    /// <summary>
    /// Exporta múltiples reportes a formato Excel.
    /// </summary>
    /// <param name="reportIds">Lista de IDs de reportes</param>
    /// <returns>Archivo Excel consolidado en bytes</returns>
    Task<byte[]> ExportMultipleReportsToExcelAsync(IEnumerable<int> reportIds);

    /// <summary>
    /// Exporta reportes de un conductor en un rango de fechas a PDF.
    /// </summary>
    /// <param name="driverId">ID del conductor</param>
    /// <param name="startDate">Fecha de inicio</param>
    /// <param name="endDate">Fecha de fin</param>
    /// <returns>Archivo PDF en bytes</returns>
    Task<byte[]> ExportDriverReportsByDateRangeToPdfAsync(int driverId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Exporta reportes de un conductor en un rango de fechas a Excel.
    /// </summary>
    /// <param name="driverId">ID del conductor</param>
    /// <param name="startDate">Fecha de inicio</param>
    /// <param name="endDate">Fecha de fin</param>
    /// <returns>Archivo Excel en bytes</returns>
    Task<byte[]> ExportDriverReportsByDateRangeToExcelAsync(int driverId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Exporta estadísticas agregadas de la flota a PDF.
    /// </summary>
    /// <param name="startDate">Fecha de inicio</param>
    /// <param name="endDate">Fecha de fin</param>
    /// <returns>Archivo PDF en bytes</returns>
    Task<byte[]> ExportFleetStatisticsToPdfAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Exporta estadísticas agregadas de la flota a Excel.
    /// </summary>
    /// <param name="startDate">Fecha de inicio</param>
    /// <param name="endDate">Fecha de fin</param>
    /// <returns>Archivo Excel en bytes</returns>
    Task<byte[]> ExportFleetStatisticsToExcelAsync(DateTime startDate, DateTime endDate);
}
