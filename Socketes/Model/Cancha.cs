using System.Collections.Generic;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Cancha : IRenderObject
    {
        TgcBox box;
        List<IRenderObject> componentes;

        public Cancha(TgcBox box, List<IRenderObject> componentes)
        {
            this.box = box;
            this.componentes = componentes;
        }

        public void render()
        {
            box.render();
            //box.BoundingBox.render();

            foreach (IRenderObject componente in componentes)
            {
                componente.render();
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

                foreach (IRenderObject componente in componentes)
                {
                    componente.AlphaBlendEnable = value;
                }
            }
        }

        public void dispose()
        {
            box.dispose();

            foreach (IRenderObject componente in componentes)
            {
                componente.dispose();
            }
        }
    }
}