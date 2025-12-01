Feature: Notificar sobre actividades
  
  Como arrendador,
  Quiero recibir notificaciones cuando un arrendatario reserve mi espacio,
  Para estar informado sobre la fecha y hora del evento.
  
  Scenario Outline: Notificación automática de nueva reserva
    Given que un <arrendatario> ha realizado una reserva para un espacio
    When se completa el envío del comprobante de pago
    Then el arrendador recibe una notificación que incluye la <fecha>, <hora> y nombre del <espacio reservado>
    
    Examples:
      | arrendatario | espacio reservado | fecha       | hora   |
      | Juan Pérez   | Sala de reuniones | 2023-10-01  | 10:00  |
      | Ana Gómez    | Oficina privada   | 2023-10-02  | 14:00  |

  Scenario Outline: Visualización en la sección de notificaciones
    Given que el arrendador desea revisar sus notificaciones
    When accede a la sección de notificaciones en la aplicación
    Then puede ver un listado con los detalles de cada <reserva> reciente, incluyendo <fecha> y <hora>
    
    Examples:
      | fecha       | hora   | reserva           |
      | 2023-10-01  | 10:00  | Sala de reuniones |
      | 2023-10-02  | 14:00  | Oficina privada   |
