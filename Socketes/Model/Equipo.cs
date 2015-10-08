using AlumnoEjemplos.Socketes.Collision;
using AlumnoEjemplos.Socketes.Model.JugadorStrategy;
using System.Collections.Generic;

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

        #endregion

        #region  Constructores

        public Equipo(string nombre, List<Jugador> jugadores, Arco arcoPropio, Arco arcoRival)
        {
            this.nombre = nombre;
            this.jugadores = jugadores;
            this.ArcoPropio = arcoPropio;
            this.ArcoRival = arcoRival;
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

        #endregion

        #region Metodos

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

        public override string ToString()
        {
            return nombre;
        }

        #endregion
    }
}