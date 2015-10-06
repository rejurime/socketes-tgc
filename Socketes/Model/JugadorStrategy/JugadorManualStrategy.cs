using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System;
using TgcViewer.Utils.Input;

namespace AlumnoEjemplos.Socketes.Model.JugadorStrategy
{
    public class JugadorManualStrategy : IJugadorMoveStrategy
    {
        private TgcD3dInput d3dInput;
        private float acumuladoPatear = 0;
        private float maximoFuerzaPatear = 5;

        public JugadorManualStrategy(TgcD3dInput d3dInput)
        {
            this.d3dInput = d3dInput;
        }

        public void Move(Jugador jugador, float elapsedTime)
        {
            this.CalcularPosicionSegunInput(jugador, elapsedTime);
        }

        /// <summary>
        /// Calculo cual es la proxima posicion en base a lo que tocan en el teclado
        /// </summary>
        /// <param name="elapsedTime"> Tiempo en segundos transcurridos desde el último frame</param>
        public void CalcularPosicionSegunInput(Jugador jugador, float elapsedTime)
        {
            //Calcular proxima posicion de personaje segun Input
            Vector3 movimiento = new Vector3(0, 0, 0);
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

            if (jugador.PelotaDominada)
            {
                //Si presiono S, paso la pelota
                if (this.d3dInput.keyDown(Key.S))
                {
                    jugador.Pelota.pasar(jugador.Companero.Position, 2);
                }

                //Si presiono D, comienzo a acumular cuanto patear
                if (this.d3dInput.keyDown(Key.D))
                {
                    if (this.acumuladoPatear < this.maximoFuerzaPatear)
                    {
                        this.acumuladoPatear += elapsedTime;
                    }
                    else
                    {
                        jugador.Pelota.patear(movimiento, this.maximoFuerzaPatear);
                        this.acumuladoPatear = 0;

                    }
                }

                //Si sueldo D pateo la pelota con la fuerza acumulada
                if (this.d3dInput.keyUp(Key.D))
                {
                    jugador.Pelota.patear(movimiento, this.acumuladoPatear);
                    this.acumuladoPatear = 0;
                }
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
        }
    }
}