using AlumnoEjemplos.Socketes.Model.Colision;
using AlumnoEjemplos.Socketes.Model.ElementosCancha;
using AlumnoEjemplos.Socketes.Model.Jugadores;
using System.Collections.Generic;
using System;

namespace AlumnoEjemplos.Socketes.Model
{
    /// <summary> Clase equipo</summary>
    public class Equipo
    {
        #region Miembros

        private string nombre;
        private List<Jugador> jugadores;
        private Arco arcoPropio;
        private Arco arcoRival;
        private bool mostrarBounding;
        private bool inteligenciaArtificial;
        private int goles = 0;

        #endregion

        #region  Constructores

        public Equipo(string nombre, List<Jugador> jugadores, Arco arcoPropio, Arco arcoRival)
        {
            this.nombre = nombre;
            this.jugadores = jugadores;
            this.arcoPropio = arcoPropio;
            this.arcoRival = arcoRival;
        }

        #endregion

        #region Propiedades

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        public List<Jugador> Jugadores
        {
            get { return jugadores; }
            set { jugadores = value; }
        }

        public bool MostrarBounding
        {
            get { return mostrarBounding; }
            set
            {
                mostrarBounding = value;

                foreach (Jugador jugador in this.jugadores)
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
                //TODO no hacia algo tan feo desde Operativos... como hace 10 años jajaja 
                //capaz teniendo un equipo humano como subclase o algo podria solucionar todos los problemas del maldito humano
                foreach (Jugador jugador in this.jugadores)
                {
                    if (jugador.Strategy.GetType().Equals(typeof(JugadorIAStrategy)))
                    {
                        ((JugadorIAStrategy)jugador.Strategy).InteligenciaArtificial = value;
                    }
                }
            }
        }

        public Arco ArcoPropio
        {
            get { return arcoPropio; }
            set { arcoPropio = value; }
        }

        public Arco ArcoRival
        {
            get { return arcoRival; }
            set { arcoRival = value; }
        }

        public int Goles
        {
            get { return goles; }
            set { goles = value; }
        }

        #endregion

        #region Metodos

        public List<IColisionablePelota> JugadoresColisionables()
        {
            List<IColisionablePelota> colisionables = new List<IColisionablePelota>();

            foreach (Jugador jugador in this.jugadores)
            {
                colisionables.Add(jugador);
            }

            return colisionables;
        }

        public void render(float elapsedTime)
        {
            foreach (Jugador jugador in this.jugadores)
            {
                jugador.animateAndRender(elapsedTime);
            }
        }

        public void renderShadow(float elapsedTime, List<Iluminacion.Luz> luces)
        {
            foreach (Jugador jugador in this.jugadores)
            {
                jugador.renderShadow(elapsedTime, luces);
            }
        }

        public void dispose()
        {
            foreach (Jugador jugador in this.jugadores)
            {
                jugador.dispose();
            }
        }

        public void NotificarPelotaDominada(Jugador jugador)
        {
            if (this.jugadores.Contains(jugador))
            {
                foreach (Jugador jugadorAtacando in this.jugadores)
                {
                    jugadorAtacando.Atacando = true;

                    if (jugador.Equals(jugadorAtacando))
                    {
                        jugadorAtacando.PelotaDominada = true;
                    }
                    else
                    {
                        jugadorAtacando.PelotaDominada = false;
                    }
                }
            }
            else
            {
                foreach (Jugador jugadorDefendiendo in this.jugadores)
                {
                    jugadorDefendiendo.Atacando = false;
                    jugadorDefendiendo.PelotaDominada = false;
                }
            }
        }

        public override string ToString()
        {
            return nombre;
        }

        public void ReiniciarPosiciones()
        {
            foreach (Jugador jugador in this.jugadores)
            {
                jugador.ReiniciarPosicion();
            }
        }

        public Jugador JugadorMasCercanoPelota()
        {
            Jugador jugadorMasCercano = null;

            foreach (Jugador jugador in this.jugadores)
            {
                if (jugadorMasCercano == null)
                {
                    jugadorMasCercano = jugador;
                }
                else if (jugadorMasCercano.DistanciaPelota() > jugador.DistanciaPelota())
                {
                    jugadorMasCercano = jugador;
                }
            }

            return jugadorMasCercano;
        }

        public Jugador JugadorMasCercanoPelota(Jugador jugadorExcluido)
        {
            Jugador jugadorMasCercano = null;

            foreach (Jugador jugador in this.jugadores)
            {
                if (!jugador.Equals(jugadorExcluido))
                {
                    if (jugadorMasCercano == null)
                    {
                        jugadorMasCercano = jugador;
                    }
                    else if (jugadorMasCercano.DistanciaPelota() > jugador.DistanciaPelota())
                    {
                        jugadorMasCercano = jugador;
                    }
                }
            }

            return jugadorMasCercano;
        }

        #endregion
    }
}