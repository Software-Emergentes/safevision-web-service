Feature: Adquirir membresía premium

  Como usuario de AlquilaFácil,
  Quiero poder adquirir una membresía mensual premium
  Para acceder a beneficios exclusivos y maximizar mis oportunidades dentro del aplicativo.

  Scenario Outline: Selección de la membresía premium
    Given que el usuario ha iniciado sesión en su cuenta de AlquilaFácil
    When accede a la sección de "Membresías" y selecciona la opción de "Membresía Premium"
    Then se le muestra una <descripción> detallada de los beneficios y el <costo> de la membresía, junto con la <información bancaria> necesaria para realizar el depósito
    
    Examples: 
      | descripción                         | costo  | información bancaria   |
      | Membresía Premium Mensual           | 20.00  | Cuenta 1234567890      |
      | Membresía Premium Anual             | 200.00 | Cuenta 9876543210      |
    

  Scenario Outline: Envío de comprobante de pago
    Given que el usuario desea adquirir la membresía mensual premium
    When realiza el <depósito> y carga la imagen del <comprobante de pago>
    Then se registra su solicitud de membresía en espera de verificación
    
    Examples: 
      | depósito  | comprobante de pago         |
      | 20.00     | imagen_comprobante_mensual  |
      | 200.00    | imagen_comprobante_anual    |

  Scenario Outline: Validación del comprobante
    Given que el usuario ha enviado el comprobante de pago
    When el administrador revisa que el <depósito> coincide con el <monto correspondiente>
    Then aprueba la solicitud, y el usuario recibe acceso a los beneficios premium; de lo contrario, la <solicitud> es rechazada
    
    Examples: 
      | depósito  | monto correspondiente | solicitud  |
      | 20.00     | 20.00                 | aprobada    |
      | 200.00    | 200.00                | aprobada    |
      | 20.00     | 15.00                 | rechazada   |
      | 200.00    | 150.00                | rechazada   |
    
    
