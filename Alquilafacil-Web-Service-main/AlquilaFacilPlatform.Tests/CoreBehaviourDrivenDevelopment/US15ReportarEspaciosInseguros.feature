Feature: Reportar espacios inseguros

  Como usuario de AlquilaFácil,
  Quiero poder reportar espacios inseguros,
  Para contribuir a la seguridad de la comunidad y alertar sobre situaciones que puedan poner en riesgo a otros usuarios.

  Scenario Outline: Selección del espacio inseguro
    Given que el usuario ha iniciado sesión en su cuenta de AlquilaFácil
    When accede a la sección de "Reportar espacio" y selecciona el <espacio> desde la lista de reservas activas o buscando un espacio específico
    Then puede ingresar un <asunto> y <descripción> del problema
    
    Examples: 
      | espacio         | asunto                | descripción                         |
      | Espacio A       | Problema de seguridad | El espacio tiene una puerta rota    |
      | Espacio B       | Problema de limpieza  | El espacio está sucio y desordenado |
    

  Scenario Outline: Confirmación del reporte
    Given que el usuario ha completado el formulario de reporte con todos los detalles requeridos
    When envía el reporte mediante el botón "Enviar"
    Then el <reporte> será <registrado> y revisado por el equipo de soporte de AlquilaFácil

    Examples: 
      | reporte         | registrado           |
      | Espacio A       | registrado            |
      | Espacio B       | registrado            |