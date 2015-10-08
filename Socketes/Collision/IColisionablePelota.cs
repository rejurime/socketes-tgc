using AlumnoEjemplos.Socketes.Model;

namespace AlumnoEjemplos.Socketes.Collision
{
    /// <summary>
    /// Todo objeto que puede ser colisionable debe implementar esta interface.
    /// Esta interface se usa desde el CollisionManager para definir los objetos colisionables.
    /// </summary>
    public interface IColisionablePelota : IColisionable
    {
        /// <summary>
        /// Le informa al objeto colisonable que colisiono con algo, 
        /// se pasa ese objeto para que se puede hacer la logica necesaria.
        /// </summary>
        /// <param name="objetoColisionado"></param>
        void ColisionasteConPelota(Pelota pelota);
    }
}