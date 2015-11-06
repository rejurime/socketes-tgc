using AlumnoEjemplos.Socketes.Utils;
using Microsoft.DirectX;
using System;
using TgcViewer;

namespace AlumnoEjemplos.Socketes.Fisica
{
    class TiroParabolicoSimple : ITiro
    {
        //direccion del movimiento q se origino
        private Vector3 direccion;

        //fuerza que le queda a cada eje
        private Vector3 fuerzaPorEje;

        //fuerza total que queda, se va decrementando con cada movimiento
        private float fuerza;

        //un factor de graveddad para que vaya cayendo en Y.
        private float gravedad = 1000f;
        private float fuerzaOriginal;

        public TiroParabolicoSimple(Vector3 direccion, float fuerza)
        {
            this.direccion = direccion;
            this.direccion.Normalize();
            this.fuerza = fuerza;
            this.fuerzaOriginal = fuerza;

            this.fuerzaPorEje = new Vector3(fuerza, fuerza / 1.4f, fuerza);
        }

        /// <summary>
        /// retorna el siguiente punto a moverse
        ///
        /// </summary>
        /// <param name="elapsedTime"></param>
        /// <returns></returns>
        public Vector3 siguienteMovimiento(float elapsedTime)
        {

            Vector3 movimiento = new Vector3(direccion.X * fuerzaPorEje.X * elapsedTime, fuerzaPorEje.Y * elapsedTime, direccion.Z * fuerzaPorEje.Z * elapsedTime);

            //decremento segun el factor la fuerza de cada eje
            fuerza -= (fuerzaOriginal * elapsedTime);
            fuerzaPorEje.X -= (fuerzaOriginal * elapsedTime);
            fuerzaPorEje.Z -= (fuerzaOriginal * elapsedTime);
            fuerzaPorEje.Y -= (fuerzaOriginal * elapsedTime);
            fuerzaPorEje.Y -= (gravedad * elapsedTime);

            return movimiento;
        }

        /// <summary>
        /// Determinar si sigue existiendo movimiento, 
        /// en este caso lo hace en base a la fuerza, 
        /// si la fuerza se acabo, deja de moverse y caera por gravedad
        /// </summary>
        /// <returns></returns>
        public bool hayMovimiento()
        {
            return fuerza > 0.5f;
        }

        public float getFuerza()
        {
            return fuerza;
        }
    }
}