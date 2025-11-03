using orders_service.Src.DTOs;
using orders_service.Src.Helpers;

namespace orders_service.Src.Interfaces
{
    public interface IOrderRepository
    {
        Task<OrderDto> CreateOrderAsync(List<CreateOrderItemDto> createOrderItemsDtos, UserDataDto userData);
        Task<string> CheckOrderStatusAsync(string customerId, string orderId);
        Task<OrderDto> UpdateOrderStatusAsync(string orderId, string status);
        Task<OrderDto> CancelOrderAsync(RequestCancelOrderDto cancelOrder, UserDataDto userData);
        Task<List<OrderDto>> GetOrdersAsync(QueryObjectOrder queryObject, UserDataDto userData);
    }
}