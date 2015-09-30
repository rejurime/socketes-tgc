using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.Socketes.Collision
{
    public class ColisionInfo
    {
        private List<Colisionable> collisiones = new List<Colisionable>();
        private Vector3 realMovementVector = Vector3.Empty;

        public ColisionInfo()
        {

        }

        internal void Add(Colisionable colision)
        {
            collisiones.Add(colision);
        }

        internal void addMovimientoRelativo(Vector3 realMovementVector)
        {
            this.realMovementVector = realMovementVector;
        }

        internal Vector3 getRealMovement()
        {
            return realMovementVector;
        }
    }
}
