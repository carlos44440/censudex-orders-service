using orders_service.Src.DTOs;
using orders_service.Src.Helpers;

namespace orders_service.Src.Interfaces
{
    /// <summary>
    /// Interfaz que define las operaciones para la gestión de pedidos dentro del sistema.
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Crea un nuevo pedido a partir de los productos seleccionados por el usuario.
        /// </summary>
        /// <param name="createOrderItemsDtos">Lista de ítems que conforman el pedido.</param>
        /// <param name="userData">Información del usuario que realiza el pedido.</param>
        /// <returns>Retorna un <see cref="OrderDto"/> con los datos del pedido creado.</returns>
        Task<OrderDto> CreateOrderAsync(List<CreateOrderItemDto> createOrderItemsDtos, UserDataDto userData);

        /// <summary>
        /// Consulta el estado actual de un pedido asociado a un usuario.
        /// </summary>
        /// <param name="customerId">Identificador del cliente.</param>
        /// <param name="orderId">Identificador del pedido.</param>
        /// <returns>Retorna una cadena con el estado del pedido.</returns>
        Task<string> CheckOrderStatusAsync(Guid customerId, Guid orderId);

        /// <summary>
        /// Actualiza el estado de un pedido existente.
        /// </summary>
        /// <param name="orderId">Identificador del pedido.</param>
        /// <param name="status">Nuevo estado que se asignará al pedido.</param>
        /// <returns>Retorna el pedido actualizado como <see cref="OrderDto"/>.</returns>
        Task<OrderDto> UpdateOrderStatusAsync(Guid orderId, string status);

        /// <summary>
        /// Cancela un pedido existente, registrando el motivo de cancelación.
        /// </summary>
        /// <param name="cancelOrder">Objeto con la información necesaria para cancelar el pedido.</param>
        /// <param name="userData">Información del usuario que solicita la cancelación.</param>
        /// <returns>Retorna el pedido cancelado como <see cref="OrderDto"/>.</returns>
        Task<OrderDto> CancelOrderAsync(RequestCancelOrderDto cancelOrder, UserDataDto userData);

        /// <summary>
        /// Obtiene una lista de pedidos filtrados según criterios específicos.
        /// </summary>
        /// <param name="queryObject">Parámetros de búsqueda y filtrado.</param>
        /// <param name="userData">Información del usuario que realiza la consulta.</param>
        /// <returns>Retorna una lista de pedidos en formato <see cref="OrderDto"/>.</returns>
        Task<List<OrderDto>> GetOrdersAsync(QueryObjectOrder queryObject, UserDataDto userData);
    }
}
