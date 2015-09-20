# socketes-tgc
Trabajo Práctico de la materia "Teoría de Gráficos por Computadora". Segundo Cuatrimestre 2015.

Repo TGCViewer
https://github.com/lebarba/tgc-viewer/

Consejo:
Para que tgc-viewer inicie por defecto nuestro proyecto y ahorrar muchisisisimo tiempo cambiar el constructor de la clase TgcViewerConfig (es una clase del proyecto TGCViewer) por lo de abajo.

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
