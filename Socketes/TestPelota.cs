﻿using AlumnoEjemplos.Properties;
using AlumnoEjemplos.Socketes.Model;
using Examples.Collision.SphereCollision;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Reflection;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class TestPelota : TgcExample
    {
        private Partido partido;
        private CollisionManager collisionManager;

        /// <summary>
        /// Categoría a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el árbol de la derecha de la pantalla.
        /// </summary>
        public override string getCategory()
        {
            return "AlumnoEjemplos";
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
            string pathRecursos = System.Environment.CurrentDirectory + "\\" + Assembly.GetExecutingAssembly().GetName().Name + "\\" + Settings.Default.mediaFolder;

            this.partido = PartidoFactory.Instance.CrearPartido(pathRecursos);

            //Crear manejador de colisiones
            collisionManager = new CollisionManager();
            collisionManager.GravityEnabled = true;

            //Configurar camara en Tercer Persona
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(this.partido.Pelota.Position, 100, -150);
        }

        public override void render(float elapsedTime)
        {
            //Calcular proxima posicion de personaje segun Input
            TgcD3dInput input = GuiController.Instance.D3dInput;

            Vector3 movement = new Vector3(0, 0, 0);
            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
            {
                movement.X = -1;
            }
            else if (input.keyDown(Key.Right) || input.keyDown(Key.D))
            {
                movement.X = 1;
            }
            if (input.keyDown(Key.Up) || input.keyDown(Key.W))
            {
                movement.Z = 1;
            }
            else if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                movement.Z = -1;
            }

            Vector3 realMovement = collisionManager.moveCharacter(this.partido.Pelota.BoundingSphere, movement, this.partido.ObstaculosPelota());
            this.partido.Pelota.mover(realMovement, elapsedTime);

            //Hacer que la camara siga al personaje en su nueva posicion
            GuiController.Instance.ThirdPersonCamera.Target = this.partido.Pelota.Position;

            //Render de todos los elementos del partido
            this.partido.render();
        }

        public override void close()
        {
            this.partido.dispose();
        }
    }
}