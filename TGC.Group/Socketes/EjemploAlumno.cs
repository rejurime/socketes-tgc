using AlumnoEjemplos.Socketes.Menu;
using AlumnoEjemplos.Socketes.Model;
using AlumnoEjemplos.Socketes.Model.Creacion;
using AlumnoEjemplos.Socketes.Model.Jugadores;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using TGC.Core.Example;
using TGC.Core.Sound;
using TGC.Group;
using TGC.Core.Direct3D;
using Microsoft.DirectX.Direct3D;

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
		private Drawer2D drawer2D;
        private CustomSprite mapa;
        private CustomSprite puntoAzul;
        private CustomSprite puntoNaranja;
		private TgcThirdPersonCamera camaraInterna;

        //TODO ver si esta se puede mejorar con un state :)
        private int pantallaActual;

        //TODO Parche feo para el tiempo
        private bool tiempo = false;

        #endregion

        #region Propiedades

        public int PantallaActual
        {
            get { return pantallaActual; }
            set { pantallaActual = value; }
        }

		#endregion

		#region Creacion

		/// <summary>
		///     Constructor del juego.
		/// </summary>
		/// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
		/// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
		public EjemploAlumno(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
			Category = Game.Default.Category;
			Name = Game.Default.Name;
			Description = Game.Default.Description;
		}

        /// <summary>
        /// Método que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aquí todo el código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void Init()
        {
            //Musica
            //GuiController.Instance.Modifiers.addBoolean("Musica", "Música", true);

            //BoundingBox
            //GuiController.Instance.Modifiers.addBoolean("BoundingBox", "BoundingBox", false);

            //Inteligencia Artificial
            //GuiController.Instance.Modifiers.addBoolean("IA", "IA", true);

            //Un boton para reiniciar las posiciones
            //GuiController.Instance.Modifiers.addButton("ReiniciarPosiciones", "Reiniciar Posiciones", new EventHandler(this.ReiniciarPosiciones_Click));

            //Luz
            //GuiController.Instance.Modifiers.addFloat("lightIntensity", 0, 100, 50);
            //GuiController.Instance.Modifiers.addFloat("lightAttenuation", 0.1f, 2, 0.20f);

            //Empiezo con un tema Random :)
            int numbreTrack = new Random().Next(Settings.Default.music.Count);
            //GuiController.Instance.Mp3Player.FileName = pathRecursos + Settings.Default.music[numbreTrack];

            //TODO Arreglar para despues :)
            Dictionary<string, TgcStaticSound> sonidos = new Dictionary<string, TgcStaticSound>();
            TgcStaticSound sonido = new TgcStaticSound();
            sonido.loadSound(MediaDir + "Audio\\pelota-tiro.wav", DirectSound.DsDevice);
            sonidos.Add("pelota-tiro", sonido);

            //Configurar camara en Tercer Persona
			camaraInterna = new TgcThirdPersonCamera();
			Camara = camaraInterna;

            this.pantallaActual = 0;

            //Creo el menu
            this.menu = new MenuInicial(MediaDir, camaraInterna, this);

            //Creo la configuracion del partido
			this.configuracionPartido = new ConfiguracionPartido(MediaDir, D3DDevice.Instance.Width, camaraInterna, this);

            //Creo el partido            
            this.partido = PartidoFactory.Instance.CrearPartido(MediaDir, Input, sonidos, camaraInterna);

			drawer2D = new Drawer2D();

            //Mapa
            this.mapa = new CustomSprite();
			this.mapa.Bitmap = new CustomBitmap(MediaDir + "Texturas\\mapa.png", D3DDevice.Instance.Device);
            //this.mapa.Scaling = new Vector2(0.75f, 0.75f);
			Size textureSize = this.mapa.Bitmap.Size;
            this.mapa.Position = new Vector2((D3DDevice.Instance.Width - textureSize.Width) / 2, (D3DDevice.Instance.Height - textureSize.Height));

            this.puntoAzul = new CustomSprite();
            this.puntoAzul.Bitmap = new CustomBitmap(MediaDir + "Texturas\\radarAzul.png", D3DDevice.Instance.Device);
            this.puntoAzul.Scaling = new Vector2(0.03f, 0.03f);

            this.puntoNaranja = new CustomSprite();
            this.puntoNaranja.Bitmap = new CustomBitmap(MediaDir + "Texturas\\radarNaranja.png", D3DDevice.Instance.Device);
            this.puntoNaranja.Scaling = new Vector2(0.03f, 0.03f);

			//Color de fondo (BackgroundColor)
			D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            //FIX que nos mando Mariano para poder bajar el Alpha en el PixelShader
			D3DDevice.Instance.Device.RenderState.ReferenceAlpha = 10;
        }

		#endregion

		#region Update
		/// <summary>
		///     Se llama en cada frame.
		///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
		///     ante ellas.
		/// </summary>
		public override void Update()
		{
			PreUpdate();
		}
		#endregion

        #region Render

        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void Render()
        {
			//Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
			PreRender();

            //Reproductor
            this.Player();

            //Menu inicial
            if (this.pantallaActual == 0)
            {
                this.menu.render(ElapsedTime);
				PostRender();
                return;
            }

            //Menu de configuracion del partido
            if (this.pantallaActual == 1)
            {
                this.configuracionPartido.render(ElapsedTime);
				PostRender();
                return;
            }

            //Partido
            if (this.pantallaActual == 2)
            {
                this.RenderPartido(ElapsedTime);
            }

			//Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
			PostRender();
        }

        private void RenderPartido(float elapsedTime)
        {
            if (Input.keyPressed(Key.F10))
            {
                this.partido.ReiniciarPosiciones();
                return;
            }

            if (Input.keyPressed(Key.F9))
            {
                this.partido.SetCamaraAerea();
                return;
            }

            if (Input.keyPressed(Key.F8))
            {
                this.partido.SetCamaraPelota();
                return;
            }

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
            //this.partido.MostrarBounding = (bool)GuiController.Instance.Modifiers["BoundingBox"];

            //Inteligencia Artificial
            //this.partido.InteligenciaArtificial = (bool)GuiController.Instance.Modifiers["IA"];

            this.partido.render(elapsedTime);

            //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
            drawer2D.BeginDrawSprite();

            //Dibujar sprite (si hubiese mas, deberian ir todos aquí)
			drawer2D.DrawSprite(this.mapa);
            this.RenderJugadoresMapa(this.partido.EquipoLocal.Jugadores, this.puntoAzul);
            this.RenderJugadoresMapa(this.partido.EquipoVisitante.Jugadores, this.puntoNaranja);

            //Finalizar el dibujado de Sprites
            drawer2D.EndDrawSprite();
        }

        private void Player()
        {
            TgcMp3Player player = new TgcMp3Player();

            //if ((bool)GuiController.Instance.Modifiers["Musica"])
            {
                if (player.getStatus() == TgcMp3Player.States.Open)
                {
                    //Reproducir MP3
					//FIXME
                    //player.play(true);
                }
                if (player.getStatus() == TgcMp3Player.States.Stopped)
                {
                    //Parar y reproducir MP3
                    player.closeFile();
                    player.play(true);
                }
            }
            /*else
            {
                if (player.getStatus() == TgcMp3Player.States.Playing)
                {
                    //Parar el MP3
                    player.stop();
                }
            }*/
        }

        private void ReiniciarPosiciones_Click(object sender, EventArgs e)
        {
            this.partido.ReiniciarPosiciones();
        }

		private void RenderJugadoresMapa(List<Jugador> jugadores, CustomSprite punto)
        {
            foreach (Jugador jugador in jugadores)
            {
                Vector2 mapPosition = this.GetMapPosition(jugador.Position, this.mapa.Bitmap.Size, this.partido.Cancha.Size);
                //El 5 es porque en el x la imagen del a foto incluye mucho pasto fuera de las lineas
				float pX = this.mapa.Position.X - 5 + (this.mapa.Bitmap.Size.Width / 2) + mapPosition.X;
                float pZ = this.mapa.Position.Y + (this.mapa.Bitmap.Size.Height / 2) - mapPosition.Y;
                punto.Position = new Vector2(pX, pZ);

				drawer2D.DrawSprite(punto);
            }
        }

        private Vector2 GetMapPosition(Vector3 posicion, Size mapSize, Vector3 canchaSize)
        {
            float posicionX = posicion.X * mapSize.Width / canchaSize.X;
            float posicionY = posicion.Z * mapSize.Height / canchaSize.Z;
            return new Vector2(posicionX, posicionY);
        }

        #endregion

        #region Cierre

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void Dispose()
        {
            this.tiempo = false;
            this.pantallaActual = 0;
            //Del menu ya hice dispose antes de empezar con el partido ;)
            this.partido.dispose();
        }

        #endregion
    }
}
