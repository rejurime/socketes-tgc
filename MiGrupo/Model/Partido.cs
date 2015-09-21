using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSkeletalAnimation;

namespace AlumnoEjemplos.MiGrupo.Model
{
    public class Partido
    {
        #region Miembros

        private TgcBox cancha;
        private TgcSphere pelota;
        private TgcBox arcoLocal;
        private TgcBox arcoVisitante;
        private TgcSkeletalMesh jugadorHumano;
        private List<TgcSkeletalMesh> jugadoresCPUAliados = new List<TgcSkeletalMesh>();
        private List<TgcSkeletalMesh> jugadoresCPURivales = new List<TgcSkeletalMesh>();

        //TODO esto no lo deberia tener la clase jugador? :)
        float velocidadCaminar = 400f;
        float velocidadRotacion = 150f;

        #endregion

        #region Propiedades

        public TgcBox Cancha
        {
            get { return cancha; }
            set { cancha = value; }
        }

        public TgcSphere Pelota
        {
            get { return pelota; }
            set { pelota = value; }
        }

        public TgcBox ArcoLocal
        {
            get { return arcoLocal; }
            set { arcoLocal = value; }
        }

        public TgcBox ArcoVisitante
        {
            get { return arcoVisitante; }
            set { arcoVisitante = value; }
        }

        public TgcSkeletalMesh JugadorHumano
        {
            get { return jugadorHumano; }
            set { jugadorHumano = value; }
        }

        public List<TgcSkeletalMesh> JugadoresCPUAliados
        {
            get { return jugadoresCPUAliados; }
            set { jugadoresCPUAliados = value; }
        }

        public List<TgcSkeletalMesh> JugadoresCPURivales
        {
            get { return jugadoresCPURivales; }
            set { jugadoresCPURivales = value; }
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

            foreach (TgcSkeletalMesh jugador in this.jugadoresCPUAliados)
            {
                jugador.animateAndRender();
            }

            foreach (TgcSkeletalMesh jugador in this.jugadoresCPURivales)
            {
                jugador.animateAndRender();
            }
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

            foreach (TgcSkeletalMesh jugador in this.jugadoresCPUAliados)
            {
                jugador.dispose();
            }

            foreach (TgcSkeletalMesh jugador in this.jugadoresCPURivales)
            {
                jugador.dispose();
            }
        }

        internal float VelocidadCaminarJugador()
        {
            return this.velocidadCaminar;
        }

        internal float VelocidadRotacion()
        {
            return this.velocidadRotacion;
        }

        internal Vector3 JugadorHumanoPosition()
        {
            return this.jugadorHumano.Position;
        }

        #endregion
    }
}