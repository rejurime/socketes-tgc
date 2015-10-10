using AlumnoEjemplos.Socketes.Model.Colision;
using System.Collections.Generic;

namespace AlumnoEjemplos.Socketes.Model.ElementosCancha
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

        public Red Red
        {
            get { return red; }
            set { red = value; }
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

        public List<IColisionablePelota> GetColisionables()
        {
            List<IColisionablePelota> colisionables = new List<IColisionablePelota>();

            foreach (Palo palo in this.palos)
            {
                colisionables.Add(palo);
            }

            colisionables.Add(this.red);

            return colisionables;
        }
    }
}