using orders_service.Src.DTOs;
using orders_service.Src.Models;

namespace orders_service.Src.Mappers
{
    /// <summary>
    /// Clase estática encargada de mapear entidades del dominio a sus respectivos DTOs.
    /// </summary>
    public static class OrderMapper
    {
        /// <summary>
        /// Convierte una entidad <see cref="OrderItem"/> en su representación <see cref="OrderItemDto"/>.
        /// </summary>
        /// <param name="orderItem">Entidad del ítem del pedido.</param>
        /// <returns>Retorna un <see cref="OrderItemDto"/> con los datos mapeados.</returns>
        /// <exception cref="ArgumentNullException">
        /// Se lanza cuando el objeto <paramref name="orderItem"/> es nulo.
        /// </exception>
        public static OrderItemDto ToDtoFromOrderItem(this OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem), "El ítem del pedido no puede ser nulo.");

            return new OrderItemDto
            {
                ProductId = orderItem.ProductId,
                ProductName = orderItem.ProductName,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                SubTotal = orderItem.SubTotal,
            };
        }

        /// <summary>
        /// Convierte una entidad <see cref="Order"/> en su representación <see cref="OrderDto"/>.
        /// </summary>
        /// <param name="order">Entidad del pedido.</param>
        /// <returns>Retorna un <see cref="OrderDto"/> con los datos mapeados.</returns>
        /// <exception cref="ArgumentNullException">
        /// Se lanza cuando el objeto <paramref name="order"/> es nulo.
        /// </exception>
        public static OrderDto ToDtoFromOrder(this Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order), "El pedido no puede ser nulo.");

            return new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                UserId = order.UserId,
                Items = order.Items.Select(ToDtoFromOrderItem).ToList(),
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                TrackingNumber = order.TrackingNumber,
                DeliveryDate = order.DeliveryDate,
                CancellationReason = order.CancellationReason
            };
        }
    }
}
