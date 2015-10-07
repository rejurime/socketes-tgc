using System;
using TgcViewer.Utils._2D;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Marcador
    {
        private TgcText2d marcador;
        private TgcText2d tiempo;
        private string nombreEquipo1;
        private string nombreEquipo2;
        private int golesEquipo1;
        private int golesEquipo2;
        private TimeSpan tiempoInicial;

        public Marcador(TgcText2d marcador, TgcText2d tiempo, string nombreEquipo1, string nombreEquipo2)
        {
            this.nombreEquipo1 = nombreEquipo1;
            this.golesEquipo1 = 0;
            this.nombreEquipo2 = nombreEquipo2;
            this.golesEquipo2 = 0;

            this.marcador = marcador;
            this.tiempo = tiempo;
        }

        public void render()
        {
            this.ActualizarMarcador();
            this.marcador.render();
            this.ActualizarTiempo();
            this.tiempo.render();
        }

        public void dispose()
        {
            this.marcador.dispose();
            this.tiempo.dispose();
        }

        public void IniciarTiempo()
        {
            this.tiempoInicial = DateTime.Now.TimeOfDay;
        }

        public void ActualizarMarcador()
        {
            this.marcador.Text = this.nombreEquipo1 + " " + this.golesEquipo1 + " - " + this.nombreEquipo2 + " " + this.golesEquipo2;
        }

        public void ActualizarTiempo()
        {
            this.tiempo.Text = (DateTime.Now.TimeOfDay - this.tiempoInicial).ToString("mm':'ss");
        }

        public void SumarGoleEquipo1()
        {
            this.golesEquipo1++;
        }

        public void SumarGoleEquipo2()
        {
            this.golesEquipo2++;
        }
    }
}