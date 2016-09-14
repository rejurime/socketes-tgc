using System.Collections.Generic;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;

namespace AlumnoEjemplos.Socketes.Model.Colision
{
	public class PelotaCollisionManager
    {
        private List<IColisionablePelota> obstaculos;

        public PelotaCollisionManager(List<IColisionablePelota> obstaculos)
        {
            this.obstaculos = obstaculos;
        }

        public ColisionInfo GetColisiones(TgcBoundingAxisAlignBox colisionable)
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