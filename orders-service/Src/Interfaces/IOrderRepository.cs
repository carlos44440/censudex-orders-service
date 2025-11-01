using orders_service.Src.DTOs;
using orders_service.Src.Helpers;

namespace orders_service.Src.Interfaces
{
    public interface IOrderRepository
    {
        Task<OrderDto> CreateOrder(List<CreateOrderItemDto> createOrderItemsDtos, string userId);
        Task<string> CheckOrderStatus(string customerId, string orderId);
        Task<OrderDto> UpdateOrderStatus(string orderId, string status);
        Task<OrderDto> CancelOrder(RequestCancelOrderDto cancelOrder);
        Task<List<OrderDto>> GetOrders(string userId, string userRole, QueryObjectOrder queryObject);
    }
}