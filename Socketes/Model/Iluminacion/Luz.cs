using Microsoft.DirectX;
using System.Drawing;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model.Iluminacion
{
    /// <summary>
    /// Clase para manejo de luces, contiene la info de la luz
    /// </summary>
    public class Luz
    {
        private Vector3 luzPosicion;
        private Color luzColor;
        private IRenderObject luzMesh;

        public Vector3 Posicion
        {
            get { return luzPosicion; }
            set { luzPosicion = value; }
        }

        public Color Color
        {
            get { return luzColor; }
            set { luzColor = value; }
        }

        public Luz(IRenderObject luzMesh, Color color, Vector3 posicion)
        {
            this.luzMesh = luzMesh;
            this.luzColor = color;
            this.luzPosicion = posicion;
            //TODO ver de mover toda la luz 5 veces.
            this.luzPosicion.Y *= 5;
        }

        public void render()
        {
            this.luzMesh.render();
        }

        public void dispose()
        {
            this.luzMesh.dispose();
        }
    }
}