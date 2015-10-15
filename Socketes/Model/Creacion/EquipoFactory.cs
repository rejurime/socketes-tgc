using AlumnoEjemplos.Socketes.Model.Colision;
using AlumnoEjemplos.Socketes.Model.ElementosCancha;
using AlumnoEjemplos.Socketes.Model.Jugadores;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Collections.Generic;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;

namespace AlumnoEjemplos.Socketes.Model.Creacion
{
    /// <summary> Factory para los equipo</summary>
    public class EquipoFactory
    {
        #region Miembros

        private static readonly EquipoFactory instance = new EquipoFactory();

        #endregion

        #region Constructores

        /// <summary> Constructor privado para poder hacer el singleton</summary>
        private EquipoFactory() { }

        #endregion

        #region Propiedades

        public static EquipoFactory Instance
        {
            get { return instance; }
        }

        #endregion

        #region Metodos

        /// <summary> Crea un equipo con un jugador humano y el resto IA</summary>
        /// <param name="pathRecursos"> De donde saco el mesh</param>
        /// <param name="input"> Input por teclado</param>
        /// <param name="pelota"> La pelota del partido</param>
        /// <returns> Un equipo formado con un humano y el resto IA pero sin colisiones</returns>
        public Equipo CrearEquipoHumanoIA(string nombre, string pathRecursos, TgcD3dInput input, Pelota pelota, Arco arcoLocal, Arco arcoRival)
        {
            List<Jugador> jugadores = new List<Jugador>();
            jugadores.Add(this.CrearJugadorHumano(pathRecursos, Settings.Default.textureTeam1, new Vector3(50, -8, 0), 125f, pelota, input));
            jugadores.Add(this.CrearJugadorIA(pathRecursos, Settings.Default.textureTeam1, new Vector3(120, -8, 100), 90f, pelota));

            Equipo equipo = new Equipo(nombre, jugadores, arcoLocal, arcoRival);

            foreach (Jugador jugador in equipo.Jugadores)
            {
                jugador.EquipoPropio = equipo;
            }

            return equipo;
        }

        /// <summary> Crea un equipo manejado todo por IA</summary>
        /// <param name="pathRecursos"> De donde saco el mesh</param>
        /// <param name="pelota"> La pelota del partido</param>
        /// <returns> Un equipo con todos jugadores IA pero sin colisiones</returns>
        public Equipo CrearEquipoIA(string nombre, string pathRecursos, Pelota pelota, Arco arcoLocal, Arco arcoRival)
        {
            float anguloEquipoCPU = 270f;

            List<Jugador> jugadores = new List<Jugador>();
            jugadores.Add(this.CrearJugadorIA(pathRecursos, Settings.Default.textureTeam2, new Vector3(-130, -8, 160), anguloEquipoCPU, pelota));
            jugadores.Add(this.CrearJugadorIA(pathRecursos, Settings.Default.textureTeam2, new Vector3(-155, -8, -160), anguloEquipoCPU, pelota));

            Equipo equipo = new Equipo(nombre, jugadores, arcoLocal, arcoRival);

            foreach (Jugador jugador in equipo.Jugadores)
            {
                jugador.EquipoPropio = equipo;
            }

            return equipo;
        }

        /// <summary>
        /// Crea el jugador que hay que manejar manualmente
        /// </summary>
        /// <param name="pathRecursos"> De donde saco el mesh</param>
        /// /// /// <param name="pelota">La pelota del partido</param>
        /// <returns> El jugador controlado manualmente</returns>
        private Jugador CrearJugadorHumano(string pathRecursos, string textura, Vector3 posicion, float angulo, Pelota pelota, TgcD3dInput input)
        {
            return this.CrearJugador(pathRecursos, textura, posicion, angulo, new JugadorManualStrategy(input), pelota);
        }

        /// /// <summary>
        /// Crea un jugador con IA
        /// </summary>
        /// <param name="pathRecursos"> De donde saco el mesh</param>
        /// /// <param name="pelota">La pelota del partido</param>
        /// <returns> Una lista de jugadores</returns>
        private Jugador CrearJugadorIA(string pathRecursos, string textura, Vector3 posicion, float angulo, Pelota pelota)
        {
            return this.CrearJugador(pathRecursos, textura, posicion, angulo, new JugadorIAStrategy(), pelota);
        }

        /// <summary>
        /// Creo un jugador basado en el Robot de TGC
        /// </summary>
        /// <param name="posicion">Posicion donde va a estar el jugador</param>
        /// <param name="angulo">El angulo donde va a mirar</param>
        /// <param name="pathRecursos"></param>
        /// <param name="nombreTextura">Que textura va a tener</param>
        /// <returns></returns>
        private Jugador CrearJugador(string pathRecursos, string nombreTextura, Vector3 posicion, float angulo, IJugadorMoveStrategy strategy, Pelota pelota)
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

            return new Jugador(personaje, strategy, pelota);
        }

        /// <summary> Le carga los colisionables a cada jugador</summary>
        /// <param name="equipo1"> Equipo a cargar</param>
        /// <param name="equipo2"> El otro equipo a cargar</param>
        /// <param name="partido"> Partido</param>
        public void CargarColisionesEquipos(Equipo equipo1, Equipo equipo2, Partido partido)
        {
            foreach (Jugador jugador in equipo1.Jugadores)
            {
                this.CargarLosObstaculosAlJugador(equipo1, equipo2, partido, jugador);
            }

            foreach (Jugador jugador in equipo2.Jugadores)
            {
                this.CargarLosObstaculosAlJugador(equipo2, equipo1, partido, jugador);
            }
        }

        /// <summary> Cargo las colisiones para un jugador particular</summary>
        /// <param name="equipoPropio"> Cargo a todos menos a mi</param>
        /// <param name="equiporival"> Cargo a todos los jugadores</param>
        /// <param name="partido"> Necesario para otras cosas colisionables</param>
        /// <param name="jugador"> Jugador al cual tengo que cargarles las colisiones</param>
        private void CargarLosObstaculosAlJugador(Equipo equipoPropio, Equipo equiporival, Partido partido, Jugador jugador)
        {
            List<IColisionable> colisionables = new List<IColisionable>();

            colisionables.AddRange(partido.Cancha.LimitesCancha);
            colisionables.Add(partido.Cancha);
            colisionables.AddRange(partido.ArcoLocal.GetColisionables());
            colisionables.AddRange(partido.ArcoVisitante.GetColisionables());

            foreach (Jugador jugadorColision in equipoPropio.Jugadores)
            {
                if (!jugador.Equals(jugadorColision))
                {
                    colisionables.Add(jugadorColision);
                }
            }

            foreach (Jugador jugadorColision in equiporival.Jugadores)
            {
                colisionables.Add(jugadorColision);
            }

            jugador.CollisionManager = new BoxCollisionManager(colisionables);
        }

        #endregion
    }
}