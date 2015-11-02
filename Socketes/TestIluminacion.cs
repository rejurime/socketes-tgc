using AlumnoEjemplos.Socketes.Model.Iluminacion;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Cargar escenario
            TgcSceneLoader loader = new TgcSceneLoader();
            scene = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Scenes\\Deposito\\Deposito-TgcScene.xml");

            //Cargar personaje con animaciones
            TgcSkeletalLoader skeletalLoader = new TgcSkeletalLoader();
            personaje = skeletalLoader.loadMeshAndAnimationsFromFile(
                GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\" + "Robot-TgcSkeletalMesh.xml",
                GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\",
                new string[] {
                    GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\" + "Caminando-TgcSkeletalAnim.xml",
                    GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\" + "Parado-TgcSkeletalAnim.xml",
                });


            //Configurar animacion inicial
            personaje.playAnimation("Parado", true);
            //Escalarlo porque es muy grande
            personaje.Position = new Vector3(10, 0, 400);
            personaje.Scale = new Vector3(0.75f, 0.75f, 0.75f);

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
            effect = TgcViewer.Utils.Shaders.TgcShaders.loadEffect(GuiController.Instance.ExamplesMediaDir + "Shaders\\MultiDiffuseLights.fx");


            //Crear 4 mesh para representar las 4 para la luces. Las ubicamos en distintas posiciones del escenario, cada una con un color distinto.

            luces.Add(new Luz(TgcBox.fromSize(new Vector3(-40, 40, 400), new Vector3(10, 10, 10), Color.Red), Color.Red, new Vector3(-40, 40, 400)));
            luces.Add(new Luz(TgcBox.fromSize(new Vector3(-40, 60, 400), new Vector3(10, 10, 10), Color.Blue), Color.Blue, new Vector3(-40, 60, 400)));
            luces.Add(new Luz(TgcBox.fromSize(new Vector3(-40, 80, 400), new Vector3(10, 10, 10), Color.Green), Color.Green, new Vector3(-40, 80, 400)));
            luces.Add(new Luz(TgcBox.fromSize(new Vector3(-40, 100, 400), new Vector3(10, 10, 10), Color.Orange), Color.Orange, new Vector3(-40, 100, 400)));

            //Modifiers
            GuiController.Instance.Modifiers.addBoolean("lightEnable", "lightEnable", true);
            GuiController.Instance.Modifiers.addFloat("lightIntensity", 0, 150, 38);
            GuiController.Instance.Modifiers.addFloat("lightAttenuation", 0.1f, 2, 0.15f);

            GuiController.Instance.Modifiers.addColor("mEmissive", Color.Black);
            GuiController.Instance.Modifiers.addColor("mDiffuse", Color.White);
        }

        public override void render(float elapsedTime)
        {
            Device device = GuiController.Instance.D3dDevice;

            //Habilitar luz
            bool lightEnable = (bool)GuiController.Instance.Modifiers["lightEnable"];
            Effect currentShader;
            String currentTechnique;
            if (lightEnable)
            {
                //Shader personalizado de iluminacion
                currentShader = this.effect;
                currentTechnique = "MultiDiffuseLightsTechnique";
            }
            else
            {
                //Sin luz: Restaurar shader default
                currentShader = GuiController.Instance.Shaders.TgcMeshShader;
                currentTechnique = GuiController.Instance.Shaders.getTgcMeshTechnique(TgcMesh.MeshRenderType.DIFFUSE_MAP);
            }

            //Aplicar a cada mesh el shader actual
            foreach (TgcMesh mesh in scene.Meshes)
            {
                mesh.Effect = currentShader;
                mesh.Technique = currentTechnique;
            }

            //this.personaje.Effect = currentShader;
            //this.personaje.Technique = currentTechnique;

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
                    mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor((Color)GuiController.Instance.Modifiers["mEmissive"]));
                    mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor((Color)GuiController.Instance.Modifiers["mDiffuse"]));
                }

                //Renderizar modelo
                mesh.render();
            }

            //Renderizar meshes de luz
            foreach (Luz luz in this.luces)
            {
                luz.render();
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