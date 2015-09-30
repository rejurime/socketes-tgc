using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;
using AlumnoEjemplos.Socketes.Collision;
using System;

namespace AlumnoEjemplos.Socketes.Model
{

    public class Jugador : IRenderObject, Colisionable
    {
        private TgcSkeletalMesh skeletalMesh;
        private float velocidadCaminar = 200f;
        private float velocidadCorrer = 500f;

        private Jugador() { }

        public Jugador(TgcSkeletalMesh skeletalMesh)
        {
            this.skeletalMesh = skeletalMesh;
        }

        public TgcSkeletalMesh SkeletalMesh
        {
            get { return skeletalMesh; }
        }

        public Vector3 Position
        {
            get { return this.skeletalMesh.Position; }
            set { this.skeletalMesh.Position = value; }
        }

        public TgcBoundingBox BoundingBox
        {
            get { return this.skeletalMesh.BoundingBox; }
            set { this.skeletalMesh.BoundingBox = value; }
        }

        public Vector3 Rotation
        {
            get { return this.SkeletalMesh.Rotation; }
            set { this.SkeletalMesh.Rotation = value; }
        }

        public float VelocidadCaminar
        {
            get { return velocidadCaminar; }
            set { velocidadCaminar = value; }
        }

        public float VelocidadCorrer
        {
            get { return velocidadCorrer; }
            set { velocidadCorrer = value; }
        }

        public bool AlphaBlendEnable
        {
            get
            {
                return this.skeletalMesh.AlphaBlendEnable;
            }

            set
            {
                this.skeletalMesh.AlphaBlendEnable = value;
            }
        }

        public void playAnimation(string animacion, bool v)
        {
            this.skeletalMesh.playAnimation(animacion, v);
        }

        public void move(Vector3 movimiento)
        {
            this.skeletalMesh.move(movimiento);
        }

        public void animateAndRender()
        {
            this.skeletalMesh.animateAndRender();
        }

        public void render()
        {
            this.skeletalMesh.render();
        }

        public void dispose()
        {
            this.skeletalMesh.dispose();
        }

        public void colisionasteCon(Colisionable objetoColisionado)
        {
            throw new NotImplementedException();
        }

        public Vector3 getDireccionDeRebote(Vector3 vectorDeImpacto)
        {
            throw new NotImplementedException();
        }

        public float getFactorDeRebote()
        {
            throw new NotImplementedException();
        }

        public TgcBoundingBox getTgcBoundingBox()
        {
            return this.skeletalMesh.BoundingBox; 
        }
    }
}