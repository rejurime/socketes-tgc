using Microsoft.DirectX;
using System.Collections.Generic;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.Socketes.Collision
{
    public class PelotaCollisionManager
    {
        private List<IColisionable> obstaculos;

        public PelotaCollisionManager(List<IColisionable> obstaculos)
        {
            this.obstaculos = obstaculos;
        }


        public ColisionInfo moveCharacter(TgcBoundingBox colisionable)
        {
            ColisionInfo colisionInfo = new ColisionInfo();

            foreach (IColisionable obstaculo in this.obstaculos)
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