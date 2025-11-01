using orders_service.Src.DTOs;
using orders_service.Src.Models;

namespace orders_service.Src.Mappers
{
    public static class OrderMapper
    {
        public static OrderItemDto ToDtoFromOrderItem(this OrderItem orderItem)
        {
            return new OrderItemDto
            {
                ProductId = orderItem.ProductId,
                ProductName = orderItem.ProductName,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                SubTotal = orderItem.SubTotal,
            };
        }

        public static OrderDto ToDtoFromOrder(this Order order)
        {
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