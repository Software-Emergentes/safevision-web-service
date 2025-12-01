Feature: Gestionar calendario de reservas

  Como usuario de AlquilaFácil,
  Quiero poder controlar una agenda de reservas
  Para tener un horario organizado y evitar conflictos futuros.

  Scenario Outline: Reserva de usuario normal
    Given que un arrendatario ha reservado un espacio
    When el arrendador accede al calendario
    Then ve el <día> <resaltado> en azul
    
    Examples: 
      | día                    | resaltado |
      | 01/02/2002 03:00:00    | azul      |
      | 02/02/2002 03:00:00    | azul      |

  Scenario Outline: Reserva de usuario premium
    Given que un usuario premium ha reservado un espacio
    When el propietario accede al calendario
    Then ve el <día> <resaltado> en amarillo
    
    Examples: 
      | día                    | resaltado |
      | 01/02/2002 03:00:00    | amarillo   |
      | 02/02/2002 03:00:00    | amarillo   |

  Scenario Outline: Reserva de espacio ajeno
    Given que un arrendatario ha reservado un espacio
    When accede al calendario
    Then ve el <día> <resaltado> en rojo

    Examples: 
      | día                    | resaltado |
      | 01/02/2002 03:00:00    | rojo      |
      | 02/02/2002 03:00:00    | rojo      |