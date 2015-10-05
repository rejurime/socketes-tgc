using Microsoft.DirectX;

namespace AlumnoEjemplos.Socketes.Model.JugadorStrategy
{
    public class JugadorIAStrategy : IJugadorMoveStrategy
    {
        public void Move(Jugador jugador, float elapsedTime)
        {
            //Multiplicar la velocidad por el tiempo transcurrido, para no acoplarse al CPU
            float velocidad = jugador.VelocidadCaminar * elapsedTime;

            Vector3 movimiento = jugador.Pelota.Position - jugador.Position;
            movimiento.Normalize();
            jugador.playAnimation(jugador.AnimacionCaminando, true);

            Vector3 lastPos = jugador.Position;
            //La velocidad de movimiento tiene que multiplicarse por el elapsedTime para hacerse independiente de la velocida de CPU
            //Ver Unidad 2: Ciclo acoplado vs ciclo desacoplado
            jugador.move(movimiento);
            //jugador.Rotation = direccion;

            //Detecto las colisiones 
            if (jugador.CollisionManager.DetectarColisiones(jugador.BoundingBox, lastPos))
            {
                //Si hubo colision, restaurar la posicion anterior
                jugador.Position = lastPos;
            }
        }
    }
}