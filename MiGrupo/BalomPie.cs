using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;

namespace AlumnoEjemplos.MiGrupo
{
    /// <summary>
    /// Intentando ver que podemos inventar :)
    /// </summary>
    public class BalomPie : TgcExample
    {
        #region Atributos

        Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

        private TgcBox cancha;
        private TgcSphere pelota;
        private TgcBox arcoLocal;
        private TgcBox arcoVisitante;
        private TgcSkeletalMesh jugadorHumano;
        private List<TgcSkeletalMesh> jugadoresCPUAliados = new List<TgcSkeletalMesh>();
        private List<TgcSkeletalMesh> jugadoresCPURivales = new List<TgcSkeletalMesh>();
        float velocidadCaminar = 400f;
        float velocidadRotacion = 150f;

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
            this.crearCancha(d3dDevice);
            this.crearPelota(d3dDevice);
            this.crearArcos(d3dDevice);
            this.crearJugadores(d3dDevice);

            //Configurar camara en Tercer Persona
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(this.jugadorHumano.Position, 400, -1100);
        }

        /// <summary>
        /// Creo la cancha donde van a estar parado los jugadores
        /// </summary>
        /// <param name="d3dDevice"></param>
        public void crearCancha(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Texturas\\cancha.jpg");
            this.cancha = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(1920, 5, 1200), pisoTexture);
        }

        /// <summary>
        /// Creo la pelota en el centro de la cancha
        /// </summary>
        /// <param name="d3dDevice"></param>
        public void crearPelota(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            //Crear esfera
            this.pelota = new TgcSphere();

            this.pelota.setTexture(TgcTexture.createTexture(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Texturas\\pelota.jpg"));
            this.pelota.Radius = 10;
            this.pelota.Position = new Vector3(0, 10, 0);

            this.pelota.updateValues();
        }

        /// <summary>
        /// Creo los 2 arcos
        /// </summary>
        /// <param name="d3dDevice"></param>
        public void crearArcos(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            TgcTexture texturaArco = TgcTexture.createTexture(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Texturas\\red.jpg");
            Vector3 size = new Vector3(20, 125, 250);

            this.arcoLocal = TgcBox.fromSize(new Vector3(875, 50, 0), size, texturaArco);
            this.arcoVisitante = TgcBox.fromSize(new Vector3(-875, 50, 0), size, texturaArco);
        }

        /// <summary>
        /// Creo los 4 jugadores, 2 de cada equipo
        /// </summary>
        /// <param name="d3dDevice"></param>
        public void crearJugadores(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            this.jugadorHumano = this.crearJugador(new Vector3(15, 0, 0), 90f, "uvw.jpg");
            this.jugadoresCPUAliados.Add(this.crearJugador(new Vector3(100, 0, 100), 90f, "uvw.jpg"));

            this.jugadoresCPURivales.Add(this.crearJugador(new Vector3(-125, 0, 160), 270f, "uvwGreen.jpg"));
            this.jugadoresCPURivales.Add(this.crearJugador(new Vector3(-150, 0, -160), 270f, "uvwGreen.jpg"));
        }

        /// <summary>
        /// Creo un jugador basado en el Robot de TGC
        /// </summary>
        /// <param name="posicion">Posicion donde va a estar el jugador</param>
        /// <param name="angulo">El angulo donde va a mirar</param>
        /// <param name="nombreTextura">Que textura va a tener</param>
        /// <returns></returns>
        public TgcSkeletalMesh crearJugador(Vector3 posicion, float angulo, string nombreTextura)
        {
            //Cargar personaje con animaciones
            TgcSkeletalMesh personaje = new TgcSkeletalLoader().loadMeshAndAnimationsFromFile(
                GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\" + "Robot-TgcSkeletalMesh.xml",
                GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\",
                new string[] { GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\" + "Caminando-TgcSkeletalAnim.xml", GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\" + "Correr-TgcSkeletalAnim.xml", GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\" + "Parado-TgcSkeletalAnim.xml", }
                );

            //Le cambiamos la textura para diferenciarlo un poco
            personaje.changeDiffuseMaps(new TgcTexture[] { TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\Textures\\" + nombreTextura) });

            //Configurar animacion inicial
            personaje.playAnimation("Parado", true);
            personaje.Position = posicion;

            //Lo Escalo porque es muy grande
            personaje.Scale = new Vector3(0.55f, 0.55f, 0.55f);
            personaje.rotateY(Geometry.DegreeToRadian(angulo));

            return personaje;
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
            this.calcularPosicionSegunInput(elapsedTime);

            this.cancha.render();
            this.pelota.render();
            this.arcoLocal.render();
            this.arcoVisitante.render();
            this.jugadorHumano.animateAndRender();

            foreach (TgcSkeletalMesh jugador in this.jugadoresCPUAliados)
            {
                jugador.animateAndRender();
            }

            foreach (TgcSkeletalMesh jugador in this.jugadoresCPURivales)
            {
                jugador.animateAndRender();
            }
        }

        public void calcularPosicionSegunInput(float elapsedTime)
        {
            //Calcular proxima posicion de personaje segun Input
            float moveForward = 0f;
            float rotate = 0;
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            bool moving = false;
            bool rotating = false;

            //Adelante
            if (d3dInput.keyDown(Key.W))
            {
                moveForward = -velocidadCaminar;
                moving = true;
            }

            //Atras
            if (d3dInput.keyDown(Key.S))
            {
                moveForward = velocidadCaminar;
                moving = true;
            }

            //Derecha
            if (d3dInput.keyDown(Key.D))
            {
                rotate = velocidadRotacion;
                rotating = true;
            }

            //Izquierda
            if (d3dInput.keyDown(Key.A))
            {
                rotate = -velocidadRotacion;
                rotating = true;
            }

            //Si hubo rotacion
            if (rotating)
            {
                //Rotar personaje y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
                float rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime);
                this.jugadorHumano.rotateY(rotAngle);
                GuiController.Instance.ThirdPersonCamera.rotateY(rotAngle);
            }

            //Si hubo desplazamiento
            if (moving)
            {
                //Activar animacion de caminando
                //this.jugadorHumano.playAnimation("Caminando", true);
                this.jugadorHumano.playAnimation("Correr", true);

                //Aplicar movimiento hacia adelante o atras segun la orientacion actual del Mesh
                Vector3 lastPos = this.jugadorHumano.Position;

                //La velocidad de movimiento tiene que multiplicarse por el elapsedTime para hacerse independiente de la velocida de CPU
                //Ver Unidad 2: Ciclo acoplado vs ciclo desacoplado
                this.jugadorHumano.moveOrientedY(moveForward * elapsedTime);

                this.detectarColisiones(lastPos);
            }
            //Si no se esta moviendo, activar animacion de Parado
            else
            {
                this.jugadorHumano.playAnimation("Parado", true);
            }

            //Hacer que la camara siga al personaje en su nueva posicion
            GuiController.Instance.ThirdPersonCamera.Target = this.jugadorHumano.Position;
        }

        /// <summary>
        /// Detecto si el jugador uno colisiona contra algo
        /// </summary>
        /// <param name="lastPos">Posicion anterior a moverse</param>
        public void detectarColisiones(Vector3 lastPos)
        {
            //Detectar colisiones
            bool collide = false;

            /* TODO René: aca es donde procesa las colisiones lo saque para terminar de armar el escenario y endriamoq eu pensar como vamos a hacer
            para tener este tema de las coliciones a tono con todo el resto de las reglas no? antes los arcos eran parte de los obstaculos pro ejemplo
            pero si no los tengo identificado a cada uno como se donde se hizo gol y esas cosas? jeje
            foreach (TgcBox obstaculo in arcos)
            {
                TgcCollisionUtils.BoxBoxResult result = TgcCollisionUtils.classifyBoxBox(this.jugadorHumano.BoundingBox, obstaculo.BoundingBox);
                if (result == TgcCollisionUtils.BoxBoxResult.Adentro || result == TgcCollisionUtils.BoxBoxResult.Atravesando)
                {
                    collide = true;
                    break;
                }
            }
            */

            //Si hubo colision, restaurar la posicion anterior
            if (collide)
            {
                this.jugadorHumano.Position = lastPos;
            }
            else
            {

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
            this.cancha.dispose();
            this.pelota.dispose();
            this.arcoLocal.dispose();
            this.arcoVisitante.dispose();
            this.jugadorHumano.dispose();

            foreach (TgcSkeletalMesh jugador in this.jugadoresCPUAliados)
            {
                jugador.dispose();
            }

            foreach (TgcSkeletalMesh jugador in this.jugadoresCPURivales)
            {
                jugador.dispose();
            }
        }

        #endregion
    }
}