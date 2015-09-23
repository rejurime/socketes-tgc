using AlumnoEjemplos.Properties;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Reflection;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes
{
    public class Pelota
    {
        TgcSphere sphere;

        bool mostrarBounding = true;
        bool mostrarNormal = true;
        TgcArrow normal = new TgcArrow();
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

        public bool MostrarNormal
        {
            get { return mostrarNormal; }
            set { mostrarNormal = value; }
        }

        public Pelota(string pathRecursos)
        {
            TgcTexture texture = TgcTexture.createTexture(pathRecursos + Settings.Default.textureFolder + Settings.Default.textureBall);

            //Crear esfera
            sphere = new TgcSphere();
            sphere.Radius = 10;
            sphere.setTexture(texture);
            sphere.Position = new Vector3(0, 90, 0);
            //apago el auto transformado, ya que la pelota lo maneja solo
            sphere.AutoTransformEnable = false;
            sphere.updateValues();
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
        private Matrix getRotationMatrix(Vector3 movimiento, float elapsedTime)
        {
            float angulo = Geometry.DegreeToRadian(velocidadRotacion * elapsedTime);
            Matrix mrot = Matrix.Identity;

            //TO DO mejorar if feos
            //TO DO Ver diagonal
            if (movimiento.X != 0)
            {
                if (movimiento.X > 0)
                    rotation.Z += -angulo;
                else
                    rotation.Z += angulo;
                mrot *= Matrix.RotationZ(rotation.Z);
            }

            if (movimiento.Y != 0)
            {
                if (movimiento.Y > 0)
                    rotation.X += angulo;
                else
                    rotation.X += -angulo;
                mrot *= Matrix.RotationX(rotation.X);
            }

            if (movimiento.Z != 0)
            {
                if (movimiento.Z > 0)
                    rotation.X += angulo;

                else
                    rotation.X += -angulo;
                mrot *= Matrix.RotationX(rotation.X);
            }

            return mrot;
        }

        internal void dispose()
        {
            sphere.dispose();
        }
    }
}