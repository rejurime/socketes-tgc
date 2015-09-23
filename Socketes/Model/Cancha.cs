using AlumnoEjemplos.Properties;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Cancha
    {
        TgcBox caja;

        public Cancha(string pathRecursos)
        {
            TgcTexture pisoTexture = TgcTexture.createTexture(pathRecursos + Settings.Default.textureFolder + Settings.Default.textureField);
            this.caja = TgcBox.fromSize(new Vector3(0, -10, 0), new Vector3(1920, 0, 1200), pisoTexture);
        }

        public TgcBoundingBox BoundingBox
        {
            get { return this.caja.BoundingBox; }
        }

        internal void render()
        {
            this.caja.render();
        }

        internal void dispose()
        {
            this.caja.dispose();
        }
    }
}