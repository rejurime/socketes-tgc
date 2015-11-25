using AlumnoEjemplos.Socketes.Model.ElementosCancha;
using AlumnoEjemplos.Socketes.Model.Jugadores;
using Microsoft.DirectX;
using System.Collections.Generic;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.Sound;

namespace AlumnoEjemplos.Socketes.Model
{
    /// <summary> Clase que tiene todo lo que se esta haciendo render</summary>
    public class Partido
    {
        #region Miembros

        private bool mostrarBounding;
        private bool inteligenciaArtificial;
        private Marcador marcador;
        private Cancha cancha;
        private Pelota pelota;
        private Arco arcoLocal;
        private Arco arcoVisitante;
        private Equipo equipoLocal;
        private Equipo equipoVisitante;
        private TgcThirdPersonCamera camara;

        private Dictionary<string, TgcStaticSound> sonidos;

        //FIXME sacar esto en cuanto se pueda NO A LOS SINGLETONS
        private static readonly Partido instance = new Partido();

        #endregion

        #region  Constructores

        /// <summary> Constructor privado para poder hacer el singleton</summary>
        private Partido() { }

        #endregion

        #region Propiedades

        public Cancha Cancha
        {
            get { return cancha; }
            set { cancha = value; }
        }

        public Pelota Pelota
        {
            get { return pelota; }
            set { pelota = value; }
        }

        public Arco ArcoLocal
        {
            get { return arcoLocal; }
            set { arcoLocal = value; }
        }

        public Arco ArcoVisitante
        {
            get { return arcoVisitante; }
            set { arcoVisitante = value; }
        }

        public Equipo EquipoLocal
        {
            get { return equipoLocal; }
            set { equipoLocal = value; }
        }

        public Equipo EquipoVisitante
        {
            get { return equipoVisitante; }
            set { equipoVisitante = value; }
        }

        public Marcador Marcador
        {
            get { return marcador; }
            set { marcador = value; }
        }

        public bool MostrarBounding
        {
            get { return mostrarBounding; }
            set
            {
                mostrarBounding = value;
                this.cancha.MostrarBounding = value;
                this.pelota.MostrarBounding = value;
                this.arcoLocal.MostrarBounding = value;
                this.arcoVisitante.MostrarBounding = value;
                this.equipoLocal.MostrarBounding = value;
                this.equipoVisitante.MostrarBounding = value;
            }
        }

        public bool InteligenciaArtificial
        {
            get { return inteligenciaArtificial; }
            set
            {
                inteligenciaArtificial = value;
                this.equipoLocal.InteligenciaArtificial = value;
                this.equipoVisitante.InteligenciaArtificial = value;
            }
        }

        public static Partido Instance
        {
            get { return instance; }
        }

        public Dictionary<string, TgcStaticSound> Sonidos
        {
            get { return sonidos; }
            set { sonidos = value; }
        }

        public TgcThirdPersonCamera Camara
        {
            get { return camara; }
            set { camara = value; }
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public void render(float elapsedTime)
        {
            this.cancha.render();

            //Sombras
            this.equipoLocal.renderShadow(elapsedTime, this.cancha.Luces);
            this.equipoVisitante.renderShadow(elapsedTime, this.cancha.Luces);
            this.pelota.renderShadow(elapsedTime, this.cancha.Luces);

            //Luz
            this.equipoLocal.renderLight(elapsedTime, this.cancha.Luces);
            this.equipoVisitante.renderLight(elapsedTime, this.cancha.Luces);
            this.pelota.updateValues(elapsedTime);
            this.pelota.renderLight(elapsedTime, this.cancha.Luces);

            this.arcoLocal.render();
            this.arcoVisitante.render();

            this.marcador.render(this.equipoLocal.Goles, this.equipoVisitante.Goles);

            //Hacer que la camara siga al personaje en su nueva posicion
            this.camara.Target = new Vector3(this.Pelota.Position.X, 0, this.Pelota.Position.Z);
        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public void dispose()
        {
            this.marcador.dispose();
            this.cancha.dispose();
            this.pelota.dispose();
            this.arcoLocal.dispose();
            this.arcoVisitante.dispose();
            this.equipoLocal.dispose();
            this.equipoVisitante.dispose();
        }

        //TODO De aca para abajo hay que ver como llegar aca por ahora se llega con el singleton PUAJIS
        public void NotificarPelotaDominada(Jugador jugador)
        {
            this.equipoLocal.NotificarPelotaDominada(jugador);
            this.equipoVisitante.NotificarPelotaDominada(jugador);
        }

        public void NotificarGol(Red red)
        {
            this.marcador.NotificarGol();

            if (this.equipoLocal.ArcoPropio.Red.Equals(red))
            {
                this.equipoVisitante.Goles += 1;
            }
            else
            {
                this.equipoLocal.Goles += 1;
            }

            this.ReiniciarPosiciones();
        }

        public void ReiniciarPosiciones()
        {
            this.equipoLocal.ReiniciarPosiciones();
            this.equipoVisitante.ReiniciarPosiciones();
            this.pelota.ReiniciarPosicion();
        }

        public void SetTexturasSeleccionadas(string pathTexturaPelota, string pathTexturaJugadorLocal, string pathTexturaJugadorVisitante)
        {
            this.pelota.SetTextura(pathTexturaPelota);
            this.equipoLocal.SetTextura(pathTexturaJugadorLocal);
            this.equipoVisitante.SetTextura(pathTexturaJugadorVisitante);
        }

        public void SetCamaraPelota()
        {
            this.camara.setCamera(new Vector3(this.Pelota.Position.X, 0, this.Pelota.Position.Z), Settings.Default.camaraOffsetHeight, Settings.Default.camaraOffsetForward);
        }

        public void SetCamaraAerea()
        {
            this.camara.setCamera(new Vector3(this.Pelota.Position.X, 0, this.Pelota.Position.Z), 800, -1);
        }

        #endregion
    }
}