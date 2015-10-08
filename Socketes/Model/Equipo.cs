using System;
using System.Collections.Generic;
using AlumnoEjemplos.Socketes.Collision;
using AlumnoEjemplos.Socketes.Model.JugadorStrategy;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Equipo
    {
        private bool mostrarBounding;
        private string nombre;
        private List<Jugador> jugadores;
        private bool inteligenciaArtificial;

        public Equipo(string nombre, List<Jugador> jugadores)
        {
            this.nombre = nombre;
            this.jugadores = jugadores;
        }

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

        public List<IColisionable> JugadoresColisionables()
        {
            List<IColisionable> colisionables = new List<IColisionable>();

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
    }
}