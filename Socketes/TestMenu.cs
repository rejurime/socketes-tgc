using AlumnoEjemplos.Properties;
using AlumnoEjemplos.Socketes.Model;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class TestMenu : TgcExample
    {
        private TgcText2d titulo;
        private TgcSphere pelota;
        private TgcBox cancha;
        private List<MenuItem> menues;

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
            return "Test Men�";
        }

        /// <summary>
        /// Completar con la descripci�n del TP
        /// </summary>
        public override string getDescription()
        {
            return "Probando cosas para el men�";
        }

        /// <summary>
        /// M�todo que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aqu� todo el c�digo de inicializaci�n: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            string pathRecursos = System.Environment.CurrentDirectory + "\\" + Assembly.GetExecutingAssembly().GetName().Name + "\\" + Settings.Default.mediaFolder;

            this.titulo = new TgcText2d();
            this.titulo.Text = "Socketes";
            this.titulo.Color = Color.White;
            this.titulo.Align = TgcText2d.TextAlign.CENTER;
            this.titulo.Position = new Point(280, 0);
            this.titulo.Size = new Size(400, 100);
            this.titulo.changeFont(new System.Drawing.Font("Arial", 35));

            //Crear esfera
            this.pelota = new TgcSphere();
            this.pelota.setTexture(TgcTexture.createTexture(pathRecursos + Settings.Default.textureFolder + Settings.Default.textureBall));
            this.pelota.Radius = 2.5f;
            this.pelota.LevelOfDetail = 4;
            this.pelota.Position = new Vector3(-4, 0, 0);
            this.pelota.updateValues();

            this.cancha = TgcBox.fromSize(new Vector3(15, 0, 15), TgcTexture.createTexture(pathRecursos + Settings.Default.textureFolder + "canchaMenu.jpg"));
            this.cancha.Position = new Vector3(0, -2.5f, 0);

            this.menues = new List<MenuItem>();
            this.menues.Add(new MenuItem(new Vector3(6, 1, 0), new Vector3(0, 1, 0), "Picadito"));
            this.menues.Add(new MenuItem(new Vector3(6, 0, 0), new Vector3(0, 0, 0), "Opciones"));
            this.menues.Add(new MenuItem(new Vector3(6, -1, 0), new Vector3(0, -1, 0), "Salir"));

            GuiController.Instance.BackgroundColor = Color.Black;
        }

        /// <summary>
        /// M�todo que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aqu� todo el c�digo referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el �ltimo frame</param>
        public override void render(float elapsedTime)
        {
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;

            //Adelante
            if (d3dInput.keyDown(Key.UpArrow))
            {

            }

            //Atras
            if (d3dInput.keyDown(Key.DownArrow))
            {

            }

            //Enter
            if (d3dInput.keyDown(Key.Return))
            {

            }

            this.titulo.render();
            this.pelota.render();
            this.cancha.render();

            foreach (MenuItem item in this.menues)
            {
                item.render();
            }
        }

        /// <summary>
        /// M�todo que se llama cuando termina la ejecuci�n del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            this.titulo.dispose();
            this.pelota.dispose();
            this.cancha.dispose();

            foreach (MenuItem item in this.menues)
            {
                item.dispose();
            }
        }
    }
}