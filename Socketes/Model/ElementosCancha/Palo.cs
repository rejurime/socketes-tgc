using AlumnoEjemplos.Socketes.Model.Colision;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model.ElementosCancha
{
    public class Palo : IColisionablePelota
    {
        private TgcMesh mesh;
        private bool mostrarBounding;

        public Palo(TgcMesh mesh)
        {
            this.mesh = mesh;
        }

        public TgcBoundingBox BoundingBox
        {
            get { return this.mesh.BoundingBox; }
        }

        public bool MostrarBounding
        {
            get { return mostrarBounding; }
            set { mostrarBounding = value; }
        }

        public void render()
        {
            this.mesh.render();

            if (this.MostrarBounding)
            {
                this.mesh.BoundingBox.render();
            }
        }

        public void dispose()
        {
            this.mesh.dispose();
        }

        public void ColisionasteConPelota(Pelota pelota)
        {
            //por ahora nada, aca tendria que hacer ruido de palo.
        }

        public Vector3 GetDireccionDeRebote(Vector3 movimiento)
        {
            //los arcos son planos parados sobre el eje X, asi q solo cambio coordenada X de movimiento.
            movimiento.Normalize();
            if (Partido.Instance.Pelota.Position.Y > BoundingBox.PMax.Y - Partido.Instance.Pelota.Diametro / 2)
            {
                movimiento.Y = 1;
            }

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
            return this.BoundingBox;
        }
    }
}