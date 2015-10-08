﻿using Microsoft.DirectX;
using System;

namespace AlumnoEjemplos.Socketes.Model.JugadorStrategy
{
    public class JugadorIAStrategy : IJugadorMoveStrategy
    {
        private bool inteligenciaArtificial;

        public bool InteligenciaArtificial
        {
            get { return inteligenciaArtificial; }
            set { inteligenciaArtificial = value; }
        }

        public void Move(Jugador jugador, float elapsedTime)
        {
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
            //pelota.Patear(new Vector3(1, 0, 1), 2);
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