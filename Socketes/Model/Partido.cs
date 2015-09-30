using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSkeletalAnimation;
using AlumnoEjemplos.Socketes.Collision;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Partido
    {
        #region Miembros

        private Cancha cancha;
        private Pelota pelota;
        private Arco arcoLocal;
        private Arco arcoVisitante;
        private Jugador jugadorHumano;
        private List<Jugador> jugadoresCPUAliados = new List<Jugador>();
        private List<Jugador> jugadoresCPURivales = new List<Jugador>();
        private List<TgcBox> tribunas = new List<TgcBox>();

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

        public List<Jugador> JugadoresCPUAliados
        {
            get { return jugadoresCPUAliados; }
            set { jugadoresCPUAliados = value; }
        }

        public List<Jugador> JugadoresCPURivales
        {
            get { return jugadoresCPURivales; }
            set { jugadoresCPURivales = value; }
        }

        public List<TgcBox> Tribunas
        {
            get { return tribunas; }
            set { tribunas = value; }
        }

        public Jugador getJugadoresCPUAliado()
        {
            //RENE: medio feo, retornar el unico elemento de la lista de jugadores. Cuando haya mas se va a romper, plaa!
            return jugadoresCPUAliados[0];
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        internal void render()
        {
            this.cancha.render();
            this.pelota.render();
            this.arcoLocal.render();
            this.arcoVisitante.render();
            this.jugadorHumano.animateAndRender();

            foreach (Jugador jugador in this.jugadoresCPUAliados)
            {
                jugador.animateAndRender();
            }

            foreach (Jugador jugador in this.jugadoresCPURivales)
            {
                jugador.animateAndRender();
            }

            foreach (TgcBox tribunas in this.Tribunas)
            {
                tribunas.BoundingBox.render();
            }
        }

        internal List<Colisionable> ObstaculosPelota()
        {
            List<Colisionable> obstaculos = new List<Colisionable>();

            //RENE ver con rene
            /*
            foreach (TgcBox obstaculo in this.tribunas)
            {
                obstaculos.Add(obstaculo);
            }
            */
            obstaculos.Add(this.cancha);
            obstaculos.Add(this.arcoLocal);
            obstaculos.Add(this.arcoVisitante);

            obstaculos.Add(jugadorHumano);

            foreach (Jugador jugador in this.jugadoresCPUAliados)
            {
                obstaculos.Add(jugador);
            }

            foreach (Jugador jugador in this.jugadoresCPURivales)
            {
                obstaculos.Add(jugador);
            }
            return obstaculos;
        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        internal void dispose()
        {
            this.cancha.dispose();
            this.pelota.dispose();
            this.arcoLocal.dispose();
            this.arcoVisitante.dispose();
            this.jugadorHumano.dispose();

            foreach (Jugador jugador in this.jugadoresCPUAliados)
            {
                jugador.dispose();
            }

            foreach (Jugador jugador in this.jugadoresCPURivales)
            {
                jugador.dispose();
            }

            foreach (TgcBox tribunas in this.Tribunas)
            {
                tribunas.render();
            }
        }

        #endregion
    }
}