using AlumnoEjemplos.Socketes.Collision;
using AlumnoEjemplos.Socketes.Model.JugadorStrategy;
using System.Collections.Generic;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model
{
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
        private Jugador jugadorHumano;
        private Jugador jugadorIAAliado;
        private List<Jugador> jugadoresIARivales = new List<Jugador>();

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

        public Jugador JugadorHumano
        {
            get { return jugadorHumano; }
            set { jugadorHumano = value; }
        }

        public Jugador JugadorIAAliado
        {
            get { return jugadorIAAliado; }
            set { jugadorIAAliado = value; }
        }

        public List<Jugador> JugadoresIARivales
        {
            get { return jugadoresIARivales; }
            set { jugadoresIARivales = value; }
        }

        public Marcador Marcador
        {
            get { return marcador; }
            set { marcador = value; }
        }

        public bool MostrarBounding
        {
            get
            {
                return mostrarBounding;
            }

            set
            {
                mostrarBounding = value;
                this.cancha.MostrarBounding = value;
                this.pelota.MostrarBounding = value;
                this.arcoLocal.MostrarBounding = value;
                this.arcoVisitante.MostrarBounding = value;
                this.jugadorHumano.MostrarBounding = value;
                this.jugadorIAAliado.MostrarBounding = value;

                foreach (Jugador jugador in this.jugadoresIARivales)
                {
                    jugador.MostrarBounding = value;
                }
            }
        }

        public bool InteligenciaArtificial
        {
            get { return inteligenciaArtificial; }
            set
            {
                inteligenciaArtificial = value;
                ((JugadorIAStrategy)this.jugadorIAAliado.Strategy).InteligenciaArtificial = value;

                foreach (Jugador jugador in this.jugadoresIARivales)
                {
                    ((JugadorIAStrategy)jugador.Strategy).InteligenciaArtificial = value;
                }
            }
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
            this.pelota.updateValues(elapsedTime);

            this.marcador.render();
            this.cancha.render();
            this.pelota.render();
            this.arcoLocal.render();
            this.arcoVisitante.render();
            this.jugadorHumano.animateAndRender(elapsedTime);
            this.jugadorIAAliado.animateAndRender(elapsedTime);

            foreach (Jugador jugador in this.jugadoresIARivales)
            {
                jugador.animateAndRender(elapsedTime);
            }
        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public void dispose()
        {
            this.marcador.render();
            this.cancha.dispose();
            this.pelota.dispose();
            this.arcoLocal.dispose();
            this.arcoVisitante.dispose();
            this.jugadorHumano.dispose();
            this.jugadorIAAliado.dispose();

            foreach (Jugador jugador in this.jugadoresIARivales)
            {
                jugador.dispose();
            }
        }

        #endregion
    }
}