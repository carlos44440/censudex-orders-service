using Grpc.Net.Client;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using ProductService;
using Microsoft.EntityFrameworkCore;
using orders_service.Src.Data;
using orders_service.Src.DTOs;
using orders_service.Src.Helpers;
using orders_service.Src.Interfaces;
using orders_service.Src.Mappers;
using orders_service.Src.Models;

namespace orders_service.Src.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly Product.ProductClient _productClient;
        public OrderRepository(AppDbContext context)
        {
            _context = context;
            var channel = GrpcChannel.ForAddress("http://localhost:5001");
            _productClient = new Product.ProductClient(channel);
        }

        public async Task<OrderDto?> CreateOrder(List<CreateOrderItemDto> createOrderItemsDtos, string userId)
        {
            //Validacion: El pedido no puede tener dos productos duplicados
            if (createOrderItemsDtos.Select(i => i.ProductId).Distinct().Count() != createOrderItemsDtos.Count())
            {
                throw new Exception("El pedido tiene dos productos duplicados");
            }

            var items = new List<OrderItem>();

            foreach (var itemDto in createOrderItemsDtos)
            {
                //Validacion: La cantidad del producto debe ser mayor a 0
                if (itemDto.Quantity <= 0) throw new Exception("La cantidad debe ser mayor a 0");

                var response = _productClient.GetProductById(new GetProductRequest { Id = itemDto.ProductId });

                //Validacion: El producto debe existir
                if (response == null) throw new Exception($"El producto con la Id {itemDto.ProductId} no existe");

                //Validacion: El precio no puede ser menor o igual a 0
                if (response.Price <= 0) throw new Exception("El precio del producto no es valido");
                
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = itemDto.ProductId,
                    ProductName = response.Name,
                    Quantity = itemDto.Quantity,
                    UnitPrice = (int)response.Price,
                    SubTotal = (int)response.Price * itemDto.Quantity
                };

                items.Add(orderItem);
            }

            var order = new Order
            {
                Id = Guid.NewGuid().ToString(),
                OrderDate = DateTime.UtcNow,
                UserId = userId,
                Items = items,
                Status = "pendiente",
                TotalAmount = items.Sum(item => item.SubTotal),
                TrackingNumber = GenerateNumber.GenerateTrackingNumber(),
                DeliveryDate = DateTime.UtcNow.AddDays(30),
                CancellationReason = null
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order.ToDtoFromOrder();
        }

        public async Task<string?> CheckOrderStatus(string customerId, string orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            //Validacion: No existe un pedido con el id entregado
            if (order == null) throw new Exception("La Id del pedido no corresponde a ningun pedido del cliente");

            return order.Status;
        }

        public async Task<OrderDto?> UpdateOrderStatus(string orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);

            // Validacion: No se encontro el pedido
            if (order == null)
            {
                throw new Exception("No existe ningun pedido con el Id ingresado");
            }

            //Validacion: El estado debe ser un valor valido
            var statusList = new[] { "pendiente", "en procesamiento", "enviado", "entregado", "cancelado" };
            if (!statusList.Contains(status.ToLower()))
            {
                throw new Exception($"El estado debe ser uno de los siguientes valores. Estados validos: {statusList}");
            }

            order.Status = status;
            await _context.SaveChangesAsync();

            return order.ToDtoFromOrder();
        }

        public async Task<OrderDto?> CancelOrder(RequestCancelOrderDto request)
        {
            if (request.UserRole == "Admin")
            {
                var order = await _context.Orders.FindAsync(request.OrderId);

                // Validacion: No se encontro el pedido
                if (order == null)
                {
                    throw new Exception("No existe ningun pedido con el Id ingresado");
                }

                //Validacion: El admin no puede cancelar un pedido que ya fue entregado o cancelado
                if (order.Status == "entregado" || order.Status == "cancelado")
                {
                    throw new Exception("No es posible cancelar un pedido que ya fue entregado o cancelado");
                }

                order.Status = "cancelado";
                order.CancellationReason = request.CancellationReason;
                await _context.SaveChangesAsync();

                return order.ToDtoFromOrder();
            }
            else if (request.UserRole == "Client")
            {
                var order = _context.Orders.Where(o => o.UserId == request.UserId && o.Id == request.OrderId).ElementAt(0);

                // Validacion: No se encontro el pedido
                if (order == null)
                {
                    throw new Exception("El cliente no tiene ningun pedido con el Id ingresado");
                }

                // Validacion: El cliente no puedo cancelar un pedido si se sobrepaso la fecha limite de reembolso
                var refundDealine = order.OrderDate.AddDays(15);
                if (DateTime.UtcNow > refundDealine)
                {
                    throw new Exception($"La fecha limite de reembolso fue sobrepasada. Fecha limite: {refundDealine.ToLocalTime():dd/MM/yyyy}");
                }

                //Validacion: El cliente no puede cancelar un pedido que ya fue enviado, entregado o cancelado
                if (order.Status == "enviado" || order.Status == "entregado" || order.Status == "cancelado")
                {
                    throw new Exception("No es posible cancelar un pedido que ya fue enviado, entregado o cancelado");
                }

                order.Status = "cancelado";
                order.CancellationReason = request.CancellationReason;
                await _context.SaveChangesAsync();

                return order.ToDtoFromOrder();
            }
            else
            {
                throw new Exception("Rol de usuario desconocido");
            }
        }

        public async Task<List<OrderDto>?> GetOrders(string userId, string userRole, QueryObjectOrder queryObject)
        {
            var orders = _context.Orders.Include(o => o.Items).AsQueryable();

            if (queryObject.OrderId != null)
            {
                orders = orders.Where(o => o.Id == queryObject.OrderId);
            }
            if (queryObject.InitialOrderDate != null && queryObject.FinalOrderDate != null)
            {
                // Validacion: El fecha de inicio no puede ser mayor a la fecha final
                if (queryObject.InitialOrderDate > queryObject.FinalOrderDate) throw new Exception("La fecha de inicio no puede ser mayor a la fecha final");

                orders = orders.Where(o => o.OrderDate > queryObject.InitialOrderDate && o.OrderDate < queryObject.FinalOrderDate);
            }

            if (userRole == "Admin")
            {
                if (queryObject.CustomerId != null)
                {
                    orders = orders.Where(o => o.UserId == queryObject.CustomerId);
                }
            }

            if (userRole == "Client") orders = orders.Where(o => o.UserId == userId);

            if(userRole != "Admin" && userRole != "Client")
            {
                throw new Exception("Rol del usuario desconocido");
            }
            
            var ordersDto = await orders.Select(o => o.ToDtoFromOrder()).ToListAsync();

            //Validacion: No se encontro el pedido
            if (!ordersDto.Any()) throw new Exception("No se encontro ningun pedido");

            return ordersDto;
        }
    }
}