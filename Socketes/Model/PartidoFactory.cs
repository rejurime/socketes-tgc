using AlumnoEjemplos.Properties;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Collections.Generic;
using System.Reflection;
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

            partido.Cancha = this.CrearCancha(pathRecursos);
            partido.ArcoLocal = this.CrearArco(new Vector3(940, 0, 25), pathRecursos);
            partido.ArcoVisitante = this.CrearArco(new Vector3(-780, 0, 25), pathRecursos);
            partido.Tribunas = this.CrearTribunas(pathRecursos);
            partido.Pelota = this.CrearPelota(pathRecursos);
            this.CrearJugadores(partido, pathRecursos);

            return partido;
        }

        /// <summary>
        /// Creo la cancha donde van a estar parado los jugadores
        /// </summary>
        /// <param name="pathRecursos"></param>
        /// <returns></returns>
        private Cancha CrearCancha(string pathRecursos)
        {
            return new Cancha(TgcBox.fromSize(new Vector3(0, -10, 0), new Vector3(1920, 0, 1200), TgcTexture.createTexture(pathRecursos + Settings.Default.textureFolder + Settings.Default.textureField)));
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
            tribunas.Add(TgcBox.fromSize(new Vector3(900, 100, 0), new Vector3(0, 220, 1200), pisoTexture));
            tribunas.Add(TgcBox.fromSize(new Vector3(-900, 100, 0), new Vector3(0, 220, 1200), pisoTexture));
            tribunas.Add(TgcBox.fromSize(new Vector3(0, 100, 580), new Vector3(1900, 220, 0), pisoTexture));
            tribunas.Add(TgcBox.fromSize(new Vector3(0, 100, -580), new Vector3(1900, 220, 0), pisoTexture));

            return tribunas;
        }

        /// <summary>
        /// Creo la pelota en el centro de la cancha
        /// </summary>
        /// <param name="pathRecursos"></param>
        /// <returns></returns>
        private Pelota CrearPelota(string pathRecursos)
        {
            //Crear esfera
            TgcSphere sphere = new TgcSphere();
            sphere.Radius = 10;
            sphere.setTexture(TgcTexture.createTexture(pathRecursos + Settings.Default.textureFolder + Settings.Default.textureBall));
            sphere.Position = new Vector3(0, 20, 0);
            sphere.updateValues();

            return new Pelota(sphere);
        }

        private Arco CrearArco(Vector3 posicion, string pathRecursos)
        {
            TgcMesh arco = new TgcSceneLoader().loadSceneFromFile(pathRecursos + "Arco\\arco-TgcScene.xml").Meshes[0];
            arco.changeDiffuseMaps(new TgcTexture[] { TgcTexture.createTexture(pathRecursos + Settings.Default.textureFolder + Settings.Default.textureNet) });
            arco.AutoUpdateBoundingBox = true;
            arco.Position = posicion;
            arco.Scale = new Vector3(1.25f, 1.25f, 1.25f);
            arco.updateBoundingBox();

            return new Arco(arco);
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