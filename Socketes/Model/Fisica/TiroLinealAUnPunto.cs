using Microsoft.DirectX;

namespace AlumnoEjemplos.Socketes.Fisica
{
    /// <summary>
    /// Movimiento linea de un punto a otro.
    /// 
    /// </summary>
    class TiroLinealAUnPunto : ITiro
    {
        private Vector3 posicionActual;
        private Vector3 posicionDestino;
        private Vector3 direccion;
        private Vector3 fuerzaPorEje;
        private float fuerza;
        private float factor = 0.98f;

        public TiroLinealAUnPunto(Vector3 posicionActual, Vector3 posicionDestino, float fuerza)
        {
            this.posicionActual = posicionActual;
            this.posicionDestino = posicionDestino;
            this.fuerza = fuerza;
            this.fuerzaPorEje = new Vector3(fuerza, fuerza, fuerza);

            //Vector resultante de los dos puntos
            direccion = posicionDestino - posicionActual;

            //normal del vector que une lo dos puntos
            direccion.Normalize();
        }

        /// <summary>
        /// retorna el siguiente punto a moverse
        ///
        /// </summary>
        /// <param name="elapsedTime"></param>
        /// <returns></returns>
        public Vector3 siguienteMovimiento(float elapsedTime)
        {
            //movimiento siguiente, no se mueve en eje Y ya que va por el piso
            Vector3 movimiento = new Vector3(direccion.X * fuerzaPorEje.X, 0, direccion.Z * fuerzaPorEje.Z);

            //decremento segun el factor la fuerza de cada eje
            fuerza *= factor;
            fuerzaPorEje.X *= factor;
            fuerzaPorEje.Z *= factor;

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