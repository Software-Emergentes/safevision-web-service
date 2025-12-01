Feature: Visualizar información del espacio

  Como arrendatario,
  Quiero poder visualizar información detallada del espacio,
  Para evaluar si cumple con los requisitos de mi evento antes de hacer una reserva.

  Scenario Outline: Visualización general del espacio
    Given que el arrendatario selecciona un espacio en AlquilaFácil. 
    When accede a la página de detalles del <espacio>. 
    Then puede visualizar información detallada como el <aforo máximo>, <descripción> del espacio y <servicios> disponibles.
  

    Examples:
      | espacio             | aforo máximo | descripción                                                | servicios                                  |
      | Salón de eventos    | 100          | Espacio amplio con iluminación moderna y sistema de sonido | Wi-Fi, aire acondicionado, estacionamiento |
      | Auditorio principal | 200          | Auditorio con proyector y sistema de sonido profesional    | Wi-Fi, proyector, estacionamiento          |

  Scenario Outline: Visualización de reseñas y calificaciones
    Given que el arrendatario está interesado en conocer opiniones de otros usuarios
    When accede a la pestaña de “Reseñas” dentro del perfil del espacio
    Then puede ver una lista con los <comentarios> de los usuarios y las <calificaciones> en estrellas

    Examples:
      | comentarios                                                            | calificaciones |
      | "Excelente espacio, perfecto para eventos grandes"                     | 5              |
      | "El lugar es bueno, pero podría mejorar la ventilación"                | 3              |
      | "Muy acogedor, ideal para reuniones pequeñas"                          | 4              |
