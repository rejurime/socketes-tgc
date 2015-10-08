using AlumnoEjemplos.Socketes.Model.JugadorStrategy;
using System.Collections.Generic;
using System;

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
        private Equipo equipoLocal;
        private Equipo equipoVisitante;

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
            this.equipoLocal.render(elapsedTime);
            this.equipoVisitante.render(elapsedTime);
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

        //TODO hay que ver como llegar aca por ahora se llega con el singleton PUAJIS
        public void NotificarPelotaDominada(Jugador jugador)
        {
            this.equipoLocal.NotificarPelotaDominada(jugador);
            this.equipoVisitante.NotificarPelotaDominada(jugador);
        }

        #endregion
    }
}