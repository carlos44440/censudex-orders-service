using Grpc.Core;
using orders_service.Src.Interfaces;
using orders_service.Src.DTOs;
using orders_service.Src.Helpers;
using OrderService;

namespace orders_service.Src.GrpcServices
{
    public class OrderGrpcService : Order.OrderBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderGrpcService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public override async Task<OrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
        {
            try
            {
                var createItems = request.Items.Select(i => new CreateOrderItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList();

                var order = await _orderRepository.CreateOrder(createItems, request.UserId);

                return new OrderResponse
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate.ToString("O"),
                    UserId = order.UserId,
                    Status = order.Status,
                    TotalAmount = (int)order.TotalAmount,
                    TrackingNumber = order.TrackingNumber ?? "",
                    DeliveryDate = order.DeliveryDate?.ToString("O") ?? "",
                    CancellationReason = order.CancellationReason ?? "",
                    Items = { order.Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = (int)i.UnitPrice,
                        SubTotal = (int)i.SubTotal
                    }) }
                };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error creating order: {ex.Message}"));
            }
        }

        public override async Task<OrderStatusResponse> CheckOrderStatus(CheckOrderStatusRequest request, ServerCallContext context)
        {
            try
            {
                var result = await _orderRepository.CheckOrderStatus(request.CustomerId, request.OrderId);

                return new OrderStatusResponse
                {
                    Status = result
                };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error checking order status: {ex.Message}"));
            }
        }

        public override async Task<OrderResponse> UpdateOrderStatus(UpdateOrderStatusRequest request, ServerCallContext context)
        {
            try
            {
                var updated = await _orderRepository.UpdateOrderStatus(request.OrderId, request.Status);

                return new OrderResponse
                {
                    Id = updated.Id,
                    OrderDate = updated.OrderDate.ToString("O"),
                    UserId = updated.UserId,
                    Status = updated.Status,
                    TotalAmount = (int)updated.TotalAmount,
                    TrackingNumber = updated.TrackingNumber ?? "",
                    DeliveryDate = updated.DeliveryDate?.ToString("O") ?? "",
                    CancellationReason = updated.CancellationReason ?? "",
                    Items = { updated.Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = (int)i.UnitPrice,
                        SubTotal = (int)i.SubTotal
                    }) }
                };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error updating order status: {ex.Message}"));
            }
        }

        public override async Task<OrderResponse> CancelOrder(CancelOrderRequest request, ServerCallContext context)
        {
            try
            {
                var cancelOrderDto = new RequestCancelOrderDto
                {
                    UserId = request.UserId,
                    UserRole = request.UserRole,
                    OrderId = request.OrderId,
                    CancellationReason = request.CancellationReason
                };

                var order = await _orderRepository.CancelOrder(cancelOrderDto);

                return new OrderResponse
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate.ToString("O"),
                    UserId = order.UserId,
                    Status = order.Status,
                    TotalAmount = (int)order.TotalAmount,
                    TrackingNumber = order.TrackingNumber ?? "",
                    DeliveryDate = order.DeliveryDate?.ToString("O") ?? "",
                    CancellationReason = order.CancellationReason ?? "",
                    Items = { order.Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = (int)i.UnitPrice,
                        SubTotal = (int)i.SubTotal
                    }) }
                };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error cancelling order: {ex.Message}"));
            }
        }

        public override async Task<GetOrdersResponse> GetOrders(GetOrdersRequest request, ServerCallContext context)
        {
            try
            {
                if (request.QueryObject == null)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "QueryObject no puede ser nulo."));
                var query = new orders_service.Src.Helpers.QueryObjectOrder
                {
                    OrderId = request.QueryObject.OrderId,
                    CustomerId = request.QueryObject.CustomerId,
                    InitialOrderDate = string.IsNullOrEmpty(request.QueryObject.InitialOrderDate)
                        ? null
                        : DateTime.Parse(request.QueryObject.InitialOrderDate),
                    FinalOrderDate = string.IsNullOrEmpty(request.QueryObject.FinalOrderDate)
                        ? null
                        : DateTime.Parse(request.QueryObject.FinalOrderDate)
                };

                var orders = await _orderRepository.GetOrders(request.UserId, request.UserRole, query);

                var response = new GetOrdersResponse();
                response.OrderDto.AddRange(orders.Select(o => new OrderResponse
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate.ToString("O"),
                    UserId = o.UserId,
                    Status = o.Status,
                    TotalAmount = (int)o.TotalAmount,
                    TrackingNumber = o.TrackingNumber ?? "",
                    DeliveryDate = o.DeliveryDate?.ToString("O") ?? "",
                    CancellationReason = o.CancellationReason ?? "",
                    Items = { o.Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = (int)i.UnitPrice,
                        SubTotal = (int)i.SubTotal
                    }) }
                }));

                return response;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error fetching orders: {ex.Message}"));
            }
        }
    }
}