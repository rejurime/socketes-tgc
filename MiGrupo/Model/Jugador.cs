using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSkeletalAnimation;

namespace AlumnoEjemplos.MiGrupo.Model
{
    public class Jugador
    {
        TgcSkeletalMesh skeletalMesh;
        float velocidadCaminar = 200f;
        float velocidadCorrer = 300f;
        float velocidadRotacion = 150f;

        private Jugador() { }

        public Jugador(TgcSkeletalMesh skeletalMesh)
        {
            this.skeletalMesh = skeletalMesh;
        }

        public TgcSkeletalMesh SkeletalMesh
        {
            get { return skeletalMesh; }
        }

        public float VelocidadCaminar
        {
            get { return velocidadCaminar; }
            set { velocidadCaminar = value; }
        }

        public float VelocidadRotacion
        {
            get { return velocidadRotacion; }
            set { velocidadRotacion = value; }
        }

        internal void animateAndRender()
        {
            this.skeletalMesh.animateAndRender();
        }

        internal void dispose()
        {
            this.skeletalMesh.dispose();
        }

        internal void rotateY(float rotAngle)
        {
            this.skeletalMesh.rotateY(rotAngle);
        }

        internal void playAnimation(string animacion, bool v)
        {
            this.skeletalMesh.playAnimation(animacion, v);
        }

        internal void moveOrientedY(float v)
        {
            this.skeletalMesh.moveOrientedY(v);
        }

        internal Vector3 Position
        {
            get { return this.skeletalMesh.Position; }
            set { this.skeletalMesh.Position = value; }
        }

        internal TgcBoundingBox BoundingBox
        {
            get { return this.skeletalMesh.BoundingBox; }
            set { this.skeletalMesh.BoundingBox = value; }
        }
    }
}