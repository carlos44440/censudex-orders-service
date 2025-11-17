using Grpc.Core;
using orders_service.Src.Interfaces;
using orders_service.Src.DTOs;
using orders_service.Src.Helpers;
using orders_service.Src.Messages;
using OrderService;

namespace orders_service.Src.GrpcServices
{
    // Servicio gRPC que expone operaciones CRUD de pedidos
    // y actúa como puente entre clientes externos y el repositorio interno.
    public class OrderGrpcService : Order.OrderBase
    {
        // Dependencia que encapsula la lógica de negocio de los pedidos.
        private readonly IOrderRepository _orderRepository;

        // Utilidad para extraer la información del usuario desde los headers gRPC.
        private readonly UserHeaderExtractor _userHeaderExtractor;

        // Constructor del servicio gRPC.
        // Inyecta el repositorio y el extractor de encabezados de usuario.
        public OrderGrpcService(IOrderRepository orderRepository, UserHeaderExtractor userHeaderExtractor)
        {
            _orderRepository = orderRepository;
            _userHeaderExtractor = userHeaderExtractor;
        }

        // Endpoint gRPC para crear pedidos.
        // Convierte el request gRPC en DTOs internos y delega la creación al repositorio.
        // Devuelve un OrderResponse estructurado para el cliente gRPC.
        public override async Task<OrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
        {
            try
            {
                var createItems = request.Items.Select(i => new CreateOrderItemDto
                {
                    ProductId = Guid.Parse(i.ProductId),
                    Quantity = i.Quantity
                }).ToList();

                // Extraer los datos del usuario desde el header.
                var userData = _userHeaderExtractor.GetUserData(context);

                // Delegación al repositorio.
                var order = await _orderRepository.CreateOrderAsync(createItems, userData);

                // Construcción de la respuesta gRPC.
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
                    Items =
                    {
                        order.Items.Select(i => new OrderItem
                        {
                            ProductId = i.ProductId.ToString(),
                            ProductName = i.ProductName,
                            Quantity = i.Quantity,
                            UnitPrice = (int)i.UnitPrice,
                            SubTotal = (int)i.SubTotal
                        })
                    }
                };
            }
            catch (Exception ex)
            {
                // Envuelve cualquier error en una excepción RPC estándar.
                throw new RpcException(new Status(StatusCode.Internal, $"Error creating order: {ex.Message}"));
            }
        }

        // Endpoint para consultar el estado actual de un pedido.
        // Utiliza el repositorio y devuelve un simple OrderStatusResponse.
        public override async Task<OrderStatusResponse> CheckOrderStatus(CheckOrderStatusRequest request, ServerCallContext context)
        {
            try
            {
                // Obtener el id del usuario desde el header.
                var customerId = _userHeaderExtractor.GetUserId(context);

                var result = await _orderRepository.CheckOrderStatusAsync(customerId, Guid.Parse(request.OrderId));

                return new OrderStatusResponse { Status = result };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error checking order status: {ex.Message}"));
            }
        }

        // Endpoint para actualizar el estado de un pedido.
        // Se valida y aplica el nuevo estado mediante el repositorio.
        public override async Task<OrderResponse> UpdateOrderStatus(UpdateOrderStatusRequest request, ServerCallContext context)
        {
            try
            {
                var updated = await _orderRepository.UpdateOrderStatusAsync(
                    Guid.Parse(request.OrderId), request.Status);

                // Mapeo del DTO al mensaje gRPC de respuesta.
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
                    Items =
                    {
                        updated.Items.Select(i => new OrderItem
                        {
                            ProductId = i.ProductId.ToString(),
                            ProductName = i.ProductName,
                            Quantity = i.Quantity,
                            UnitPrice = (int)i.UnitPrice,
                            SubTotal = (int)i.SubTotal
                        })
                    }
                };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error updating order status: {ex.Message}"));
            }
        }

        // Endpoint para cancelar pedidos.
        // Soporta lógica diferente según el rol del usuario (Admin / Client).
        // Devuelve el pedido ya cancelado.
        public override async Task<OrderResponse> CancelOrder(CancelOrderRequest request, ServerCallContext context)
        {
            try
            {
                var requestCancelOrderDto = new RequestCancelOrderDto
                {
                    OrderId = Guid.Parse(request.RequestCancelOrder.OrderId),
                    CancellationReason = request.RequestCancelOrder.CancellationReason
                };

                // Extraer los datos del usuario desde el header.
                var userData = _userHeaderExtractor.GetUserData(context);

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
                    Items =
                    {
                        order.Items.Select(i => new OrderItem
                        {
                            ProductId = i.ProductId.ToString(),
                            ProductName = i.ProductName,
                            Quantity = i.Quantity,
                            UnitPrice = (int)i.UnitPrice,
                            SubTotal = (int)i.SubTotal
                        })
                    }
                };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error cancelling order: {ex.Message}"));
            }
        }

        // Endpoint para obtener pedidos aplicando filtros.
        // Mapea múltiples entidades OrderDto hacia OrderResponse para la respuesta gRPC.
        public override async Task<GetOrdersResponse> GetOrders(GetOrdersRequest request, ServerCallContext context)
        {
            try
            {
                var query = new Helpers.QueryObjectOrder
                {
                    OrderId = Guid.TryParse(request.QueryObject.OrderId, out var cId) ? cId : Guid.Empty,
                    CustomerId = Guid.TryParse(request.QueryObject.CustomerId, out var oId) ? oId : Guid.Empty,
                    InitialOrderDate = string.IsNullOrEmpty(request.QueryObject.InitialOrderDate)
                        ? null
                        : DateTime.ParseExact(
                            request.QueryObject.InitialOrderDate,
                            "O",
                            System.Globalization.CultureInfo.InvariantCulture
                        ),
                    FinalOrderDate = string.IsNullOrEmpty(request.QueryObject.FinalOrderDate)
                        ? null
                        : DateTime.ParseExact(
                            request.QueryObject.FinalOrderDate,
                            "O",
                            System.Globalization.CultureInfo.InvariantCulture
                        )
                };

                // Extraer los datos del usuario desde el header.
                var userData = _userHeaderExtractor.GetUserData(context);

                var orders = await _orderRepository.GetOrdersAsync(query, userData);

                // Construcción de la lista de respuestas gRPC.
                var response = new GetOrdersResponse();
                response.OrderDto.AddRange(
                    orders.Select(o => new OrderResponse
                    {
                        Id = o.Id.ToString(),
                        OrderDate = o.OrderDate.ToString("O"),
                        UserId = o.UserId.ToString(),
                        Status = o.Status,
                        TotalAmount = (int)o.TotalAmount,
                        TrackingNumber = o.TrackingNumber ?? "",
                        DeliveryDate = o.DeliveryDate?.ToString("O") ?? "",
                        CancellationReason = o.CancellationReason ?? "",
                        Items =
                        {
                            o.Items.Select(i => new OrderItem
                            {
                                ProductId = i.ProductId.ToString(),
                                ProductName = i.ProductName,
                                Quantity = i.Quantity,
                                UnitPrice = (int)i.UnitPrice,
                                SubTotal = (int)i.SubTotal
                            })
                        }
                    })
                );

                return response;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error fetching orders: {ex.Message}"));
            }
        }
    }
}
