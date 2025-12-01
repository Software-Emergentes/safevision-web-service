Feature: Iniciar sesión

  Como usuario registrado de la aplicación de AlquilaFácil,
  Quiero poder iniciar sesión fácilmente en mi cuenta, 
  Para gestionar mi espacio en alquiler y acceder a mis reservas.

  Scenario Outline: Inicio de sesión exitoso
    Given un usuario registrado desea acceder a su cuenta en AlquilaFácil
    When el usuario ingresa su <correo electrónico> y <contraseña> correctos en el formulario de inicio de sesión
    Then el usuario es autenticado exitosamente y se le otorga acceso a su cuenta

    Examples:
      | correo electrónico    | contraseña  |
      | juan@example.com      | Secreta!1234 |
      | maria@example.com     | Password123 |

  Scenario Outline: Error en el inicio de sesión por credenciales incorrectas
    Given un usuario registrado intenta acceder a su cuenta
    When el usuario ingresa una combinación incorrecta de <correo electrónico> y <contraseña>
    Then se muestra un <mensaje de error> indicando que las credenciales son incorrectas

    Examples:
      | correo electrónico    | contraseña  |  mensaje de error
      | juan@example.com      | Incorrecta  |  Correo o contraseña incorrectos
      | maria@example.com     | WrongPass   |  Correo o contraseña incorrectos
