using Microsoft.DirectX;
using System.Collections.Generic;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.Socketes.Collision
{
    public class PelotaCollisionManager
    {
        /// <summary>
        /// Vector que representa la fuerza de gravedad.
        /// Debe tener un valor negativo en Y para que la fuerza atraiga hacia el suelo
        /// </summary>
        private Vector3 gravityForce = new Vector3(0, -1.5f, 0);

        /// <summary>
        /// Habilita o deshabilita la aplicación de fuerza de gravedad
        /// </summary>
        private bool gravityEnabled = true;

        private List<IColisionablePelota> obstaculos;

        public PelotaCollisionManager(List<IColisionablePelota> obstaculos)
        {
            this.obstaculos = obstaculos;
        }

        public ColisionInfo moveCharacter(TgcBoundingBox colisionable)
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

            if(this.gravityEnabled)
            {
                colisionInfo.addMovimientoRelativo(this.gravityForce);
            }

            return colisionInfo;
        }
    }
}