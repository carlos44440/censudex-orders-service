using orders_service.Src.DTOs;
using orders_service.Src.Helpers;
using orders_service.Src.Models;

namespace orders_service.Src.Interfaces
{
    public interface IOrderRepository
    {
        Task<OrderDto?> CreateOrder(List<OrderItemDto> orderItemsDtos, string customerId);
        Task<List<CheckOrderStatusDto>?> CheckOrderStatus(string customerId, string orderId);
        Task<OrderDto?> UpdateOrderStatus(string orderId, string status);
        Task<OrderDto?> CancelOrderClient(CancelOrderClientDto cancelOrderClient);
        Task<OrderDto?> CancelOrderAdmin(string orderId, string? cancellationReason);
        Task<List<OrderDto>?> GetOrdersClient(string customerId, QueryObjectOrder queryObject);
        Task<List<OrderDto>?> GetOrdersAdmin(QueryObjectOrderAdmin queryObject);
    }
}