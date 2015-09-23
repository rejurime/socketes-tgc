using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Arco
    {
        public TgcMesh mesh;

        public bool mostrarBounding = true;

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
    }
}