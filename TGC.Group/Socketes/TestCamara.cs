using AlumnoEjemplos.Socketes.Model;
using AlumnoEjemplos.Socketes.Model.Creacion;
using Microsoft.DirectX;
using System.Collections.Generic;
using System.Reflection;
using TGC.Core.Example;
using TGC.Core.Sound;
using TGC.Group;

namespace AlumnoEjemplos.Socketes
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class TestCamara : TgcExample
    {
        private Partido partido;
		private TgcFpsCamera camaraInterna;

		/// <summary>
		///     Constructor del test.
		/// </summary>
		/// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
		/// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
		public TestCamara(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
			Category = "SocketesTest";
			Name = "Test cámara";
			Description = "Test para poder volar feliz con la cámara.";
		}

        /// <summary>
        /// Método que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aquí todo el código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void Init()
        {
            //TODO Arreglar para despues :)
            Dictionary<string, TgcStaticSound> sonidos = new Dictionary<string, TgcStaticSound>();
            TgcStaticSound sonido = new TgcStaticSound();
            sonido.loadSound(MediaDir + "Audio\\pelota-tiro.wav", DirectSound.DsDevice);
            sonidos.Add("pelota-tiro", sonido);

			//FIXME arreglar para que el partido no tenga que recibir la camara.
			//this.partido = PartidoFactory.Instance.CrearPartido(pathRecursos, Input, sonidos, this.camaraInterna);

            //BoundingBox
            //GuiController.Instance.Modifiers.addBoolean("BoundingBox", "BoundingBox", false);

            //Luz
            //GuiController.Instance.Modifiers.addFloat("lightIntensity", 0, 100, 50);
            //GuiController.Instance.Modifiers.addFloat("lightAttenuation", 0.1f, 2, 0.20f);

			//Camara en 1ra persona
			this.camaraInterna = new TgcFpsCamera(new Vector3(260f, 170f, 390f), 400f, 300f, Input);
			Camara = this.camaraInterna;
            //GuiController.Instance.FpsCamera.setCamera(new Vector3(0, 10, -20), new Vector3(0, 0, 0));
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

		/// <summary>
		/// Método que se llama cada vez que hay que refrescar la pantalla.
		/// Escribir aquí todo el código referido al renderizado.
		/// Borrar todo lo que no haga falta
		/// </summary>
		/// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
		public override void Render()
        {
			//BoundingBox
			//this.partido.MostrarBounding = (bool)GuiController.Instance.Modifiers["BoundingBox"];
			this.partido.MostrarBounding = true;
            this.partido.render(ElapsedTime);
        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void Dispose()
        {
            this.partido.dispose();
        }
    }
}