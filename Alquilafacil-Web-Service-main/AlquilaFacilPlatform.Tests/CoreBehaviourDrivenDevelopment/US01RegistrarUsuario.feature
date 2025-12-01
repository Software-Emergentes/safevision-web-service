Feature: Registrar usuario

  Como usuario de la aplicación de AlquilaFácil
  Quiero poder registrarme fácilmente en AlquilaFácil 
  Para ofrecer mi espacio en alquiler y llegar a más clientes potenciales.
  
  Scenario Outline: Validación de datos
    Given un usuario completa el formulario de registro en AlquilaFácil con los siguientes datos:
      | campo           | valor              |
      | <campo>         | <valor>            |
    When el usuario envía el formulario
    Then los datos proporcionados se validan para garantizar la precisión y la autenticidad

    Examples:
      | campo          | valor               |
      | Username       | juanperez           |
      | Password       | Secreta!1234        |
      | Email          | juan@example.com    |
      | Name           | Juan Pérez          |
      | FatherName     | Carlos Pérez        |
      | MotherName     | Laura Pérez         |
      | DateOfBirth    | 01-01-1990          |
      | DocumentNumber | 1234567890          |
      | Phone          | 123456789           |

  Scenario Outline: Validación de CAPTCHA
    Given un usuario ha completado el formulario de registro con los siguientes datos:
      | campo           | valor              |
      | <campo>         | <valor>            |
    When el usuario intenta enviarlo
    Then el sistema solicita completar un CAPTCHA para verificar que no se trata de un bot

    Examples:
      | campo          | valor               |
      | Username       | juanperez           |
      | Password       | Secreta!1234        |
      | Email          | juan@example.com    |
      | Name           | Juan Pérez          |
      | FatherName     | Carlos Pérez        |
      | MotherName     | Laura Pérez         |
      | DateOfBirth    | 1990-01-01          |
      | DocumentNumber | 1234567890          |
      | Phone          | 123456789           |

  Scenario Outline: Registro exitoso
    Given un usuario ha completado correctamente el formulario y superado la validación de CAPTCHA con los siguientes datos:
      | campo           | valor              |
      | <campo>         | <valor>            |
    When el usuario envía el formulario con toda la información válida
    Then el usuario recibe una confirmación de registro y puede acceder a su cuenta

    Examples:
      | campo          | valor               |
      | Username       | juanperez           |
      | Password       | Secreta!1234        |
      | Email          | juan@example.com    |
      | Name           | Juan Pérez          |
      | FatherName     | Carlos Pérez        |
      | MotherName     | Laura Pérez         |
      | DateOfBirth    | 1990-01-01          |
      | DocumentNumber | 1234567890          |
      | Phone          | 123456789           |
