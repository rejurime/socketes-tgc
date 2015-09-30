using AlumnoEjemplos.Socketes.Collision;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Arco : IRenderObject, Colisionable
    {
        public TgcMesh mesh;

        public bool mostrarBounding = true;

        public TgcBoundingBox BoundingBox
        {
            get { return this.mesh.BoundingBox; }
        }

        public bool AlphaBlendEnable
        {
            get
            {
                return this.mesh.AlphaBlendEnable;
            }

            set
            {
                this.mesh.AlphaBlendEnable = value;
            }
        }

        private Arco() { }

        public Arco(TgcMesh arco)
        {
            this.mesh = arco;
        }

        public void render()
        {
            this.mesh.render();

            if (mostrarBounding)
            {
                this.mesh.BoundingBox.render();
            }
        }

        public void dispose()
        {
            this.mesh.dispose();
        }

        public void colisionasteCon(Colisionable objetoColisionado)
        {
            throw new System.NotImplementedException();
        }

        public Vector3 getDireccionDeRebote(Vector3 vectorDeImpacto)
        {
            throw new System.NotImplementedException();
        }

        public float getFactorDeRebote()
        {
            throw new System.NotImplementedException();
        }

        public TgcBoundingBox getTgcBoundingBox()
        {
            return this.mesh.BoundingBox;
        }
    }
}