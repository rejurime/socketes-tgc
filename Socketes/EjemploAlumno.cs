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
using TgcViewer.Utils.Sound;

namespace AlumnoEjemplos.Socketes
{
    /// <summary>
    /// Intentando ver que podemos inventar :)
    /// </summary>
    public class EjemploAlumno : TgcExample
    {
        #region Atributos

        private MenuInicial menu;
        private Partido partido;

        //TODO Parche feo para el tiempo
        private bool tiempo = false;

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

            DialogResult result = MessageBox.Show("¿Ejecutar en modo pantalla completa?", "Confirmación", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                //Se ejecuta en FullScreen
                GuiController.Instance.FullScreenEnable = true;
            }
            else
            {
                //Se ejecuta en modo normal
                GuiController.Instance.FullScreenEnable = false;
            }

            //Musica
            GuiController.Instance.Modifiers.addBoolean("Musica", "Música", true);
            //GuiController.Instance.Modifiers.addBoolean("Musica", "Música", false);

            //BoundingBox
            GuiController.Instance.Modifiers.addBoolean("BoundingBox", "BoundingBox", false);

            //Inteligencia Artificial
            GuiController.Instance.Modifiers.addBoolean("IA", "IA", true);
            //GuiController.Instance.Modifiers.addBoolean("IA", "IA", false);

            //Un boton para reiniciar las posiciones
            GuiController.Instance.Modifiers.addButton("ReiniciarPosiciones", "Reiniciar Posiciones", new EventHandler(this.ReiniciarPosiciones_Click));

            //Luz
            GuiController.Instance.Modifiers.addBoolean("Luz", "Luz", true);
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

            //Creo el menu
            this.menu = new MenuInicial(pathRecursos, GuiController.Instance.ThirdPersonCamera);

            //Creo el partido            
            this.partido = PartidoFactory.Instance.CrearPartido(pathRecursos, GuiController.Instance.D3dInput, sonidos);

            //Color de fondo
            GuiController.Instance.BackgroundColor = Color.Black;

            //FIX que nos mando Mariano para poder bajar el Alpha en el PixelShader
            GuiController.Instance.D3dDevice.RenderState.ReferenceAlpha = 10;
        }

        private void ReiniciarPosiciones_Click(object sender, EventArgs e)
        {
            this.partido.ReiniciarPosiciones();
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
            this.Player();

            if (this.menu.Enable)
            {
                this.menu.render(elapsedTime);
            }
            else
            {
                if (!this.tiempo)
                {
                    this.tiempo = true;
                    this.partido.Marcador.IniciarTiempo();
                    GuiController.Instance.ThirdPersonCamera.setCamera(new Vector3(this.partido.Pelota.Position.X, 0, this.partido.Pelota.Position.Z), Settings.Default.camaraOffsetHeight, Settings.Default.camaraOffsetForward);
                }
                //BoundingBox
                this.partido.MostrarBounding = (bool)GuiController.Instance.Modifiers["BoundingBox"];

                //Inteligencia Artificial
                this.partido.InteligenciaArtificial = (bool)GuiController.Instance.Modifiers["IA"];

                //Habilitar luz
                this.partido.Luz = (bool)GuiController.Instance.Modifiers["Luz"];

                this.partido.render(elapsedTime);

                //Hacer que la camara siga al personaje en su nueva posicion
                GuiController.Instance.ThirdPersonCamera.Target = new Vector3(this.partido.Pelota.Position.X, 0, this.partido.Pelota.Position.Z);
            }
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

        #endregion

        #region Cierre

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            this.tiempo = false;
            //Del menu ya hice dispose antes de empezar con el partido ;)
            this.partido.dispose();
        }

        #endregion
    }
}