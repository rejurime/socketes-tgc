using Microsoft.DirectX;
using System;

namespace AlumnoEjemplos.Socketes.Model.Jugadores
{
    public class ArqueroIAStrategy : IJugadorMoveStrategy
    {
        private bool inteligenciaArtificial;
        private Random semilla;
        int maximoFuerzaPatear = 10;
        private int MULTIPLICADOR_FUERZA_PATEAR = 80;

        public bool InteligenciaArtificial
        {
            get { return inteligenciaArtificial; }
            set { inteligenciaArtificial = value; }
        }

        public ArqueroIAStrategy()
        {
            this.semilla = new Random();
        }

        public void AccionSinPelota(Jugador jugador, float elapsedTime)
        {
            if (!this.inteligenciaArtificial)
            {
                return;
            }

            if (!this.TengoQueMoverme(jugador))
            {
                jugador.playAnimation(jugador.AnimacionParado, true);
                return;
            }

            //Multiplicar la velocidad por el tiempo transcurrido, para no acoplarse al CPU
            float velocidad = jugador.VelocidadCaminar * elapsedTime;

            //Obtengo el vector direccion donde esta la pelota
            Vector3 movimiento = new Vector3(jugador.Pelota.Position.X - jugador.Position.X, 0, jugador.Pelota.Position.Z - jugador.Position.Z);
            movimiento.Normalize();

            jugador.playAnimation(jugador.AnimacionCorriendo, true);


            Vector3 lastPos = jugador.Position;
            jugador.move(movimiento * velocidad);

            //Calculo para donde tengo que rotar dependiendo de donde apunta la direccion
            this.CalcularRotacion(jugador, movimiento);

            //Detecto las colisiones 
            if (jugador.CollisionManager.DetectarColisiones(jugador, lastPos))
            {
                //Si hubo colision, restaurar la posicion anterior
                jugador.Position = lastPos;
            }
        }

        public void AccionConPelota(Jugador jugador, float elapsedTimePelota, Pelota pelota)
        {
            if (!this.inteligenciaArtificial)
            {
                return;
            }

            //TODO revisar ¿Porque con pelot no se mveria??
            if (!this.TengoQueMoverme(jugador))
            {
                jugador.playAnimation(jugador.AnimacionParado, true);
                return;
            }

            Vector3 direccion = jugador.EquipoPropio.ArcoRival.Red.GetPosition() - jugador.Position;
            direccion.Normalize();

            //Por ahora hago esto... :p, pero hay que pensar una IA real :)
            double tamanoCanchaParcial = Partido.Instance.Cancha.Size.Length() / 3;
            double distanciaArco = (jugador.EquipoPropio.ArcoRival.Red.GetPosition() - jugador.Position).Length();
            double distanciaArcoPropio = (jugador.EquipoPropio.ArcoPropio.Red.GetPosition() - jugador.Position).Length();

            if (jugador.Atacando)
            {
                if (distanciaArco < tamanoCanchaParcial)
                {
                    pelota.Patear(direccion, this.semilla.Next(this.maximoFuerzaPatear * MULTIPLICADOR_FUERZA_PATEAR));
                }
                else
                {
                    if (distanciaArcoPropio < tamanoCanchaParcial / 3)
                    {
                        pelota.Patear(direccion, this.maximoFuerzaPatear * MULTIPLICADOR_FUERZA_PATEAR);
                    }
                    else
                    {
                        pelota.Mover(direccion);
                    }
                }
            }
            else
            {
                pelota.Patear(direccion, this.semilla.Next(this.maximoFuerzaPatear * MULTIPLICADOR_FUERZA_PATEAR));
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
