using AlumnoEjemplos.Properties;
using AlumnoEjemplos.Socketes.Collision;
using AlumnoEjemplos.Socketes.Model;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Reflection;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class TestPelota : TgcExample
    {
        private Partido partido;
        private CollisionManager collisionManager;

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

            this.partido = PartidoFactory.Instance.CrearPartido(pathRecursos);

            //VER CON RENE, ahora la pelota maneja las colisiones, y debe conocer otdos los obstaculos, pero en la creaicon depende de caundo se crean las cosas
            //Crear manejador de colisiones
            collisionManager = new CollisionManager(this.partido.ObstaculosPelota());
            collisionManager.GravityEnabled = true;
            this.partido.Pelota.collisionManager = collisionManager;
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
                this.partido.Pelota.patear(movement, 10);
            }

            //pase de pelota
            if (input.keyDown(Key.X))
            {
                this.partido.Pelota.pasar(partido.getJugadoresCPUAliado().Position, 2);
            }

            this.partido.Pelota.updateValues(elapsedTime);
            //Hacer que la camara siga al personaje en su nueva posicion
            GuiController.Instance.ThirdPersonCamera.Target = this.partido.Pelota.Position;

            //Render de todos los elementos del partido
            this.partido.render();
        }

        public override void close()
        {
            this.partido.dispose();
        }
    }
}