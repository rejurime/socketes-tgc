using Microsoft.DirectX;
using System;

namespace AlumnoEjemplos.Socketes.Model.Jugadores
{
    public class JugadorIAStrategy : IJugadorMoveStrategy
    {
        private bool inteligenciaArtificial;
        private Random semilla;
        int maximoFuerzaPatear = 10;

        public bool InteligenciaArtificial
        {
            get { return inteligenciaArtificial; }
            set { inteligenciaArtificial = value; }
        }

        public JugadorIAStrategy()
        {
            this.semilla = new Random();
        }

        public void Move(Jugador jugador, float elapsedTime)
        {
            if (!this.TengoQueMoverme(jugador))
            {
                return;
            }

            if (!this.inteligenciaArtificial)
            {
                return;
            }

            //Multiplicar la velocidad por el tiempo transcurrido, para no acoplarse al CPU
            float velocidad = jugador.VelocidadCaminar * elapsedTime;

            //Obtengo el vector direccion donde esta la pelota
            Vector3 movimiento = new Vector3(jugador.Pelota.Position.X - jugador.Position.X, 0, jugador.Pelota.Position.Z - jugador.Position.Z); ;
            movimiento.Normalize();

            jugador.playAnimation(jugador.AnimacionCaminando, true);

            Vector3 lastPos = jugador.Position;
            jugador.move(movimiento);

            //Calculo para donde tengo que retar dependiendo de donde apunta la direccion
            this.CalcularRotacion(jugador, movimiento);

            //Detecto las colisiones 
            if (jugador.CollisionManager.DetectarColisiones(jugador, lastPos))
            {
                //Si hubo colision, restaurar la posicion anterior
                jugador.Position = lastPos;
            }
        }

        public void PelotaDominada(Jugador jugador, float elapsedTimePelota, Pelota pelota)
        {
            if (!this.TengoQueMoverme(jugador))
            {
                return;
            }

            if (!this.inteligenciaArtificial)
            {
                return;
            }

            Vector3 direccion = new Vector3(jugador.EquipoPropio.ArcoRival.Red.GetPosition().X, jugador.EquipoPropio.ArcoRival.Red.GetPosition().Y, jugador.EquipoPropio.ArcoRival.Red.GetPosition().Z);
            direccion.Normalize();

            //Por ahora hago esto... :p, pero hay que pensar una IA real :)
            double tamanoCanchaParcial = Partido.Instance.Cancha.Tamano().Length() / 3;
            double distanciaArco = (jugador.EquipoPropio.ArcoRival.Red.GetPosition() - jugador.Position).Length();

            if (jugador.Atacando)
            {
                if (distanciaArco < tamanoCanchaParcial)
                {
                    pelota.Patear(direccion, this.semilla.Next(this.maximoFuerzaPatear / 2));
                }
                else
                {
                    pelota.Mover(direccion);
                }
            }
            else
            {
                pelota.Patear(direccion, this.semilla.Next(this.maximoFuerzaPatear));
            }

            jugador.PelotaDominada = false;
        }

        private bool TengoQueMoverme(Jugador jugador)
        {
            if (jugador.Equals(jugador.EquipoPropio.JugadorMasCercanoPelota()))
            {
                return true;
            }

            return false;
        }

        private void CalcularRotacion(Jugador jugador, Vector3 movimiento)
        {
            if (movimiento.X < 0)
            {
                if (movimiento.Z > 0)
                {
                    jugador.Rotation = new Vector3(0, -(float)Math.PI * 5 / 4, 0);
                }
                else if (movimiento.Z < 0)
                {
                    jugador.Rotation = new Vector3(0, (float)Math.PI * 2.5f, 0);
                }
                else
                {
                    jugador.Rotation = new Vector3(0, (float)Math.PI / 2, 0);
                }
            }
            else if (movimiento.X > 0)
            {
                if (movimiento.Z > 0)
                {
                    jugador.Rotation = new Vector3(0, (float)Math.PI * 5 / 4, 0);
                }
                else if (movimiento.Z < 0)
                {
                    jugador.Rotation = new Vector3(0, -(float)Math.PI / 4, 0);
                }
                else
                {
                    jugador.Rotation = new Vector3(0, -(float)Math.PI / 2, 0);
                }
            }
            else
            {
                if (movimiento.Z > 0)
                {
                    jugador.Rotation = new Vector3(0, -(float)Math.PI, 0);
                }
                else
                {
                    jugador.Rotation = new Vector3(0, 0, 0);
                }
            }
        }
    }
}