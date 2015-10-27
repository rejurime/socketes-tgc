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
        public Partido CrearPartido(string pathRecursos, TgcD3dInput input, Dictionary<string, TgcStaticSound> sonidos)
        {
            string nombreEquipoLocal = "SKTS";
            string nombreEquipoVisitante = "TGCV";

            Partido partido = Partido.Instance;

            partido.Sonidos = sonidos;
            partido.Marcador = this.CrearMarcador(nombreEquipoLocal, nombreEquipoVisitante);
            partido.Cancha = this.CrearCancha(pathRecursos);
            partido.Luces = this.CrearLuces(pathRecursos);

            partido.ArcoLocal = this.CrearArco(new Vector3(860, -8, -12), pathRecursos);
            partido.ArcoVisitante = this.CrearArco(new Vector3(-860, -8, -12), pathRecursos);
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

        private List<Luz> CrearLuces(string pathRecursos)
        {
            //TODO impelmentar creacion de luces
            List<Luz> luces = new List<Luz>();

            return luces;
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
            TgcMesh tribuna1 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileTribunePl).Meshes[0];
            tribuna1.move(new Vector3(0, 70, 800));
            tribuna1.rotateY(-(float)Math.PI / 2);
            tribuna1.Scale = new Vector3(10, 10, 14);

            TgcMesh tribuna2 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileTribunePl).Meshes[0];
            tribuna2.move(new Vector3(0, 70, -800));
            tribuna2.rotateY((float)Math.PI / 2);
            tribuna2.Scale = new Vector3(10, 10, 14);

            //Atras del arco
            TgcMesh tribuna3 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileTribunePo).Meshes[0];
            tribuna3.move(new Vector3(1100, 55, 0));
            tribuna3.rotateY(-(float)Math.PI / 2);
            tribuna3.Scale = new Vector3(10, 10, 15);

            TgcMesh tribuna4 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileTribunePo).Meshes[0];
            tribuna4.move(new Vector3(-1100, 55, 0));
            tribuna4.rotateY((float)Math.PI / 2);
            tribuna4.Scale = new Vector3(10, 10, 15);

            TgcBox boxField = TgcBox.fromSize(new Vector3(0, -10, 0), new Vector3(1920, 0, 1200), TgcTexture.createTexture(pathRecursos + Settings.Default.textureFolder + Settings.Default.textureField));
            TgcBox boxFloor = TgcBox.fromSize(new Vector3(0, -11, 0), new Vector3(2350, 0, 1750), TgcTexture.createTexture(pathRecursos + Settings.Default.textureFolder + Settings.Default.textureFloor));

            List<IRenderObject> componentes = new List<IRenderObject>();
            componentes.Add(tribuna1);
            componentes.Add(tribuna2);
            componentes.Add(tribuna3);
            componentes.Add(tribuna4);
            componentes.Add(boxFloor);

            return new Cancha(boxField, componentes, this.CrearLimitesCancha());
        }

        /// <summary>
        /// Creo los limites de la cancha
        /// </summary>
        /// <returns> Lista con los limites de la cancha</returns>
        private List<LimiteCancha> CrearLimitesCancha()
        {
            List<LimiteCancha> limites = new List<LimiteCancha>();
            limites.Add(new LimiteCancha(TgcBox.fromSize(new Vector3(860, 270, 0), new Vector3(0, 550, 1200))));
            limites.Add(new LimiteCancha(TgcBox.fromSize(new Vector3(-860, 270, 0), new Vector3(0, 550, 1200))));
            limites.Add(new LimiteCancha(TgcBox.fromSize(new Vector3(0, 270, 575), new Vector3(1920, 550, 0))));
            limites.Add(new LimiteCancha(TgcBox.fromSize(new Vector3(0, 270, -575), new Vector3(1920, 550, 0))));

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
            //sphere.Radius = 10; Original
            sphere.Radius = 8;
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
            List<Palo> palos = new List<Palo>();
            TgcMesh palo1 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileGoal).Meshes[0];
            palo1.Position = posicion;
            palo1.Scale = new Vector3(1.1f, 1.1f, 1.1f);

            TgcMesh palo2 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileGoal).Meshes[1];
            palo2.Position = posicion;
            palo2.Scale = new Vector3(1.1f, 1.1f, 1.1f);

            TgcMesh palo3 = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileGoal).Meshes[2];
            palo3.Position = posicion;
            palo3.Scale = new Vector3(1.1f, 1.1f, 1.1f);

            palos.Add(new Palo(palo1));
            palos.Add(new Palo(palo2));
            palos.Add(new Palo(palo3));

            Vector3 posicionRed = new Vector3(posicion.X, posicion.Y + 58, posicion.Z);
            Vector3 tamanoRed = new Vector3(0, 118, 266);
            return new Arco(palos, new Red(TgcBox.fromSize(posicionRed, tamanoRed)));
        }

        #endregion
    }
}