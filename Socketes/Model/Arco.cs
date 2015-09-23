using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Arco
    {
        public TgcMesh arco;
        public TgcBoundingBox boundingBoxArco;

        private Arco() { }

        public Arco(Vector3 posicion, string pathRecursos, TgcTexture texturaArco)
        {
            //Cargar modelos para el arco
            this.arco = new TgcSceneLoader().loadSceneFromFile(pathRecursos + "Arco\\arco-TgcScene.xml").Meshes[0];
            this.arco.changeDiffuseMaps(new TgcTexture[] { texturaArco });
            this.arco.AutoUpdateBoundingBox = true;
            this.arco.Position = posicion;
            this.arco.Scale = new Vector3(1.25f, 1.25f, 1.25f);
            this.arco.updateBoundingBox();
            this.boundingBoxArco = arco.BoundingBox;
        }

        public void render()
        {
            this.arco.render();
            this.boundingBoxArco.render();
        }

        public void dispose()
        {
            this.arco.dispose();
        }
    }
}