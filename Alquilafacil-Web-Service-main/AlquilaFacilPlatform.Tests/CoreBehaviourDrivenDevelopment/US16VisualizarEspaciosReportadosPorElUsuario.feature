Feature: Visualizar espacios reportados por el usuario

  Como usuario de AlquilaFácil,
  Quiero poder visualizar los espacios que he reportado,
  Para realizar un seguimiento de mis reportes y gestionarlos.

  Scenario Outline: Navegar a la sección de "Ver Espacios Reportados"
    Given que el usuario ha iniciado sesión en su cuenta de AlquilaFácil
    When accede a la sección de "Ver Espacios Reportados" desde el panel de control
    Then puede ver una lista de los espacios que ha reportado, incluyendo información como el <nombre> del espacio, la <fecha> del reporte, y el <motivo> del reporte
    
    Examples:
      | nombre         | fecha       | motivo               |
      | Espacio A     | 2023-10-01   | Infracción de normas |
      | Espacio B     | 2023-10-02   | Problemas de limpieza|
      | Espacio C     | 2023-10-03   | Ruido excesivo      |

  Scenario Outline: Eliminar un reporte deslizando hacia la izquierda
    Given que el usuario se encuentra en la lista de espacios reportados
    When desliza uno de los reportes hacia la izquierda
    Then aparece una opción para confirmar la <eliminación> del <reporte>. Y si confirma, el reporte se elimina de la lista
    
    Examples:
      | eliminación     | reporte       |
      | eliminación     | Espacio A    |
      | eliminación     | Espacio B    |
      | eliminación     | Espacio C    |