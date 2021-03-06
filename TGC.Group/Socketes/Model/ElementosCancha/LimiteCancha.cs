﻿using AlumnoEjemplos.Socketes.Model.Colision;
using Microsoft.DirectX;
using TGC.Core.BoundingVolumes;
using TGC.Core.Geometry;

namespace AlumnoEjemplos.Socketes.Model.ElementosCancha
{
	public class LimiteCancha : IColisionablePelota
    {
        private TgcBox box;
        private bool mostrarBounding;

        public LimiteCancha(TgcBox box)
        {
            this.box = box;
			//TODO cambiar por matrices
			this.box.AutoTransformEnable = true;
        }

        public TgcBoundingAxisAlignBox BoundingBox
        {
            get { return this.box.BoundingBox; }
        }

        public bool MostrarBounding
        {
            get { return mostrarBounding; }
            set { mostrarBounding = value; }
        }

        public void ColisionasteConPelota(Pelota pelota)
        {
            //TODO Avisar al partido que se fue la pelota
        }

        public void render()
        {
            //this.box.render();

            if (this.MostrarBounding)
            {
                this.box.BoundingBox.render();
            }
        }

        public void dispose()
        {
            this.box.dispose();
        }

        public Vector3 GetDireccionDeRebote(Vector3 movimiento)
        {
            //TODO Ver que hacer jeje
            if (box.Size.Z == 0)
            {
                movimiento.Z *= -1;
            }

            if (box.Size.X == 0)
            {
                movimiento.X *= -1;
            }

            movimiento.Normalize();
            return movimiento;
        }

        public float GetFuerzaRebote(Vector3 movimiento, float fuerzaRestante)
        {
            return 0.98f * fuerzaRestante;
        }

        public TgcBoundingAxisAlignBox GetTgcBoundingBox()
        {
            return this.BoundingBox;
        }
    }
}