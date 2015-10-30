using System;
using AlumnoEjemplos.Socketes.Model.Colision;
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
        private TgcBox box;
        private float velocidadCaminar = 120f;
        private float velocidadCorrer = 220f;
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
        private bool cambiandoStrategy;

        #endregion

        #region Constructores

        private Jugador() { }

        public Jugador(TgcSkeletalMesh skeletalMesh, IJugadorMoveStrategy strategy, Pelota pelota)
        {
            this.skeletalMesh = skeletalMesh;
            this.box = TgcBox.fromExtremes(this.skeletalMesh.BoundingBox.PMin, this.skeletalMesh.BoundingBox.PMax);
            this.strategy = strategy;
            this.pelota = pelota;
            this.posicionOriginal = skeletalMesh.Position;
        }

        #endregion

        #region Propiedades

        public Vector3 Position
        {
            get { return this.skeletalMesh.Position; }
            set
            {
                this.skeletalMesh.Position = value;
                this.box.Position = value + new Vector3(0, (this.box.BoundingBox.PMax.Y - this.box.BoundingBox.PMin.Y) / 2, 0);
            }
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
            //get { return this.skeletalMesh.BoundingBox; }
            get { return this.box.BoundingBox; }
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
            this.box.move(movimiento);
        }

        public void animateAndRender(float elapsedTime)
        {
            if (this.pelotaDominada)
            {
                this.strategy.AccionConPelota(this, elapsedTime, pelota);
            }
            else
            {
                this.strategy.AccionSinPelota(this, elapsedTime);
            }

            this.skeletalMesh.animateAndRender();

            if (this.mostrarBounding)
            {
                //this.skeletalMesh.BoundingBox.render();
                this.box.BoundingBox.render();
            }
        }

        public void ReiniciarPosicion()
        {
            this.Position = this.posicionOriginal;
            this.playAnimation(this.AnimacionParado, true);
        }

        public void render()
        {
            this.skeletalMesh.render();

            if (this.mostrarBounding)
            {
                this.skeletalMesh.BoundingBox.render();
            }
        }

        public void renderShadow(float elapsedTime, System.Collections.Generic.List<Iluminacion.Luz> luces)
        {
            //TODO implementar....
        }

        public void dispose()
        {
            this.skeletalMesh.dispose();
        }

        public void CambiarStrategy(IJugadorMoveStrategy jugadorIAStrategy)
        {
            
        }

        public void ColisionasteConPelota(Pelota pelota)
        {
            Partido.Instance.NotificarPelotaDominada(this);
        }

        public Vector3 GetDireccionDeRebote(Vector3 movimiento)
        {
            //por defecto la direccion de movimiento es la que tiene
            Vector3 direccion = new Vector3(movimiento.X, movimiento.Y, movimiento.Z);
            direccion.Normalize();
            //indica si la pelota esta arriba del jugador, osea que colisiono con la cabeza
            if (pelota.Position.Y > BoundingBox.PMax.Y - pelota.Diametro)
            {
                direccion.Y = 1;
            }

            if (movimiento.X != 0)
            {
                direccion.X *= -1;
            }

            //si la pelota se mueve en Z, cambio esa direccion
            if (movimiento.Z != 0)
            {
                direccion.Z *= -1;
            }

            if (movimiento.X == 0 && movimiento.Z == 0 && direccion.Y != 0)
            {
                //si no habia movimiento de la pelota, la cabece para algun lado
                Vector3 distancia = BoundingBox.Position - pelota.Position;
                distancia.Normalize();
                direccion.X = distancia.X * -1;
                direccion.Z = distancia.Z * -1;
            }


            return direccion;
        }

        public float GetFuerzaRebote(Vector3 movimiento, float fuerzaRestante)
        {
            //indica si la pelota esta arriba del jugador, osea que colisiono con la cabeza
            if (pelota.Position.Y > BoundingBox.PMax.Y - pelota.Diametro)
            {
                return 4;
            }

            return fuerzaRestante * 0.4f;
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