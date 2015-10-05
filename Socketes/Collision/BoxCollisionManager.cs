using Microsoft.DirectX;
using System.Collections.Generic;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.Socketes.Collision
{
    public class BoxCollisionManager
    {
        private List<TgcBoundingBox> obstaculos = new List<TgcBoundingBox>();

        public List<TgcBoundingBox> Obstaculos
        {
            get { return obstaculos; }
            set { obstaculos = value; }
        }

        /// <summary>
        /// Detecto si el jugador uno colisiona contra algo
        /// </summary>
        /// <param name="lastPos">Posicion anterior a moverse</param>
        public bool DetectarColisiones(TgcBoundingBox boundingBox, Vector3 lastPos)
        {
            foreach (TgcBoundingBox obstaculo in this.obstaculos)
            {
                TgcCollisionUtils.BoxBoxResult result = TgcCollisionUtils.classifyBoxBox(boundingBox, obstaculo);

                if (result == TgcCollisionUtils.BoxBoxResult.Adentro || result == TgcCollisionUtils.BoxBoxResult.Atravesando)
                {
                    return true;
                }
            }

            return false;
        }
    }
}