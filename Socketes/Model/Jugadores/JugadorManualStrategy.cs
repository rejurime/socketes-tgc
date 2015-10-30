using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System;
using TgcViewer;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.Socketes.Model.Jugadores
{
    public class JugadorManualStrategy : IJugadorMoveStrategy
    {
        private TgcD3dInput d3dInput;
        private float acumuladoPatear = 0;
        private float maximoFuerzaPatear = 10;

        public JugadorManualStrategy(TgcD3dInput d3dInput)
        {
            this.d3dInput = d3dInput;
        }

        public void AccionSinPelota(Jugador jugador, float elapsedTime)
        {
            //Si presiono S cambio de jugador
            if (this.d3dInput.keyPressed(Key.S))
            {
                this.CambiarJugador(jugador);
            }
            else
            {
                this.CalcularPosicionSegunInput(jugador, elapsedTime);
            }
        }

        private void CambiarJugador(Jugador jugador)
        {
            jugador.CambiarStrategy(new JugadorIAStrategy());
            Jugador jugadorCercano = jugador.EquipoPropio.JugadorMasCercanoPelota(jugador);
            jugadorCercano.CambiarStrategy(new JugadorManualStrategy(d3dInput));
        }

        //TODO esto lo deberia ejecutar desde el animate and render o desde colisionastecon?
        public void AccionConPelota(Jugador jugador, float elapsedTime, Pelota pelota)
        {
            Vector3 movimiento = this.CalcularPosicionSegunInput(jugador, elapsedTime);

            //Si presiono S, paso la pelota
            if (this.d3dInput.keyPressed(Key.S))
            {
                jugador.Pelota.Pasar(Partido.Instance.EquipoLocal.Jugadores[1].Position, 300);
                jugador.PelotaDominada = false;
                return;
            }

            //Si presiono A, paso la pelota
            if (this.d3dInput.keyPressed(Key.A))
            {
                //TODO MATI ACA VA EL CENTRO!!!!!!!!!!!!!!!!!!!!
                GuiController.Instance.Logger.log("Altoooooooooooooo centro :)");
                jugador.PelotaDominada = false;
                return;
            }

            //Si presiono D, comienzo a acumular cuanto patear
            if (this.d3dInput.keyDown(Key.D))
            {
                if (this.acumuladoPatear < this.maximoFuerzaPatear)
                {
                    this.acumuladoPatear += elapsedTime + 0.2f;
                }
                else
                {
                    Vector3 direccion = CalcularDireccionDePateado(jugador, pelota, movimiento);
                    jugador.Pelota.Patear(direccion, this.maximoFuerzaPatear);
                    jugador.PelotaDominada = false;
                    this.acumuladoPatear = 0;

                    return;
                }
            }

            //Si suelto D pateo la pelota con la fuerza acumulada
            if (this.d3dInput.keyUp(Key.D) && this.acumuladoPatear != 0)
            {
                Vector3 direccion = CalcularDireccionDePateado(jugador, pelota, movimiento);
                jugador.Pelota.Patear(direccion, this.acumuladoPatear);
                jugador.PelotaDominada = false;
                this.acumuladoPatear = 0;

                return;
            }

            //RENE: Revisar esto, si el jugador deja de coslionar con la pelota, entonces la suelta y no la tieen mas, esto arregla el tema de ir para atras, el codigo no es lindo, hace tu magia!
            if (this.SigoColisionadoConPelota(pelota, jugador))
            {
                if (movimiento != Vector3.Empty)
                {
                    pelota.Mover(movimiento);
                }
            }
            else
            {
                jugador.PelotaDominada = false;
            }
        }

        private Vector3 CalcularDireccionDePateado(Jugador jugador, Pelota pelota, Vector3 movimiento)
        {
            Vector3 direccion = Vector3.Empty;
            if (movimiento.Equals(Vector3.Empty))
            {
                direccion = pelota.Position - jugador.Position;
            }
            else
            {
                direccion = new Vector3(movimiento.X, movimiento.Y, movimiento.Z);
            }

            direccion.Normalize();

            return direccion;
        }

        private static Vector3 CalcularDireccionDePateado(Jugador jugador, Pelota pelota)
        {
            return pelota.Position - jugador.Position;
        }

        private bool SigoColisionadoConPelota(Pelota pelota, Jugador jugador)
        {
            TgcCollisionUtils.BoxBoxResult result = TgcCollisionUtils.classifyBoxBox(pelota.GetTgcBoundingBox(), jugador.GetTgcBoundingBox());

            if (result == TgcCollisionUtils.BoxBoxResult.Adentro || result == TgcCollisionUtils.BoxBoxResult.Atravesando)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculo cual es la proxima posicion en base a lo que tocan en el teclado
        /// </summary>
        /// <param name="elapsedTime"> Tiempo en segundos transcurridos desde el último frame</param>
        public Vector3 CalcularPosicionSegunInput(Jugador jugador, float elapsedTime)
        {
            //Calcular proxima posicion de personaje segun Input
            Vector3 movimiento = Vector3.Empty;
            Vector3 direccion = new Vector3(0, 0, 0);
            bool moving = false;
            bool correr = false;

            //Multiplicar la velocidad por el tiempo transcurrido, para no acoplarse al CPU
            float velocidad = jugador.VelocidadCaminar * elapsedTime;

            //Si presiono W corre
            if (this.d3dInput.keyDown(Key.W))
            {
                //Multiplicar la velocidad por el tiempo transcurrido, para no acoplarse al CPU
                velocidad = jugador.VelocidadCorrer * elapsedTime;
                correr = true;
            }

            //Si suelto, vuelve a caminar
            if (this.d3dInput.keyUp(Key.W))
            {
                correr = false;
            }

            //Adelante
            if (this.d3dInput.keyDown(Key.UpArrow))
            {
                movimiento.Z = velocidad;
                direccion.Y = (float)Math.PI;
                moving = true;
            }

            //Atras
            if (this.d3dInput.keyDown(Key.DownArrow))
            {
                movimiento.Z = -velocidad;
                //No hago nada en este caso por la rotacion
                moving = true;
            }

            //Derecha
            if (this.d3dInput.keyDown(Key.RightArrow))
            {
                movimiento.X = velocidad;
                direccion.Y = -(float)Math.PI / 2;
                moving = true;
            }

            //Izquierda
            if (this.d3dInput.keyDown(Key.LeftArrow))
            {
                movimiento.X = -velocidad;
                direccion.Y = (float)Math.PI / 2;
                moving = true;
            }

            //Diagonales, lo unico que hace es girar al jugador, el movimiento se calcula con el ingreso de cada tecla.
            if (this.d3dInput.keyDown(Key.UpArrow) && d3dInput.keyDown(Key.Right))
            {
                direccion.Y = (float)Math.PI * 5 / 4;
            }

            if (this.d3dInput.keyDown(Key.UpArrow) && d3dInput.keyDown(Key.LeftArrow))
            {
                direccion.Y = (float)Math.PI * 3 / 4;
            }
            if (this.d3dInput.keyDown(Key.DownArrow) && d3dInput.keyDown(Key.LeftArrow))
            {
                direccion.Y = (float)Math.PI / 4;
            }
            if (this.d3dInput.keyDown(Key.DownArrow) && d3dInput.keyDown(Key.RightArrow))
            {
                direccion.Y = (float)Math.PI * 7 / 4;
            }

            //Si hubo desplazamiento
            if (moving)
            {
                //Activar animacion que corresponda
                if (correr)
                {
                    jugador.playAnimation(jugador.AnimacionCorriendo, true);
                }
                else
                {
                    jugador.playAnimation(jugador.AnimacionCaminando, true);
                }

                //Aplicar movimiento hacia adelante o atras segun la orientacion actual del Mesh
                Vector3 lastPos = jugador.Position;

                jugador.move(movimiento);
                jugador.Rotation = direccion;

                //Detecto las colisiones                
                if (jugador.CollisionManager.DetectarColisiones(jugador, lastPos))
                {
                    //Si hubo colision, restaurar la posicion anterior
                    jugador.Position = lastPos;
                }
            }
            //Si no se esta moviendo, activar animacion de Parado
            else
            {
                jugador.playAnimation(jugador.AnimacionParado, true);
            }

            return movimiento;
        }
    }
}