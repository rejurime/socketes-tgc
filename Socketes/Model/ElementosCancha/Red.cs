using AlumnoEjemplos.Socketes.Model.Colision;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.Socketes.Model.ElementosCancha
{
    public class Red : IColisionablePelota
    {
        private TgcBox box;
        private bool mostrarBounding;

        public Red(TgcBox box)
        {
            this.box = box;
        }

        public bool MostrarBounding
        {
            get { return mostrarBounding; }
            set { mostrarBounding = value; }
        }

        public void render()
        {
            //this.box.render();

            if (this.mostrarBounding)
            {
                this.box.BoundingBox.render();
            }
        }

        public void dispose()
        {
            this.box.dispose();
        }

        public void ColisionasteConPelota(Pelota pelota)
        {
            Partido.Instance.NotificarGol(this);
        }

        public Vector3 GetDireccionDeRebote(Vector3 movimiento)
        {
            //los arcos son planos parados sobre el eje X, asi q solo cambio coordenada X de movimiento.
            movimiento.Normalize();
            movimiento.X *= -1;
            return movimiento;
        }

        public float GetFuerzaRebote(Vector3 movimiento, float fuerzaRestante)
        {
            //factor de fuerza de rebote, hay q ver que onda estos valores.
            return 0.9f * fuerzaRestante;
        }

        public TgcBoundingBox GetTgcBoundingBox()
        {
            return this.box.BoundingBox;
        }

        public Vector3 GetPosition()
        {
            return this.box.Position;
        }
    }
}