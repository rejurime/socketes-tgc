using AlumnoEjemplos.Socketes.Fisica;
using AlumnoEjemplos.Socketes.Model.Colision;
using AlumnoEjemplos.Socketes.Model.Jugadores;
using AlumnoEjemplos.Socketes.Utils;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Reflection;
using TgcViewer;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.Socketes.Model
{
    public class Pelota : IColisionable
    {
        private Vector3 posicionOriginalSphere;
        private Vector3 posicionOriginalBox;
        private TgcSphere sphere;
        private bool mostrarBounding = true;
        private ITiro tiro;
        private PelotaCollisionManager collisionManager;

        private float gravityForce = 0.8f;

        private TgcBox box;
        private float VELOCIDAD_DE_ROTACION_DEFAULT = 100;

        //sonido para patear
        private TgcMp3Player sonidoPatear;
        private Vector3 movimiento = Vector3.Empty;

        //solo guardo la rotacion, el resto de las matrices se arman en el momento
        private Matrix matrixrotacion = Matrix.Identity;
        private float diametro;

        public float Diametro
        {
            get { return diametro; }
            set { diametro = value; }
        }

  

        public TgcBoundingSphere BoundingSphere
        {
            get { return sphere.BoundingSphere; }
        }

        public Vector3 Position
        {
            get { return sphere.Position; }
            set { sphere.Position = Position; }
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
            //apago el auto transformado, ya que la pelota lo maneja solo
            sphere.AutoTransformEnable = false;
            diametro = sphere.Radius * 2;
            this.box = TgcBox.fromSize(sphere.Position, new Vector3(sphere.Radius * 2, sphere.Radius * 2, sphere.Radius * 2));
            string pathRecursos = Environment.CurrentDirectory + "\\" + Assembly.GetExecutingAssembly().GetName().Name + "\\" + Settings.Default.mediaFolder;

            this.sonidoPatear = new TgcMp3Player();
            this.sonidoPatear.FileName = pathRecursos + "\\Audio\\patear-pelota.mp3";
            this.sphere = sphere;

            this.posicionOriginalSphere = new Vector3(sphere.Position.X, sphere.Position.Y, sphere.Position.Z);
            this.posicionOriginalBox = new Vector3(box.Position.X, box.Position.Y, box.Position.Z);

            this.sphere.Transform = getScalingMatrix() *
               Matrix.Translation(sphere.Position);

            sphere.updateValues();
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
            if (sphere.Position.Y > gravityForce && !hayTiro())
            {
                movimiento.Y -= gravityForce;
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
                if (isLogEnable())
                    GuiController.Instance.Logger.log("Objetos colsionados: " + objetoColisionado);
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

            if (isLogEnable())
                GuiController.Instance.Logger.log("Patear en direccion: " + VectorUtils.PrintVectorSinSaltos(direccion) + " con fuerza: " + fuerza);
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


            if (isLogEnable())
                GuiController.Instance.Logger.log("Movimiento real: " + VectorUtils.PrintVectorSinSaltos(movimiento));

            moveTo(movimiento);

            ColisionInfo colisionInfo = collisionManager.GetColisiones(box.BoundingBox);

            if (isLogEnable())
                GuiController.Instance.Logger.log("Se colisiono con: " + colisionInfo.getColisiones().Count + " obstaculo(s)");

            foreach (IColisionablePelota objetoColisionado in colisionInfo.getColisiones())
            {
                objetoColisionado.ColisionasteConPelota(this);
                if (isLogEnable())
                    GuiController.Instance.Logger.log("Objetos colsionados: " + objetoColisionado);

                if (hayTiro())
                {
                    //aca uso el movimiento real, sin tener en cuenta la colision, para saber la direccion que toma el tiro en el rebote
                    tiro = new TiroParabolicoSimple(objetoColisionado.GetDireccionDeRebote(movimiento),
                        objetoColisionado.GetFuerzaRebote(movimiento, tiro.getFuerza()));
                }
                else if (hayMovimiento(movimiento))
                {
                    //aca uso el movimiento real, sin tener en cuenta la colision, para saber la direccion que toma el tiro en el rebote
                    tiro = new TiroParabolicoSimple(objetoColisionado.GetDireccionDeRebote(movimiento),
                        objetoColisionado.GetFuerzaRebote(movimiento, 5));
                }
            }

            //arma la transformacion en base al escalado + rotacion + traslacion
            sphere.Transform = getScalingMatrix() * getRotationMatrix(movimiento, elapsedTime) *
               Matrix.Translation(sphere.Position);
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
            this.sphere.Position = this.posicionOriginalSphere;
            this.box.Position = this.posicionOriginalBox;
            resetMovimiento();
            resetTiro();
        }

        private bool isLogEnable()
        {

            return (bool)GuiController.Instance.Modifiers["Log"];
        }

        private bool hayMovimiento(Vector3 movimiento)
        {
            if (isLogEnable())
                GuiController.Instance.Logger.log("Movimiento: " + VectorUtils.PrintVectorSinSaltos(movimiento) + ", hay movimiento?: " + (movimiento.X != 0 || movimiento.Y != 0 || movimiento.Z != 0));
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
            float velocidadRotacion = VELOCIDAD_DE_ROTACION_DEFAULT * direccion.Length();
            direccion.Normalize();
            
            if (isLogEnable())
                GuiController.Instance.Logger.log("Direccion de rotacion: " + VectorUtils.PrintVectorSinSaltos(direccion));

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
            if (!hayTiro())
                this.movimiento = movement;
        }

        public Vector3 GetDireccionDeRebote(Vector3 movimiento)
        {
            throw new NotImplementedException();
        }

        public float GetFuerzaRebote(Vector3 movimiento, float fuerzaRestante)
        {
            throw new NotImplementedException();
        }

        public TgcBoundingBox GetTgcBoundingBox()
        {
            return box.BoundingBox;
        }

        public void renderShadow(float elapsedTime, System.Collections.Generic.List<Iluminacion.Luz> luces)
        {
            //TODO implementar......
        }
    }
}