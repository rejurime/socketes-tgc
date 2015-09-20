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

        public Pelota(Device d3dDevice)
        {
            //no se si sirve para algo, por ahora la pelota se crea y se guarda el device
            this.d3dDevice = GuiController.Instance.D3dDevice;

            TgcTexture texture = TgcTexture.createTexture(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Textures\\Metal\\OM106-SO.jpg");

            
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
            sphere.BoundingSphere.render();
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

        public void move(Vector3 vector)
        {

            sphere.move(vector);
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
