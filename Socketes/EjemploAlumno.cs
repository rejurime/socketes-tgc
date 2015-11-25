using AlumnoEjemplos.Socketes.Menu;
using AlumnoEjemplos.Socketes.Model;
using AlumnoEjemplos.Socketes.Model.Creacion;
using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes
{
    /// <summary>
    /// Intentando ver que podemos inventar :)
    /// </summary>
    public class EjemploAlumno : TgcExample
    {
        #region Atributos

        private MenuInicial menu;
        private ConfiguracionPartido configuracionPartido;
        private Partido partido;
        private TgcSprite mapa;

        //TODO ver si esta se puede mejorar con un state :)
        private int pantallaActual;
        private bool cambiandoPantalla;

        //TODO Parche feo para el tiempo
        private bool tiempo = false;

        #endregion

        #region Propiedades

        public int PantallaActual
        {
            get { return pantallaActual; }
            set { pantallaActual = value; }
        }

        public bool CambiandoPantalla
        {
            get { return cambiandoPantalla; }
            set { cambiandoPantalla = value; }
        }

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
            string pathRecursos = Environment.CurrentDirectory + "\\" + Assembly.GetExecutingAssembly().GetName().Name + "\\" + Settings.Default.mediaFolder;

            //Musica
            GuiController.Instance.Modifiers.addBoolean("Musica", "Música", true);

            //BoundingBox
            GuiController.Instance.Modifiers.addBoolean("BoundingBox", "BoundingBox", false);

            //Inteligencia Artificial
            GuiController.Instance.Modifiers.addBoolean("IA", "IA", true);

            //Un boton para reiniciar las posiciones
            GuiController.Instance.Modifiers.addButton("ReiniciarPosiciones", "Reiniciar Posiciones", new EventHandler(this.ReiniciarPosiciones_Click));

            //Luz
            GuiController.Instance.Modifiers.addFloat("lightIntensity", 0, 100, 50);
            GuiController.Instance.Modifiers.addFloat("lightAttenuation", 0.1f, 2, 0.20f);

            //Empiezo con un tema Random :)
            int numbreTrack = new Random().Next(Settings.Default.music.Count);
            GuiController.Instance.Mp3Player.FileName = pathRecursos + Settings.Default.music[numbreTrack];

            //TODO Arreglar para despues :)
            Dictionary<string, TgcStaticSound> sonidos = new Dictionary<string, TgcStaticSound>();
            TgcStaticSound sonido = new TgcStaticSound();
            sonido.loadSound(pathRecursos + "Audio\\pelota-tiro.wav");
            sonidos.Add("pelota-tiro", sonido);

            //Configurar camara en Tercer Persona
            GuiController.Instance.ThirdPersonCamera.Enable = true;

            this.pantallaActual = 0;

            //Creo el menu
            this.menu = new MenuInicial(pathRecursos, GuiController.Instance.ThirdPersonCamera, this);

            //Creo la configuracion del partido
            this.configuracionPartido = new ConfiguracionPartido(pathRecursos, GuiController.Instance.Panel3d.Size, GuiController.Instance.ThirdPersonCamera, this);

            //Creo el partido            
            this.partido = PartidoFactory.Instance.CrearPartido(pathRecursos, GuiController.Instance.D3dInput, sonidos, GuiController.Instance.ThirdPersonCamera);

            //Mapa
            this.mapa = new TgcSprite();
            this.mapa.Texture = TgcTexture.createTexture(pathRecursos + "Texturas\\mapa.png");
            this.mapa.Scaling = new Vector2(0.75f, 0.75f);
            Size screenSize = GuiController.Instance.Panel3d.Size;
            Size textureSize = this.mapa.Texture.Size;
            this.mapa.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSize.Width / 2, 0), FastMath.Max((screenSize.Height - textureSize.Height), 0));

            //Color de fondo
            GuiController.Instance.BackgroundColor = Color.Black;

            //FIX que nos mando Mariano para poder bajar el Alpha en el PixelShader
            GuiController.Instance.D3dDevice.RenderState.ReferenceAlpha = 10;

            //FullScreen
            GuiController.Instance.FullScreenEnable = this.EjecutarFullScreen();
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
            //Reproductor
            this.Player();

            if (this.cambiandoPantalla)
            {
                this.cambiandoPantalla = false;
                return;
            }

            //Menu inicial
            if (this.pantallaActual == 0)
            {
                this.menu.render(elapsedTime);
                return;
            }

            //Menu de configuracion del partido
            if (this.pantallaActual == 1)
            {
                this.configuracionPartido.render(elapsedTime);
                return;
            }

            //Partido
            if (this.pantallaActual == 2)
            {
                this.RenderPartido(elapsedTime);
            }
        }

        private void RenderPartido(float elapsedTime)
        {
            //Todavia no inicio el partido
            if (!this.tiempo)
            {
                //TODO podria ser que Configuracion reciba el partido y haga como si fuera su builder, no lo termino de ver aun :P
                string pathTexturaPelota = this.configuracionPartido.GetPelotaActual().Texture.FilePath;
                string pathTexturaJugadorLocal = this.configuracionPartido.GetJugadorLocalActual().DiffuseMaps[0].FilePath;
                string pathTexturaJugadorVisitante = this.configuracionPartido.GetJugadorVisitanteActual().DiffuseMaps[0].FilePath;
                string estiloDeCamara = this.configuracionPartido.GetCamara();

                if (estiloDeCamara.Equals("Pelota"))
                {
                    this.partido.SetCamaraPelota();
                }

                if (estiloDeCamara.Equals("Aérea"))
                {
                    this.partido.SetCamaraAerea();
                }

                this.menu.close();
                this.configuracionPartido.close();

                this.partido.SetTexturasSeleccionadas(pathTexturaPelota, pathTexturaJugadorLocal, pathTexturaJugadorVisitante);
                this.partido.ReiniciarPosiciones();

                this.tiempo = true;
                this.partido.Marcador.IniciarTiempo();
            }

            //BoundingBox
            this.partido.MostrarBounding = (bool)GuiController.Instance.Modifiers["BoundingBox"];

            //Inteligencia Artificial
            this.partido.InteligenciaArtificial = (bool)GuiController.Instance.Modifiers["IA"];

            this.partido.render(elapsedTime);

            //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
            GuiController.Instance.Drawer2D.beginDrawSprite();

            //Dibujar sprite (si hubiese mas, deberian ir todos aquí)
            this.mapa.render();

            //Finalizar el dibujado de Sprites
            GuiController.Instance.Drawer2D.endDrawSprite();
        }

        private void Player()
        {
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
        }

        private void ReiniciarPosiciones_Click(object sender, EventArgs e)
        {
            this.partido.ReiniciarPosiciones();
        }

        private bool EjecutarFullScreen()
        {
            DialogResult result = MessageBox.Show("¿Ejecutar en modo pantalla completa?", "Confirmación", MessageBoxButtons.YesNo);

            //Por si toque enter y no hice click no lo capturen los menus que ya estan creados ;)
            this.cambiandoPantalla = true;
            return result == DialogResult.Yes;
        }

        #endregion

        #region Cierre

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            this.tiempo = false;
            this.pantallaActual = 0;
            //Del menu ya hice dispose antes de empezar con el partido ;)
            this.partido.dispose();
        }

        #endregion
    }
}