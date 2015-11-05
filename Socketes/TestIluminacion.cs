using AlumnoEjemplos.Socketes.Model;
using AlumnoEjemplos.Socketes.Model.Iluminacion;
using AlumnoEjemplos.Socketes.Model.Jugadores;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;

namespace AlumnoEjemplos.Socketes
{
    public class TestIluminacion : TgcExample
    {
        Pelota pelota;
        Jugador jugador;
        Effect meshMultiDiffuseLights;
        Effect skeletalMeshPointLight;

        List<Luz> luces;

        public override string getCategory()
        {
            return "SocketesTest";
        }

        public override string getName()
        {
            return "Test iluminacion";
        }

        public override string getDescription()
        {
            return "Iluminación dinámicas con 4 luces Diffuse a la vez para un mismo mesh";
        }

        public override void init()
        {
            string pathRecursos = Environment.CurrentDirectory + "\\" + Assembly.GetExecutingAssembly().GetName().Name + "\\" + Settings.Default.mediaFolder;

            //Cargar jugador y pelota
            this.pelota = this.CrearPelota(pathRecursos);
            this.jugador = this.CrearJugador(pathRecursos);

            //Camara en 1ra persona
            GuiController.Instance.FpsCamera.Enable = true;
            GuiController.Instance.FpsCamera.MovementSpeed = 400f;
            GuiController.Instance.FpsCamera.JumpSpeed = 300f;
            GuiController.Instance.FpsCamera.setCamera(new Vector3(-210.0958f, 114.911f, -109.2159f), new Vector3(-209.559f, 114.8029f, -108.3791f));

            //Cargar Shader personalizado de MultiDiffuseLights
            /*
             * Cargar Shader personalizado de MultiDiffuseLights
             * Este Shader solo soporta TgcMesh con RenderType DIFFUSE_MAP (que son las unicas utilizadas en este ejemplo)
             * El shader toma 4 luces a la vez para iluminar un mesh.
             * Pero como hacer 4 veces los calculos en el shader es costoso, de cada luz solo calcula el componente Diffuse.
             */
            this.meshMultiDiffuseLights = TgcShaders.loadEffect(pathRecursos + "Shaders\\MeshMultiplePointLight.fx");
            this.skeletalMeshPointLight = TgcShaders.loadEffect(pathRecursos + "Shaders\\SkeletalMeshMultiplePointLight.fx");

            this.luces = new List<Luz>();
            //Crear 4 mesh para representar las 4 para la luces. Las ubicamos en distintas posiciones del escenario, cada una con un color distinto.
            luces.Add(new Luz(TgcBox.fromSize(new Vector3(40, 100, 440), new Vector3(10, 10, 10), Color.HotPink), Color.HotPink, new Vector3(-40, 40, 400)));
            luces.Add(new Luz(TgcBox.fromSize(new Vector3(-40, 100, 440), new Vector3(10, 10, 10), Color.Blue), Color.Blue, new Vector3(-40, 60, 400)));
            luces.Add(new Luz(TgcBox.fromSize(new Vector3(40, 100, 340), new Vector3(10, 10, 10), Color.Green), Color.Green, new Vector3(-40, 80, 400)));
            luces.Add(new Luz(TgcBox.fromSize(new Vector3(-40, 100, 340), new Vector3(10, 10, 10), Color.Orange), Color.Orange, new Vector3(-40, 100, 400)));

            //Modifiers
            GuiController.Instance.Modifiers.addFloat("lightIntensity", 0, 150, 38);
            GuiController.Instance.Modifiers.addFloat("lightAttenuation", 0.1f, 2, 0.15f);

            //Color de fondo
            GuiController.Instance.BackgroundColor = Color.Black;
        }

        public override void render(float elapsedTime)
        {
            //Renderizar meshes de luz
            foreach (Luz luz in this.luces)
            {
                luz.render();
            }

            //Aplicar al mesh el shader actual
            this.jugador.LightEffect = this.skeletalMeshPointLight;
            this.jugador.renderLight(elapsedTime, luces);

            //Aplicar al mesh el shader actual
            this.pelota.LightEffect = this.meshMultiDiffuseLights;
            this.pelota.renderLight(elapsedTime, luces);
        }

        public override void close()
        {
            this.meshMultiDiffuseLights.Dispose();
            this.skeletalMeshPointLight.Dispose();

            foreach (Luz luz in this.luces)
            {
                luz.dispose();
            }

            this.jugador.dispose();
            this.pelota.dispose();
        }

        public Pelota CrearPelota(string pathRecursos)
        {
            int radio = 10;

            //Crear esfera
            TgcSphere sphere = new TgcSphere();
            sphere.setTexture(TgcTexture.createTexture(pathRecursos + Settings.Default.textureBall));
            sphere.Radius = radio;
            sphere.Position = new Vector3(-50, 10, 400);
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
            personaje.Position = new Vector3(10, 0, 400);

            //Lo Escalo porque es muy grande
            personaje.Scale = new Vector3(0.75f, 0.75f, 0.75f);

            //Recalculo las normales para evitar problemas con la luz
            personaje.computeNormals();

            return new Jugador(personaje, new JugadorIAStrategy(), null);
        }
    }
}