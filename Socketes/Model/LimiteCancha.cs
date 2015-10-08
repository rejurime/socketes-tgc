using AlumnoEjemplos.Socketes.Collision;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.Socketes.Model
{
    public class LimiteCancha : IColisionable
    {
        private TgcBox box;
        private bool mostrarBounding;

        public LimiteCancha(TgcBox box)
        {
            this.box = box;
        }

        public TgcBoundingBox BoundingBox
        {
            get { return this.box.BoundingBox; }
        }

        public bool MostrarBounding
        {
            get { return mostrarBounding; }
            set { mostrarBounding = value; }
        }

        public void ColisionasteConPelota(Pelota pelota)
        {
            //TODO Avisar al partido que se fue la pelota
        }

        public void dispose()
        {
            this.box.dispose();
        }

        public Vector3 GetDireccionDeRebote(Vector3 movimiento)
        {
            //TODO Ver que hacer jeje
            movimiento.Y *= -1;
            return movimiento;
        }

        public float GetFuerzaRebote(Vector3 movimiento)
        {
            return 0.1f;
        }

        public TgcBoundingBox GetTgcBoundingBox()
        {
            return this.BoundingBox;
        }

        public void render()
        {
            if (this.MostrarBounding)
            {
                this.box.BoundingBox.render();
            }
        }
    }
}