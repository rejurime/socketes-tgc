using System;

namespace AlumnoEjemplos.Socketes.Model.JugadorStrategy
{
    public interface IJugadorMoveStrategy
    {
		void Move(Jugador jugador, float elapsedTime);
    }
}
