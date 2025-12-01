Feature: Controlar espacios favoritos

  Como arrendatario,
  Quiero poder agregar un espacio a mis favoritos,
  Para acceder rápidamente a lugares de interés para futuras reservas.
  
  Scenario Outline: Agregar a favoritos
    Given que el arrendatario está visualizando la página de detalles de un espacio
    When selecciona la opción "Agregar a favoritos"
    Then el <espacio> se guarda temporalmente para permitir su consulta rápida más adelante
    
    Examples:
      | espacio                 |
      | Salón de baile elegante |
      | Casa moderna de playa   |

  Scenario Outline: Eliminar de favoritos
    Given que un espacio ha sido marcado como favorito
    When el arrendatario selecciona la opción "Eliminar de favoritos"
    Then dicho <espacio> se retira de la lista personal de favoritos y deja de mostrarse como tal
    
    Examples: 
        | espacio                 |
        | Salón de baile elegante |
        | Casa moderna de playa   |
