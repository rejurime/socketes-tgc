﻿using AlumnoEjemplos.Properties;
using AlumnoEjemplos.Socketes.Collision;
using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model.Creacion
{
    public class PartidoFactory
    {
        #region Miembros

        private static readonly PartidoFactory instance = new PartidoFactory();

        #endregion

        #region Constructores

        /// <summary> Constructor privado para poder hacer el singleton</summary>
        private PartidoFactory() { }

        #endregion

        #region Propiedades

        public static PartidoFactory Instance
        {
            get { return instance; }
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Crea un partido con todos sus componentes Cancha, Arcos, Jugadores y Pelota
        /// </summary>
        /// <param name="alumnoEjemplosMediaDir"> Carpeta donde estan los recursos </param>
        /// <returns> Un partido listo para comenzar a jugar :)</returns>
        public Partido CrearPartido(string pathRecursos, TgcD3dInput input)
        {
            string nombreEquipoLocal = "SKTS";
            string nombreEquipoVisitante = "TGCV";

            Partido partido = Partido.Instance;

            partido.Marcador = this.CrearMarcador(nombreEquipoLocal, nombreEquipoVisitante);
            partido.Cancha = this.CrearCancha(pathRecursos);
            partido.ArcoLocal = this.CrearArco(new Vector3(940, 0, 25), pathRecursos);
            partido.ArcoVisitante = this.CrearArco(new Vector3(-780, 0, 25), pathRecursos);
            partido.Pelota = this.CrearPelota(pathRecursos);
            partido.EquipoLocal = EquipoFactory.Instance.CrearEquipoHumanoIA(nombreEquipoLocal, pathRecursos, input, partido.Pelota);
            partido.EquipoVisitante = EquipoFactory.Instance.CrearEquipoIA(nombreEquipoVisitante, pathRecursos, partido.Pelota);

            //Creo la pelota con todos sus obstaculos
            List<IColisionable> obstaculosPelota = new List<IColisionable>();
            //obstaculosPelota.Add(partido.ArcoLocal);
            //obstaculosPelota.Add(partido.ArcoVisitante);
            obstaculosPelota.Add(partido.Cancha);
            obstaculosPelota.AddRange(partido.Cancha.LimitesCancha);
            obstaculosPelota.AddRange(partido.EquipoLocal.JugadoresColisionables());
            obstaculosPelota.AddRange(partido.EquipoVisitante.JugadoresColisionables());

            partido.Pelota.CollisionManager = new SphereCollisionManager(obstaculosPelota);
            partido.Pelota.CollisionManager.GravityEnabled = true;

            //Cargo las colisiones de los jugadores
            EquipoFactory.Instance.CargarColisionesEquipos(partido.EquipoLocal, partido.EquipoVisitante, partido);

            return partido;
        }

        /// <summary>
        /// Crea el marcador del partido que tiene los goles y el tiempo
        /// </summary>
        /// <returns> Un Marcador con resultado y tiempo</returns>
        private Marcador CrearMarcador(string nombreEquipoLocal, string nombreEquipoVisitante)
        {
            //Marcador
            TgcText2d marcador = new TgcText2d();
            marcador.Color = Color.White;
            marcador.Align = TgcText2d.TextAlign.CENTER;
            marcador.Position = new Point(0, 20);
            marcador.Size = new Size(150, 100);
            marcador.changeFont(new System.Drawing.Font("Arial", 14, FontStyle.Bold));

            //Contador de Tiempo
            TgcText2d tiempo = new TgcText2d();
            tiempo.Color = Color.White;
            tiempo.Align = TgcText2d.TextAlign.CENTER;
            tiempo.Position = new Point(0, 40);
            tiempo.Size = new Size(150, 100);
            tiempo.changeFont(new System.Drawing.Font("Arial", 14));

            return new Marcador(marcador, tiempo, nombreEquipoLocal, nombreEquipoVisitante);
        }

        /// <summary>
        /// Creo la cancha donde van a estar parado los jugadores
        /// </summary>
        /// <param name="pathRecursos"></param>
        /// <returns>Una Cancha</returns>
        private Cancha CrearCancha(string pathRecursos)
        {
            TgcMesh tribuna1 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileTribunePl).Meshes[0];
            tribuna1.move(new Vector3(0, 80, 800));
            tribuna1.rotateY(-(float)Math.PI / 2);
            tribuna1.Scale = new Vector3(10, 10, 10);

            TgcMesh tribuna2 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileTribunePl).Meshes[0];
            tribuna2.move(new Vector3(0, 80, -800));
            tribuna2.rotateY((float)Math.PI / 2);
            tribuna2.Scale = new Vector3(10, 10, 10);

            TgcMesh tribuna3 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileTribunePo).Meshes[0];
            tribuna3.move(new Vector3(1000, 60, 0));
            tribuna3.rotateY(-(float)Math.PI / 2);
            tribuna3.Scale = new Vector3(10, 10, 10);

            TgcMesh tribuna4 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileTribunePo).Meshes[0];
            tribuna4.move(new Vector3(-1000, 60, 0));
            tribuna4.rotateY((float)Math.PI / 2);
            tribuna4.Scale = new Vector3(10, 10, 10);

            TgcBox box = TgcBox.fromSize(new Vector3(0, -10, 0), new Vector3(1920, 0, 1200), TgcTexture.createTexture(pathRecursos + Settings.Default.textureFolder + Settings.Default.textureField));
            TgcBox box2 = TgcBox.fromSize(new Vector3(0, -11, 0), new Vector3(2048, 0, 1600), Color.SlateGray);

            List<IRenderObject> componentes = new List<IRenderObject>();
            componentes.Add(tribuna1);
            componentes.Add(tribuna2);
            componentes.Add(tribuna3);
            componentes.Add(tribuna4);
            componentes.Add(box2);

            return new Cancha(box, componentes, this.CrearLimitesCancha());
        }

        /// <summary>
        /// Creo los limites de la cancha
        /// </summary>
        /// <returns> Lista con los limites de la cancha</returns>
        private List<LimiteCancha> CrearLimitesCancha()
        {
            List<LimiteCancha> limites = new List<LimiteCancha>();
            limites.Add(new LimiteCancha(TgcBox.fromSize(new Vector3(900, 100, 0), new Vector3(0, 220, 1200))));
            limites.Add(new LimiteCancha(TgcBox.fromSize(new Vector3(-900, 100, 0), new Vector3(0, 220, 1200))));
            limites.Add(new LimiteCancha(TgcBox.fromSize(new Vector3(0, 100, 580), new Vector3(1900, 220, 0))));
            limites.Add(new LimiteCancha(TgcBox.fromSize(new Vector3(0, 100, -580), new Vector3(1900, 220, 0))));

            return limites;
        }

        /// <summary>
        /// Creo la pelota en el centro de la cancha
        /// </summary>
        /// <param name="pathRecursos"> De donde saco la textura</param>
        /// <returns> Una pelota</returns>
        private Pelota CrearPelota(string pathRecursos)
        {
            //Crear esfera
            TgcSphere sphere = new TgcSphere();
            sphere.setTexture(TgcTexture.createTexture(pathRecursos + Settings.Default.textureFolder + Settings.Default.textureBall));
            sphere.Radius = 10;
            sphere.Position = new Vector3(0, 10, 0);
            sphere.updateValues();

            return new Pelota(sphere);
        }

        /// <summary>
        /// Creo un arco
        /// </summary>
        /// <param name="posicion">Donde va a estar ubicado el Arco</param>
        /// <param name="pathRecursos">De donde sacar el mesh</param>
        /// <returns> Un arco</returns>
        private Arco CrearArco(Vector3 posicion, string pathRecursos)
        {
            List<TgcMesh> palos = new List<TgcMesh>();
            TgcMesh palo1 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileGoal).Meshes[0];
            palo1.Position = posicion;
            palo1.Scale = new Vector3(1.25f, 1.25f, 1.25f);

            TgcMesh palo2= new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileGoal).Meshes[1];
            palo1.Position = posicion;
            palo1.Scale = new Vector3(1.25f, 1.25f, 1.25f);

            TgcMesh palo3 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileGoal).Meshes[2];
            palo1.Position = posicion;
            palo1.Scale = new Vector3(1.25f, 1.25f, 1.25f);

            palos.Add(palo1);
            palos.Add(palo2);
            palos.Add(palo3);

            return new Arco(palos);
        }

        #endregion
    }
}