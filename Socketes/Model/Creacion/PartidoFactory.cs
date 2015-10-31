using AlumnoEjemplos.Socketes.Model.Colision;
using AlumnoEjemplos.Socketes.Model.ElementosCancha;
using AlumnoEjemplos.Socketes.Model.Iluminacion;
using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.Terrain;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model.Creacion
{
    /// <summary> Factory para crear todos los objetos relacionados con el Partido </summary>
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
        public Partido CrearPartido(string pathRecursos, TgcD3dInput input, Dictionary<string, TgcStaticSound> sonidos)
        {
            string nombreEquipoLocal = "SKTS";
            string nombreEquipoVisitante = "TGCV";

            Partido partido = Partido.Instance;

            partido.Sonidos = sonidos;
            partido.Marcador = this.CrearMarcador(nombreEquipoLocal, nombreEquipoVisitante);
            partido.Cancha = this.CrearCancha(pathRecursos);
            partido.ArcoLocal = this.CrearArco(new Vector3(860, -10, -12), pathRecursos);
            partido.ArcoVisitante = this.CrearArco(new Vector3(-860, -10, -12), pathRecursos);
            partido.Pelota = this.CrearPelota(pathRecursos);
            partido.EquipoLocal = EquipoFactory.Instance.CrearEquipoHumanoIA(nombreEquipoLocal, pathRecursos, input, partido.Pelota, partido.ArcoLocal, partido.ArcoVisitante);
            partido.EquipoVisitante = EquipoFactory.Instance.CrearEquipoIA(nombreEquipoVisitante, pathRecursos, partido.Pelota, partido.ArcoVisitante, partido.ArcoLocal);

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
            marcador.Size = new Size(200, 100);
            marcador.changeFont(new System.Drawing.Font("Arial", 14, FontStyle.Bold));

            //Contador de Tiempo
            TgcText2d tiempo = new TgcText2d();
            tiempo.Color = Color.White;
            tiempo.Align = TgcText2d.TextAlign.CENTER;
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
            //Laterales
            TgcMesh tribuna1 = CrearTribuna(pathRecursos + Settings.Default.meshFileTribunePl, new Vector3(0, 70, 800), -(float)Math.PI / 2, new Vector3(10, 12, 14));
            TgcMesh tribuna2 = CrearTribuna(pathRecursos + Settings.Default.meshFileTribunePl, new Vector3(0, 70, -800), (float)Math.PI / 2, new Vector3(10, 12, 14));

            //Atras del arco
            TgcMesh tribuna3 = CrearTribuna(pathRecursos + Settings.Default.meshFileTribunePo, new Vector3(1100, 55, 0), -(float)Math.PI / 2, new Vector3(14, 12, 15));
            TgcMesh tribuna4 = CrearTribuna(pathRecursos + Settings.Default.meshFileTribunePo, new Vector3(-1100, 55, 0), (float)Math.PI / 2, new Vector3(14, 12, 15));

            //Cesped
            TgcBox boxField = TgcBox.fromSize(new Vector3(0, -10, 0), new Vector3(1920, 0, 1200), TgcTexture.createTexture(pathRecursos + Settings.Default.textureField));

            //Piso
            TgcBox boxFloor = TgcBox.fromSize(new Vector3(0, -11, 0), new Vector3(2600, 0, 2000), TgcTexture.createTexture(pathRecursos + Settings.Default.textureFloor));

            List<IRenderObject> componentes = new List<IRenderObject>();
            componentes.Add(tribuna1);
            componentes.Add(tribuna2);
            componentes.Add(tribuna3);
            componentes.Add(tribuna4);
            componentes.Add(boxFloor);
            componentes.Add(CrearSkyBox(pathRecursos));
            componentes.Add(TgcBox.fromSize(new Vector3(1275, 89, 0), new Vector3(0, 200, 1950), TgcTexture.createTexture(pathRecursos + Settings.Default.textureWall)));
            componentes.Add(TgcBox.fromSize(new Vector3(-1275, 89, 0), new Vector3(0, 200, 1950), TgcTexture.createTexture(pathRecursos + Settings.Default.textureWall)));
            componentes.Add(TgcBox.fromSize(new Vector3(0, 89, 970), new Vector3(2550, 200, 0), TgcTexture.createTexture(pathRecursos + Settings.Default.textureWall)));
            componentes.Add(TgcBox.fromSize(new Vector3(0, 89, -970), new Vector3(2550, 200, 0), TgcTexture.createTexture(pathRecursos + Settings.Default.textureWall)));

            return new Cancha(boxField, componentes, this.CrearLimitesCancha(), this.CrearLuces(pathRecursos));
        }

        /// <summary> 
        /// Crea una tribuna desde un mesh, pero por como esta creada se necesita acomodarla en la cancha y escalarla 
        /// </summary>
        /// <param name="pathMesh"> ruta donde esta el mesh de la tribuna </param>
        /// <param name="position"> donde la tengo que ubicar a la tribuna</param>
        /// <param name="rotateY"> como la tengo que rotar para que quede en la horientacion correcta </param>
        /// <param name="scale"> factor de escalado para que quede armonica a la cancha </param>
        /// <returns></returns>
        private static TgcMesh CrearTribuna(string pathMesh, Vector3 position, float rotateY, Vector3 scale)
        {
            TgcMesh tribuna = new TgcSceneLoader().loadSceneFromFile(pathMesh).Meshes[0];
            tribuna.Position = position;
            tribuna.rotateY(rotateY);
            tribuna.Scale = scale;
            return tribuna;
        }

        /// <summary>
        /// Creo los limites de la cancha
        /// </summary>
        /// <returns> Lista con los limites de la cancha</returns>
        private List<LimiteCancha> CrearLimitesCancha()
        {
            List<LimiteCancha> limites = new List<LimiteCancha>();
            limites.Add(new LimiteCancha(TgcBox.fromSize(new Vector3(860, 265, 0), new Vector3(0, 550, 1200))));
            limites.Add(new LimiteCancha(TgcBox.fromSize(new Vector3(-860, 265, 0), new Vector3(0, 550, 1200))));
            limites.Add(new LimiteCancha(TgcBox.fromSize(new Vector3(0, 265, 575), new Vector3(1920, 550, 0))));
            limites.Add(new LimiteCancha(TgcBox.fromSize(new Vector3(0, 265, -575), new Vector3(1920, 550, 0))));

            return limites;
        }

        /// <summary>
        /// Crea un SkyBox que rodea toda la cancha y da una sensacion mas de realizamo
        /// </summary>
        /// <param name="pathRecursos"> la ruta donde se encuentran los recursos </param>
        /// <returns></returns>
        private static TgcSkyBox CrearSkyBox(string pathRecursos)
        {
            //Crear SkyBox 
            TgcSkyBox skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 100, 0);
            skyBox.Size = new Vector3(3000, 3000, 3000);

            //Configurar las texturas para cada una de las 6 caras
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, pathRecursos + Settings.Default.textureSkyFacesUp);
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, pathRecursos + Settings.Default.textureSkyFaceDown);
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, pathRecursos + Settings.Default.textureSkyFaceLeft);
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, pathRecursos + Settings.Default.textureSkyFaceRight);

            //Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, pathRecursos + Settings.Default.textureSkyFaceFront);
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, pathRecursos + Settings.Default.textureSkyFaceBack);

            //Actualizar todos los valores para crear el SkyBox
            skyBox.updateValues();
            return skyBox;
        }

        /// <summary>
        /// Crea los 4 reflectores que posee el estadio
        /// </summary>
        /// <param name="pathRecursos"> ruta donde esta el mesh </param>
        /// <returns></returns>
        private List<Luz> CrearLuces(string pathRecursos)
        {
            List<Luz> luces = new List<Luz>();

            //TODO cargar las luces correctamente
            TgcMesh luzMesh1 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + "Poste\\Poste-TgcScene.xml").Meshes[0];
            luzMesh1.Position = new Vector3(-1000, 98, 700);
            luzMesh1.rotateY((float)Math.PI * 3 / 4);
            luzMesh1.Scale = new Vector3(3, 3, 3);
            Luz luz1 = new Luz(luzMesh1);

            TgcMesh luzMesh2 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + "Poste\\Poste-TgcScene.xml").Meshes[0];
            luzMesh2.Position = new Vector3(1000, 98, 700);
            luzMesh2.rotateY(-(float)Math.PI * 3 / 4);
            luzMesh2.Scale = new Vector3(3, 3, 3);
            Luz luz2 = new Luz(luzMesh2);

            TgcMesh luzMesh3 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + "Poste\\Poste-TgcScene.xml").Meshes[0];
            luzMesh3.Position = new Vector3(-1000, 98, -700);
            luzMesh3.rotateY((float)Math.PI / 4);
            luzMesh3.Scale = new Vector3(3, 3, 3);
            Luz luz3 = new Luz(luzMesh3);

            TgcMesh luzMesh4 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + "Poste\\Poste-TgcScene.xml").Meshes[0];
            luzMesh4.Position = new Vector3(1000, 98, -700);
            luzMesh4.rotateY(-(float)Math.PI / 4);
            luzMesh4.Scale = new Vector3(3, 3, 3);
            Luz luz4 = new Luz(luzMesh4);

            luces.Add(luz1);
            luces.Add(luz2);
            luces.Add(luz3);
            luces.Add(luz4);

            return luces;
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
            sphere.setTexture(TgcTexture.createTexture(pathRecursos + Settings.Default.textureBall));
            //sphere.Radius = 10; Original
            sphere.Radius = 6;
            sphere.Position = new Vector3(0, 5, 0);
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
            List<Palo> palos = new List<Palo>();
            TgcMesh palo1 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileGoal).Meshes[0];
            palo1.Position = posicion;
            palo1.Scale = new Vector3(0.8f, 0.8f, 0.8f);

            TgcMesh palo2 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileGoal).Meshes[1];
            palo2.Position = posicion;
            palo2.Scale = new Vector3(0.8f, 0.8f, 0.8f);

            TgcMesh palo3 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileGoal).Meshes[2];
            palo3.Position = posicion;
            palo3.Scale = new Vector3(0.8f, 0.8f, 0.8f);

            palos.Add(new Palo(palo1));
            palos.Add(new Palo(palo2));
            palos.Add(new Palo(palo3));

            Vector3 posicionRed = new Vector3(posicion.X, posicion.Y + 42, posicion.Z);
            Vector3 tamanoRed = new Vector3(0, 80, 190);
            return new Arco(palos, new Red(TgcBox.fromSize(posicionRed, tamanoRed)));
        }

        #endregion
    }
}