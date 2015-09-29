using AlumnoEjemplos.Properties;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;

namespace AlumnoEjemplos.Socketes.Model
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
            get
            {
                return instance;
            }
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Crea un partido con todos sus componentes Cancha, Arcos, Jugadores y Pelota
        /// </summary>
        /// <param name="alumnoEjemplosMediaDir"> Carpeta donde estan los recursos </param>
        /// <returns> Un partido listo para comenzar a jugar :)</returns>
        public Partido CrearPartido(string pathRecursos)
        {
            Partido partido = new Partido();

            partido.Marcador = this.CrearMarcador();
            partido.Cancha = this.CrearCancha(pathRecursos);
            partido.ArcoLocal = this.CrearArco(new Vector3(940, 0, 25), pathRecursos);
            partido.ArcoVisitante = this.CrearArco(new Vector3(-780, 0, 25), pathRecursos);
            partido.Pelota = this.CrearPelota(pathRecursos);
            partido.JugadorHumano = this.CrearJugadorHumano(pathRecursos);
            partido.JugadoresCPUAliados = this.CrearJugadoresAliados(pathRecursos);
            partido.JugadoresCPURivales = this.CrearJugadoresRivales(pathRecursos);

            return partido;
        }

        /// <summary>
        /// Crea el marcador del partido que tiene los goles y el tiempo
        /// </summary>
        /// <returns> Un Marcador con resultado y tiempo</returns>
        private Marcador CrearMarcador()
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

            return new Marcador(marcador, tiempo, "SKTS", "TGCV");
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
        private List<TgcBox> CrearLimitesCancha()
        {
            List<TgcBox> limites = new List<TgcBox>();
            limites.Add(TgcBox.fromSize(new Vector3(900, 100, 0), new Vector3(0, 220, 1200)));
            limites.Add(TgcBox.fromSize(new Vector3(-900, 100, 0), new Vector3(0, 220, 1200)));
            limites.Add(TgcBox.fromSize(new Vector3(0, 100, 580), new Vector3(1900, 220, 0)));
            limites.Add(TgcBox.fromSize(new Vector3(0, 100, -580), new Vector3(1900, 220, 0)));

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
            TgcMesh arco = new TgcSceneLoader().loadSceneFromFile(pathRecursos + Settings.Default.meshFileGoal).Meshes[0];
            arco.Position = posicion;
            arco.Scale = new Vector3(1.25f, 1.25f, 1.25f);
            return new Arco(arco);
        }

        /// <summary>
        /// Crea el jugador que hay que manejar manualmente
        /// </summary>
        /// <param name="pathRecursos"> De donde saco el mesh</param>
        /// <returns> El jugador controlado manualmente</returns>
        private Jugador CrearJugadorHumano(string pathRecursos)
        {
            return this.CrearJugador(new Vector3(50, 0, 0), 125f, pathRecursos, Settings.Default.textureTeam1);
        }

        /// <summary>
        /// Crea los oponentes al equipo manejado por el jugador
        /// </summary>
        /// <param name="pathRecursos"> De donde saco el mesh</param>
        /// <returns> Una lista de jugadores</returns>
        private List<Jugador> CrearJugadoresRivales(string pathRecursos)
        {
            float anguloEquipoHumano = 90f;
            List<Jugador> jugadores = new List<Jugador>();

            jugadores.Add(this.CrearJugador(new Vector3(120, 0, 100), anguloEquipoHumano, pathRecursos, Settings.Default.textureTeam1));
            return jugadores;
        }

        /// <summary>
        /// Crea los compañeros del jugador humano
        /// </summary>
        /// <param name="pathRecursos"> De donde saco el mesh</param>
        /// <returns> Una lista de jugadores</returns>
        private List<Jugador> CrearJugadoresAliados(string pathRecursos)
        {
            float anguloEquipoCPU = 270f;
            List<Jugador> jugadores = new List<Jugador>();

            jugadores.Add(this.CrearJugador(new Vector3(-130, 0, 160), anguloEquipoCPU, pathRecursos, Settings.Default.textureTeam2));
            jugadores.Add(this.CrearJugador(new Vector3(-155, 0, -160), anguloEquipoCPU, pathRecursos, Settings.Default.textureTeam2));
            return jugadores;
        }

        /// <summary>
        /// Creo un jugador basado en el Robot de TGC
        /// </summary>
        /// <param name="posicion">Posicion donde va a estar el jugador</param>
        /// <param name="angulo">El angulo donde va a mirar</param>
        /// <param name="pathRecursos"></param>
        /// <param name="nombreTextura">Que textura va a tener</param>
        /// <returns></returns>
        private Jugador CrearJugador(Vector3 posicion, float angulo, string pathRecursos, string nombreTextura)
        {
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
            personaje.Position = posicion;

            //Lo Escalo porque es muy grande
            personaje.Scale = new Vector3(0.75f, 0.75f, 0.75f);
            personaje.rotateY(Geometry.DegreeToRadian(angulo));

            return new Jugador(personaje);
        }

        #endregion
    }
}