using AlumnoEjemplos.Socketes.Model;
using AlumnoEjemplos.Socketes.Model.Creacion;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Reflection;
using TGC.Core.Example;
using TGC.Core.Sound;

namespace AlumnoEjemplos.Socketes
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class TestPelota : TgcExample
    {
        private Partido partido;

		/// <summary>
		///     Constructor del test.
		/// </summary>
		/// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
		/// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
		public TestPelota(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
			Category = "SocketesTest";
			Name = "Test pelota";
			Description = "Juego de futbol by Socketes";
		}

        public override void Init()
        {
            string pathRecursos = System.Environment.CurrentDirectory + "\\" + Assembly.GetExecutingAssembly().GetName().Name + "\\" + Settings.Default.mediaFolder;

            //BoundingBox
            //GuiController.Instance.Modifiers.addBoolean("BoundingBox", "BoundingBox", false);

            //Luz
            //GuiController.Instance.Modifiers.addFloat("lightIntensity", 0, 100, 50);
            //GuiController.Instance.Modifiers.addFloat("lightAttenuation", 0.1f, 2, 0.20f);

            //TODO Arreglar para despues :)
            Dictionary<string, TgcStaticSound> sonidos = new Dictionary<string, TgcStaticSound>();
            TgcStaticSound sonido = new TgcStaticSound();
            sonido.loadSound(pathRecursos + "Audio\\pelota-tiro.wav", DirectSound.DsDevice);
            sonidos.Add("pelota-tiro", sonido);

            //this.partido = PartidoFactory.Instance.CrearPartido(pathRecursos, Input, sonidos, GuiController.Instance.ThirdPersonCamera);

            //Configurar camara en Tercer Persona
            //GuiController.Instance.ThirdPersonCamera.Enable = true;
            //GuiController.Instance.ThirdPersonCamera.setCamera(this.partido.Pelota.Position, 200, -250);
        }

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

		public override void Render()
        {
			//Calcular proxima posicion de personaje segun Input
            Vector3 movement = new Vector3(0, 0, 0);
            if (Input.keyDown(Key.Left) || Input.keyDown(Key.A))
            {
                movement.X = -1;
            }

            if (Input.keyDown(Key.Right) || Input.keyDown(Key.D))
            {
                movement.X = 1;
            }
            if (Input.keyDown(Key.Up) || Input.keyDown(Key.W))
            {
                movement.Z = 1;
            }

            if (Input.keyDown(Key.Down) || Input.keyDown(Key.S))
            {
                movement.Z = -1;
            }

            //Si presiono D, comienzo a acumular cuanto patear
            if (Input.keyDown(Key.Z))
            {
                this.partido.Pelota.Patear(movement, 10);
            }
            else if (Input.keyDown(Key.X))
            {
                //TODO Rene claramente cuando tengamos mas esto va a romper jeje
                this.partido.Pelota.Pasar(partido.EquipoLocal.Jugadores[1].Position, 2);
            }
            else if (movement.X != 0 || movement.Z != 0)
            {
                this.partido.Pelota.Mover(movement);
            }

            //Hacer que la camara siga al personaje en su nueva posicion
            //GuiController.Instance.ThirdPersonCamera.Target = this.partido.Pelota.Position;

            //Render de todos los elementos del partido
            this.partido.render(ElapsedTime);
        }

        public override void Dispose()
        {
            this.partido.dispose();
        }
    }
}