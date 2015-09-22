using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;
using Examples.Collision.SphereCollision;

namespace AlumnoEjemplos.MiGrupo
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class TestPelota : TgcExample
    {
        TgcBox piso;
        List<TgcBox> boxs;
        List<TgcBoundingBox> boundigboxs;
        Pelota pelota;
        SphereCollisionManager collisionManager;


        /// <summary>
        /// Categoría a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el árbol de la derecha de la pantalla.
        /// </summary>
        public override string getCategory()
        {
            return "Socketes";
        }

        /// <summary>
        /// Completar nombre del grupo en formato Grupo NN
        /// </summary>
        public override string getName()
        {
            return "Test Pelota";
        }

        /// <summary>
        /// Completar con la descripción del TP
        /// </summary>
        public override string getDescription()
        {
            return "Juego de futbol by Socketes";
        }

        public override void init()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            pelota = new Pelota(GuiController.Instance.D3dDevice);
            //Crear piso
            TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "Texturas\\tierra.jpg");
            piso = TgcBox.fromSize(new Vector3(0, -60, 0), new Vector3(1000, 5, 1000), pisoTexture);

            //Crear manejador de colisiones
            collisionManager = new SphereCollisionManager();
            collisionManager.GravityEnabled = false;

            //Cargar obstaculos y posicionarlos. Los obstáculos se crean con TgcBox en lugar de cargar un modelo.
            boxs = new List<TgcBox>();
            boundigboxs = new List<TgcBoundingBox>();

            TgcBox obstaculo;


            //Obstaculo 1
            obstaculo = TgcBox.fromSize(
                new Vector3(-100, 0, 0),
                new Vector3(80, 150, 80),
                TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "Texturas\\baldosaFacultad.jpg"));
            boundigboxs.Add(obstaculo.BoundingBox);
            boxs.Add(obstaculo);
            //Obstaculo 2
            obstaculo = TgcBox.fromSize(
                new Vector3(50, 0, 200),
                new Vector3(80, 300, 80),
                TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "Texturas\\madera.jpg"));
            boundigboxs.Add(obstaculo.BoundingBox);
            boxs.Add(obstaculo);

            //Obstaculo 3
            obstaculo = TgcBox.fromSize(
                new Vector3(300, 0, 100),
                new Vector3(80, 100, 150),
                TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "Texturas\\granito.jpg"));
            boundigboxs.Add(obstaculo.BoundingBox);
            boxs.Add(obstaculo);



            //Configurar camara en Tercer Persona
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(pelota.Position, 100, -150);


            //Modifier para ver BoundingBox
            GuiController.Instance.Modifiers.addBoolean("showBoundingBox", "Bouding Box", false);

            //Modifiers para desplazamiento del personaje
            GuiController.Instance.Modifiers.addFloat("VelocidadCaminar", 1f, 400f, 250f);
            GuiController.Instance.Modifiers.addFloat("VelocidadRotacion", 1f, 360f, 120f);

        }


        public override void render(float elapsedTime)
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            //Obtener boolean para saber si hay que mostrar Bounding Box
            bool showBB = (bool)GuiController.Instance.Modifiers.getValue("showBoundingBox");


            //obtener velocidades de Modifiers
            float velocidadCaminar = (float)GuiController.Instance.Modifiers.getValue("VelocidadCaminar");
            float velocidadRotacion = (float)GuiController.Instance.Modifiers.getValue("VelocidadRotacion");


            //Calcular proxima posicion de personaje segun Input
            float moveForward = 0f;
            float rotate = 0;
            TgcD3dInput input = GuiController.Instance.D3dInput;
            bool moving = false;
            bool rotating = false;

            Vector3 movement = new Vector3(0, 0, 0);
            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
            {
                moving = true;
                movement.X = -1;
            }
            else if (input.keyDown(Key.Right) || input.keyDown(Key.D))
            {
                moving = true;
                movement.X = 1;
            }
            if (input.keyDown(Key.Up) || input.keyDown(Key.W))
            {
                moving = true;
                movement.Z = 1;
            }
            else if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                moving = true;
                movement.Z = -1;
            }

            //Si hubo desplazamiento
            if (moving)
            {
                Vector3 realMovement = collisionManager.moveCharacter(pelota.BoundingSphere, movement, boundigboxs);
                pelota.mover(realMovement, elapsedTime);
            }

            //Hacer que la camara siga al personaje en su nueva posicion
            GuiController.Instance.ThirdPersonCamera.Target = pelota.Position;



            //Render piso
            piso.render();


            //Render obstaculos
            foreach (TgcBox obstaculo in boxs)
            {
                obstaculo.render();
                if (showBB)
                {
                    obstaculo.BoundingBox.render();
                }

            }

            if (showBB)
            {
                pelota.BoundingSphere.render();
            }
            //Render personaje
            pelota.updateValues();
            pelota.render(elapsedTime);

        }

        public override void close()
        {
            piso.dispose();
            foreach (TgcBox obstaculo in boxs)
            {
                obstaculo.dispose();
            }
            pelota.dispose();
        }

    }
}
