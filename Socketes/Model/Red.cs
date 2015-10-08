using AlumnoEjemplos.Socketes.Collision;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Red : IColisionable
    {
        private TgcBox box;
        private bool mostrarBounding;

        public bool MostrarBounding
        {
            get { return mostrarBounding; }
            set { mostrarBounding = value; }
        }

        public Red(TgcBox box)
        {
            this.box = box;
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
            //por ahora nada, aca tendria que ir la logica de si la pelota hizo gol o no.
        }

        public Vector3 GetDireccionDeRebote(Vector3 movimiento)
        {
            //los arcos son planos parados sobre el eje X, asi q solo cambio coordenada X de movimiento.
            movimiento.Normalize();
            movimiento.X *= -1;
            return movimiento;
        }

        public float GetFuerzaRebote(Vector3 movimiento)
        {
            //factor de fuerza de rebote, hay q ver que onda estos valores.
            return 0.9f;
        }

        public TgcBoundingBox GetTgcBoundingBox()
        {
            return this.box.BoundingBox;
        }
    }
}