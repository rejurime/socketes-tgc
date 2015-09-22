using AlumnoEjemplos.Properties;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Collections.Generic;
using System.Reflection;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;

namespace AlumnoEjemplos.MiGrupo.Model
{
    public class PartidoFactory
    {
        #region Miembros

        private static readonly PartidoFactory instance = new PartidoFactory();

        private string pathRecursos = System.Environment.CurrentDirectory + "\\" + Assembly.GetExecutingAssembly().GetName().Name + "\\" + Settings.Default.mediaFolder;

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
        /// <param name="d3dDevice"> Device donde esta el partido </param>
        /// <param name="alumnoEjemplosMediaDir"> Carpeta donde estan los recursos </param>
        /// <returns> Un partido listo para comenzar a jugar :)</returns>
        public Partido CrearPartido(Device d3dDevice)
        {
            Partido partido = new Partido();

            partido.Cancha = this.CrearCancha(d3dDevice, pathRecursos);
            partido.Pelota = this.CrearPelota(d3dDevice, pathRecursos);
            this.CrearArcos(partido, d3dDevice, pathRecursos);
            this.CrearJugadores(partido, d3dDevice, pathRecursos);
            partido.Tribunas = this.CrearTribunas(d3dDevice, pathRecursos);

            return partido;
        }

        /// <summary>
        /// Creo la cancha donde van a estar parado los jugadores
        /// </summary>
        /// <param name="pathRecursos"></param>
        /// <returns></returns>
        private TgcBox CrearCancha(string pathRecursos)
        {
            TgcTexture pisoTexture = TgcTexture.createTexture(pathRecursos + Settings.Default.textureFolder + Settings.Default.textureField);
            return TgcBox.fromSize(new Vector3(0, -10, 0), new Vector3(1920, 0, 1200), pisoTexture);
        }

        /// <summary>
        /// Creo las tribunas que me sirven como limie de la cancla
        /// </summary>
        /// <param name="pathRecursos"></param>
        /// <returns></returns>
        private List<TgcBox> CrearTribunas(string pathRecursos)
        {
            TgcTexture pisoTexture = TgcTexture.createTexture(pathRecursos + Settings.Default.textureFolder + Settings.Default.texturePeople);

            List<TgcBox> tribunas = new List<TgcBox>();
            tribunas.Add(TgcBox.fromSize(new Vector3(900, 200, 0), new Vector3(0, 400, 1200), pisoTexture));
            tribunas.Add(TgcBox.fromSize(new Vector3(-900, 200, 0), new Vector3(0, 400, 1200), pisoTexture));
            tribunas.Add(TgcBox.fromSize(new Vector3(0, 200, 580), new Vector3(1900, 400, 0), pisoTexture));
            tribunas.Add(TgcBox.fromSize(new Vector3(0, 200, -580), new Vector3(1900, 400, 0), pisoTexture));

            return tribunas;
        }

        /// <summary>
        /// Creo la pelota en el centro de la cancha
        /// </summary>
        /// <param name="pathRecursos"></param>
        /// <returns></returns>
        private Pelota CrearPelota(string pathRecursos)
        {
            return new Pelota();
        }

        /// <summary>
        /// Creo los 2 arcos
        /// </summary>
        /// <param name="partido"></param>
        /// <param name="pathRecursos"></param>
        private void CrearArcos(Partido partido, string pathRecursos)
        {
            TgcTexture texturaArco = TgcTexture.createTexture(d3dDevice, pathRecursos + Settings.Default.textureFolder + Settings.Default.textureNet);
            Vector3 size = new Vector3(20, 170, 350);

            partido.ArcoLocal = TgcBox.fromSize(new Vector3(875, 75, 5), size, texturaArco);
            partido.ArcoVisitante = TgcBox.fromSize(new Vector3(-875, 75, 5), size, texturaArco);
        }

        /// <summary>
        /// Creo los 4 jugadores, 2 de cada equipo
        /// </summary>
        /// <param name="partido"></param>
        /// <param name="pathRecursos"></param>
        private void CrearJugadores(Partido partido, string pathRecursos)
        {
            float anguloEquipoHumano = 90f;
            float anguloEquipoCPU = 270f;

            partido.JugadorHumano = this.CrearJugador(new Vector3(30, -8, 0), 125f, pathRecursos, Settings.Default.textureTeam1);
            partido.JugadoresCPUAliados.Add(this.CrearJugador(new Vector3(120, -8, 100), anguloEquipoHumano, pathRecursos, Settings.Default.textureTeam1));

            partido.JugadoresCPURivales.Add(this.CrearJugador(new Vector3(-130, -8, 160), anguloEquipoCPU, pathRecursos, Settings.Default.textureTeam2));
            partido.JugadoresCPURivales.Add(this.CrearJugador(new Vector3(-155, -8, -160), anguloEquipoCPU, pathRecursos, Settings.Default.textureTeam2));
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
                TgcTexture.createTexture(pathRecursos + Settings.Default.meshFolderPlayer + Settings.Default.meshTextureFolder + nombreTextura)
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