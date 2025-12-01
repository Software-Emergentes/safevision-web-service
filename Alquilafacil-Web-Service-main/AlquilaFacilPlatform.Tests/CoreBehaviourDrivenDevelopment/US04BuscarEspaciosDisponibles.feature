Feature: Buscar espacios disponibles

  Como arrendatario,
  Quiero poder buscar fácilmente espacios disponibles en AlquilaFácil
  Para encontrar el lugar perfecto para mi evento.
  
  Scenario Outline: Búsqueda general desde la página principal
    Given un arrendatario desea explorar todas las opciones disponibles
    When accede a la página principal sin ingresar filtros o criterios específicos
    Then se muestran todos los espacios disponibles: <espacios>

    Examples:
      | espacios                                                       |
      | Oficina A, Estudio B, Local Comercial C, Sala de Reunión D    |
    

  Scenario Outline: Búsqueda por distrito desde la barra de navegación
    Given un arrendatario desea buscar espacios en un distrito específico
    When selecciona el <distrito> desde la barra de navegación
    Then se muestran únicamente los <espacios> disponibles dentro de ese distrito

    Examples:
      | distrito         | espacios                             |
      | Distrito Norte   | Oficina A, Estudio B                 |
      | Distrito Sur     | Local Comercial C, Sala de Reunión D |
