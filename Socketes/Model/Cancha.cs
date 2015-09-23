using AlumnoEjemplos.Properties;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Cancha
    {
        TgcBox box;

        public Cancha(TgcBox box)
        {
            this.box = box;
        }

        public TgcBoundingBox BoundingBox
        {
            get { return this.box.BoundingBox; }
        }

        internal void render()
        {
            this.box.render();
        }

        internal void dispose()
        {
            this.box.dispose();
        }
    }
}