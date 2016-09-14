using Microsoft.DirectX;
using System.Collections.Generic;
using TGC.Core.Collision;

namespace AlumnoEjemplos.Socketes.Model.Colision
{
	/// <summary> 
	/// Encargado de manejar las colisiones entre cajas
	/// </summary>
	public class BoxCollisionManager
    {
        private List<IColisionable> obstaculos;

        public BoxCollisionManager(List<IColisionable> colisionables)
        {
            this.obstaculos = colisionables;
        }

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
                TgcCollisionUtils.BoxBoxResult result = TgcCollisionUtils.classifyBoxBox(colisionable.GetTgcBoundingBox(), obstaculo.GetTgcBoundingBox());

                if (result == TgcCollisionUtils.BoxBoxResult.Adentro || result == TgcCollisionUtils.BoxBoxResult.Atravesando)
                {
                    return true;
                }
            }

            return false;
        }
    }
}