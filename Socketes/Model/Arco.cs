using AlumnoEjemplos.Socketes.Collision;
using Microsoft.DirectX;
using System.Collections.Generic;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Arco
    {
        private List<Palo> palos;
        private Red red;
        private bool mostrarBounding;

        public bool MostrarBounding
        {
            get { return mostrarBounding; }
            set
            {
                mostrarBounding = value;

                foreach (Palo palo in this.palos)
                {
                    palo.MostrarBounding = value;
                }

                this.red.MostrarBounding = value;
            }
        }

        public Arco(List<Palo> palos, Red red)
        {
            this.palos = palos;
            this.red = red;
        }

        public void render()
        {
            foreach (Palo palo in this.palos)
            {
                palo.render();
            }

            this.red.render();
        }

        public void dispose()
        {
            foreach (Palo palo in this.palos)
            {
                palo.dispose();
            }

            this.red.dispose();
        }

        public List<IColisionable> GetColisionables()
        {
            List<IColisionable> colisionables = new List<IColisionable>();

            foreach (Palo palo in this.palos)
            {
                colisionables.Add(palo);
            }

            colisionables.Add(this.red);

            return colisionables;
        }
    }
}