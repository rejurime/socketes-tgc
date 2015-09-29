using System;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Marcador : IRenderObject
    {
        private TgcText2d marcador;
        private TgcText2d tiempo;
        private string nombreEquipo1;
        private string nombreEquipo2;
        private int golesEquipo1;
        private int golesEquipo2;
        private TimeSpan tiempoInicial;

        //TODO TGCV me pide esto para que sea IRender...
        public bool AlphaBlendEnable
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Marcador(TgcText2d marcador, TgcText2d tiempo, string nombreEquipo1, string nombreEquipo2)
        {
            this.tiempoInicial = DateTime.Now.TimeOfDay;
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
