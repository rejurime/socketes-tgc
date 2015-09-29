using System.Collections.Generic;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Cancha : IRenderObject
    {
        TgcBox box;
        List<IRenderObject> tribunas;
        List<TgcBox> limitesCancha;

        public Cancha(TgcBox box, List<IRenderObject> componentes, List<TgcBox> limitesCancha)
        {
            this.box = box;
            this.tribunas = componentes;
            this.limitesCancha = limitesCancha;
        }

        public void render()
        {
            box.render();
            //box.BoundingBox.render();

            foreach (IRenderObject componente in this.tribunas)
            {
                componente.render();
            }

            foreach (TgcBox limite in this.limitesCancha)
            {
                limite.BoundingBox.render();
            }
        }

        public TgcBoundingBox BoundingBoxCesped
        {
            get
            {
                return this.box.BoundingBox;
            }
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

                foreach (TgcBox box in this.limitesCancha)
                {
                    obstaculos.Add(box.BoundingBox);
                }
                return obstaculos;
            }
        }

        public void dispose()
        {
            box.dispose();

            foreach (IRenderObject componente in tribunas)
            {
                componente.dispose();
            }

            foreach (TgcBox limite in this.limitesCancha)
            {
                limite.dispose();
            }
        }
    }
}