using AlumnoEjemplos.Socketes.Collision;
using System;
using System.Collections.Generic;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Cancha : Colisionable
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

        public void colisionasteCon(Colisionable objetoColisionado)
        {
            throw new NotImplementedException();
        }

        public Microsoft.DirectX.Vector3 getDireccionDeRebote(Microsoft.DirectX.Vector3 vectorDeImpacto)
        {
            throw new NotImplementedException();
        }

        public float getFactorDeRebote()
        {
            throw new NotImplementedException();
        }

        public TgcBoundingBox getTgcBoundingBox()
        {
            return this.box.BoundingBox;
        }
    }
}