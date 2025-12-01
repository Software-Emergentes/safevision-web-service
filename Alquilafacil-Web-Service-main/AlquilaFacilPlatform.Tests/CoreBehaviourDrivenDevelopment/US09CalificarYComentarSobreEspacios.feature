Feature: Calificar y comentar sobre espacios

  Como arrendatario,
  Quiero poder publicar mi reseña sobre un espacio que he reservado
  Para que otros usuarios puedan conocer mi experiencia antes de reservar.

  Scenario Outline: Disponibilidad para calificar
    Given que un arrendatario ha realizado una reserva en AlquilaFácil
    When accede a los detalles de la reserva desde su calendario
    Then solo podrá publicar una reseña si la <fecha> del evento ya ha finalizado; de lo contrario, la <opción> estará deshabilitada
    
    Examples:
        | fecha       | opción         |
        | 2023-10-01  | deshabilitada  |
        | 2023-10-15  | habilitada     |

  Scenario Outline: Publicación de reseña
    Given que el arrendatario tiene una reserva finalizada y desea compartir su experiencia
    When completa los campos de <calificación> y <comentario>, y presiona el botón de publicar reseña
    Then la reseña se guarda correctamente y se muestra públicamente en el perfil del local

    Examples: 
        | calificación | comentario               |
        | 5            | Excelente experiencia     |
        | 3            | Aceptable, pero mejorable |
        | 1            | Muy mala experiencia      |