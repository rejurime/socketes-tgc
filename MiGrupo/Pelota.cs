using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MiGrupo
{
    class Pelota
    {
        TgcSphere sphere;
        Device d3dDevice;
        float velocidadRotacion = 200;

        public Pelota(Device d3dDevice)
        {
            //no se si sirve para algo, por ahora la pelota se crea y se guarda el device
            this.d3dDevice = GuiController.Instance.D3dDevice;

            TgcTexture texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Texturas\\pelota.jpg");

            
            //se definen configuracion globlales de la pelota fea
            GuiController.Instance.Modifiers.addInterval("level of detail", new object[] { 0, 1, 2, 3, 4 }, 2);
            
            sphere = new TgcSphere();
            sphere.Radius =     10;
            sphere.setTexture(texture);
            sphere.Position =  new Vector3(0, 30, 0);
            sphere.LevelOfDetail = (int)GuiController.Instance.Modifiers["level of detail"];

        }

        
        public void render(float elapsedTime)
        {
            s
            sphere.render();

        }

        public void updateValues()
        {
            sphere.updateValues();

        }

        public Vector3 Position
        {
            get { return sphere.Position; }
            set
            {
                sphere.Position = Position;
            }
        }

        /// <summary>
        /// Mueve la pelota hacia el punto indicado, 
        /// el movimiento hacia ese punto es lineal, 
        /// en base a ese movimiento tambien hace la rotacion de la pelota
        /// </summary>
        public void mover(Vector3 vector, float elapsedTime)
        {
            rotarSimple(vector, elapsedTime);
            sphere.move(vector);
        }

        private void rotarSimple(Vector3 movimiento, float elapsedTime)
        {
            //TO DO la rotacion sobre X se caga, porque la camara se mentiene estable en en Z y se mueve en X, esto hace que la rotacion no sea correcta, no tengo idea como arregarlo xD

            if (movimiento.X != 0)
            {
                float rotAngle = Geometry.DegreeToRadian(velocidadRotacion * elapsedTime * movimiento.X);
                sphere.rotateZ(-rotAngle);
            }
            
            //la rotacion sobre Y solo se da cuando la pelota esta en el aire, ahi la rotacion puede ser en cualquier eje, ya que no esta sobre el piso
            if (movimiento.Y != 0)
            {
                float rotAngle = Geometry.DegreeToRadian(velocidadRotacion * elapsedTime * movimiento.Y);
                sphere.rotateX(-rotAngle);
            }

            if (movimiento.Z != 0)
            {
                float rotAngle = Geometry.DegreeToRadian(velocidadRotacion * elapsedTime * movimiento.Z);
                sphere.rotateX(rotAngle);
            }
            
            
            
        } 

        internal void dispose()
        {
            sphere.dispose();
        }

        internal void rotateY(float rotAngle)
        {
            sphere.rotateY(rotAngle);
        }

        public TgcBoundingSphere BoundingSphere { get {return sphere.BoundingSphere;} set {} }
    }
}
