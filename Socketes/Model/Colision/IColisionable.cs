using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.Socketes.Model.Colision
{
    /// <summary>
    /// Todo objeto que puede ser colisionable debe implementar esta interface.
    /// Esta interface se usa desde el CollisionManager para definir los objetos colisionables.
    /// </summary>
    public interface IColisionable
    {
        /// <summary>
        /// Todo objeto colisionable debe retornar el vector que indica el rebote.
        /// 
        /// Se entrega el vector de impacto, para que el mismo internamente 
        /// calcula hacia que lado rebota retornando un vector con la direccion de rebote.
        /// </summary>
        /// <returns></returns>
        Vector3 GetDireccionDeRebote(Vector3 movimiento);

        /// <summary>
        /// Retorna el factor de rebote, para el pasto puede ser mas leve, para los postes puede ser mas fuerte.
        /// </summary>
        /// <returns></returns>
        float GetFuerzaRebote(Vector3 movimiento, float fuerzaRestante);

        /// <summary>
        /// Se debe retornar el boundingbox del objeto colisionable.
        /// </summary>
        /// <returns></returns>
        TgcBoundingBox GetTgcBoundingBox();
    }
}