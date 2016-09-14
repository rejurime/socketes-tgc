using AlumnoEjemplos.Socketes.Model.Colision;
using AlumnoEjemplos.Socketes.Model.Iluminacion;
using Microsoft.DirectX;
using System.Collections.Generic;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;
using TGC.Core.BoundingVolumes;

namespace AlumnoEjemplos.Socketes.Model.ElementosCancha
{
	public class Cancha : IColisionablePelota
    {
        #region Miembros

        private TgcBox box;
        private List<IRenderObject> componentesEstaticos;
        private List<LimiteCancha> limitesCancha;
        private List<Luz> luces;
        private List<TgcBox> carteles;
        private bool mostrarBounding;
        private float time;

        #endregion

        #region  Constructores

        public Cancha(TgcBox box, List<IRenderObject> componentes, List<LimiteCancha> limitesCancha, List<Luz> luces, List<TgcBox> carteles)
        {
            this.box = box;
            this.componentesEstaticos = componentes;
            this.limitesCancha = limitesCancha;
            this.Luces = luces;
            this.carteles = carteles;
            this.time = 0;
        }

        #endregion

        #region Propiedades

        public TgcBoundingAxisAlignBox BoundingBoxCesped
        {
            get { return this.box.BoundingBox; }
        }

        public List<TgcBoundingAxisAlignBox> BoundingBoxes
        {
            get
            {
                List<TgcBoundingAxisAlignBox> obstaculos = new List<TgcBoundingAxisAlignBox>();

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

        public List<Luz> Luces
        {
            get { return luces; }
            set { luces = value; }
        }

        public Vector3 Size
        {
            get { return this.box.Size; }
        }

        public Vector3 Position
        {
            get { return this.box.Position; }
        }

        #endregion

        #region Metodos

        public void render(float elapsedTime)
        {
            this.time += elapsedTime;

            box.render();

            if (this.mostrarBounding)
            {
                box.BoundingBox.render();
            }

            foreach (IRenderObject componente in this.componentesEstaticos)
            {
                componente.render();
            }

            foreach (LimiteCancha limite in this.limitesCancha)
            {
                limite.render();
            }

            foreach (Luz lux in this.luces)
            {
                lux.render();
            }

            foreach (TgcBox cartel in this.carteles)
            {
                cartel.Effect.SetValue("time", time);
                cartel.render();
            }
        }

        public void dispose()
        {
            box.dispose();

            foreach (IRenderObject componente in componentesEstaticos)
            {
                componente.dispose();
            }

            foreach (LimiteCancha limite in this.limitesCancha)
            {
                limite.dispose();
            }
        }

        public void ColisionasteConPelota(Pelota pelota)
        {
            //TODO feo que este vacio este metodo pero la Pelota no hace nada con la cancha
        }

        public Vector3 GetDireccionDeRebote(Vector3 movimiento)
        {
            movimiento.Normalize();
            movimiento.Y *= -1;
            return movimiento;
        }

        public float GetFuerzaRebote(Vector3 movimiento, float fuerzaRestante)
        {
            return 0.9f * fuerzaRestante;
        }

        public TgcBoundingAxisAlignBox GetTgcBoundingBox()
        {
            return this.BoundingBoxCesped;
        }

        public virtual string toString()
        {
            return "Cancha";
        }

        #endregion
    }
}