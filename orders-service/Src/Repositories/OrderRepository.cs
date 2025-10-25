using orders_service.Src.DTOs;
using orders_service.Src.Helpers;
using orders_service.Src.Interfaces;
using orders_service.Src.Models;

namespace orders_service.Src.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        public async Task<Order?> createOrder(List<OrderItemDto> orderItemsDtos, string customerId)
        {
            throw new NotImplementedException();
        }

        public Task<List<CheckOrderStatusDto>?> checkOrderStatus(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDto?> updateOrderStatus(string orderId, string status)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDto?> cancelOrder(string orderId, string? cancellationReason)
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderDto>?> getOrders(QueryObjectOrder queryObject)
        {
            throw new NotImplementedException();
        }
    }
}