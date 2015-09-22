using AlumnoEjemplos.MiGrupo.Model;
using AlumnoEjemplos.Properties;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.MiGrupo
{
    /// <summary>
    /// Intentando ver que podemos inventar :)
    /// </summary>
    public class BalomPie : TgcExample
    {
        #region Atributos

        Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
        private Partido partido;
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
            return "Pruebas locas para jugar al balómpie";
        }

        /// <summary>
        /// Método que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aquí todo el código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            this.partido = PartidoFactory.Instance.CrearPartido(this.d3dDevice);
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
            float moveForward = 0f;
            float rotate = 0;
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            bool moving = false;
            bool rotating = false;

            //Adelante
            if (d3dInput.keyDown(Key.UpArrow))
            {
                moveForward = -this.partido.JugadorHumano.VelocidadCaminar;
                moving = true;
            }

            //Atras
            if (d3dInput.keyDown(Key.DownArrow))
            {
                moveForward = this.partido.JugadorHumano.VelocidadCaminar;
                moving = true;
            }

            //Derecha
            if (d3dInput.keyDown(Key.RightArrow))
            {
                rotate = this.partido.JugadorHumano.VelocidadRotacion;
                rotating = true;
            }

            //Izquierda
            if (d3dInput.keyDown(Key.LeftArrow))
            {
                rotate = -this.partido.JugadorHumano.VelocidadRotacion;
                rotating = true;
            }

            //Si hubo rotacion
            if (rotating)
            {
                //Rotar personaje y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
                float rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime);
                this.partido.JugadorHumano.rotateY(rotAngle);
                GuiController.Instance.ThirdPersonCamera.rotateY(rotAngle);
            }

            //Si hubo desplazamiento
            if (moving)
            {
                //Activar animacion de caminando
                //this.jugadorHumano.playAnimation("Caminando", true);
                this.partido.JugadorHumano.playAnimation(this.animacionCaminando, true);

                //Aplicar movimiento hacia adelante o atras segun la orientacion actual del Mesh
                Vector3 lastPos = this.partido.JugadorHumano.Position;

                //La velocidad de movimiento tiene que multiplicarse por el elapsedTime para hacerse independiente de la velocida de CPU
                //Ver Unidad 2: Ciclo acoplado vs ciclo desacoplado
                this.partido.JugadorHumano.moveOrientedY(moveForward * elapsedTime);

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