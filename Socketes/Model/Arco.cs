using AlumnoEjemplos.Socketes.Collision;
using Microsoft.DirectX;
using System.Collections.Generic;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Arco
    {
        private List<TgcMesh> palos;
        private bool mostrarBounding;

        public bool MostrarBounding
        {
            get { return mostrarBounding; }
            set { mostrarBounding = value; }
        }

        public Arco(List<TgcMesh> palos)
        {
            this.palos = palos;
        }

        public void render()
        {
            foreach (TgcMesh palo in this.palos)
            {
                palo.render();

                if (this.mostrarBounding)
                {
                    palo.BoundingBox.render();
                }
            }
        }

        public void dispose()
        {
            foreach (TgcMesh palo in this.palos)
            {
                palo.dispose();
            }
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
    }
}