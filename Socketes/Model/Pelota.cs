using AlumnoEjemplos.Properties;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Reflection;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.Socketes.Model;
using AlumnoEjemplos.Socketes.Fisica;
using TgcViewer;
using AlumnoEjemplos.Socketes.Collision.SphereCollision;

namespace AlumnoEjemplos.Socketes
{
    public class Pelota
    {
        TgcSphere sphere;

        private float angulo = 0f;

        bool mostrarBounding = true;

        private Tiro tiro;
        
        private float coeficienteRotacion = 60;
        
        public CollisionManager collisionManager;

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

        public void updateValues(float elapsedTime)
        {
            if (tiro != null && tiro.hayMovimiento())
            {
                Vector3 siguientespoisicion = tiro.siguienteMovimiento(elapsedTime);
                mover(siguientespoisicion, elapsedTime, tiro.getFuerza());
            }
            else
            {
                //llamo a mover para que la pelota caiga por gravedad si no hay movimiento
                mover(new Vector3(0, 0, 0), elapsedTime, 2);
            }
            sphere.updateValues();

        }


        /// <summary>
        /// Metodo para patear una pelota, recibe el vector de direccion y la fuerza.
        ///
        /// </summary>
        /// <param name="direccion"></param>
        /// <param name="fuerza"></param>
        public void patear(Vector3 direccion, float fuerza)
        {
            tiro = new TiroParabolicoSimple(direccion, fuerza);
        }
        /// <summary>
        /// Mueve la pelota hacia el punto indicado, 
        /// el movimiento hacia ese punto es lineal, 
        /// en base a ese movimiento tambien hace la rotacion de la pelota
        /// </summary>
        public void mover(Vector3 movimiento, float elapsedTime, float velocidadRotacion)
        {
            Vector3 realMovement = collisionManager.moveCharacter(sphere.BoundingSphere, movimiento);
            //valido que haya movimiento, sacar despues de que no se llame todo el tiempo
            if (realMovement.X != 0 || realMovement.Y != 0 || realMovement.Z != 0)
            {
                sphere.move(realMovement);

                //arma la transformacion en base a el escalado + rotacion + traslacion
                sphere.Transform = getScalingMatrix() *
                    getRotationMatrix(realMovement, elapsedTime, velocidadRotacion) * 
                    Matrix.Translation(sphere.Position);
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
        private Matrix getRotationMatrix(Vector3 movimiento, float elapsedTime, float velocidadRotacion)
        {
            angulo += Geometry.DegreeToRadian(velocidadRotacion * coeficienteRotacion * elapsedTime);
            Matrix matrixrotacion = Matrix.RotationAxis(getVectorRotacion(movimiento), angulo);
            return matrixrotacion;
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

            //solo roto en Y si la pelota esta cayendo, si se esta moviendo en Z o X entonces no hago nada
            if (movimiento.Y != 0 && movimiento.Z == 0 && movimiento.X == 0)
            {
                //caundo cae roto en un solo sentido
                vectorrotacion.X = -1;
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