using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.Socketes.Model.Iluminacion
{
    /// <summary>
    /// Clase para manejo de luces, contiene la info de la luz
    /// </summary>
    public class Luz
    {
        private Vector3 posicion;

        public Vector3 Posicion
        {
            get { return posicion; }
            set { posicion = value; }
        }
        private Color color;

        private TgcBox box;

        public void render()
        {
            //TODO implementar magia de luz
        }

    }
}
