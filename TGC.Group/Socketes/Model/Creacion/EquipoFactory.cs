﻿using AlumnoEjemplos.Socketes.Model.Colision;
using AlumnoEjemplos.Socketes.Model.Jugadores;
using Microsoft.DirectX;
using System.Collections.Generic;
using TGC.Core.Input;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Shaders;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.Socketes.Model.Creacion
{
	/// <summary> Factory para crear los equipo</summary>
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
        public Equipo CrearEquipoHumanoIA(string nombre, string pathRecursos, TgcD3dInput input, Partido partido)
        {
            List<Jugador> jugadores = new List<Jugador>();
            jugadores.Add(this.CrearJugadorIA(pathRecursos, Settings.Default.textureTeam1, new Vector3(0, 1, 30), 45f, partido.Pelota));
            jugadores.Add(this.CrearJugadorHumano(pathRecursos, Settings.Default.textureTeam1, new Vector3(-180, 1, -150), 270, partido.Pelota, input));
            jugadores.Add(this.CrearJugadorIA(pathRecursos, Settings.Default.textureTeam1, new Vector3(-400, 1, -150), 270f, partido.Pelota));
            jugadores.Add(this.CrearJugadorIA(pathRecursos, Settings.Default.textureTeam1, new Vector3(-400, 1, 150), 270f, partido.Pelota));
            jugadores.Add(this.CrearArqueroIA(pathRecursos, Settings.Default.textureTeam1, new Vector3(partido.ArcoLocal.Red.GetPosition().X + 60, 1, partido.ArcoLocal.Red.GetPosition().Z), 270f, partido.Pelota));

            Equipo equipo = new Equipo(nombre, jugadores, partido.ArcoLocal, partido.ArcoVisitante);

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
        public Equipo CrearEquipoIA(string nombre, string pathRecursos, Partido partido)
        {
            float anguloEquipoCPU = 90f;

            List<Jugador> jugadores = new List<Jugador>();
            jugadores.Add(this.CrearJugadorIA(pathRecursos, Settings.Default.textureTeam2, new Vector3(180, 1, -150), anguloEquipoCPU, partido.Pelota));
            jugadores.Add(this.CrearJugadorIA(pathRecursos, Settings.Default.textureTeam2, new Vector3(180, 1, 150), anguloEquipoCPU, partido.Pelota));
            jugadores.Add(this.CrearJugadorIA(pathRecursos, Settings.Default.textureTeam2, new Vector3(400, 1, -150), anguloEquipoCPU, partido.Pelota));
            jugadores.Add(this.CrearJugadorIA(pathRecursos, Settings.Default.textureTeam2, new Vector3(400, 1, 150), anguloEquipoCPU, partido.Pelota));
            jugadores.Add(this.CrearArqueroIA(pathRecursos, Settings.Default.textureTeam1, new Vector3(partido.ArcoVisitante.Red.GetPosition().X - 60, 1, partido.ArcoVisitante.Red.GetPosition().Z), anguloEquipoCPU, partido.Pelota));

            Equipo equipo = new Equipo(nombre, jugadores, partido.ArcoVisitante, partido.ArcoLocal);

            foreach (Jugador jugador in equipo.Jugadores)
            {
                jugador.EquipoPropio = equipo;
            }

            return equipo;
        }

        private Jugador CrearArqueroIA(string pathRecursos, string textura, Vector3 posicion, float angulo, Pelota pelota)
        {
            //TODO deberia usar la inteligencia del arquero no la IA pero no la puedo hacer andar :(
            return this.CrearJugador(pathRecursos, textura, posicion, angulo, new JugadorIAStrategy(), pelota);
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
            //personaje.changeDiffuseMaps(new TgcTexture[] {
            //    TgcTexture.createTexture(pathRecursos + Settings.Default.meshFolderPlayer + nombreTextura)
            //    });

			//TODO cambiar por matrices
			personaje.AutoTransformEnable = true;

            //Configurar animacion inicial
            personaje.playAnimation(Settings.Default.animationStopPlayer, true);
            personaje.Position = posicion;

            //Lo Escalo porque es muy grande
            personaje.Scale = new Vector3(0.5f, 0.5f, 0.5f);
            personaje.rotateY(Geometry.DegreeToRadian(angulo));

            //Recalculo las normales para evitar problemas con la luz
            personaje.computeNormals();

            Jugador jugador = new Jugador(personaje, strategy, pelota);
            jugador.ShadowEffect = TgcShaders.loadEffect(pathRecursos + "Shaders\\MeshPlanarShadows.fx");
            jugador.LightEffect = TgcShaders.loadEffect(pathRecursos + "Shaders\\SkeletalMeshMultiplePointLight.fx");
            return jugador;
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

            /* TODO quito las colosiones hasta tenerlas mejor lo pidio el profe
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
            }*/

            jugador.CollisionManager = new BoxCollisionManager(colisionables);
        }

        #endregion
    }
}