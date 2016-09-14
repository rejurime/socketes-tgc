using System;
using System.Diagnostics;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Text;
using TGC.Core.Utils;

namespace AlumnoEjemplos.Socketes.Model
{
	public class Marcador
    {
        private TgcText2D marcador;
        private TgcText2D tiempo;
        private string nombreEquipo1;
        private string nombreEquipo2;
        private int golesEquipo1;
        private int golesEquipo2;
        private TimeSpan tiempoInicial;
        private Stopwatch tiempoMensajeGol;
        private TgcText2D mensajeGol;

        public Marcador(TgcText2D marcador, TgcText2D tiempo, string nombreEquipo1, string nombreEquipo2)
        {
            this.nombreEquipo1 = nombreEquipo1;
            this.golesEquipo1 = 0;
            this.nombreEquipo2 = nombreEquipo2;
            this.golesEquipo2 = 0;

            this.marcador = marcador;
            this.tiempo = tiempo;

            //TODO sacar GUIController lo uso para Ubicarlo centrado en la pantalla, se cambio por el singleton de device sigue siendo malo
            //Size screenSize = GuiController.Instance.Panel3d.Size;
            Size textoSize = new Size(800, 100);

            this.mensajeGol = new TgcText2D();
            this.mensajeGol.Text = "";
            this.mensajeGol.Color = Color.White;
            this.mensajeGol.Align = TgcText2D.TextAlign.CENTER;
            this.mensajeGol.Position = new Point(FastMath.Max(D3DDevice.Instance.Width / 2 - textoSize.Width / 2, 0), FastMath.Max(D3DDevice.Instance.Height / 2 - textoSize.Height / 2, 0));
            this.mensajeGol.Size = textoSize;
            this.mensajeGol.changeFont(new Font("Arial", 48, FontStyle.Bold));

            this.tiempoMensajeGol = new Stopwatch();
        }

        public void render()
        {
            this.ActualizarMarcador();
            this.marcador.render();
            this.ActualizarTiempo();
            this.tiempo.render();
            this.mensajeGol.render();
        }

        public void render(int goles1, int goles2)
        {
            this.golesEquipo1 = goles1;
            this.golesEquipo2 = goles2;

            if (this.tiempoMensajeGol.IsRunning && this.tiempoMensajeGol.Elapsed.Seconds >= 1)
            {
                this.tiempoMensajeGol.Stop();
                this.mensajeGol.Text = "";
            }

            this.render();
        }

        public void dispose()
        {
            this.marcador.Dispose();
            this.tiempo.Dispose();
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

        public void NotificarGol()
        {
            //GOL
            this.mensajeGol.Text = "GOOOLLL!!!";
            this.tiempoMensajeGol = Stopwatch.StartNew();
        }
    }
}