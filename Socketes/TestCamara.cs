using AlumnoEjemplos.Properties;
using AlumnoEjemplos.Socketes.Model;
using AlumnoEjemplos.Socketes.Model.Creacion;
using Microsoft.DirectX;
using System.Reflection;
using TgcViewer;
using TgcViewer.Example;

namespace AlumnoEjemplos.Socketes
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class TestCamara : TgcExample
    {
        private Partido partido;

        /// <summary>
        /// Categor�a a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el �rbol de la derecha de la pantalla.
        /// </summary>
        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        /// <summary>
        /// Completar nombre del grupo en formato Grupo NN
        /// </summary>
        public override string getName()
        {
            return "Test c�mara";
        }

        /// <summary>
        /// Completar con la descripci�n del TP
        /// </summary>
        public override string getDescription()
        {
            return "Test para poder volar feliz con la c�mara.";
        }

        /// <summary>
        /// M�todo que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aqu� todo el c�digo de inicializaci�n: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            string pathRecursos = System.Environment.CurrentDirectory + "\\" + Assembly.GetExecutingAssembly().GetName().Name + "\\" + Settings.Default.mediaFolder;
            this.partido = PartidoFactory.Instance.CrearPartido(pathRecursos, GuiController.Instance.D3dInput);

            //BoundingBox
            GuiController.Instance.Modifiers.addBoolean("BoundingBox", "BoundingBox", false);

            //para prender y apagar logs
            GuiController.Instance.Modifiers.addBoolean("Log", "Log", false);

            ///////////////CONFIGURAR CAMARA PRIMERA PERSONA//////////////////
            //Camara en primera persona, tipo videojuego FPS
            //Solo puede haber una camara habilitada a la vez. Al habilitar la camara FPS se deshabilita la camara rotacional
            //Por default la camara FPS viene desactivada
            GuiController.Instance.FpsCamera.Enable = true;
            //Configurar posicion y hacia donde se mira
            GuiController.Instance.FpsCamera.setCamera(new Vector3(0, 0, -20), new Vector3(0, 0, 0));
        }

        /// <summary>
        /// M�todo que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aqu� todo el c�digo referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el �ltimo frame</param>
        public override void render(float elapsedTime)
        {
            //BoundingBox
            this.partido.MostrarBounding = (bool)GuiController.Instance.Modifiers["BoundingBox"];

            this.partido.render(elapsedTime);
        }

        /// <summary>
        /// M�todo que se llama cuando termina la ejecuci�n del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            this.partido.dispose();
        }
    }
}