Feature: Visualizar espacios propios publicados

  Como arrendador,
  Quiero poder visualizar todos los espacios que he publicado en la plataforma,
  Para gestionar fácilmente la información de mis propiedades.

  Scenario Outline: Listado de espacios publicados
    Given que el arrendador ha iniciado sesión
    When accede a la sección "Mis espacios"
    Then puede ver una lista de todos los espacios publicados con <nombre> y <ubicación>
    
    Examples:
      | nombre    | ubicación   |
      | Espacio 1 | Ubicación 1 |
      | Espacio 2 | Ubicación 2 |
      | Espacio 3 | Ubicación 3 |

  Scenario Outline: Acceso a detalles
    Given que visualiza la lista de espacios
    When selecciona un <espacio>
    Then puede acceder a los <detalles> de este mismo
    
    Examples:
        | espacio    | detalles   |
        | Espacio 1  | Aforo = 5, Ubicación = Ubicación 1, Precio = 100, Descripción = Descripción 1 |
        | Espacio 2  | Aforo = 10, Ubicación = Ubicación 2, Precio = 200, Descripción = Descripción 2 |
        | Espacio 3  | Aforo = 15, Ubicación = Ubicación 3, Precio = 300, Descripción = Descripción 3 |