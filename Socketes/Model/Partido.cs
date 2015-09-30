using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSkeletalAnimation;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.Socketes.Collision;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Partido : IRenderObject
    {
        #region Miembros

        private Marcador marcador;
        private Cancha cancha;
        private Pelota pelota;
        private Arco arcoLocal;
        private Arco arcoVisitante;
        private Jugador jugadorHumano;
        private List<Jugador> jugadoresCPUAliados = new List<Jugador>();
        private List<Jugador> jugadoresCPURivales = new List<Jugador>();

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

        public bool AlphaBlendEnable
        {
            get
            {
                return this.cancha.AlphaBlendEnable;
            }

            set
            {
                this.cancha.AlphaBlendEnable = value;
                this.pelota.AlphaBlendEnable = value;
                this.arcoLocal.AlphaBlendEnable = value;
                this.arcoVisitante.AlphaBlendEnable = value;
                this.jugadorHumano.AlphaBlendEnable = value;

                foreach (Jugador jugador in this.jugadoresCPUAliados)
                {
                    jugador.AlphaBlendEnable = value;
                }

                foreach (Jugador jugador in this.JugadoresCPURivales)
                {
                    jugador.AlphaBlendEnable = value;
                }
            }
        }

        public Marcador Marcador
        {
            get { return marcador; }
            set { marcador = value; }
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
        public void render()
        {
            this.marcador.render();
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
        }

       internal List<Colisionable> ObstaculosPelota()

        {
            List<Colisionable> obstaculos = new List<Colisionable>();

            //RENE ver con rene, hay que transformar tribunas en objetos colisionables
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

            //RENE ver con rene, hay que transformar los limites en objetos colisionables
            //obstaculos.AddRange(this.cancha.BoundingBoxes);


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

        public List<TgcBoundingBox> ObstaculosJugadorHumano()
        {
            List<TgcBoundingBox> obstaculos = new List<TgcBoundingBox>();

            obstaculos.AddRange(this.cancha.BoundingBoxes);
            obstaculos.Add(this.cancha.BoundingBoxCesped);
            obstaculos.Add(this.arcoLocal.BoundingBox);
            obstaculos.Add(this.arcoVisitante.BoundingBox);

            return obstaculos;
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

            foreach (Jugador jugador in this.jugadoresCPUAliados)
            {
                jugador.dispose();
            }

            foreach (Jugador jugador in this.jugadoresCPURivales)
            {
                jugador.dispose();
            }
        }

        #endregion
    }
}