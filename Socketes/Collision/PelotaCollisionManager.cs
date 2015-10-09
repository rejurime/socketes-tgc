using Microsoft.DirectX;
using System.Collections.Generic;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.Socketes.Collision
{
    public class PelotaCollisionManager
    {
        /// <summary>
        /// Habilita o deshabilita la aplicación de fuerza de gravedad
        /// </summary>
        private bool gravityEnabled = true;

        private List<IColisionablePelota> obstaculos;

        public PelotaCollisionManager(List<IColisionablePelota> obstaculos)
        {
            this.obstaculos = obstaculos;
        }

        internal ColisionInfo GetColisiones(TgcBoundingBox colisionable)
        {
            ColisionInfo colisionInfo = new ColisionInfo();

            foreach (IColisionablePelota obstaculo in this.obstaculos)
            {
                TgcCollisionUtils.BoxBoxResult result = TgcCollisionUtils.classifyBoxBox(colisionable, obstaculo.GetTgcBoundingBox());

                if (result == TgcCollisionUtils.BoxBoxResult.Adentro || result == TgcCollisionUtils.BoxBoxResult.Atravesando)
                {
                    colisionInfo.Add(obstaculo);
                }
            }

            return colisionInfo;
        }
    }
}