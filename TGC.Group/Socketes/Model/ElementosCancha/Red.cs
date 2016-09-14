using AlumnoEjemplos.Socketes.Model.Colision;
using Microsoft.DirectX;
using TGC.Core.BoundingVolumes;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;

namespace AlumnoEjemplos.Socketes.Model.ElementosCancha
{
	public class Red : IColisionablePelota
    {
        private TgcBox box;
        private TgcMesh red;
        private bool mostrarBounding;

        public Red(TgcBox box, TgcMesh red)
        {
            this.box = box;
            this.red = red;
        }

        public bool MostrarBounding
        {
            get { return mostrarBounding; }
            set { mostrarBounding = value; }
        }

        public void render()
        {
            //this.box.render();
            this.red.render();

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

        public TgcBoundingAxisAlignBox GetTgcBoundingBox()
        {
            return this.box.BoundingBox;
        }

        public Vector3 GetPosition()
        {
            return this.box.Position;
        }
    }
}