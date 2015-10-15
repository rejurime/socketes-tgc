﻿using AlumnoEjemplos.Socketes.Model.Colision;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;

namespace AlumnoEjemplos.Socketes.Model.Jugadores
{
    public class Jugador : IRenderObject, IColisionablePelota
    {
        #region Miembros

        private Vector3 posicionOriginal;
        private TgcSkeletalMesh skeletalMesh;
        private float velocidadCaminar = 100f;
        private float velocidadCorrer = 500f;
        private IJugadorMoveStrategy strategy;
        private Pelota pelota;
        private Equipo equipoPropio;
        private BoxCollisionManager collisionManager;
        private string animacionCorriendo = Settings.Default.animationRunPlayer;
        private string animacionCaminando = Settings.Default.animationWalkPlayer;
        private string animacionParado = Settings.Default.animationStopPlayer;
        private bool mostrarBounding;
        //TODO ver si esta se puede mejorar con un state :)
        private bool pelotaDominada;
        private bool atacando;

        #endregion

        #region Constructores

        private Jugador() { }

        public Jugador(TgcSkeletalMesh skeletalMesh, IJugadorMoveStrategy strategy, Pelota pelota)
        {
            this.skeletalMesh = skeletalMesh;
            this.strategy = strategy;
            this.pelota = pelota;
            this.posicionOriginal = skeletalMesh.Position;
        }

        #endregion

        #region Propiedades

        public Vector3 Position
        {
            get { return this.skeletalMesh.Position; }
            set { this.skeletalMesh.Position = value; }
        }

        public bool AlphaBlendEnable
        {
            get { return this.skeletalMesh.AlphaBlendEnable; }
            set { this.skeletalMesh.AlphaBlendEnable = value; }
        }

        public Vector3 Rotation
        {
            get { return this.skeletalMesh.Rotation; }
            set { this.skeletalMesh.Rotation = value; }
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

        public string AnimacionCorriendo
        {
            get { return animacionCorriendo; }
            set { animacionCorriendo = value; }
        }

        public string AnimacionCaminando
        {
            get { return animacionCaminando; }
            set { animacionCaminando = value; }
        }

        public string AnimacionParado
        {
            get { return animacionParado; }
            set { animacionParado = value; }
        }

        public TgcBoundingBox BoundingBox
        {
            get { return this.skeletalMesh.BoundingBox; }
        }

        public BoxCollisionManager CollisionManager
        {
            get { return this.collisionManager; }
            set { this.collisionManager = value; }
        }

        public Pelota Pelota
        {
            get { return pelota; }
            set { pelota = value; }
        }

        public Equipo EquipoPropio
        {
            get { return equipoPropio; }
            set { equipoPropio = value; }
        }

        public bool MostrarBounding
        {
            get { return mostrarBounding; }
            set { mostrarBounding = value; }
        }

        public IJugadorMoveStrategy Strategy
        {
            get { return strategy; }
            set { strategy = value; }
        }

        public bool PelotaDominada
        {
            get { return pelotaDominada; }
            set { pelotaDominada = value; }
        }

        public bool Atacando
        {
            get { return atacando; }
            set { atacando = value; }
        }

        #endregion

        #region Metodos

        public void playAnimation(string animacion, bool v)
        {
            this.skeletalMesh.playAnimation(animacion, v);
        }

        public void move(Vector3 movimiento)
        {
            this.skeletalMesh.move(movimiento);
        }

        public void animateAndRender(float elapsedTime)
        {
            if (this.pelotaDominada)
            {
                this.strategy.PelotaDominada(this, elapsedTime, pelota);
            }
            else
            {
                this.strategy.Move(this, elapsedTime);
            }

            this.skeletalMesh.animateAndRender();

            if (this.mostrarBounding)
            {
                this.skeletalMesh.BoundingBox.render();
            }
        }

        public void ReiniciarPosicion()
        {
            this.skeletalMesh.Position = this.posicionOriginal;
        }

        public void render()
        {
            this.skeletalMesh.render();

            if (this.mostrarBounding)
            {
                this.skeletalMesh.BoundingBox.render();
            }
        }

        public void dispose()
        {
            this.skeletalMesh.dispose();
        }

        public void ColisionasteConPelota(Pelota pelota)
        {
            Partido.Instance.NotificarPelotaDominada(this);
        }

        public Vector3 GetDireccionDeRebote(Vector3 movimiento)
        {
            //Ver esto, como pensamos las colisiones, los jugadores reciben la pelota y la pueden tener, si hacemos esto le rebotaria
            movimiento.Normalize();
            //si la pelota se esta moviendo en X, entonces cambio esa direccion
            if (movimiento.X != 0)
            {
                movimiento.X *= -1;
            }

            //si la pelota se mueve en Z, cambio esa direccion
            if (movimiento.Z != 0)
            {
                movimiento.Z *= -1;
            }

            return movimiento;
        }

        public float GetFuerzaRebote(Vector3 movimiento)
        {
            return 0.10f;
        }

        public TgcBoundingBox GetTgcBoundingBox()
        {
            return this.BoundingBox;
        }

        public double DistanciaPelota()
        {
            Vector3 vector = this.Position - this.pelota.Position;
            //Math.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y) + (vector.Z * vector.Z));
            return vector.Length();
        }

        #endregion
    }
}