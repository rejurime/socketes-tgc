using Microsoft.DirectX;

namespace AlumnoEjemplos.Socketes.Fisica
{
    /// <summary>
    /// Interface para implementar los tiros
    /// </summary>
    public interface ITiro
    {
        /// <summary>
        /// Retoran el siguiente movimiento del tiro
        /// </summary>
        /// <param name="elapsedTime"></param>
        /// <returns></returns>
        Vector3 siguienteMovimiento(float elapsedTime);

        /// <summary>
        /// Determinar si se termino el movimiento
        /// </summary>
        /// <returns></returns>
        bool hayMovimiento();
        
        /// <summary>
        /// Retorna la fuerza que le queda el tiro, 
        /// esto se usa para el caso de la pelota para determinar la rotacion, 
        /// hay que ver si esta bien en la interface
        /// </summary>
        /// <returns></returns>
        float getFuerza();
    }
}