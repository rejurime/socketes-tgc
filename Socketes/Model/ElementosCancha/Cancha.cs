using AlumnoEjemplos.Socketes.Model.Colision;
using Microsoft.DirectX;
using System.Collections.Generic;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using System;

namespace AlumnoEjemplos.Socketes.Model.ElementosCancha
{
    public class Cancha : IColisionablePelota
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

        public Vector3 Tamano()
        {
            return this.box.Size;
        }

        public void ColisionasteConPelota(Pelota pelota)
        {
            //TODO feo que este vacio este metodo pero la Pelota no hace nada con la cancha
        }

        public Vector3 GetDireccionDeRebote(Vector3 movimiento)
        {
            movimiento.Normalize();
            movimiento.Y = 1;
            return movimiento;
        }

        public float GetFuerzaRebote(Vector3 movimiento, float fuerzaRestante)
        {
            return 0.9f * fuerzaRestante;
        }

        public TgcBoundingBox GetTgcBoundingBox()
        {
            return this.BoundingBoxCesped;
        }

        public virtual string toString()
        {
            return "Cancha";
        }
    }
}