using AlumnoEjemplos.Socketes.Model;
using AlumnoEjemplos.Socketes.Model.Iluminacion;
using AlumnoEjemplos.Socketes.Model.Jugadores;
using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Textures;
using TGC.Core.SkeletalAnimation;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.Socketes
{
    public class TestIluminacion : TgcExample
    {
        private Pelota pelota;
        private Jugador jugador;
        private Effect meshMultiDiffuseLights;
        private Effect skeletalMeshPointLight;
        private List<Luz> luces;
        private TgcBox piso;
        private TgcMesh cartel1;
        private Effect cartelEffect;
        private float time;

		/// <summary>
		///     Constructor del test.
		/// </summary>
		/// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
		/// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
		public TestIluminacion(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
			Category = "SocketesTest";
			Name = "Test iluminación";
			Description = "Iluminación dinámicas con 4 luces Diffuse a la vez para un mismo mesh";
		}

        public override void Init()
        {
            //Cargar jugador y pelota
            this.pelota = this.CrearPelota(MediaDir);
            this.jugador = this.CrearJugador(MediaDir);

            //Camara en 1ra persona
            //GuiController.Instance.FpsCamera.Enable = true;
            //GuiController.Instance.FpsCamera.MovementSpeed = 400f;
            //GuiController.Instance.FpsCamera.JumpSpeed = 300f;
            //GuiController.Instance.FpsCamera.setCamera(new Vector3(0, 150, 200), new Vector3(0, 80, 0));

            //Cargar Shader personalizado de MultiDiffuseLights
            /*
             * Cargar Shader personalizado de MultiDiffuseLights
             * Este Shader solo soporta TgcMesh con RenderType DIFFUSE_MAP (que son las unicas utilizadas en este ejemplo)
             * El shader toma 4 luces a la vez para iluminar un mesh.
             * Pero como hacer 4 veces los calculos en el shader es costoso, de cada luz solo calcula el componente Diffuse.
             */
			this.meshMultiDiffuseLights = TgcShaders.loadEffect(ShadersDir + "Shaders\\MeshMultiplePointLight.fx");

			this.skeletalMeshPointLight = TgcShaders.loadEffect(ShadersDir + "Shaders\\SkeletalMeshMultiplePointLight.fx");

            this.luces = new List<Luz>();
            //Crear 4 mesh para representar las 4 para la luces. Las ubicamos en distintas posiciones del escenario, cada una con un color distinto.
            luces.Add(new Luz(TgcBox.fromSize(new Vector3(40, 150, 100), new Vector3(10, 10, 10), Color.BlueViolet), Color.HotPink, new Vector3(-40, 40, 400)));
            luces.Add(new Luz(TgcBox.fromSize(new Vector3(-40, 150, 100), new Vector3(10, 10, 10), Color.LightSteelBlue), Color.Blue, new Vector3(-40, 60, 400)));
            luces.Add(new Luz(TgcBox.fromSize(new Vector3(40, 150, -100), new Vector3(10, 10, 10), Color.Green), Color.Green, new Vector3(-40, 80, 400)));
            luces.Add(new Luz(TgcBox.fromSize(new Vector3(-40, 150, -100), new Vector3(10, 10, 10), Color.Orange), Color.Orange, new Vector3(-40, 100, 400)));

			this.piso = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(400, 0, 400), TgcTexture.createTexture(MediaDir + Settings.Default.textureMenuField));

            //Modifiers
            //GuiController.Instance.Modifiers.addFloat("lightIntensity", 0, 150, 38);
            //GuiController.Instance.Modifiers.addFloat("lightAttenuation", 0.1f, 2, 0.15f);

            //Color de fondo
            //GuiController.Instance.BackgroundColor = Color.Black;

            //test carteles
            this.cartel1 = new TgcSceneLoader().loadSceneFromFile(MediaDir + "Cartel\\Cartel-TgcScene.xml").Meshes[0];

			this.cartelEffect = TgcShaders.loadEffect(ShadersDir + "Shaders\\CartelShader.fx");


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
            time += ElapsedTime;
            //Renderizar meshes de luz
            foreach (Luz luz in this.luces)
            {
                //luz.render();
            }

            cartelEffect.SetValue("time", time);
            this.cartel1.Effect = cartelEffect;
            this.cartel1.Technique = "CartelFallando";
            this.cartel1.render();
            //Aplicar al mesh el shader actual
            //this.jugador.LightEffect = this.skeletalMeshPointLight;
            //this.jugador.renderLight(elapsedTime, luces);
            //this.jugador.renderShadow(elapsedTime, luces);

            //Aplicar al mesh el shader actual
            //this.pelota.renderLight(elapsedTime, luces);
            //this.pelota.renderShadow(elapsedTime, luces);

            this.piso.render();
        }

        public override void Dispose()
        {
            this.meshMultiDiffuseLights.Dispose();
            this.skeletalMeshPointLight.Dispose();

            foreach (Luz luz in this.luces)
            {
                luz.dispose();
            }

            this.jugador.dispose();
            this.pelota.dispose();
            this.piso.dispose();
        }

        public Pelota CrearPelota(string pathRecursos)
        {
            int radio = 10;

            //Crear esfera
            TgcSphere sphere = new TgcSphere();
            sphere.setTexture(TgcTexture.createTexture(pathRecursos + Settings.Default.textureBall));
            sphere.Radius = radio;
            sphere.Position = new Vector3(50, 25, 0);
            sphere.updateValues();

            return new Pelota(sphere);
        }

        private Jugador CrearJugador(string pathRecursos)
        {
            string nombreTextura = Settings.Default.textureTeam2;

            //Cargar personaje con animaciones
            TgcSkeletalMesh personaje = new TgcSkeletalLoader().loadMeshAndAnimationsFromFile(
                pathRecursos + Settings.Default.meshFolderPlayer + Settings.Default.meshFilePlayer,
                pathRecursos + Settings.Default.meshFolderPlayer,
                new string[] {
                    pathRecursos + Settings.Default.meshFolderPlayer + Settings.Default.animationWalkFilePlayer,
                    pathRecursos + Settings.Default.meshFolderPlayer + Settings.Default.animationRunFilePlayer,
                    pathRecursos + Settings.Default.meshFolderPlayer + Settings.Default.animationStopFilePlayer,
                    }
                );

            //Le cambiamos la textura
            personaje.changeDiffuseMaps(new TgcTexture[] {
                TgcTexture.createTexture(pathRecursos + Settings.Default.meshFolderPlayer + nombreTextura)
                });

            //Configurar animacion inicial
            personaje.playAnimation(Settings.Default.animationStopPlayer, true);
            personaje.Position = new Vector3(0, 0, 0);

            //Lo Escalo porque es muy grande
            personaje.Scale = new Vector3(0.75f, 0.75f, 0.75f);

            //Recalculo las normales para evitar problemas con la luz
            personaje.computeNormals();

            return new Jugador(personaje, new JugadorIAStrategy(), null);
        }
    }
}