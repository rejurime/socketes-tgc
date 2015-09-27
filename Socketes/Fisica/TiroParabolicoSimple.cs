using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.Socketes.Fisica;
using Microsoft.DirectX;

namespace AlumnoEjemplos.Socketes.Model
{
    class TiroParabolicoSimple : Tiro
    {
        //direccion del movimiento q se origino
        private Vector3 direccion;

        //fuerza que le queda a cada eje
        private Vector3 fuerzaPorEje;

        //fuerza total que queda, se va decrementando con cada movimiento
        private float fuerza;

        //factor de cambio de movimiento, este factor se usa para ir decrementando X y Z en cada movimiento.
        private float factor = 0.98f;

        //un factor de graveddad para que vaya cayendo en Y.
        private float gravedad = 0.20f;

        public TiroParabolicoSimple(Vector3 direccion, float fuerza)
        {
            this.direccion = direccion;
            this.fuerza = fuerza;

            this.fuerzaPorEje = new Vector3(fuerza, fuerza, fuerza);
        }

        /// <summary>
        /// retorna el siguiente punto a moverse
        ///
        /// </summary>
        /// <param name="elapsedTime"></param>
        /// <returns></returns>
        public Vector3 siguienteMovimiento(float elapsedTime)
        {
            
            Vector3 movimiento = new Vector3(direccion.X * fuerzaPorEje.X, fuerzaPorEje.Y, direccion.Z * fuerzaPorEje.Z);

            //decremento segun el factor la fuerza de cada eje
            fuerza *= factor;
            fuerzaPorEje.X *= factor;
            fuerzaPorEje.Z *= factor;
            fuerzaPorEje.Y = fuerzaPorEje.Y * factor - gravedad;

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
            return !(fuerza < 0.02f && fuerza > -0.02f);
        }

        public float getFuerza()
        {
            return fuerza;
        }
    }
}
