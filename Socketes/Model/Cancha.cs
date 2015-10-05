using AlumnoEjemplos.Socketes.Collision;
using Microsoft.DirectX;
using System.Collections.Generic;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Cancha : IRenderObject, IColisionable
    {
        private TgcBox box;
        private List<IRenderObject> tribunas;
        private List<LimiteCancha> limitesCancha;
        private bool mostrarBounding;

        public Cancha(TgcBox box, List<IRenderObject> tribunas, List<LimiteCancha> limitesCancha)
        {
            this.box = box;
            this.tribunas = tribunas;
            this.limitesCancha = limitesCancha;
        }

        public void render()
        {
            box.render();

            if (this.mostrarBounding)
            {
                box.BoundingBox.render();
            }

            foreach (IRenderObject componente in this.tribunas)
            {
                componente.render();
            }

            foreach (LimiteCancha limite in this.limitesCancha)
            {
                limite.render();
            }
        }

        public TgcBoundingBox BoundingBoxCesped
        {
            get { return this.box.BoundingBox; }
        }

        public bool AlphaBlendEnable
        {
            get
            {
                //TODO estoy asumiendo que todos tienen el mismo alphablend :P
                return this.box.AlphaBlendEnable;
            }

            set
            {
                this.box.AlphaBlendEnable = value;

                foreach (IRenderObject componente in tribunas)
                {
                    componente.AlphaBlendEnable = value;
                }
            }
        }

        public List<TgcBoundingBox> BoundingBoxes
        {
            get
            {
                List<TgcBoundingBox> obstaculos = new List<TgcBoundingBox>();

                foreach (LimiteCancha limite in this.limitesCancha)
                {
                    obstaculos.Add(limite.BoundingBox);
                }
                return obstaculos;
            }
        }

        public bool MostrarBounding
        {
            get { return mostrarBounding; }
            set
            {
                mostrarBounding = value;

                foreach (LimiteCancha limite in this.limitesCancha)
                {
                    limite.MostrarBounding = value;
                }
            }
        }

        public List<LimiteCancha> LimitesCancha
        {
            get { return limitesCancha; }
            set { limitesCancha = value; }
        }

        public void dispose()
        {
            box.dispose();

            foreach (IRenderObject componente in tribunas)
            {
                componente.dispose();
            }

            foreach (LimiteCancha limite in this.limitesCancha)
            {
                limite.dispose();
            }
        }

        public void colisionasteConPelota(Pelota pelota)
        {
            //le avisa a la pelota que esta tocando el piso, asi no intenta colisionar mas
            pelota.estasEnElPiso();
        }

        public Vector3 getDireccionDeRebote(Vector3 movimiento)
        {
            movimiento.Y *= -1;
            return movimiento;
        }

        public float getFactorDeRebote()
        {
            return 0.70f;
        }

        public TgcBoundingBox getTgcBoundingBox()
        {
            return this.BoundingBoxCesped;
        }
    }
}