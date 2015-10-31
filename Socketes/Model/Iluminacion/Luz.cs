using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model.Iluminacion
{
    /// <summary>
    /// Clase para manejo de luces, contiene la info de la luz
    /// </summary>
    public class Luz
    {
        private Vector3 posicion;
        private Color color;
        private TgcMesh luzMesh;

        public Luz(TgcMesh luzMesh)
        {
            this.luzMesh = luzMesh;
        }

        public void render()
        {
            this.luzMesh.render();
        }

        public void close()
        {
            this.luzMesh.dispose();
        }

    }
}
