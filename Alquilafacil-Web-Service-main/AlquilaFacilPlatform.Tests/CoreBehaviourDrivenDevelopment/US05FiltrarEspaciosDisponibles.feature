Feature: Filtrar espacios disponibles

  Como arrendatario,
  Quiero poder filtrar los espacios disponibles por capacidad y categoría,
  Para encontrar uno que cumpla con mis criterios específicos.
  
  Scenario Outline: Filtrado por capacidad
    Given un arrendatario desea un espacio con <capacidad> para un número específico de personas
    When aplica un filtro de capacidad en la búsqueda
    Then se muestran solo los <espacios> que cumplen con ese criterio

    Examples:
      | capacidad   |  espacios                                    |
      | 1-5 personas  |  Salón de eventos Ab, Casa de playa Ac     |
      | 6-10 personas  |  Salón de eventos Ab, Salón de bodas Ab   |

  Scenario Outline: Filtrado por categoría
    Given un arrendatario desea un espacio de una <categoría> en específico
    When aplica un filtro de categoría
    Then se muestran solo los <espacios> que cumplen con ese criterio

    Examples:
      | categoría        |  espacios                                  |
      | Salón elegante   |  Salón de eventos Ele, Salón de bodas Ab   |  
      | Casa de playa    |  Casa de playa Ab, Casa de playa Ac        |
