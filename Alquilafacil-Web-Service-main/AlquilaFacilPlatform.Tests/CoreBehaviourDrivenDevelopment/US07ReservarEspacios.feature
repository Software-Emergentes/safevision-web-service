Feature: Reservar espacios
  
  Como arrendatario,
  Quiero poder reservar un espacio para mi evento en AlquilaFácil
  Para garantizar su disponibilidad en la fecha deseada.
  
  Scenario Outline: Selección de espacio y fecha
    Given que un arrendatario desea reservar un espacio
    When selecciona el <espacio> deseado y la <fecha> del evento
    Then se muestra el <precio> total a pagar junto con los datos de la cuenta <bancaria> e <interbancaria> del arrendador
    
    Examples:
      | espacio     | fecha                                                   | precio | bancaria | interbancaria  |
      | Sala A      | 2023-10-01 01:00:00  -  2023-10-01 02:00:00             | 100.00 | 123456789 | 987654321     |
      | Sala B      | 2023-10-02 01:00:00  -  2023-10-02 03:00:00             | 150.00 | 123456789 | 987654321     |

  Scenario Outline: Envío del comprobante de pago
    Given que el arrendatario ha realizado el pago externo mediante transferencia
    When carga una foto del <voucher> de pago y completa los <datos solicitados>
    Then el sistema registra la reserva como pendiente de validación
    
    Examples:
      | voucher         | datos solicitados                                                              |
      | voucher1.jpg    | Fecha inicio = 2023-10-01 01:00:00  -  Fecha fin =  2023-10-01 02:00:00        |
      | voucher2.jpg    | Fecha inicio = 2023-10-02 01:00:00  -  Fecha fin =  2023-10-02 03:00:00        |

  Scenario Outline: Validación del pago por parte del arrendador
    Given que se ha creado una nueva reserva con comprobante adjunto
    When el arrendador revisa el <voucher> y verifica que el monto es correcto
    Then el arrendador puede aceptar la <reserva>, o en caso contrario, rechazarla

    Examples:
      | voucher         | reserva                                              |
      | voucher1.jpg    | No realizar ninguna acción para aceptar              |
      | voucher2.jpg    | Rechazar                                             |