using Microsoft.DirectX;
using System.Collections.Generic;

namespace AlumnoEjemplos.Socketes.Collision
{
    public class ColisionInfo
    {
        private List<IColisionable> collisiones = new List<IColisionable>();
        private Vector3 realMovementVector = Vector3.Empty;

        public ColisionInfo()
        {

        }

        public void Add(IColisionable colision)
        {
            collisiones.Add(colision);
        }

        public void addMovimientoRelativo(Vector3 realMovementVector)
        {
            this.realMovementVector = realMovementVector;
        }

        public Vector3 getRealMovement()
        {
            return realMovementVector;
        }

        public List<IColisionable> getColisiones()
        {
            return collisiones;
        }
    }
}