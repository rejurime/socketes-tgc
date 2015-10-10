namespace AlumnoEjemplos.Socketes.Model.Colision
{
    /// <summary>
    /// Todo objeto que puede ser colisionable con la pelota debe implementar esta interface.
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