Feature: Actualizar perfil y sesión de usuario

  Como usuario de AlquilaFácil,
  Quiero poder modificar mi perfil, incluyendo la opción de cambiar mi nombre de usuario y cerrar sesión,
  Para mantener actualizada mi información personal y gestionar mi acceso a la aplicación de manera conveniente.
  
  Scenario Outline: Modificación del nombre de usuario
    Given que el usuario ha iniciado sesión en su cuenta de AlquilaFácil
    When accede a la sección de configuración de perfil y edita el campo "<Nombre de usuario>"
    Then puede guardar los cambios, y el <nuevo nombre de usuario> se refleja en su perfil y en todas las interacciones futuras en la plataforma
    
    Examples:
      | Nombre de usuario  | nuevo nombre de usuario |
      | JuanPérez          | JuanPérez123            |
      | MariaLopez         | MariaLopez456           |
      | CarlosGomez        | CarlosGomez789          |

  Scenario Outline: Cierre de sesión
    Given que el <usuario> desea salir de su cuenta en AlquilaFácil
    When selecciona la opción "Cerrar sesión" en la configuración de perfil
    Then es desconectado de su cuenta y redirigido a la pantalla de inicio de sesión, asegurando que su <sesión> se haya cerrado de manera segura
    
    Examples:
      | usuario            |    sesión            |
      | JuanPérez          |    cerrada           |
      | MariaLopez         |    cerrada           |
      | CarlosGomez        |    cerrada           |
