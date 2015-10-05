using AlumnoEjemplos.Socketes.Collision;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Arco : IRenderObject, IColisionable
    {
        private TgcMesh mesh;
        private bool mostrarBounding;

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

        public bool MostrarBounding
        {
            get { return mostrarBounding; }
            set { mostrarBounding = value; }
        }

        private Arco() { }

        public Arco(TgcMesh arco)
        {
            this.mesh = arco;
        }

        public void render()
        {
            this.mesh.render();

            if (this.mostrarBounding)
            {
                this.mesh.BoundingBox.render();
            }
        }

        public void dispose()
        {
            this.mesh.dispose();
        }

        public void colisionasteConPelota(Pelota pelota)
        {
            //por ahora nada, aca tendria que ir la logica de si la pelota hizo gol o no.
        }

        public Vector3 getDireccionDeRebote(Vector3 movimiento)
        {
            //los arcos son planos parados sobre el eje X, asi q solo cambio coordenada X de movimiento.
            movimiento.X *= -1;
            return movimiento;
        }

        public float getFactorDeRebote()
        {
            //factor de fuerza de rebote, hay q ver que onda estos valores.
            return 0.50f;
        }

        public TgcBoundingBox getTgcBoundingBox()
        {
            return this.mesh.BoundingBox;
        }
    }
}