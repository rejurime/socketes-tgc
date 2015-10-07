using AlumnoEjemplos.Properties;
using AlumnoEjemplos.Socketes.Collision;
using AlumnoEjemplos.Socketes.Fisica;
using AlumnoEjemplos.Socketes.Model;
using AlumnoEjemplos.Socketes.Utils;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Reflection;
using TgcViewer;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.Socketes
{
    public class Pelota
    {
        private TgcSphere sphere;
        private float angulo = 0f;
        private bool mostrarBounding = true;
        private Tiro tiro;
        private SphereCollisionManager collisionManager;

        //para controlar que no se intente colisionar todo el tiempo con el piso.
        private bool piso = false;
        private float VELOCIDAD_DE_ROTACION_DEFAULT = 100;

        //sonido para patear
        private TgcMp3Player sonidoPatear;
        private Vector3 movimiento = Vector3.Empty;

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

        public SphereCollisionManager CollisionManager
        {
            get { return collisionManager; }
            set { collisionManager = value; }
        }

        public Pelota(TgcSphere sphere)
        {
            //apago el auto transformado, ya que la pelota lo maneja solo
            sphere.AutoTransformEnable = false;
            sphere.updateValues();

            string pathRecursos = Environment.CurrentDirectory + "\\" + Assembly.GetExecutingAssembly().GetName().Name + "\\" + Settings.Default.mediaFolder;

            this.sonidoPatear = new TgcMp3Player();
            this.sonidoPatear.FileName = pathRecursos + "\\Audio\\patear-pelota.mp3";
            this.sphere = sphere;
        }

        public void render()
        {
            sphere.render();

            if (this.mostrarBounding)
            {
                sphere.BoundingSphere.render();
            }
        }

        public void updateValues(float elapsedTime)
        {
            //activo o no gravedad si esta en el piso
            collisionManager.GravityEnabled = !piso;

            //calulo el movimiento
            Vector3 movimiento = calcularMovimiento(elapsedTime);

            //muevo la pelota, y el mismo metodo retornar las colisiones
            ColisionInfo colisionInfo = mover(movimiento, elapsedTime);

            //informo a todos los objetos que se colisiono
            foreach (IColisionable objetoColisionado in colisionInfo.getColisiones())
            {
                objetoColisionado.colisionasteConPelota(this);
            }
                
            sphere.updateValues();
        }

        private Vector3 calcularMovimiento(float elapsedTime)
        {
            Vector3 movimiento = Vector3.Empty;

            if (hayTiro())
            {
                //existe un tiro en proceso
                movimiento = tiro.siguienteMovimiento(elapsedTime);
            }
            else if (hayMovimiento())
            {
                movimiento = this.movimiento;
                this.movimiento = Vector3.Empty;
            }

            return movimiento;
        }

        private bool hayMovimiento()
        {
            return movimiento != Vector3.Empty;
        }

        /// <summary>
        /// Metodo para patear una pelota, recibe el vector de direccion y la fuerza.
        /// </summary>
        /// <param name="direccion"></param>
        /// <param name="fuerza"></param>
        public void patear(Vector3 direccion, float fuerza)
        {
            tiro = new TiroParabolicoSimple(direccion, fuerza);
            reproducirSonidoPatear();
        }

        private void reproducirSonidoPatear()
        {
            if (sonidoPatear.getStatus() == TgcMp3Player.States.Open)
            {
                sonidoPatear.play(false);
            }
            if (sonidoPatear.getStatus() == TgcMp3Player.States.Stopped)
            {
                sonidoPatear.closeFile();
                sonidoPatear.play(false);
            }

            if (sonidoPatear.getStatus() == TgcMp3Player.States.Playing)
            {
                sonidoPatear.closeFile();
                sonidoPatear.play(false);
            }
        }

        /// <summary>
        /// Mueve la pelota hacia el punto indicado, 
        /// el movimiento hacia ese punto es lineal, 
        /// en base a ese movimiento tambien hace la rotacion de la pelota
        /// </summary>
        private ColisionInfo mover(Vector3 movimiento, float elapsedTime)
        {
            ColisionInfo colisionInfo = collisionManager.moveCharacter(sphere.BoundingSphere, movimiento);
            Vector3 realMovement = colisionInfo.getRealMovement();

            if (isLogEnable())
                GuiController.Instance.Logger.log("Se colisiono con: " + colisionInfo.getColisiones().Count + " obstaculo(s)");

            if (hayMovimiento(realMovement))
            {
                //si hay que mover Y, entonces NO estoy en el piso
                if (realMovement.Y != 0)
                {
                    piso = false;
                }

                sphere.move(realMovement);

                //arma la transformacion en base al escalado + rotacion + traslacion
                sphere.Transform = getScalingMatrix() *
                   getRotationMatrix(realMovement, elapsedTime) *
                   Matrix.Translation(sphere.Position);

                foreach (IColisionable objetoColisionado in colisionInfo.getColisiones())
                {
                    if (isLogEnable())
                        GuiController.Instance.Logger.log("Objetos colsionados: " + objetoColisionado);

                    if (hayTiro())
                    {
                        //aca uso el movimiento real, sin tener en cuenta la colision, para saber la direccion que toma el tiro en el rebote
                        tiro = new TiroParabolicoSimple(objetoColisionado.getDireccionDeRebote(movimiento),
                            objetoColisionado.getFuerzaRebote(movimiento) * tiro.getFuerza());
                    }
                }
            }

            return colisionInfo;
        }

        private bool isLogEnable()
        {

            return (bool)GuiController.Instance.Modifiers["Log"];
        }

        private bool hayMovimiento(Vector3 movimiento)
        {
            if (isLogEnable())
                GuiController.Instance.Logger.log("Movimiento: " + VectorUtils.printVectorSinSaltos(movimiento) + ", hay movimiento?: " + (movimiento.X != 0 || movimiento.Y != 0 || movimiento.Z != 0));
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
            angulo += Geometry.DegreeToRadian(velocidadRotacion * elapsedTime);
            Matrix matrixrotacion = Matrix.RotationAxis(getVectorRotacion(direccion), angulo);
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
        public void pasar(Vector3 posicionJugador, float fuerza)
        {
            tiro = new TiroLinealAUnPunto(sphere.Position, posicionJugador, fuerza);
        }

        public void estasEnElPiso()
        {
            piso = true;
        }

        public void mover(Vector3 movement)
        {
            this.movimiento = movement;
        }
    }
}