using AlumnoEjemplos.Properties;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
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

            this.CrearCancha(partido, d3dDevice, pathRecursos);
            this.CrearPelota(partido, d3dDevice, pathRecursos);
            this.CrearArcos(partido, d3dDevice, pathRecursos);
            this.CrearJugadores(partido, d3dDevice, pathRecursos);

            return partido;
        }

        /// <summary>
        /// Creo la cancha donde van a estar parado los jugadores
        /// </summary>
        /// <param name="d3dDevice"></param>
        private void CrearCancha(Partido partido, Device d3dDevice, string pathRecursos)
        {
            TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, pathRecursos + Settings.Default.textureFolder + Settings.Default.textureField);
            partido.Cancha = TgcBox.fromSize(new Vector3(0, -10, 0), new Vector3(1920, 0, 1200), pisoTexture);
        }

        private void CrearTribunas(Partido partido, Device d3dDevice, string pathRecursos)
        {
            //TgcBox.fromSize(new Vector3(0, -10, 0), new Vector3(0, 1920, 1200));
            //TgcBox.fromSize(new Vector3(0, -10, 0), new Vector3(1920, 0, 1200));
            //TgcBox.fromSize(new Vector3(0, -10, 0), new Vector3(1920, 0, 1200));
            //TgcBox.fromSize(new Vector3(0, -10, 0), new Vector3(1920, 0, 1200));
        }

        /// <summary>
        /// Creo la pelota en el centro de la cancha
        /// </summary>
        /// <param name="d3dDevice"></param>
        private void CrearPelota(Partido partido, Device d3dDevice, string pathRecursos)
        {
            //Crear esfera
            partido.Pelota = new TgcSphere();

            partido.Pelota.setTexture(TgcTexture.createTexture(d3dDevice, pathRecursos + Settings.Default.textureFolder + Settings.Default.textureBall));
            partido.Pelota.Radius = 10;
            partido.Pelota.Position = new Vector3(0, 0, 0);

            partido.Pelota.updateValues();
        }

        /// <summary>
        /// Creo los 2 arcos
        /// </summary>
        /// <param name="d3dDevice"></param>
        private void CrearArcos(Partido partido, Device d3dDevice, string pathRecursos)
        {
            TgcTexture texturaArco = TgcTexture.createTexture(d3dDevice, pathRecursos + Settings.Default.textureFolder + Settings.Default.textureNet);
            Vector3 size = new Vector3(20, 125, 250);

            partido.ArcoLocal = TgcBox.fromSize(new Vector3(875, 52, 10), size, texturaArco);
            partido.ArcoVisitante = TgcBox.fromSize(new Vector3(-875, 52, 10), size, texturaArco);
        }

        /// <summary>
        /// Creo los 4 jugadores, 2 de cada equipo
        /// </summary>
        /// <param name="d3dDevice"></param>
        private void CrearJugadores(Partido partido, Device d3dDevice, string pathRecursos)
        {
            float anguloEquipoHumano = 90f;
            float anguloEquipoCPU = 270f;

            partido.JugadorHumano = this.CrearJugador(d3dDevice, new Vector3(15, -9, 0), anguloEquipoHumano, pathRecursos, Settings.Default.textureTeam1);
            partido.JugadoresCPUAliados.Add(this.CrearJugador(d3dDevice, new Vector3(100, -9, 100), anguloEquipoHumano, pathRecursos, Settings.Default.textureTeam1));

            partido.JugadoresCPURivales.Add(this.CrearJugador(d3dDevice, new Vector3(-125, -9, 160), anguloEquipoCPU, pathRecursos, Settings.Default.textureTeam2));
            partido.JugadoresCPURivales.Add(this.CrearJugador(d3dDevice, new Vector3(-150, -9, -160), anguloEquipoCPU, pathRecursos, Settings.Default.textureTeam2));
        }

        /// <summary>
        /// Creo un jugador basado en el Robot de TGC
        /// </summary>
        /// <param name="posicion">Posicion donde va a estar el jugador</param>
        /// <param name="angulo">El angulo donde va a mirar</param>
        /// <param name="nombreTextura">Que textura va a tener</param>
        /// <returns></returns>
        private TgcSkeletalMesh CrearJugador(Device d3dDevice, Vector3 posicion, float angulo, string pathRecursos, string nombreTextura)
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
                TgcTexture.createTexture(d3dDevice, pathRecursos + Settings.Default.meshFolderPlayer + Settings.Default.meshTextureFolder + nombreTextura)
                });

            //Configurar animacion inicial
            personaje.playAnimation(Settings.Default.animationStopPlayer, true);
            personaje.Position = posicion;

            //Lo Escalo porque es muy grande
            personaje.Scale = new Vector3(0.55f, 0.55f, 0.55f);
            personaje.rotateY(Geometry.DegreeToRadian(angulo));

            return personaje;
        }

        #endregion
    }
}