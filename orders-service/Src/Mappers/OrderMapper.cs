using orders_service.Src.DTOs;
using orders_service.Src.Models;

namespace orders_service.Src.Mappers
{
    public static class OrderMapper
    {
        public static OrderItem ToOrderItemFromDto(OrderItemDto orderItemDto)
        {
            return new OrderItem
            {
                Id = Guid.NewGuid().ToString(),
                ProductId = orderItemDto.ProductId,
                ProductName = orderItemDto.ProductName,
                Quantity = orderItemDto.Quantity,
                UnitPrice = orderItemDto.UnitPrice,
                SubTotal = orderItemDto.Quantity * orderItemDto.UnitPrice,
                OrderId = string.Empty,
            };
        }

        public static OrderItemDto ToDtoFromOrderItem(OrderItem orderItem)
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

        public static OrderDto ToDtoFromOrder(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId,
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