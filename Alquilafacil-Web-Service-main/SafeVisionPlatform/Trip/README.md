# Bounded Context: Trip

## Descripción General

El Bounded Context **Trip** abarca la gestión del ciclo de vida de los viajes realizados por los conductores. Su propósito es registrar y controlar los viajes desde su inicio hasta su finalización o cancelación, asegurando la integridad de la información y su comunicación con los contextos Monitoring y Notification.

## Arquitectura por Capas

### 1. Domain Layer (Capa de Dominio)

La capa de dominio contiene las entidades, objetos de valor y servicios que encapsulan las reglas de negocio.

#### Agregados
- **TripAggregate**: Entidad raíz que representa un viaje. Contiene información sobre el conductor, vehículo, horarios, estado y alertas asociadas.

#### Entidades
- **Alert**: Representa alertas detectadas durante el viaje (somnolencia, distracción, micro-sueño, etc.)
- **Report**: Registra la información del viaje al finalizar, incluyendo métricas y destino de envío.

#### Objetos de Valor
- **TripStatus**: Enum que define los estados válidos del viaje (Initiated, InProgress, Completed, Cancelled)
- **TripTime**: Agrupa las marcas temporales del viaje (inicio y fin)
- **TripDataPolicy**: Encapsula las reglas de sincronización de datos a la nube

#### Servicios de Dominio
- **ITripManagerService**: Coordina operaciones principales del viaje y valida transiciones de estado
- **ITripReportGenerator**: Genera reportes y calcula métricas del viaje

#### Repositorios (Interfaces)
- **ITripRepository**: Operaciones CRUD para viajes
- **IReportRepository**: Operaciones CRUD para reportes
- **IAlertRepository**: Operaciones CRUD para alertas

#### Eventos de Dominio
- **TripStartedEvent**: Disparado al iniciar un viaje
- **TripEndedEvent**: Disparado al finalizar un viaje
- **TripCancelledEvent**: Disparado al cancelar un viaje
- **TripDataSentToCloudEvent**: Disparado cuando los datos se sincronizan a la nube
- **TripReportGeneratedEvent**: Disparado al generar un reporte

### 2. Application Layer (Capa de Aplicación)

Coordina los flujos de negocio y orquesta la comunicación entre las capas.

#### DTOs
- **TripDTO**: Transferencia de datos de viaje
- **TripReportDTO**: Transferencia de datos de reporte
- **AlertDTO**: Transferencia de datos de alertas
- **CreateTripDTO, CreateAlertDTO, GenerateReportDTO**: DTOs de entrada

#### Command Handlers
- **IStartTripCommandHandler**: Inicia un nuevo viaje
- **IEndTripCommandHandler**: Finaliza un viaje
- **ICancelTripCommandHandler**: Cancela un viaje
- **ISyncTripDataCommandHandler**: Sincroniza datos del viaje

#### Query Services
- **ITripQueryService**: Consultas sobre viajes
- **IReportQueryService**: Consultas sobre reportes
- **IAlertQueryService**: Consultas sobre alertas

#### Application Services
- **ITripApplicationService**: Fachada principal de casos de uso
- **ITripReportService**: Gestión de creación y envío de reportes

#### Event Handlers
- **ITripEndedEventHandler**: Maneja eventos de finalización
- **ITripDataSentEventHandler**: Maneja eventos de sincronización
- **ITripCancelledEventHandler**: Maneja eventos de cancelación

### 3. Infrastructure Layer (Capa de Infraestructura)

Implementa la persistencia y la integración con sistemas externos.

#### Persistencia (Entity Framework Core)
- **TripRepository**: Implementación de persistencia de viajes
- **ReportRepository**: Implementación de persistencia de reportes
- **AlertRepository**: Implementación de persistencia de alertas

#### Servicios de Dominio Implementados
- **TripManagerService**: Implementación de validaciones y operaciones del viaje
- **TripReportGenerator**: Generador de reportes y cálculo de métricas

#### Servicios de Integración
- **ICloudSyncService**: Sincronización de datos con la nube (AWS, Azure, etc.)
- **INotificationPublisher**: Publicación de eventos a otros bounded contexts mediante Message Broker

#### Configuración
- **TripServiceCollectionExtensions**: Registra todos los servicios de inyección de dependencias
- **TripEntityConfigurations**: Configuración de mapeos de Entity Framework

### 4. Interface Layer (Capa de Interfaz)

Expone los servicios del dominio mediante APIs REST.

#### Controladores REST
- **TripsController**: Endpoints para gestión de viajes
  - `POST /api/trips/start`: Inicia un viaje
  - `PUT /api/trips/{id}/end`: Finaliza un viaje
  - `PUT /api/trips/{id}/cancel`: Cancela un viaje
  - `GET /api/trips/{id}`: Obtiene detalles de un viaje
  - `GET /api/trips/driver/{driverId}`: Historial de viajes de un conductor
  - `GET /api/trips/vehicle/{vehicleId}`: Viajes de un vehículo
  - `POST /api/trips/{id}/sync`: Sincroniza datos del viaje

- **TripsReportsController**: Endpoints para reportes
  - `GET /api/trips/reports`: Obtiene todos los reportes
  - `GET /api/trips/reports/driver/{driverId}`: Reportes de un conductor
  - `GET /api/trips/reports/trip/{tripId}`: Reporte de un viaje específico
  - `POST /api/trips/reports/generate`: Genera un reporte
  - `POST /api/trips/reports/{reportId}/send`: Envía un reporte

- **TripsAlertsController**: Endpoints para alertas
  - `GET /api/trips/alerts/trip/{tripId}`: Alertas de un viaje
  - `GET /api/trips/alerts/type/{alertType}`: Alertas por tipo y rango de fechas
  - `POST /api/trips/alerts`: Agrega una alerta
  - `POST /api/trips/alerts/{alertId}/acknowledge`: Reconoce una alerta

#### Ensambladores (Assemblers)
- **TripAssembler**: Transforma TripAggregate a TripDTO
- **ReportAssembler**: Transforma Report a TripReportDTO
- **AlertAssembler**: Transforma Alert a AlertDTO

#### Fachada de Contexto (ACL)
- **ITripContextFacade**: Interface para acceso desde otros bounded contexts
- **TripContextFacade**: Implementación de la fachada

## Flujos Principales

### 1. Iniciar un Viaje
```
Cliente HTTP
    ↓
TripsController.StartTrip(CreateTripDTO)
    ↓
TripApplicationService.StartTripAsync()
    ↓
StartTripCommandHandler
    ↓
TripManagerService.ValidateDriverAndVehicleAvailabilityAsync()
    ↓
TripAggregate.StartTrip()
    ↓
TripRepository.AddAsync()
    ↓
Event: TripStartedEvent (publicado)
    ↓
NotificationPublisher (comunica a otros contextos)
```

### 2. Finalizar un Viaje
```
Cliente HTTP
    ↓
TripsController.EndTrip(int tripId)
    ↓
TripApplicationService.EndTripAsync()
    ↓
EndTripCommandHandler
    ↓
TripAggregate.EndTrip()
    ↓
TripRepository.Update()
    ↓
Event: TripEndedEvent
    ↓
TripEndedEventHandler
    ↓
TripReportGenerator.GenerateReportAsync()
    ↓
ReportRepository.AddAsync()
    ↓
Event: TripReportGeneratedEvent
    ↓
NotificationPublisher (comunica a Notification context)
```

### 3. Sincronizar Datos del Viaje
```
Cliente HTTP / Sistema Automático
    ↓
TripsController.SyncTripData(int tripId)
    ↓
TripApplicationService.SyncTripDataAsync()
    ↓
SyncTripDataCommandHandler
    ↓
CloudSyncService.SyncTripDataAsync()
    ↓
Event: TripDataSentToCloudEvent
    ↓
TripDataSentEventHandler
    ↓
NotificationPublisher
```

## Integración con Otros Contextos

### Contexto: Driver
- **Referencia**: Valida disponibilidad del conductor antes de iniciar un viaje
- **Método**: Via `ITripManagerService.ValidateDriverAndVehicleAvailabilityAsync()`

### Contexto: Management (Vehicles)
- **Referencia**: Valida disponibilidad del vehículo
- **Método**: Via `ITripManagerService.ValidateDriverAndVehicleAvailabilityAsync()`

### Contexto: Monitoring
- **Referencia**: Recibe alertas detectadas durante el viaje
- **Método**: Via `TripsAlertsController.AddAlert()` / `TripApplicationService.AddAlertAsync()`

### Contexto: Notification
- **Referencia**: Recibe eventos de viaje para notificaciones
- **Eventos publicados**:
  - TripStartedEvent
  - TripEndedEvent
  - TripCancelledEvent
  - TripReportGeneratedEvent

## Configuración en Program.cs

Para registrar el Bounded Context Trip en la aplicación:

```csharp
// En Program.cs
builder.Services.AddTripBoundedContext();

// En DbContext
modelBuilder.ApplyConfiguration(new TripAggregateConfiguration());
modelBuilder.ApplyConfiguration(new AlertConfiguration());
modelBuilder.ApplyConfiguration(new ReportConfiguration());
```

## Base de Datos

### Tablas Principales
- **Trips**: Almacena los agregados TripAggregate
- **Alerts**: Almacena las alertas detectadas durante viajes
- **Reports**: Almacena los reportes generados

### Índices
- `Alerts.TripId`: Búsqueda rápida de alertas por viaje
- `Alerts.AlertType`: Filtrado por tipo de alerta
- `Alerts.DetectedAt`: Filtrado por rango de fechas
- `Reports.TripId`: Referencia única a viaje
- `Reports.DriverId`: Búsqueda de reportes por conductor
- `Reports.Status`: Filtrado por estado del reporte

## Políticas de Datos (TripDataPolicy)

Cada viaje tiene una política que define:
- `SyncToCloud`: Si debe sincronizarse a la nube (default: true)
- `SyncIntervalMinutes`: Intervalo de sincronización en minutos (default: 5)
- `IncludeAlerts`: Si incluir alertas en la sincronización (default: true)
- `IncludeMetrics`: Si incluir métricas en la sincronización (default: true)

## Estados del Viaje (TripStatus)

1. **Initiated** (1): Viaje creado pero no iniciado
2. **InProgress** (2): Viaje en curso
3. **Completed** (3): Viaje finalizado
4. **Cancelled** (4): Viaje cancelado

## Tipos de Alertas (AlertType)

1. **Drowsiness** (1): Somnolencia
2. **Distraction** (2): Distracción
3. **MicroSleep** (3): Micro-sueño
4. **SpeedViolation** (4): Violación de velocidad
5. **LaneDeviation** (5): Desviación de carril

## Métricas del Viaje (TripMetrics)

- `DurationMinutes`: Duración total en minutos
- `DistanceKm`: Distancia recorrida en kilómetros
- `AverageSpeed`: Velocidad promedio
- `AlertCount`: Número total de alertas
- `CriticalAlertCount`: Alertas críticas (severidad > 0.7)
- `SafetyScore`: Puntuación de seguridad (0-100)

## Patrones Aplicados

- **Domain-Driven Design (DDD)**: Estructurado alrededor del modelo de dominio
- **Clean Architecture**: Separación clara de responsabilidades por capas
- **Repository Pattern**: Abstracción de la persistencia
- **Command Query Responsibility Segregation (CQRS)**: Separación entre escritura (Commands) y lectura (Queries)
- **Event Sourcing**: Uso de eventos de dominio para comunicación asíncrona
- **Dependency Injection**: Inversión de control para desacoplamiento
- **Anti-Corruption Layer (ACL)**: Fachada para comunicación con otros contextos

## Próximas Mejoras

1. Implementar sincronización automática de datos con CloudSyncService
2. Integrar con Message Broker (RabbitMQ/Kafka) para publicación de eventos
3. Agregar caché distribuido para consultas frecuentes
4. Implementar escalabilidad automática de alertas
5. Agregar análisis de datos y machine learning para predicción de alertas
6. Implementar auditoría completa de cambios de estado

