using Grpc.Core;
using orders_service.Src.Interfaces;
using orders_service.Src.DTOs;
using orders_service.Src.Helpers;
using orders_service.Src.Messages;
using OrderService;

namespace orders_service.Src.GrpcServices
{
    public class OrderGrpcService : Order.OrderBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly UserHeaderExtractor _userHeaderExtractor;

        public OrderGrpcService(IOrderRepository orderRepository, UserHeaderExtractor userHeaderExtractor)
        {
            _orderRepository = orderRepository;
            _userHeaderExtractor = userHeaderExtractor;
        }

        public override async Task<OrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
        {
            try
            {
                var createItems = request.Items.Select(i => new CreateOrderItemDto
                {
                    ProductId = Guid.Parse(i.ProductId),
                    Quantity = i.Quantity
                }).ToList();

                // Implementacion final
                // var userData = _userHeaderExtractor.GetUserData(context);

                // Para pruebas
                var userData = new UserDataDto
                {
                    Id = Guid.Parse(request.UserData.Id),
                    Name = request.UserData.Name,
                    Role = request.UserData.Role,
                    EmailAddress = request.UserData.EmailAddress
                };

                var order = await _orderRepository.CreateOrderAsync(createItems, userData);

                return new OrderResponse
                {
                    Id = order.Id.ToString(),
                    OrderDate = order.OrderDate.ToString("O"),
                    UserId = order.UserId.ToString(),
                    Status = order.Status,
                    TotalAmount = (int)order.TotalAmount,
                    TrackingNumber = order.TrackingNumber ?? "",
                    DeliveryDate = order.DeliveryDate?.ToString("O") ?? "",
                    CancellationReason = order.CancellationReason ?? "",
                    Items = { order.Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId.ToString(),
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
                // Implementacion final
                // var customerId = _userHeaderExtractor.GetUserId(context);
                // var result = await _orderRepository.CheckOrderStatusAsync(customerId, request.OrderId);
                
                // Para pruebas
                var result = await _orderRepository.CheckOrderStatusAsync(Guid.Parse(request.CustomerId), Guid.Parse(request.OrderId));

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
                var updated = await _orderRepository.UpdateOrderStatusAsync(Guid.Parse(request.OrderId), request.Status);

                return new OrderResponse
                {
                    Id = updated.Id.ToString(),
                    OrderDate = updated.OrderDate.ToString("O"),
                    UserId = updated.UserId.ToString(),
                    Status = updated.Status,
                    TotalAmount = (int)updated.TotalAmount,
                    TrackingNumber = updated.TrackingNumber ?? "",
                    DeliveryDate = updated.DeliveryDate?.ToString("O") ?? "",
                    CancellationReason = updated.CancellationReason ?? "",
                    Items = { updated.Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId.ToString(),
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
                var requestCancelOrderDto = new RequestCancelOrderDto
                {
                    OrderId = Guid.Parse(request.RequestCancelOrder.OrderId),
                    CancellationReason = request.RequestCancelOrder.CancellationReason
                };

                // Implementacion final
                // var userData = _userHeaderExtractor.GetUserData(context);

                // Para pruebas
                var userData = new UserDataDto
                {
                    Id = Guid.Parse(request.UserData.Id),
                    Name = request.UserData.Name,
                    Role = request.UserData.Role,
                    EmailAddress = request.UserData.EmailAddress
                };

                var order = await _orderRepository.CancelOrderAsync(requestCancelOrderDto, userData);

                return new OrderResponse
                {
                    Id = order.Id.ToString(),
                    OrderDate = order.OrderDate.ToString("O"),
                    UserId = order.UserId.ToString(),
                    Status = order.Status,
                    TotalAmount = (int)order.TotalAmount,
                    TrackingNumber = order.TrackingNumber ?? "",
                    DeliveryDate = order.DeliveryDate?.ToString("O") ?? "",
                    CancellationReason = order.CancellationReason ?? "",
                    Items = { order.Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId.ToString(),
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
                var query = new Helpers.QueryObjectOrder
                {
                    OrderId = Guid.Parse(request.QueryObject.OrderId),
                    CustomerId = Guid.Parse(request.QueryObject.CustomerId),
                    InitialOrderDate = string.IsNullOrEmpty(request.QueryObject.InitialOrderDate)
                        ? null
                        : DateTime.Parse(request.QueryObject.InitialOrderDate),
                    FinalOrderDate = string.IsNullOrEmpty(request.QueryObject.FinalOrderDate)
                        ? null
                        : DateTime.Parse(request.QueryObject.FinalOrderDate)
                };

                // Implementacion final
                // var userData = _userHeaderExtractor.GetUserData(context);

                // Para pruebas
                var userData = new UserDataDto
                {
                    Id = Guid.Parse(request.UserData.Id),
                    Name = request.UserData.Name,
                    Role = request.UserData.Role,
                    EmailAddress = request.UserData.EmailAddress
                };

                var orders = await _orderRepository.GetOrdersAsync(query, userData);

                var response = new GetOrdersResponse();
                response.OrderDto.AddRange(orders.Select(o => new OrderResponse
                {
                    Id = o.Id.ToString(),
                    OrderDate = o.OrderDate.ToString("O"),
                    UserId = o.UserId.ToString(),
                    Status = o.Status,
                    TotalAmount = (int)o.TotalAmount,
                    TrackingNumber = o.TrackingNumber ?? "",
                    DeliveryDate = o.DeliveryDate?.ToString("O") ?? "",
                    CancellationReason = o.CancellationReason ?? "",
                    Items = { o.Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId.ToString(),
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