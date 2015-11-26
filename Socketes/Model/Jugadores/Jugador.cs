using AlumnoEjemplos.Socketes.Model.Colision;
using AlumnoEjemplos.Socketes.Model.Iluminacion;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Collections.Generic;
using System.Drawing;
using TgcViewer;
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
        private float velocidadCorrer = 200f;
        private IJugadorMoveStrategy strategy;
        private Pelota pelota;
        private Equipo equipoPropio;
        private BoxCollisionManager collisionManager;
        private string animacionCorriendo = Settings.Default.animationRunPlayer;
        private string animacionCaminando = Settings.Default.animationWalkPlayer;
        private string animacionParado = Settings.Default.animationStopPlayer;
        private bool mostrarBounding;
        private Effect shadowEffect;
        private Effect lightEffect;

        //TODO ver si esta se puede mejorar con un state :)
        private bool pelotaDominada = false;
        private bool atacando = false;
        private bool cambiandoStrategy = false;

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

        public bool CambiandoStrategy
        {
            get { return cambiandoStrategy; }
            set { cambiandoStrategy = value; }
        }

        public Effect ShadowEffect
        {
            get { return shadowEffect; }
            set { shadowEffect = value; }
        }

        public Effect LightEffect
        {
            get { return lightEffect; }
            set { lightEffect = value; }
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
            Device device = GuiController.Instance.D3dDevice;
            Effect originalEffect = skeletalMesh.Effect;
            string originalTechnique = this.skeletalMesh.Technique;

            this.skeletalMesh.Effect = shadowEffect;
            this.skeletalMesh.Technique = "RenderMeshShadows";

            foreach (Luz luz in luces)
            {
                this.skeletalMesh.AlphaBlendEnable = true;
                device.RenderState.ZBufferEnable = false;
                this.shadowEffect.SetValue("lightIntensity", (float)GuiController.Instance.Modifiers["lightIntensity"]);
                this.shadowEffect.SetValue("lightAttenuation", (float)GuiController.Instance.Modifiers["lightAttenuation"]);
                this.shadowEffect.SetValue("matViewProj", device.Transform.View * device.Transform.Projection);
                this.shadowEffect.SetValue("g_vLightPos", new Vector4(luz.Posicion.X, luz.Posicion.Y, luz.Posicion.Z, 1));
                this.skeletalMesh.render();
            }

            device.RenderState.ZBufferEnable = true;
            this.skeletalMesh.AlphaBlendEnable = false;
            this.skeletalMesh.Effect = originalEffect;
            this.skeletalMesh.Technique = originalTechnique;
        }

        public void SetTextura(string pathTexturaJugador)
        {
            //Le cambiamos la textura
            this.skeletalMesh.changeDiffuseMaps(new TgcTexture[] { TgcTexture.createTexture(pathTexturaJugador) });
        }

        public void renderLight(float elapsedTime, List<Luz> luces)
        {
            //Configurar los valores de cada luz
            ColorValue[] lightColors = new ColorValue[luces.Count];
            Vector4[] pointLightPositions = new Vector4[luces.Count];
            float[] pointLightIntensity = new float[luces.Count];
            float[] pointLightAttenuation = new float[luces.Count];

            for (int i = 0; i < luces.Count; i++)
            {
                Luz lightMesh = luces[i];
                lightColors[i] = ColorValue.FromColor(lightMesh.Color);
                pointLightPositions[i] = TgcParserUtils.vector3ToVector4(lightMesh.Posicion);
                pointLightIntensity[i] = (float)GuiController.Instance.Modifiers["lightIntensity"];
                pointLightAttenuation[i] = (float)GuiController.Instance.Modifiers["lightAttenuation"];
            }

            //Cargar variables shader de la luz
            this.lightEffect.SetValue("lightColor", lightColors);
            this.lightEffect.SetValue("lightPosition", pointLightPositions);
            this.lightEffect.SetValue("lightIntensity", pointLightIntensity);
            this.lightEffect.SetValue("lightAttenuation", pointLightAttenuation);
            this.lightEffect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(GuiController.Instance.FpsCamera.getPosition()));

            //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
            this.lightEffect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
            this.lightEffect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
            this.lightEffect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
            this.lightEffect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
            this.lightEffect.SetValue("materialSpecularExp", 20f);

            Effect originalEffect = this.skeletalMesh.Effect;
            string originalTechnique = this.skeletalMesh.Technique;

            this.skeletalMesh.Effect = this.lightEffect;
            //El Technique depende del tipo RenderType del mesh "VERTEX_COLOR"; "DIFFUSE_MAP";
            this.skeletalMesh.Technique = "DIFFUSE_MAP";

            this.animateAndRender(elapsedTime);

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
            if (pelota.Position.Y > BoundingBox.PMax.Y - pelota.Diametro / 2)
            {
                direccion.Y = 1;
            }
            else
            {
                direccion.Y = 0;
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