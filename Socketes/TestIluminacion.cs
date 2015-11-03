using AlumnoEjemplos.Socketes.Model.Iluminacion;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Interpolation;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;

namespace AlumnoEjemplos.Socketes
{
    public class TestIluminacion : TgcExample
    {
        TgcScene scene;
        TgcSkeletalMesh personaje;
        Effect effect;

        List<Luz> luces = new List<Luz>();

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

            //Cargar escenario
            TgcSceneLoader loader = new TgcSceneLoader();
            scene = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Scenes\\Deposito\\Deposito-TgcScene.xml");

            //Cargar personaje con animaciones
            TgcSkeletalLoader skeletalLoader = new TgcSkeletalLoader();
            personaje = skeletalLoader.loadMeshAndAnimationsFromFile(pathRecursos + "SkeletalAnimations\\Robot\\Robot-TgcSkeletalMesh.xml", pathRecursos + "SkeletalAnimations\\Robot\\",
                new string[] { pathRecursos + "SkeletalAnimations\\Robot\\Caminando-TgcSkeletalAnim.xml", pathRecursos + "SkeletalAnimations\\Robot\\Parado-TgcSkeletalAnim.xml" });

            //Configurar animacion inicial
            personaje.playAnimation("Parado", true);
            //Escalarlo porque es muy grande
            personaje.Position = new Vector3(20, 0, 400);
            personaje.Scale = new Vector3(0.75f, 0.75f, 0.75f);
            personaje.computeNormals();

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
            effect = TgcViewer.Utils.Shaders.TgcShaders.loadEffect(pathRecursos + "Shaders\\MultiDiffuseLights.fx");

            //Crear 4 mesh para representar las 4 para la luces. Las ubicamos en distintas posiciones del escenario, cada una con un color distinto.
            luces.Add(new Luz(TgcBox.fromSize(new Vector3(40, 100, 440), new Vector3(10, 10, 10), Color.HotPink), Color.HotPink, new Vector3(-40, 40, 400)));
            luces.Add(new Luz(TgcBox.fromSize(new Vector3(-40, 100, 440), new Vector3(10, 10, 10), Color.Blue), Color.Blue, new Vector3(-40, 60, 400)));
            luces.Add(new Luz(TgcBox.fromSize(new Vector3(40, 100, 340), new Vector3(10, 10, 10), Color.Green), Color.Green, new Vector3(-40, 80, 400)));
            luces.Add(new Luz(TgcBox.fromSize(new Vector3(-40, 100, 340), new Vector3(10, 10, 10), Color.Orange), Color.Orange, new Vector3(-40, 100, 400)));

            //Modifiers
            GuiController.Instance.Modifiers.addBoolean("lightEnable", "lightEnable", true);
            GuiController.Instance.Modifiers.addFloat("lightIntensity", 0, 150, 38);
            GuiController.Instance.Modifiers.addFloat("lightAttenuation", 0.1f, 2, 0.15f);
        }

        public override void render(float elapsedTime)
        {
            Device device = GuiController.Instance.D3dDevice;

            //Habilitar luz
            bool lightEnable = (bool)GuiController.Instance.Modifiers["lightEnable"];
            Effect currentShader;
            Effect currentSkeletalShader;
            string currentTechnique;

            if (lightEnable)
            {
                //Shader personalizado de iluminacion
                currentShader = this.effect;
                currentTechnique = "MultiDiffuseLightsTechnique";

                //Con luz: Cambiar el shader actual por el shader default que trae el framework para iluminacion dinamica con PointLight para Skeletal Mesh
                currentSkeletalShader = GuiController.Instance.Shaders.TgcSkeletalMeshPointLightShader;
            }
            else
            {
                //Sin luz: Restaurar shader default
                currentShader = GuiController.Instance.Shaders.TgcMeshShader;
                currentTechnique = GuiController.Instance.Shaders.getTgcMeshTechnique(TgcMesh.MeshRenderType.DIFFUSE_MAP);

                //Sin luz: Restaurar shader default
                currentSkeletalShader = GuiController.Instance.Shaders.TgcSkeletalMeshShader;
            }

            //Aplicar a cada mesh el shader actual
            foreach (TgcMesh mesh in scene.Meshes)
            {
                mesh.Effect = currentShader;
                mesh.Technique = currentTechnique;
            }

            //Configurar los valores de cada luz
            ColorValue[] lightColors = new ColorValue[this.luces.Count];
            Vector4[] pointLightPositions = new Vector4[this.luces.Count];
            float[] pointLightIntensity = new float[this.luces.Count];
            float[] pointLightAttenuation = new float[this.luces.Count];

            for (int i = 0; i < this.luces.Count; i++)
            {
                Luz lightMesh = this.luces[i];
                lightColors[i] = ColorValue.FromColor(lightMesh.Color);
                pointLightPositions[i] = TgcParserUtils.vector3ToVector4(lightMesh.Posicion);
                pointLightIntensity[i] = (float)GuiController.Instance.Modifiers["lightIntensity"];
                pointLightAttenuation[i] = (float)GuiController.Instance.Modifiers["lightAttenuation"];
            }

            //Renderizar meshes
            foreach (TgcMesh mesh in scene.Meshes)
            {
                if (lightEnable)
                {
                    //Cargar variables de shader
                    mesh.Effect.SetValue("lightColor", lightColors);
                    mesh.Effect.SetValue("lightPosition", pointLightPositions);
                    mesh.Effect.SetValue("lightIntensity", pointLightIntensity);
                    mesh.Effect.SetValue("lightAttenuation", pointLightAttenuation);
                    mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                    mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                }

                //Renderizar modelo
                mesh.render();
            }

            //Renderizar meshes de luz
            foreach (Luz luz in this.luces)
            {
                luz.render();
            }

            //Aplicar al mesh el shader actual
            this.personaje.Effect = currentSkeletalShader;
            //El Technique depende del tipo RenderType del mesh
            this.personaje.Technique = GuiController.Instance.Shaders.getTgcSkeletalMeshTechnique(this.personaje.RenderType);

            //Renderizar mesh
            if (lightEnable)
            {
                //Cargar variables shader de la luz
                this.personaje.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                this.personaje.Effect.SetValue("lightPosition", pointLightPositions[0]);
                this.personaje.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(GuiController.Instance.FpsCamera.getPosition()));
                this.personaje.Effect.SetValue("lightIntensity", pointLightIntensity[0]);
                this.personaje.Effect.SetValue("lightAttenuation", pointLightAttenuation[0]);

                //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                this.personaje.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                this.personaje.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                this.personaje.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                this.personaje.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                this.personaje.Effect.SetValue("materialSpecularExp", 9f);
            }

            this.personaje.animateAndRender();
        }

        public override void close()
        {
            scene.disposeAll();
            effect.Dispose();
            foreach (Luz luz in this.luces)
            {
                luz.dispose();
            }

            this.personaje.dispose();
        }
    }
}