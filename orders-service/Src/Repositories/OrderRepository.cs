using orders_service.Src.DTOs;
using orders_service.Src.Helpers;
using orders_service.Src.Interfaces;
using orders_service.Src.Models;

namespace orders_service.Src.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        public Task<Order?> createOrder(CreateOrderDto createOrderDto)
        {
            throw new NotImplementedException();
        }

        public Task<List<CheckOrderStatusDto>?> checkOrderStatus(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<Order?> updateOrderStatus(string orderId, string status)
        {
            throw new NotImplementedException();
        }

        public Task<Order?> cancelOrder(string orderId, string? cancellationReason)
        {
            throw new NotImplementedException();
        }

        public Task<List<Order>?> getOrders(QueryObjectOrder queryObject)
        {
            throw new NotImplementedException();
        }
    }
}