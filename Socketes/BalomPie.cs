using AlumnoEjemplos.Properties;
using AlumnoEjemplos.Socketes.Model;
using AlumnoEjemplos.Socketes.Collision;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System;
using System.Reflection;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.Socketes
{
    /// <summary>
    /// Intentando ver que podemos inventar :)
    /// </summary>
    public class BalomPie : TgcExample
    {
        #region Atributos

        Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
        private Partido partido;
        private CollisionManager collisionManager;
        private string animacionCorriendo = Settings.Default.animationRunPlayer;
        private string animacionCaminando = Settings.Default.animationWalkPlayer;
        private string animacionParado = Settings.Default.animationStopPlayer;

        #endregion

        #region Creacion

        /// <summary>
        /// Categoría a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el árbol de la derecha de la pantalla.
        /// </summary>
        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        /// <summary> Socketes </summary>
        public override string getName()
        {
            return "Balompié";
        }

        /// <summary> Completar con la descripción del TP </summary>
        public override string getDescription()
        {
            return "Juego de fútbol by Socketes";
        }

        /// <summary>
        /// Método que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aquí todo el código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            string pathRecursos = System.Environment.CurrentDirectory + "\\" + Assembly.GetExecutingAssembly().GetName().Name + "\\" + Settings.Default.mediaFolder;

            //Musica
            GuiController.Instance.Modifiers.addBoolean("Musica", "Música", false);
            GuiController.Instance.Mp3Player.FileName = pathRecursos + "Audio\\Chumbawamba - Tubthumping.mp3";

            this.partido = PartidoFactory.Instance.CrearPartido(pathRecursos);

            //Crear manejador de colisiones
            collisionManager = new CollisionManager();
            collisionManager.GravityEnabled = true;

            //Configurar camara en Tercer Persona
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(this.partido.JugadorHumano.Position, Settings.Default.camaraOffsetHeight, Settings.Default.camaraOffsetForward);
        }

        #endregion

        #region Render

        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            //Contro del reproductor por Modifiers
            TgcMp3Player player = GuiController.Instance.Mp3Player;

            if ((bool)GuiController.Instance.Modifiers["Musica"])
            {
                if (player.getStatus() == TgcMp3Player.States.Open)
                {
                    //Reproducir MP3
                    player.play(true);
                }
                if (player.getStatus() == TgcMp3Player.States.Stopped)
                {
                    //Parar y reproducir MP3
                    player.closeFile();
                    player.play(true);
                }
            }
            else
            {
                if (player.getStatus() == TgcMp3Player.States.Playing)
                {
                    //Parar el MP3
                    player.stop();
                }
            }

            this.CalcularPosicionSegunInput(elapsedTime);
            this.partido.render();
        }

        /// <summary>
        /// Calculo cual es la proxima posicion en base a lo que tocan en el teclado
        /// </summary>
        /// <param name="elapsedTime"> Tiempo en segundos transcurridos desde el último frame</param>
        public void CalcularPosicionSegunInput(float elapsedTime)
        {
            //Calcular proxima posicion de personaje segun Input
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;

            Vector3 movimiento = new Vector3(0, 0, 0);
            Vector3 direccion = new Vector3(0, 0, 0);
            bool moving = false;
            bool correr = false;

            //Multiplicar la velocidad por el tiempo transcurrido, para no acoplarse al CPU
            float velocidad = this.partido.JugadorHumano.VelocidadCaminar * elapsedTime;

            //Si presiono W corre
            if (d3dInput.keyDown(Key.W))
            {
                //Multiplicar la velocidad por el tiempo transcurrido, para no acoplarse al CPU
                velocidad = this.partido.JugadorHumano.VelocidadCorrer * elapsedTime;
                correr = true;
            }

            //Si suelto, vuelve a caminar
            if (d3dInput.keyUp(Key.W))
            {
                correr = false;
            }

            //Adelante
            if (d3dInput.keyDown(Key.UpArrow))
            {
                movimiento.Z = velocidad;
                direccion.Y = (float)Math.PI;
                moving = true;
            }

            //Atras
            if (d3dInput.keyDown(Key.DownArrow))
            {
                movimiento.Z = -velocidad;
                //No hago nada en este caso por la rotacion
                moving = true;
            }

            //Derecha
            if (d3dInput.keyDown(Key.RightArrow))
            {
                movimiento.X = velocidad;
                direccion.Y = -(float)Math.PI / 2;
                moving = true;
            }

            //Izquierda
            if (d3dInput.keyDown(Key.LeftArrow))
            {
                movimiento.X = -velocidad;
                direccion.Y = (float)Math.PI / 2;
                moving = true;
            }

            //Diagonales, lo unico que hace es girar al jugador, el movimiento se calcula con el ingreso de cada tecla.
            if (d3dInput.keyDown(Key.UpArrow) && d3dInput.keyDown(Key.Right))
            {
                direccion.Y = (float)Math.PI * 5 / 4;
            }

            if (d3dInput.keyDown(Key.UpArrow) && d3dInput.keyDown(Key.LeftArrow))
            {
                direccion.Y = (float)Math.PI * 3 / 4;
            }
            if (d3dInput.keyDown(Key.DownArrow) && d3dInput.keyDown(Key.LeftArrow))
            {
                direccion.Y = (float)Math.PI / 4;
            }
            if (d3dInput.keyDown(Key.DownArrow) && d3dInput.keyDown(Key.RightArrow))
            {
                direccion.Y = (float)Math.PI * 7 / 4;
            }

            //Si presiono S, paso la pelota
            if (d3dInput.keyDown(Key.S))
            {

            }

            //Si presiono D, comienzo a acumular cuanto patear
            if (d3dInput.keyDown(Key.D))
            {

            }

            //Si sueldo D pateo la pelota con la fuerza acumulada
            if (d3dInput.keyUp(Key.D))
            {

            }


            //Si hubo desplazamiento
            if (moving)
            {
                //Activar animacion que corresponda
                if (correr)
                {
                    this.partido.JugadorHumano.playAnimation(this.animacionCorriendo, true);
                }
                else
                {
                    this.partido.JugadorHumano.playAnimation(this.animacionCaminando, true);
                }

                //Aplicar movimiento hacia adelante o atras segun la orientacion actual del Mesh
                Vector3 lastPos = this.partido.JugadorHumano.Position;

                //La velocidad de movimiento tiene que multiplicarse por el elapsedTime para hacerse independiente de la velocida de CPU
                //Ver Unidad 2: Ciclo acoplado vs ciclo desacoplado
                this.partido.JugadorHumano.move(movimiento);
                this.partido.JugadorHumano.Rotation = direccion;

                //Detecto las colisiones
                this.DetectarColisiones(lastPos);
            }
            //Si no se esta moviendo, activar animacion de Parado
            else
            {
                this.partido.JugadorHumano.playAnimation(this.animacionParado, true);
            }

            //Hacer que la camara siga al personaje en su nueva posicion
            GuiController.Instance.ThirdPersonCamera.Target = this.partido.JugadorHumano.Position;
        }

        /// <summary>
        /// Detecto si el jugador uno colisiona contra algo
        /// </summary>
        /// <param name="lastPos">Posicion anterior a moverse</param>
        public void DetectarColisiones(Vector3 lastPos)
        {
            //Detectar colisiones
            bool collide = false;

            foreach (TgcBox obstaculo in this.partido.Tribunas)
            {
                TgcCollisionUtils.BoxBoxResult result = TgcCollisionUtils.classifyBoxBox(this.partido.JugadorHumano.BoundingBox, obstaculo.BoundingBox);
                if (result == TgcCollisionUtils.BoxBoxResult.Adentro || result == TgcCollisionUtils.BoxBoxResult.Atravesando)
                {
                    collide = true;
                    break;
                }
            }

            //Si hubo colision, restaurar la posicion anterior
            if (collide)
            {
                this.partido.JugadorHumano.Position = lastPos;
            }
        }

        #endregion

        #region Cierre

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            this.partido.dispose();
        }

        #endregion
    }
}