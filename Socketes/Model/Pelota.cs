using AlumnoEjemplos.Properties;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Reflection;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes
{
    public class Pelota
    {
        TgcSphere sphere;

        private float angulo = 0f;

        bool mostrarBounding = true;
        Vector3 rotation = new Vector3(0, 0, 0);
        float velocidadRotacion = 200;

        public TgcBoundingSphere BoundingSphere
        {
            get { return sphere.BoundingSphere; }
        }

        public Vector3 Position
        {
            get { return sphere.Position; }
            set { sphere.Position = Position; }
        }

        public bool MostrarBounding
        {
            get { return mostrarBounding; }
            set { mostrarBounding = value; }
        }

        public Pelota(TgcSphere sphere)
        {
            //apago el auto transformado, ya que la pelota lo maneja solo
            sphere.AutoTransformEnable = false;
            sphere.updateValues();

            this.sphere = sphere;
        }

        public void render()
        {
            sphere.render();

            if (mostrarBounding)
            {
                sphere.BoundingSphere.render();
            }
        }

        public void updateValues()
        {
            sphere.updateValues();
        }

        /// <summary>
        /// Mueve la pelota hacia el punto indicado, 
        /// el movimiento hacia ese punto es lineal, 
        /// en base a ese movimiento tambien hace la rotacion de la pelota
        /// </summary>
        public void mover(Vector3 movimiento, float elapsedTime)
        {
            //valido que haya movimiento, sacar despues de que no se llame todo el tiempo
            if (movimiento.X != 0 || movimiento.Y != 0 || movimiento.Z != 0)
            {
                //Muevo la pelota hacia el punto dado
                sphere.move(movimiento);
                //arma la transformacion en base a el escalado + rotacion + traslacion
                sphere.Transform = getScalingMatrix() * getRotationMatrix(movimiento, elapsedTime) * Matrix.Translation(sphere.Position);
            }
        }

        /// <summary>
        /// Matriz de escalado en base al radio de la esfera
        /// </summary>
        /// <returns></returns>
        private Matrix getScalingMatrix()
        {
            return Matrix.Scaling(sphere.Radius, sphere.Radius, sphere.Radius);
        }

        /// <summary>
        /// Retorna la matriz de rotacion en base al movimiento dado
        /// </summary>
        /// <param name="movimiento"></param>
        /// <param name="elapsedTime"></param>
        /// <returns></returns>
        /// <summary>
        /// Retorna la matriz de rotacion en base al movimiento dado
        /// </summary>
        /// <param name="movimiento"></param>
        /// <param name="elapsedTime"></param>
        /// <returns></returns>
        private Matrix getRotationMatrix(Vector3 movimiento, float elapsedTime)
        {
            angulo += Geometry.DegreeToRadian(velocidadRotacion * elapsedTime);
            return Matrix.RotationAxis(getVectorRotacion(movimiento), angulo);
            ;

        }

        /// <summary>
        /// Retorna el vector de rotacion en base a la direccion de movimiento
        /// 
        /// </summary>
        /// <param name="movimiento"></param>
        /// <returns></returns>
        private Vector3 getVectorRotacion(Vector3 movimiento)
        {
            Vector3 vectorrotacion = new Vector3(0, 0, 0);
            if (movimiento.Y != 0)
            {
                vectorrotacion.X = Math.Sign(movimiento.Y);
            }

            if (movimiento.Z != 0)
            {
                vectorrotacion.X = Math.Sign(movimiento.Z);
            }

            if (movimiento.X != 0)
            {
                vectorrotacion.Z = -Math.Sign(movimiento.X);
            }

            if (movimiento.Z != 0 && movimiento.X != 0)
            {
                vectorrotacion.X = 0.7074f / Math.Sign(movimiento.Z);
                vectorrotacion.Z = 0.7074f / -Math.Sign(movimiento.X);
            }

            return vectorrotacion;
        }

        internal void dispose()
        {
            sphere.dispose();
        }
    }
}