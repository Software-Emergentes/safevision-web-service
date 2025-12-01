Feature: Registrar espacio

  Como arrendador de un espacio para eventos en AlquilaFácil
  Quiero poder registrar mi espacio
  Para comenzar a recibir solicitudes de reserva lo antes posible.
  
  Scenario Outline: Registro paso a paso del espacio
    Given un arrendador desea registrar su espacio en AlquilaFácil
    When ingresa los <datos requeridos> en cada <campo> del formulario
    Then puede avanzar progresivamente hasta el resumen final del registro

    Examples:
      | campo            | datos requeridos                         |
      | Distrito         | Ingresar el nombre del distrito          |
      | Calle            | Ingresar el nombre de la calle           |
      | Tipo de Local    | Seleccionar el tipo de local             |
      | País             | Ingresar el país del espacio             |
      | Ciudad           | Ingresar la ciudad del espacio           |
      | Precio           | Ingresar el precio del espacio           |
      | Foto             | Subir la URL de una foto del espacio     |
      | Descripción      | Escribir una descripción del espacio     |
      | Categoría        | Seleccionar la categoría del espacio     |
      | Características  | Detallar las características del espacio |
      | Capacidad        | Ingresar la capacidad del local          |

  Scenario Outline: Validación final de los datos requeridos
    Given un arrendador ha completado todos los pasos del registro
    When intenta enviar la solicitud de publicación del espacio con los siguientes <datos>
    Then el sistema valida que toda la información requerida esté completa
    And si hay <campos> faltantes o incorrectos, se muestra un mensaje de error indicando qué se debe corregir

    Examples:
      | campos           | datos                          |
      | Distrito         | Distrito Norte                 |
      | Calle            | Calle Ficticia 123             |
      | Tipo de Local    | Oficina                        |
      | País             | Perú                           |
      | Ciudad           | Lima                           |
      | Precio           | 1500                           |
      | Descripción      | Espacio luminoso y cómodo      |
      | Categoría        | Oficina                        |
      | Características  | Wi-Fi, Aire acondicionado      |
      | Capacidad        | 4                              |

  Scenario Outline: Espacio publicado correctamente
    Given un arrendador ha completado correctamente el <registro> con los siguientes <datos>
    When el arrendador envía la solicitud de publicación
    Then el espacio se publica correctamente en la plataforma

    Examples:
      | registro         | datos                       |
      | Distrito         | Distrito Norte                 |
      | Calle            | Calle Ficticia 123             |
      | Tipo de Local    | Oficina                        |
      | País             | Perú                           |
      | Ciudad           | Lima                           |
      | Precio           | 1500                           |
      | Descripción      | Espacio luminoso y cómodo      |
      | Categoría        | Oficina                        |
      | Características  | Wi-Fi, Aire acondicionado      |
      | Capacidad        | 4                              |
