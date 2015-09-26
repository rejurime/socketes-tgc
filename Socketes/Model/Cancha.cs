using System;
using System.Collections.Generic;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Cancha
    {
        TgcBox box;
        List<IRenderObject> componentes;

        public Cancha(TgcBox box, List<IRenderObject> componentes)
        {
            this.box = box;
            this.componentes = componentes;
        }

        internal void render()
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

        internal void dispose()
        {
            box.dispose();

            foreach (IRenderObject componente in componentes)
            {
                componente.dispose();
            }
        }
    }
}