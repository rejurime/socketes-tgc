using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model
{
    public class MenuItem : IRenderObject
    {
        private TgcText2d texto;
        private TgcBoxLine opcion;

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

        public MenuItem(Vector3 vectorOrigen, Vector3 vectorFin, string texto)
        {
            this.opcion = TgcBoxLine.fromExtremes(vectorOrigen, vectorFin, Color.SteelBlue, 0.3f);

            this.texto = new TgcText2d();
            this.texto.Text = texto;
            this.texto.Color = Color.White;
            this.texto.Align = TgcText2d.TextAlign.CENTER;
            //TODO Claramente aca la estoy peteando tengo que hacerlo con alguna transformacion no?...
            this.texto.Position = new Point(Convert.ToInt16(vectorOrigen.X) + 60, Convert.ToInt16(vectorOrigen.Y) + 180);
            this.texto.Size = new Size(150, 100);
            this.texto.changeFont(new System.Drawing.Font("Arial", 14, FontStyle.Bold));
        }

        public void render()
        {
            this.texto.render();
            this.opcion.render();
        }

        public void dispose()
        {
            this.texto.dispose();
            this.opcion.dispose();
        }
    }
}