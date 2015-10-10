using AlumnoEjemplos.Properties;
using AlumnoEjemplos.Socketes.Model;
using AlumnoEjemplos.Socketes.Model.Creacion;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Reflection;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.Sound;

namespace AlumnoEjemplos.Socketes
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class TestPelota : TgcExample
    {
        private Partido partido;

        /// <summary>
        /// Categoría a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el árbol de la derecha de la pantalla.
        /// </summary>
        public override string getCategory()
        {
            return "Socketes";
        }

        /// <summary>
        /// Completar nombre del grupo en formato Grupo NN
        /// </summary>
        public override string getName()
        {
            return "Test Pelota";
        }

        /// <summary>
        /// Completar con la descripción del TP
        /// </summary>
        public override string getDescription()
        {
            return "Juego de futbol by Socketes";
        }

        public override void init()
        {
            string pathRecursos = System.Environment.CurrentDirectory + "\\" + Assembly.GetExecutingAssembly().GetName().Name + "\\" + Settings.Default.mediaFolder;

            GuiController.Instance.Modifiers.addBoolean("Log", "Log", false);

            //TODO Arreglar para despues :)
            Dictionary<string, TgcStaticSound> sonidos = new Dictionary<string, TgcStaticSound>();
            TgcStaticSound sonido = new TgcStaticSound();
            sonido.loadSound(pathRecursos + "Audio\\pelota-tiro.wav");
            sonidos.Add("pelota-tiro", sonido);

            this.partido = PartidoFactory.Instance.CrearPartido(pathRecursos, GuiController.Instance.D3dInput, sonidos);

            //Configurar camara en Tercer Persona
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(this.partido.Pelota.Position, 200, -250);
        }

        public override void render(float elapsedTime)
        {
            //Calcular proxima posicion de personaje segun Input
            TgcD3dInput input = GuiController.Instance.D3dInput;

            Vector3 movement = new Vector3(0, 0, 0);
            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
            {
                movement.X = -1;
            }

            if (input.keyDown(Key.Right) || input.keyDown(Key.D))
            {
                movement.X = 1;
            }
            if (input.keyDown(Key.Up) || input.keyDown(Key.W))
            {
                movement.Z = 1;
            }

            if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                movement.Z = -1;
            }

            //Si presiono D, comienzo a acumular cuanto patear
            if (input.keyDown(Key.Z))
            {
                this.partido.Pelota.Patear(movement, 10);
            }
            else if (input.keyDown(Key.X))
            {
                //TODO Rene claramente cuando tengamos mas esto va a romper jeje
                this.partido.Pelota.Pasar(partido.EquipoLocal.Jugadores[1].Position, 2);
            }
            else if (movement.X != 0 || movement.Z != 0)
            {
                this.partido.Pelota.Mover(movement);
            }

            //Hacer que la camara siga al personaje en su nueva posicion
            GuiController.Instance.ThirdPersonCamera.Target = this.partido.Pelota.Position;

            //Render de todos los elementos del partido
            this.partido.render(elapsedTime);
        }

        public override void close()
        {
            this.partido.dispose();
        }
    }
}