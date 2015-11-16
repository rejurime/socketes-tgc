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

namespace AlumnoEjemplos.Socketes
{
    public class TestParcial : TgcExample
    {
        private string pathRecursos;
        private float tiempo;
        private Random random = new Random();
        private List<TgcArrow> ejes;
        private List<TgcQuad> planos;
        private List<TgcArrow> normales;
        private Effect effect;
        private TgcSphere esfera1;
        private TgcSphere esfera2;
        private Texture heightMap;
        private TgcMesh mesh;

        public override string getCategory()
        {
            return "SocketesTest";
        }

        public override string getName()
        {
            return "Test parcial";
        }

        public override string getDescription()
        {
            return "Probando ejercicios de la guia :)";
        }

        public override void init()
        {
            this.pathRecursos = Environment.CurrentDirectory + "\\" + Assembly.GetExecutingAssembly().GetName().Name + "\\" + Settings.Default.mediaFolder;

            //Color de fondo
            GuiController.Instance.BackgroundColor = Color.Black;

            //Camara en 1ra persona
            GuiController.Instance.FpsCamera.Enable = true;
            GuiController.Instance.FpsCamera.MovementSpeed = 100f;
            GuiController.Instance.FpsCamera.JumpSpeed = 50f;
            GuiController.Instance.FpsCamera.setCamera(new Vector3(20, 120, 200), new Vector3(0, 0, 0));

            this.ejes = new List<TgcArrow>();
            this.planos = new List<TgcQuad>();
            this.normales = new List<TgcArrow>();

            this.effect = TgcShaders.loadEffect(this.pathRecursos + "Shaders\\ParcialShader.fx");
            this.heightMap = TextureLoader.FromFile(GuiController.Instance.D3dDevice, GuiController.Instance.ExamplesDir + @"\Shaders\WorkshopShaders\Media\Heighmaps\Heightmap6.jpg");

            this.CrearEjes();
            this.CrearPlanos();
            this.CrearEsferas();
            CrearTanque();
        }

        public override void render(float elapsedTime)
        {
            this.tiempo += elapsedTime;

            foreach (TgcArrow eje in this.ejes)
            {
                eje.render();
            }

            foreach (TgcQuad plano in this.planos)
            {
                plano.Effect.SetValue("time", this.tiempo);
                plano.render();
            }

            foreach (TgcArrow normal in this.normales)
            {
                normal.render();
            }

            this.esfera1.updateValues();
            this.esfera1.render();

            this.esfera2.updateValues();
            this.esfera2.render();

            this.mesh.render();
        }

        public override void close()
        {
            foreach (TgcArrow eje in this.ejes)
            {
                eje.dispose();
            }

            foreach (TgcQuad plano in this.planos)
            {
                plano.dispose();
            }

            foreach (TgcArrow normal in this.normales)
            {
                normal.dispose();
            }

            this.esfera1.dispose();
            this.esfera2.dispose();
            this.mesh.dispose();
        }

        private void CrearEjes()
        {
            Vector3 centroEjes = new Vector3(50, 10, 0);

            TgcArrow normalX = new TgcArrow();
            normalX.PStart = centroEjes;
            normalX.PEnd = centroEjes + new Vector3(10, 0, 0);
            normalX.updateValues();
            this.normales.Add(normalX);

            TgcArrow normalY = new TgcArrow();
            normalY.PStart = centroEjes;
            normalY.PEnd = centroEjes + new Vector3(0, 10, 0);
            normalY.updateValues();
            this.normales.Add(normalY);

            TgcArrow normalZ = new TgcArrow();
            normalZ.PStart = centroEjes;
            normalZ.PEnd = centroEjes + new Vector3(0, 0, 10);
            normalZ.updateValues();
            this.normales.Add(normalZ);
        }

        private void CrearPlanos()
        {
            Vector3 centroOriginal = new Vector3(0, 0, 0);
            Vector3 centroActual = centroOriginal;

            for (int i = 0; i < 20; i++)
            {
                centroActual = new Vector3(i * 5, 0, 0);

                for (int j = 0; j < 20; j++)
                {
                    centroActual = centroActual + new Vector3(0, 0, 5);

                    //Crear un quad (pequeño plano) con la clase TgcQuad para poder dibujar el plano que contiene al triangulo
                    TgcQuad quad = new TgcQuad();
                    quad.Center = centroActual;
                    quad.Color = this.AdaptColorRandom(Color.SteelBlue);
                    quad.Size = new Vector2(5, 5);
                    quad.Effect = this.effect;
                    quad.Technique = "VS1";
                    quad.updateValues();
                    this.planos.Add(quad);

                    TgcArrow normal = new TgcArrow();
                    normal.PStart = quad.Center;
                    normal.PEnd = quad.Center + quad.Normal * 2;
                    normal.updateValues();
                    this.normales.Add(normal);
                }
            }
        }

        private void CrearEsferas()
        {
            this.esfera1 = new TgcSphere();
            this.esfera1.setColor(Color.DarkSeaGreen);
            this.esfera1.Radius = 20;
            this.esfera1.Position = new Vector3(-40, 20, 100);
            //this.esfera.LevelOfDetail = 4;
            this.esfera1.Effect = this.effect;
            this.esfera1.Effect.SetValue("height_map", this.heightMap);
            this.esfera1.Technique = "VS2";
            this.esfera1.updateValues();

            this.esfera2 = new TgcSphere();
            this.esfera2.setColor(Color.DarkSeaGreen);
            this.esfera2.Radius = 20;
            this.esfera2.Position = new Vector3(-40, 20, 10);
            //this.esfera.LevelOfDetail = 4;
            this.esfera2.Effect = this.effect;
            this.esfera2.Effect.SetValue("height_map", this.heightMap);
            this.esfera2.Technique = "VSP";
            this.esfera2.updateValues();
        }

        private void CrearTanque()
        {
            this.mesh = new TgcSceneLoader().loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Vehiculos\\TanqueFuturistaRuedas\\TanqueFuturistaRuedas-TgcScene.xml").Meshes[0];
            this.mesh.Position = new Vector3(10, 20, -10);
            this.mesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);
            this.mesh.Effect = this.effect;
            this.mesh.Technique = "VS3";
        }

        public Color AdaptColorRandom(Color c)
        {
            int r = random.Next(0, 150);
            return Color.FromArgb((int)FastMath.Min(c.R + r, 255), (int)FastMath.Min(c.G + r, 255), (int)FastMath.Min(c.B + r, 255));
        }
    }
}