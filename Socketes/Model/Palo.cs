﻿using System;
using AlumnoEjemplos.Socketes.Collision;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Palo : IColisionable
    {
        private TgcMesh mesh;
        private bool mostrarBounding;

        public Palo(TgcMesh mesh)
        {
            this.mesh = mesh;
        }

        public TgcBoundingBox BoundingBox
        {
            get { return this.mesh.BoundingBox; }
        }

        public bool MostrarBounding
        {
            get { return mostrarBounding; }
            set { mostrarBounding = value; }
        }

        public void ColisionasteConPelota(Pelota pelota)
        {
            //por ahora nada, aca tendria que hacer ruido de palo.
        }

        public Vector3 GetDireccionDeRebote(Vector3 movimiento)
        {
            //los arcos son planos parados sobre el eje X, asi q solo cambio coordenada X de movimiento.
            movimiento.Normalize();
            movimiento.X *= -1;
            return movimiento;
        }

        public float GetFuerzaRebote(Vector3 movimiento)
        {
            //factor de fuerza de rebote, hay q ver que onda estos valores.
            return 0.9f;
        }

        public void render()
        {
            this.mesh.render();

            if (this.MostrarBounding)
            {
                this.mesh.BoundingBox.render();
            }
        }

        public void dispose()
        {
            this.mesh.dispose();
        }

        public TgcBoundingBox GetTgcBoundingBox()
        {
            return this.BoundingBox;
        }
    }
}