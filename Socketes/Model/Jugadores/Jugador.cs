using System;
using AlumnoEjemplos.Socketes.Model.Colision;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;
using AlumnoEjemplos.Socketes.Model.Iluminacion;
using System.Collections.Generic;
using TgcViewer;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

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

        private Effect shadowEffect;

        //TODO ver si esta se puede mejorar con un state :)
        private bool pelotaDominada = false;
        private bool atacando = false;
        private bool cambiandoStrategy = false;

        #endregion

        #region Constructores

        private Jugador() { }

        public Jugador(TgcSkeletalMesh skeletalMesh, IJugadorMoveStrategy strategy, Pelota pelota, Effect shadowEffect)
        {
            this.skeletalMesh = skeletalMesh;
            this.box = TgcBox.fromExtremes(this.skeletalMesh.BoundingBox.PMin, this.skeletalMesh.BoundingBox.PMax);
            this.strategy = strategy;
            this.pelota = pelota;
            this.posicionOriginal = skeletalMesh.Position;
            this.shadowEffect = shadowEffect;
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

        public bool CambiandoStrategy
        {
            get { return cambiandoStrategy; }
            set { cambiandoStrategy = value; }
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
            if (this.cambiandoStrategy)
            {
                this.cambiandoStrategy = false;
            }
            else
            {
                if (this.pelotaDominada)
                {
                    this.strategy.AccionConPelota(this, elapsedTime, pelota);
                }
                else
                {
                    this.strategy.AccionSinPelota(this, elapsedTime);
                }
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

        public void renderShadow(float elapsedTime, List<Luz> luces)
        {
            Microsoft.DirectX.Direct3D.Device device = GuiController.Instance.D3dDevice;
            device.RenderState.ZBufferEnable = false;
            Effect originalEffect = skeletalMesh.Effect;
            this.skeletalMesh.AlphaBlendEnable = true;
            string originalTechnique = this.skeletalMesh.Technique;

            this.skeletalMesh.Effect = shadowEffect;
            this.skeletalMesh.Technique = "RenderShadows";

            foreach (Luz luz in luces)
            {
                this.shadowEffect.SetValue("matViewProj", device.Transform.View * device.Transform.Projection);
                this.shadowEffect.SetValue("g_vLightPos", new Vector4(luz.Posicion.X, luz.Posicion.Y, luz.Posicion.Z, 1));
                this.skeletalMesh.render();
            }
            device.RenderState.ZBufferEnable = true;
            this.skeletalMesh.AlphaBlendEnable = false;
            this.skeletalMesh.Effect = originalEffect;
            this.skeletalMesh.Technique = originalTechnique;

        }

        public void dispose()
        {
            this.skeletalMesh.dispose();
        }

        public void CambiarStrategy(IJugadorMoveStrategy jugadorIAStrategy)
        {
            this.cambiandoStrategy = true;
            this.strategy = jugadorIAStrategy;
            this.playAnimation(this.AnimacionParado, true);
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
                return 300;
            }

            return fuerzaRestante * 0.9f;
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

        public double Distancia(Jugador jugadorReferencia)
        {
            Vector3 vector = this.Position - jugadorReferencia.Position;
            return vector.Length();
        }

        #endregion
    }
}