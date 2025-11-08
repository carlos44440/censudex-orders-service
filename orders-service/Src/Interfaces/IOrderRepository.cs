using orders_service.Src.DTOs;
using orders_service.Src.Helpers;

namespace orders_service.Src.Interfaces
{
    public interface IOrderRepository
    {
        Task<OrderDto> CreateOrderAsync(List<CreateOrderItemDto> createOrderItemsDtos, UserDataDto userData);
        Task<string> CheckOrderStatusAsync(Guid customerId, Guid orderId);
        Task<OrderDto> UpdateOrderStatusAsync(Guid orderId, string status);
        Task<OrderDto> CancelOrderAsync(RequestCancelOrderDto cancelOrder, UserDataDto userData);
        Task<List<OrderDto>> GetOrdersAsync(QueryObjectOrder queryObject, UserDataDto userData);
    }
}