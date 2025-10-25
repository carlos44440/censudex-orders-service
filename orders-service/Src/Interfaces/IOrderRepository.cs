using orders_service.Src.DTOs;
using orders_service.Src.Helpers;
using orders_service.Src.Models;

namespace orders_service.Src.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> createOrder(CreateOrderDto createOrderDto);
        Task<List<CheckOrderStatusDto>?> checkOrderStatus(string Id);
        Task<Order?> updateOrderStatus(string orderId, string status);
        Task<Order?> cancelOrder(string orderId, string? cancellationReason);
        Task<List<Order>?> getOrders(QueryObjectOrder queryObject);
    }
}