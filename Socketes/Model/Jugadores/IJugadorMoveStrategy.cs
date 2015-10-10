namespace AlumnoEjemplos.Socketes.Model.Jugadores
{
    /// <summary>
    /// Interfaz para la logica del movimiento del Jugador ya que los jugadores se pueden mover por Inputs o IA
    /// </summary>
    public interface IJugadorMoveStrategy
    {
        /// <summary>
        /// Se invoca cuando el jugador se tiene que mover sin pelota
        /// </summary>
        /// <param name="jugador"> El Jugador que se va a mover</param>
        /// <param name="elapsedTime"> Tiempo en segundos transcurridos desde el último frame</param>
        void Move(Jugador jugador, float elapsedTime);

        /// <summary>
        /// Se invoca cuando el jugador se tiene que mover con pelota
        /// </summary>
        /// <param name="jugador"></param>
        /// <param name="elapsedTimePelota"> Tiempo en segundos transcurridos desde el último frame</param>
        /// <param name="pelota"> La Pelota que se tiene dominada</param>
        void PelotaDominada(Jugador jugador, float elapsedTimePelota, Pelota pelota);
    }
}