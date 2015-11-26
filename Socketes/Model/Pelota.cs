using AlumnoEjemplos.Socketes.Fisica;
using AlumnoEjemplos.Socketes.Model.Colision;
using AlumnoEjemplos.Socketes.Model.Iluminacion;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Pelota
    {
        private Vector3 posicionOriginalSphere;
        private Vector3 posicionOriginalBox;
        private TgcSphere sphere;
        private bool mostrarBounding;
        private ITiro tiro;
        private PelotaCollisionManager collisionManager;

        private Effect shadowEffect;
        private Effect lightEffect;

        private float gravityForce = 0.8f;

        private TgcBox box;
        private float VELOCIDAD_DE_ROTACION_DEFAULT = 100;

        private Vector3 movimiento = Vector3.Empty;

        //solo guardo la rotacion, el resto de las matrices se arman en el momento
        private Matrix matrixrotacion = Matrix.Identity;

        public float Diametro
        {
            get { return this.sphere.Radius * 2; }
        }

        public TgcBoundingBox BoundingBox
        {
            get { return this.box.BoundingBox; }
        }

        public Vector3 Position
        {
            get { return sphere.Position; }
            set { sphere.Position = value; }
        }

        public bool MostrarBounding
        {
            get { return mostrarBounding; }
            set { mostrarBounding = value; }
        }

        public PelotaCollisionManager CollisionManager
        {
            get { return collisionManager; }
            set { collisionManager = value; }
        }

        public Pelota(TgcSphere sphere)
        {
            this.sphere = sphere;
            //apago el auto transformado, ya que la pelota lo maneja solo
            this.sphere.AutoTransformEnable = false;
            this.sphere.Transform = this.getScalingMatrix() * Matrix.Translation(this.sphere.Position);
            this.sphere.updateValues();

            this.box = TgcBox.fromSize(sphere.Position, new Vector3(this.Diametro, this.Diametro, this.Diametro));

            this.posicionOriginalSphere = new Vector3(this.sphere.Position.X, this.sphere.Position.Y, this.sphere.Position.Z);
            this.posicionOriginalBox = new Vector3(this.box.Position.X, this.box.Position.Y, this.box.Position.Z);
        }

        public void render()
        {
            sphere.updateValues();
            sphere.render();

            if (this.mostrarBounding)
            {
                sphere.BoundingSphere.render();
                box.BoundingBox.render();
            }
        }

        public void updateValues(float elapsedTime)
        {
            //movimiento que se guarda cuando se llama a mover directo
            Vector3 movimiento = this.movimiento;

            //manejo de gravedad
            //if (sphere.Position.Y > gravityForce && !hayTiro())
            if (this.box.BoundingBox.PMin.Y > gravityForce && !hayTiro())
            {
                movimiento.Y -= gravityForce;
            }

            if (this.box.BoundingBox.PMin.Y < 0 && !hayTiro())
            {
                //si el boundingbox se va para abajo, entonces lo arreglo.
                movimiento.Y = -this.box.BoundingBox.PMin.Y + 0.01f;
            }
            //manejo de gravedad
            if (hayMovimiento(movimiento))
            {
                Mover(movimiento, elapsedTime);
                resetTiro();
                resetMovimiento();
                return;
            }

            if (hayTiro())
            {
                //existe un tiro en proceso
                Mover(tiro.siguienteMovimiento(elapsedTime), elapsedTime);
                return;
            }

            //si llego hasta aca no habia movimiento y no habia tiro activo, me fijo si la pelota colisiono con algo
            ColisionInfo colisionInfo = collisionManager.GetColisiones(box.BoundingBox);

            foreach (IColisionablePelota objetoColisionado in colisionInfo.getColisiones())
            {
                objetoColisionado.ColisionasteConPelota(this);
            }
        }

        private void resetMovimiento()
        {
            this.movimiento = Vector3.Empty;
        }

        private void resetTiro()
        {
            this.tiro = null;
        }

        /// <summary>
        /// Metodo para patear una pelota, recibe el vector de direccion y la fuerza.
        /// </summary>
        /// <param name="direccion"></param>
        /// <param name="fuerza"></param>
        public void Patear(Vector3 direccion, float fuerza)
        {
            tiro = new TiroParabolicoSimple(direccion, fuerza);

            ReproducirSonidoPatear();
        }

        private void ReproducirSonidoPatear()
        {
            Partido.Instance.Sonidos["pelota-tiro"].play();
        }

        /// <summary>
        /// Mueve la pelota hacia el punto indicado, 
        /// el movimiento hacia ese punto es lineal, 
        /// en base a ese movimiento tambien hace la rotacion de la pelota
        /// </summary>
        private void Mover(Vector3 movimiento, float elapsedTime)
        {
            Vector3 lastsphereposition = sphere.Position;
            Vector3 lastboxposition = box.Position;

            moveTo(movimiento);

            ColisionInfo colisionInfo = collisionManager.GetColisiones(box.BoundingBox);

            foreach (IColisionablePelota objetoColisionado in colisionInfo.getColisiones())
            {
                objetoColisionado.ColisionasteConPelota(this);

                if (hayTiro())
                {
                    //solo rebota con el tiro, con el pase no hace nada
                    if (tiro is TiroParabolicoSimple)
                    {
                        //aca uso el movimiento real, sin tener en cuenta la colision, para saber la direccion que toma el tiro en el rebote
                        tiro = new TiroParabolicoSimple(objetoColisionado.GetDireccionDeRebote(movimiento),
                            objetoColisionado.GetFuerzaRebote(movimiento, tiro.getFuerza()));
                    }
                }
                else if (hayMovimiento(movimiento))
                {
                    //aca uso el movimiento real, sin tener en cuenta la colision, para saber la direccion que toma el tiro en el rebote
                    tiro = new TiroParabolicoSimple(objetoColisionado.GetDireccionDeRebote(movimiento),
                        objetoColisionado.GetFuerzaRebote(movimiento, 50));
                }
            }

            //arma la transformacion en base al escalado + rotacion + traslacion
            sphere.Transform = getScalingMatrix() * getRotationMatrix(movimiento, elapsedTime) *
               Matrix.Translation(sphere.Position);
        }

        public void SetTextura(string pathTexturaPelota)
        {
            this.sphere.setTexture(TgcTexture.createTexture(pathTexturaPelota));
        }

        private void rollbackPosition(Vector3 lastsphereposition, Vector3 lastboxposition)
        {
            sphere.Position = lastsphereposition;
            box.Position = lastboxposition;
        }

        private void moveTo(Vector3 movimiento)
        {
            sphere.move(movimiento);
            box.move(movimiento);
        }

        public void ReiniciarPosicion()
        {
            resetMovimiento();
            resetTiro();
            this.sphere.Position = this.posicionOriginalSphere;
            this.box.Position = this.posicionOriginalBox;
        }

        private bool hayMovimiento(Vector3 movimiento)
        {
            return movimiento.X != 0 || movimiento.Y != 0 || movimiento.Z != 0;
        }

        private bool hayTiro()
        {
            return tiro != null && tiro.hayMovimiento();
        }

        /// <summary>
        /// Matriz de escalado en base al radio de la esfera
        /// </summary>
        /// <returns></returns>
        private Matrix getScalingMatrix()
        {
            return Matrix.Scaling(sphere.Radius, sphere.Radius, sphere.Radius);
        }

        /// <summary>
        /// Retorna la matriz de rotacion en base al movimiento dado
        /// </summary>
        /// <param name="movimiento"></param>
        /// <param name="elapsedTime"></param>
        /// <returns></returns>
        /// <summary>
        /// Retorna la matriz de rotacion en base al movimiento dado
        /// </summary>
        /// <param name="movimiento"></param>
        /// <param name="elapsedTime"></param>
        /// <returns></returns>
        private Matrix getRotationMatrix(Vector3 movimiento, float elapsedTime)
        {
            Vector3 direccion = new Vector3(movimiento.X, movimiento.Y, movimiento.Z);
            direccion.Normalize();
            float velocidadRotacion = VELOCIDAD_DE_ROTACION_DEFAULT * direccion.Length();

            matrixrotacion *= Matrix.RotationAxis(getVectorRotacion(direccion), Geometry.DegreeToRadian(velocidadRotacion * elapsedTime));
            return matrixrotacion;
        }

        /// <summary>
        /// Retorna el vector de rotacion en base a la direccion de movimiento
        /// </summary>
        /// <param name="movimiento"></param>
        /// <returns></returns>
        private Vector3 getVectorRotacion(Vector3 movimiento)
        {
            Vector3 vectorrotacion = new Vector3(0, 0, 0);

            //solo roto en Y si la pelota esta cayendo, si se esta moviendo en Z o X entonces no hago nada
            if (movimiento.Y != 0 && movimiento.Z == 0 && movimiento.X == 0)
            {
                //caundo cae roto en un solo sentido
                vectorrotacion.X = -Math.Sign(movimiento.Y);
            }

            if (movimiento.Z != 0)
            {
                vectorrotacion.X = Math.Sign(movimiento.Z);
            }

            if (movimiento.X != 0)
            {
                vectorrotacion.Z = -Math.Sign(movimiento.X);
            }

            if (movimiento.Z != 0 && movimiento.X != 0)
            {
                vectorrotacion.X = 0.7074f / Math.Sign(movimiento.Z);
                vectorrotacion.Z = 0.7074f / -Math.Sign(movimiento.X);
            }

            return vectorrotacion;
        }

        public void dispose()
        {
            sphere.dispose();
            box.dispose();
        }

        /// <summary>
        /// Tira la pelota hacia un punto al raz del piso
        /// </summary>
        /// <param name="posicionJugador"></param>
        /// <param name="fuerza"></param>
        public void Pasar(Vector3 posicionJugador, float fuerza)
        {
            this.tiro = new TiroLinealAUnPunto(sphere.Position, posicionJugador, fuerza);
        }

        public void Mover(Vector3 movement)
        {
            this.movimiento = movement;
        }

        public void renderShadow(float elapsedTime, List<Luz> luces)
        {
            Device device = GuiController.Instance.D3dDevice;
            Effect originalEffect = sphere.Effect;
            string originalTechnique = this.sphere.Technique;

            this.sphere.Effect = shadowEffect;
            this.sphere.Technique = "RenderShadows";

            foreach (Luz luz in luces)
            {
                device.RenderState.ZBufferEnable = false;
                this.sphere.AlphaBlendEnable = true;
                this.shadowEffect.SetValue("lightIntensity", (float)GuiController.Instance.Modifiers["lightIntensity"]);
                this.shadowEffect.SetValue("lightAttenuation", (float)GuiController.Instance.Modifiers["lightAttenuation"]);
                this.shadowEffect.SetValue("matViewProj", device.Transform.View * device.Transform.Projection);
                this.shadowEffect.SetValue("g_vLightPos", new Vector4(luz.Posicion.X, luz.Posicion.Y, luz.Posicion.Z, 1));
                this.sphere.render();
            }

            device.RenderState.ZBufferEnable = true;
            this.sphere.AlphaBlendEnable = false;
            this.sphere.Effect = originalEffect;
            this.sphere.Technique = originalTechnique;
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

            //Cargar variables de shader
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

            Effect originalEffect = this.sphere.Effect;
            string originalTechnique = this.sphere.Technique;

            this.sphere.Effect = this.lightEffect;
            //El Technique depende del tipo RenderType del mesh "VERTEX_COLOR"; "DIFFUSE_MAP";
            this.sphere.Technique = "DIFFUSE_MAP";

            this.render();

            this.sphere.Effect = originalEffect;
            this.sphere.Technique = originalTechnique;
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

        public void Centro(Vector3 posicionJugador, float fuerza)
        {
            Vector3 direccion = posicionJugador - sphere.Position;
            direccion.Normalize();
            tiro = new TiroParabolicoSimple(direccion, fuerza);
            ReproducirSonidoPatear();
        }
    }
}