using AlumnoEjemplos.Socketes.Model;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.Socketes.Collision
{
    /// <summary>
    /// Todo objeto que puede ser colisionable debe implementar esta interface.
    /// 
    /// Esta interface se usa desde el CollisionManager para definir los objetos colisionables.
    /// 
    /// </summary>
    public interface IColisionable
    {
        /// <summary>
        /// Le informa al objeto colisonable que colisiono con algo, 
        /// se pasa ese objeto para que se puede hacer la logica necesaria.
        /// </summary>
        /// <param name="objetoColisionado"></param>
        void ColisionasteConPelota(Pelota pelota);

        /// <summary>
        /// Todo objeto colisionable debe retornar el vector que indica el rebote.
        /// 
        /// Se entrega el vector de impacto, para que el mismo internamente 
        /// calcula hacia que lado rebota retornando un vector con la direccion de rebote.
        /// 
        /// </summary>
        /// <returns></returns>
        Vector3 GetDireccionDeRebote(Vector3 movimiento);

        /// <summary>
        /// 
        /// Retorna el factor de rebote, para el pasto puede ser mas leve, para los postes puede ser mas fuerte.
        /// 
        /// </summary>
        /// <returns></returns>
        float GetFuerzaRebote(Vector3 movimiento);

        /// <summary>
        /// Se debe retornar el boundingbox del objeto colisionable.
        /// </summary>
        /// <returns></returns>
        TgcBoundingBox GetTgcBoundingBox();
    }
}