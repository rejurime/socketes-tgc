# :soccer: socketes-tgc :soccer:

## Synopsis
Ultra precario juego de fútbol :stuck_out_tongue_closed_eyes:
## Motivation
*Trabajo Práctico de la materia* **"Técnicas de Gráficos por Computadora"**. *Segundo Cuatrimestre 2015.*
## Installation
Se pisa la carpeta AlumnoEjemplos del proyecto TGCViewer.
### Consejo:
Para que tgc-viewer inicie por defecto nuestro proyecto y ahorrar muchisisisimo tiempo cambiar el constructor de la clase `TgcViewerConfig` (es una clase del proyecto TGCViewer) por lo de abajo.

```csharp
/// <summary>
/// Crear con configuracion default
/// </summary>
public TgcViewerConfig()
{
    fullScreenMode = false;
    defaultExampleName = "Balompié";
    defaultExampleCategory = "AlumnoEjemplos";
    showModifiersPanel = true;
    title = "TgcViewer - Técnicas de Gráficos por Computadora - UTN - FRBA";
    showTitleBar = true;
}
```

## API Reference
* [DirectX SDK](http://www.microsoft.com/en-us/download/details.aspx?displaylang=en&id=6812)
* [Visual Studio Community Edition](https://www.visualstudio.com/es-ar/products/visual-studio-community-vs)
* [TGCViewer](https://github.com/tgc-utn/tgc-viewer)

## Contributors
* Matías Leonel Rege
* René Juan Rico Mendoza
