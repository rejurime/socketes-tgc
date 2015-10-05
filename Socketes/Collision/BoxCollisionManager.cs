using Microsoft.DirectX;
using System.Collections.Generic;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.Socketes.Collision
{
    public class BoxCollisionManager
    {
        private List<IColisionable> obstaculos = new List<IColisionable>();

        public List<IColisionable> Obstaculos
        {
            get { return obstaculos; }
            set { obstaculos = value; }
        }

        /// <summary>
        /// Detecto si el jugador uno colisiona contra algo
        /// </summary>
        /// <param name="lastPos">Posicion anterior a moverse</param>
        public bool DetectarColisiones(IColisionable colisionable, Vector3 lastPos)
        {
            foreach (IColisionable obstaculo in this.obstaculos)
            {
                TgcCollisionUtils.BoxBoxResult result = TgcCollisionUtils.classifyBoxBox(colisionable.getTgcBoundingBox(), obstaculo.getTgcBoundingBox());

                if (result == TgcCollisionUtils.BoxBoxResult.Adentro || result == TgcCollisionUtils.BoxBoxResult.Atravesando)
                {
                    return true;
                }
            }

            return false;
        }
    }
}