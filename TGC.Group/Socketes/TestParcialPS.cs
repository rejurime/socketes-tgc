using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Drawing;
using System.Reflection;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Utils;
using TGC.Core.Direct3D;

namespace AlumnoEjemplos.Socketes
{
    public class TestParcialPS : TgcExample
    {
        private string pathRecursos;
        private TgcBox boxFloor;
        private TgcBox boxWall;
        private TgcBox boxFigure;
        private TgcBox boxLuz;
        private Effect effect;
        private TgcMesh meshTetera;
        private Texture g_pNormals;
        private TgcMesh meshBuggy;

		/// <summary>
		///     Constructor del test.
		/// </summary>
		/// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
		/// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
		public TestParcialPS(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
			Category = "SocketesTest";
			Name = "Test parcial PS";
			Description = "Probando ejercicios de la guia :)";
		}

        public override void Init()
        {
            //Color de fondo
            //GuiController.Instance.BackgroundColor = Color.DarkGray;

            //Camara en 1ra persona
            //GuiController.Instance.FpsCamera.Enable = true;
            //GuiController.Instance.FpsCamera.MovementSpeed = 100f;
            //GuiController.Instance.FpsCamera.JumpSpeed = 50f;
            //GuiController.Instance.FpsCamera.setCamera(new Vector3(40, 30, 35), new Vector3(0, 0, 0));

            this.effect = TgcShaders.loadEffect(this.pathRecursos + "Shaders\\ParcialPixelShader.fx");

            //Piso
            this.boxFloor = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(30, 0, 30), Color.DarkSeaGreen);
            this.boxFloor.Effect = this.effect;

            //Pared
            this.boxWall = TgcBox.fromSize(new Vector3(0, 10, -15), new Vector3(30, 20, 0), Color.Red);
            this.boxWall.Effect = this.effect;

            //Figura
            this.boxFigure = TgcBox.fromSize(new Vector3(0, 5, 0), new Vector3(10, 10, 5), Color.SteelBlue);
            this.boxFigure.Effect = this.effect;

            //Luz
            this.boxLuz = TgcBox.fromSize(new Vector3(0, 20, 20), new Vector3(2, 2, 2), Color.LightYellow);

            this.CrearTetera();
            this.CrearBuggy();
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
            this.boxFloor.Technique = "PS1";
            this.boxFloor.Effect.SetValue("g_vLightPos", TgcParserUtils.vector3ToVector4(this.boxLuz.Position));
            this.boxFloor.Effect.SetValue("g_vLightDir", TgcParserUtils.vector3ToVector4(new Vector3(0, 0, 0) - this.boxLuz.Position));
            this.boxWall.Technique = "PS1";
            this.boxFigure.Technique = "PS1";

            this.boxFloor.render();
            this.boxWall.render();
            this.boxFigure.render();
            this.boxLuz.render();

            this.meshTetera.Technique = "PS2";
            this.meshTetera.render();

            this.meshBuggy.render();
        }

        public override void Dispose()
        {
            this.boxFloor.dispose();
            this.boxWall.dispose();
            this.boxFigure.dispose();
            this.boxLuz.dispose();
            this.meshTetera.dispose();
            this.meshBuggy.dispose();
        }

        public void CrearTetera()
        {
			Device d3dDevice = D3DDevice.Instance.Device;

            //Cargar mesh
            this.meshTetera = new TgcSceneLoader().loadSceneFromFile(MediaDir + "ModelosTgc\\Teapot\\Teapot-TgcScene.xml").Meshes[0];
            this.meshTetera.Scale = new Vector3(0.5f, 0.5f, 0.5f);
            this.meshTetera.Position = new Vector3(30, 10, 0);

            // Arreglo las normales
            int[] adj = new int[this.meshTetera.D3dMesh.NumberFaces * 3];
            this.meshTetera.D3dMesh.GenerateAdjacency(0, adj);
            this.meshTetera.D3dMesh.ComputeNormals(adj);

            // inicializo el mapa de normales
            g_pNormals = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth,
                d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.A16B16G16R16F, Pool.Default);

            this.effect.SetValue("g_Normals", g_pNormals);

            // Resolucion de pantalla
            this.effect.SetValue("screen_dx", d3dDevice.PresentationParameters.BackBufferWidth);
            this.effect.SetValue("screen_dy", d3dDevice.PresentationParameters.BackBufferHeight);

            this.meshTetera.Effect = this.effect;
        }

        public void CrearBuggy()
        {
            //Cargar modelo estatico
            this.meshBuggy = new TgcSceneLoader().loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Buggy\\Buggy-TgcScene.xml").Meshes[0];
            this.meshBuggy.Scale = new Vector3(0.5f, 0.5f, 0.5f);
            this.meshBuggy.Position = new Vector3(-30, 10, 0);
            this.meshBuggy.Effect = this.effect;
            this.meshBuggy.Technique = "PS3";
        }
    }
}
