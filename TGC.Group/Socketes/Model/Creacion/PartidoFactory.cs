﻿using AlumnoEjemplos.Socketes.Model.Colision;
using AlumnoEjemplos.Socketes.Model.ElementosCancha;
using AlumnoEjemplos.Socketes.Model.Iluminacion;
using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Input;
using TGC.Core.Sound;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Terrain;
using TGC.Core.Utils;
using TGC.Group;
using TGC.Core.Textures;
using TGC.Core.Text;

namespace AlumnoEjemplos.Socketes.Model.Creacion
{
	/// <summary> Factory para crear todos los objetos relacionados con el Partido </summary>
	public class PartidoFactory
    {
        #region Miembros

        private static readonly PartidoFactory instance = new PartidoFactory();
        private object boxField;

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
        public Partido CrearPartido(string pathRecursos, TgcD3dInput input, Dictionary<string, TgcStaticSound> sonidos, TgcThirdPersonCamera camara)
        {
            string nombreEquipoLocal = "SKTS";
            string nombreEquipoVisitante = "TGCV";

            Partido partido = Partido.Instance;

            partido.Sonidos = sonidos;
            partido.Marcador = this.CrearMarcador(nombreEquipoLocal, nombreEquipoVisitante);
            partido.Cancha = this.CrearCancha(pathRecursos);
            partido.ArcoLocal = this.CrearArco(pathRecursos, partido.Cancha, -1);
            partido.ArcoVisitante = this.CrearArco(pathRecursos, partido.Cancha, 1);
            partido.Pelota = this.CrearPelota(pathRecursos, partido.Cancha);
            partido.EquipoLocal = EquipoFactory.Instance.CrearEquipoHumanoIA(nombreEquipoLocal, pathRecursos, input, partido);
            partido.EquipoVisitante = EquipoFactory.Instance.CrearEquipoIA(nombreEquipoVisitante, pathRecursos, partido);

            //Creo la pelota con todos sus obstaculos
            List<IColisionablePelota> obstaculosPelota = new List<IColisionablePelota>();
            obstaculosPelota.AddRange(partido.ArcoLocal.GetColisionables());
            obstaculosPelota.AddRange(partido.ArcoVisitante.GetColisionables());
            obstaculosPelota.Add(partido.Cancha);
            obstaculosPelota.AddRange(partido.Cancha.LimitesCancha);
            obstaculosPelota.AddRange(partido.EquipoLocal.JugadoresColisionables());
            obstaculosPelota.AddRange(partido.EquipoVisitante.JugadoresColisionables());

            partido.Pelota.CollisionManager = new PelotaCollisionManager(obstaculosPelota);
            //partido.Pelota.CollisionManager = new SphereCollisionManager(obstaculosPelota);

            //Cargo las colisiones de los jugadores
            EquipoFactory.Instance.CargarColisionesEquipos(partido.EquipoLocal, partido.EquipoVisitante, partido);

            partido.Camara = camara;

            return partido;
        }

        /// <summary>
        /// Crea el marcador del partido que tiene los goles y el tiempo
        /// </summary>
        /// <returns> Un Marcador con resultado y tiempo</returns>
        private Marcador CrearMarcador(string nombreEquipoLocal, string nombreEquipoVisitante)
        {
            //Marcador
            TgcText2D marcador = new TgcText2D();
            marcador.Color = Color.White;
            marcador.Align = TgcText2D.TextAlign.CENTER;
            marcador.Position = new Point(0, 20);
            marcador.Size = new Size(200, 100);
            marcador.changeFont(new System.Drawing.Font("Arial", 14, FontStyle.Bold));

            //Contador de Tiempo
            TgcText2D tiempo = new TgcText2D();
            tiempo.Color = Color.White;
            tiempo.Align = TgcText2D.TextAlign.CENTER;
            tiempo.Position = new Point(0, 40);
            tiempo.Size = new Size(200, 100);
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
            //Cesped
            TgcBox boxField = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(1920, 0, 1200), TgcTexture.createTexture(pathRecursos + Settings.Default.textureField));
			//TODO cambiar por matrices
			boxField.AutoTransformEnable = true;
            
			//Piso
            TgcBox boxFloor = TgcBox.fromSize(new Vector3(boxField.Position.X, boxField.Position.Y - 1, boxField.Position.Z), boxField.Size * 2, TgcTexture.createTexture(pathRecursos + Settings.Default.textureFloor));
			//TODO cambiar por matrices
			boxFloor.AutoTransformEnable = true;

            //Laterales
            TgcMesh tribuna1 = this.CrearTribunaPlatea(pathRecursos, boxField, 1);
            //TODO cambiar por matrices
			tribuna1.AutoTransformEnable = true;
			TgcMesh tribuna2 = this.CrearTribunaPlatea(pathRecursos, boxField, -1);
			//TODO cambiar por matrices
			tribuna2.AutoTransformEnable = true;

            //Atras del arco
            TgcMesh tribuna3 = this.CrearTribunaPopular(pathRecursos, boxField, 1);
            //TODO cambiar por matrices
			tribuna3.AutoTransformEnable = true;
			TgcMesh tribuna4 = this.CrearTribunaPopular(pathRecursos, boxField, -1);
			//TODO cambiar por matrices
			tribuna4.AutoTransformEnable = true;

            List<TgcBox> carteles = new List<TgcBox>();

            TgcBox cartel1 = TgcBox.fromSize(new Vector3(0, 15, boxField.Size.Z / 2), new Vector3(150, 30, 0), TgcTexture.createTexture(pathRecursos + "Texturas\\gnome-logo.png"));
			//TODO cambiar por matrices
			cartel1.AutoTransformEnable = true;
			cartel1.Effect = TgcShaders.loadEffect(pathRecursos + "Shaders\\CartelShader.fx");
            cartel1.Technique = "Rotacion";
            cartel1.updateValues();

            TgcBox cartel2 = TgcBox.fromSize(new Vector3(200, 15, boxField.Size.Z / 2), new Vector3(150, 30, 0), TgcTexture.createTexture(pathRecursos + "Texturas\\blizzard_logo.jpg"));
            //TODO cambiar por matrices
			cartel2.AutoTransformEnable = true;
			cartel2.Effect = TgcShaders.loadEffect(pathRecursos + "Shaders\\CartelShader.fx");
            cartel2.Technique = "Rotacion";
            cartel2.updateValues();

            TgcBox cartel3 = TgcBox.fromSize(new Vector3(-200, 15, boxField.Size.Z / 2), new Vector3(150, 30, 0), TgcTexture.createTexture(pathRecursos + "Texturas\\github-logo.jpg"));
            //TODO cambiar por matrices
			cartel3.AutoTransformEnable = true;
			cartel3.Effect = TgcShaders.loadEffect(pathRecursos + "Shaders\\CartelShader.fx");
            cartel3.Technique = "Rotacion";
            cartel3.updateValues();

            TgcBox cartel4 = TgcBox.fromSize(new Vector3(400, 15, boxField.Size.Z / 2), new Vector3(150, 30, 0), TgcTexture.createTexture(pathRecursos + "Texturas\\atom-editor-logo.png"));
            //TODO cambiar por matrices
			cartel4.AutoTransformEnable = true;
			cartel4.Effect = TgcShaders.loadEffect(pathRecursos + "Shaders\\CartelShader.fx");
            cartel4.Technique = "Rotacion";
            cartel4.updateValues();

            TgcBox cartel5 = TgcBox.fromSize(new Vector3(-400, 15, boxField.Size.Z / 2), new Vector3(150, 30, 0), TgcTexture.createTexture(pathRecursos + "Texturas\\visual-studio-logo.png"));
            //TODO cambiar por matrices
			cartel5.AutoTransformEnable = true;
			cartel5.Effect = TgcShaders.loadEffect(pathRecursos + "Shaders\\CartelShader.fx");
            cartel5.Technique = "CartelFallando";
            cartel5.updateValues();

            carteles.Add(cartel1);
            carteles.Add(cartel2);
            carteles.Add(cartel3);
            carteles.Add(cartel4);
            carteles.Add(cartel5);

            //SkyBox
            TgcSkyBox skyBox = this.CrearSkyBox(pathRecursos, boxFloor);

            //Limites de la cancha
            List<LimiteCancha> limites = this.CrearLimitesCancha(boxField);

            //Luces
            List<Luz> luces = this.CrearLuces(pathRecursos, boxField);

            List<IRenderObject> componentes = new List<IRenderObject>();
            componentes.Add(tribuna1);
            componentes.Add(tribuna2);
            componentes.Add(tribuna3);
            componentes.Add(tribuna4);
            componentes.Add(boxFloor);
            componentes.Add(skyBox);
            int altoMuralla = 200;
            componentes.Add(TgcBox.fromSize(new Vector3(boxFloor.Position.X, boxFloor.Position.Y + altoMuralla / 2, boxFloor.Size.Z / 2), new Vector3(boxFloor.Size.X, altoMuralla, 0), TgcTexture.createTexture(pathRecursos + Settings.Default.textureWall)));
            componentes.Add(TgcBox.fromSize(new Vector3(boxFloor.Position.X, boxFloor.Position.Y + altoMuralla / 2, -boxFloor.Size.Z / 2), new Vector3(boxFloor.Size.X, altoMuralla, 0), TgcTexture.createTexture(pathRecursos + Settings.Default.textureWall)));
            componentes.Add(TgcBox.fromSize(new Vector3(boxFloor.Size.X / 2, boxFloor.Position.Y + altoMuralla / 2, boxFloor.Position.Z), new Vector3(0, altoMuralla, boxFloor.Size.Z), TgcTexture.createTexture(pathRecursos + Settings.Default.textureWall)));
            componentes.Add(TgcBox.fromSize(new Vector3(-boxFloor.Size.X / 2, boxFloor.Position.Y + altoMuralla / 2, boxFloor.Position.Z), new Vector3(0, altoMuralla, boxFloor.Size.Z), TgcTexture.createTexture(pathRecursos + Settings.Default.textureWall)));
            componentes.Add(cartel1);

            return new Cancha(boxField, componentes, limites, luces, carteles);
        }

        private TgcMesh CrearTribunaPlatea(string pathRecursos, TgcBox cancha, int sentido)
        {
            TgcMesh platea = this.CrearTribuna(pathRecursos + Settings.Default.meshFileTribunePl, sentido * -(float)Math.PI / 2, new Vector3(10, 12, 14));

            platea.Position = new Vector3(cancha.Position.X, cancha.Position.Y + platea.BoundingBox.PMax.Y, sentido * (cancha.Size.Z / 2 + platea.BoundingBox.PMax.X + 120));
            return platea;
        }

        private TgcMesh CrearTribunaPopular(string pathRecursos, TgcBox cancha, int sentido)
        {
            TgcMesh platea = this.CrearTribuna(pathRecursos + Settings.Default.meshFileTribunePo, sentido * -(float)Math.PI / 2, new Vector3(14, 12, 15));
            platea.Position = new Vector3(sentido * (cancha.Size.X / 2 + platea.BoundingBox.PMax.Z + 120), cancha.Position.Y + platea.BoundingBox.PMax.Y, cancha.Position.Z);
            return platea;
        }

        /// <summary> 
        /// Crea una tribuna desde un mesh, pero por como esta creada se necesita acomodarla en la cancha y escalarla 
        /// </summary>
        /// <param name="pathMesh"> ruta donde esta el mesh de la tribuna </param>
        /// <param name="position"> donde la tengo que ubicar a la tribuna</param>
        /// <param name="rotateY"> como la tengo que rotar para que quede en la horientacion correcta </param>
        /// <param name="scale"> factor de escalado para que quede armonica a la cancha </param>
        /// <returns></returns>
        private TgcMesh CrearTribuna(string pathMesh, float rotateY, Vector3 scale)
        {
            TgcMesh tribuna = new TgcSceneLoader().loadSceneFromFile(pathMesh).Meshes[0];
            tribuna.rotateY(rotateY);
            tribuna.Scale = scale;
            return tribuna;
        }

        /// <summary>
        /// Creo los limites de la cancha
        /// </summary>
        /// <returns> Lista con los limites de la cancha</returns>
        private List<LimiteCancha> CrearLimitesCancha(TgcBox cancha)
        {
            int altoLimite = 600;
            List<LimiteCancha> limites = new List<LimiteCancha>();
            limites.Add(new LimiteCancha(TgcBox.fromSize(new Vector3(cancha.Position.X, cancha.Position.Y + altoLimite / 2, cancha.Size.Z / 2 - 22), new Vector3(cancha.Size.X, altoLimite, 0))));
            limites.Add(new LimiteCancha(TgcBox.fromSize(new Vector3(cancha.Position.X, cancha.Position.Y + altoLimite / 2, -cancha.Size.Z / 2 + 22), new Vector3(cancha.Size.X, altoLimite, 0))));
            limites.Add(new LimiteCancha(TgcBox.fromSize(new Vector3(cancha.Size.X / 2 - 100, cancha.Position.Y + altoLimite / 2, cancha.Position.Z), new Vector3(0, altoLimite, cancha.Size.Z))));
            limites.Add(new LimiteCancha(TgcBox.fromSize(new Vector3(-cancha.Size.X / 2 + 100, cancha.Position.Y + altoLimite / 2, cancha.Position.Z), new Vector3(0, altoLimite, cancha.Size.Z))));

            return limites;
        }

        /// <summary>
        /// Crea un SkyBox que rodea toda la cancha y da una sensacion mas de realizamo
        /// </summary>
        /// <param name="pathRecursos"> la ruta donde se encuentran los recursos </param>
        /// <returns></returns>
        private TgcSkyBox CrearSkyBox(string pathRecursos, TgcBox pisoCancha)
        {
            //Crear SkyBox 
            TgcSkyBox skyBox = new TgcSkyBox();
            skyBox.Size = new Vector3(pisoCancha.Size.X + 100, 1000, pisoCancha.Size.Z + 100);
            skyBox.Center = new Vector3(pisoCancha.Position.X, pisoCancha.Position.Y - 1 + skyBox.Size.Y / 2, pisoCancha.Position.Z);

            //Configurar las texturas para cada una de las 6 caras
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, pathRecursos + Settings.Default.textureSkyFacesUp);
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, pathRecursos + Settings.Default.textureSkyFaceDown);
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, pathRecursos + Settings.Default.textureSkyFaceLeft);
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, pathRecursos + Settings.Default.textureSkyFaceRight);

            //Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, pathRecursos + Settings.Default.textureSkyFaceFront);
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, pathRecursos + Settings.Default.textureSkyFaceBack);

            //Actualizar todos los valores para crear el SkyBox
            skyBox.Init();
            return skyBox;
        }

        /// <summary>
        /// Crea los 4 reflectores que posee el estadio
        /// </summary>
        /// <param name="pathRecursos"> ruta donde esta el mesh </param>
        /// <returns></returns>
        private List<Luz> CrearLuces(string pathRecursos, TgcBox cancha)
        {
            List<Luz> luces = new List<Luz>();
            Luz luz1 = this.CrearLuz(pathRecursos, cancha, -1, 1, (float)Math.PI * 3 / 4);
            Luz luz2 = this.CrearLuz(pathRecursos, cancha, 1, 1, -(float)Math.PI * 3 / 4);
            Luz luz3 = this.CrearLuz(pathRecursos, cancha, -1, -1, (float)Math.PI / 4);
            Luz luz4 = this.CrearLuz(pathRecursos, cancha, 1, -1, -(float)Math.PI / 4);

            luces.Add(luz1);
            luces.Add(luz2);
            luces.Add(luz3);
            luces.Add(luz4);
            return luces;
        }

        private Luz CrearLuz(string pathRecursos, TgcBox cancha, int signoX, int signoZ, float rotateY)
        {
            TgcMesh luzMesh = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFilePoste).Meshes[0];
			//TODO cambiar por matrices
			luzMesh.AutoTransformEnable = true;
			luzMesh.rotateY(rotateY);
            luzMesh.Scale = new Vector3(3, 4, 3);
            luzMesh.Position = new Vector3(signoX * (cancha.Size.X / 2 + 50), cancha.Position.Y + luzMesh.BoundingBox.PMax.Y, signoZ * (cancha.Size.Z / 2 + 100));
            Luz luz = new Luz(luzMesh, Color.White, luzMesh.BoundingBox.PMax);
            return luz;
        }

        /// <summary>
        /// Creo la pelota en el centro de la cancha
        /// </summary>
        /// <param name="pathRecursos"> De donde saco la textura</param>
        /// <returns> Una pelota</returns>
        private Pelota CrearPelota(string pathRecursos, Cancha cancha)
        {
            //int radio = 10; Original
            int radio = 6;
            //Crear esfera
            TgcSphere sphere = new TgcSphere();
			//TODO cambiar por matrices
			sphere.AutoTransformEnable = true;
            //sphere.setTexture(TgcTexture.createTexture(pathRecursos + Settings.Default.textureBall));
            sphere.Radius = radio;
            sphere.Position = new Vector3(cancha.Position.X, cancha.Position.Y + radio, cancha.Position.Z);
            sphere.updateValues();

            Pelota pelota = new Pelota(sphere);
            pelota.ShadowEffect = TgcShaders.loadEffect(pathRecursos + "Shaders\\MeshPlanarShadows.fx");
            pelota.LightEffect = TgcShaders.loadEffect(pathRecursos + "Shaders\\MeshMultiplePointLight.fx");
            return pelota;
        }

        /// <summary>
        /// Creo un arco
        /// </summary>
        /// <param name="posicion">Donde va a estar ubicado el Arco</param>
        /// <param name="pathRecursos">De donde sacar el mesh</param>
        /// <returns> Un arco</returns>
        private Arco CrearArco(string pathRecursos, Cancha cancha, int direccion)
        {
            Vector3 posicion = new Vector3(direccion * (cancha.Position.X + cancha.Size.X / 2 - 100), cancha.Position.Y, cancha.Position.Z);

            List<Palo> palos = new List<Palo>();
            TgcMesh palo1 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileGoal).Meshes[0];
            //TODO cambiar por matrices
			palo1.AutoTransformEnable = true;
			palo1.Position = posicion;
            palo1.Scale = new Vector3(0.8f, 0.8f, 0.8f);

            TgcMesh palo2 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileGoal).Meshes[1];
            //TODO cambiar por matrices
			palo2.AutoTransformEnable = true;
			palo2.Position = posicion;
            palo2.Scale = new Vector3(0.8f, 0.8f, 0.8f);

            TgcMesh palo3 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileGoal).Meshes[2];
            //TODO cambiar por matrices
			palo3.AutoTransformEnable = true;
			palo3.Position = posicion;
            palo3.Scale = new Vector3(0.8f, 0.8f, 0.8f);

            palos.Add(new Palo(palo1));
            palos.Add(new Palo(palo2));
            palos.Add(new Palo(palo3));

            Vector3 posicionRed = new Vector3(posicion.X, posicion.Y + palo3.BoundingBox.PMin.Y / 2, posicion.Z);
            Vector3 tamanoRed = new Vector3(0, palo3.BoundingBox.PMin.Y, palo3.BoundingBox.PMax.Z * 2 - 14);

            TgcMesh red = new TgcSceneLoader().loadSceneFromFile(pathRecursos + "Arco\\Red-TgcScene.xml").Meshes[0];
            //TODO cambiar por matrices
			red.AutoTransformEnable = true;
			red.Position = posicionRed + new Vector3(direccion * 65, 0, 0);
            //TODO esto significa que los arcos no son correctos... porque no escalo igual
            red.Scale = new Vector3(3f, 3f, 2f);
            red.AlphaBlendEnable = true;

            //Roto uno de los 2 ya que uso el mismo mesh
            if (direccion == -1)
            {
                red.rotateY(FastMath.PI);
            }

			return new Arco(palos, new Red(TgcBox.fromSize(posicionRed, tamanoRed), red));
        }

        #endregion
    }
}