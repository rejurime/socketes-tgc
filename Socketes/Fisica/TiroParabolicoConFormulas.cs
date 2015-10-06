using Microsoft.DirectX;
using System;

namespace AlumnoEjemplos.Socketes.Fisica
{

    /// <summary>
    /// Se quizo hacer en base a estos sitios:
    /// 
    /// http://ingciv-sandrus.blogspot.com.ar/2008/05/tiro-parablico-en-tres-dimensiones.html
    /// http://www.fisicanet.com.ar/fisica/cinematica/ap06_tiro_parabolico.php
    /// </summary>
    class TiroParabolicoConFormulas : Tiro
    {
        private Vector3 posicionInicial;
        private Vector3 direccion;
        private float fuerza;
        private float tiempototal;
        private float alpha;
        private float beta;

        //sacar afuera
        private float gravedad = 9.5f;

        public TiroParabolicoConFormulas(Vector3 posicionInicial, Vector3 direccion, float fuerza)
        {
            this.posicionInicial = posicionInicial;
            this.direccion = direccion;
            this.fuerza = fuerza;
            this.tiempototal = 0;

            //angulo ente el vector direccion y el eje x
            this.alpha = (float)Math.Acos(direccion.X / (Math.Sqrt(Math.Pow(direccion.X, 2) + Math.Pow(direccion.Y, 2) + Math.Pow(direccion.Z, 2))));


            //angulo ente el vector direccion y el eje z
            this.beta = (float)Math.Acos(direccion.Z / (Math.Sqrt(Math.Pow(direccion.X, 2) + Math.Pow(direccion.Y, 2) + Math.Pow(direccion.Z, 2))));
        }

        /// <summary>
        /// retorna el siguiente punto de la parabola segun el tiempo pasado.
        /// 
        /// Revise el tiempo transcurrido entre el ultimo llamado
        /// 
        /// </summary>
        /// <param name="elapsedTime"></param>
        /// <returns></returns>
        public Vector3 siguienteMovimiento(float elapsedTime)
        {
            Vector3 position = new Vector3(0, 0, 0);

            //acumulo el tiempo
            tiempototal += elapsedTime;

            position.Y = posicionInicial.Y + (fuerza * (float)Math.Sin(alpha) * tiempototal - ((gravedad * (float)Math.Pow(tiempototal, 2) / 2)));
            position.Z = posicionInicial.Z + (fuerza * (float)Math.Cos(alpha) * (float)Math.Cos(beta) * tiempototal);
            position.X = posicionInicial.X + (fuerza * (float)Math.Cos(alpha) * (float)Math.Sin(beta) * tiempototal);

            return position;
        }

        public bool hayMovimiento()
        {
            throw new NotImplementedException();
        }

        public float getFuerza()
        {
            throw new NotImplementedException();
        }
    }
}
