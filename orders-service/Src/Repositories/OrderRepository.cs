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
        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderDto?> CreateOrder(List<OrderItemDto> orderItemsDtos, string customerId)
        {
            var order = new Order
            {
                Id = Guid.NewGuid().ToString(),
                OrderDate = DateTime.UtcNow,
                CustomerId = customerId,
                Items = orderItemsDtos.Select(item => item.ToOrderItemFromDto()).ToList(),
                Status = "pendiente",
                TotalAmount = orderItemsDtos.Sum(item => item.SubTotal),
                TrackingNumber = GenerateNumber.GenerateTrackingNumber(),
                DeliveryDate = DateTime.UtcNow.AddDays(30),
                CancellationReason = null
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order.ToDtoFromOrder();
        }

        public async Task<List<CheckOrderStatusDto>?> CheckOrderStatus(string customerId, string? orderId)
        {
            var orders = _context.Orders.Where(o => o.CustomerId == customerId).AsQueryable();
            
            //Validacion: El usuario no tiene pedidos
            if (orders == null || !orders.Any())
            {
                throw new Exception("El usuario no tiene ningun pedido");
            }

            if (orderId != null)
            {
                var order = orders.Where(o => o.Id == orderId).ElementAt(0);
                var orderStatusDto = new CheckOrderStatusDto
                {
                    OrderId = order.Id,
                    Status = order.Status
                };
                List<CheckOrderStatusDto> orderStatusDtos = new List<CheckOrderStatusDto> { orderStatusDto };
                return orderStatusDtos;
            }
            else
            {
                List<CheckOrderStatusDto> orderStatusDtos = new List<CheckOrderStatusDto>();
                foreach (var x in orders)
                {
                    CheckOrderStatusDto orderStatusDto = new CheckOrderStatusDto
                    {
                        OrderId = x.Id,
                        Status = x.Status
                    };
                    orderStatusDtos.Add(orderStatusDto);
                }
                return orderStatusDtos;
            }
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

        public async Task<OrderDto?> CancelOrderClient(CancelOrderClientDto request)
        {
            var order = _context.Orders.Where(o => o.CustomerId == request.CustomerId && o.Id == request.OrderId).ElementAt(0);

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
        
        public async Task<OrderDto?> CancelOrderAdmin(string orderId, string? cancellationReason)
        {
            var order = await _context.Orders.FindAsync(orderId);

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
            order.CancellationReason = cancellationReason;
            await _context.SaveChangesAsync();

            return order.ToDtoFromOrder();
        }

        public async Task<List<OrderDto>?> GetOrdersClient(string customerId, QueryObjectOrder queryObject)
        {
            var orders = _context.Orders.Where(o => o.CustomerId == customerId).Include(o => o.Items).AsQueryable();

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

            var ordersDto = await orders.Select(o => o.ToDtoFromOrder()).ToListAsync();
            
            //Validacion: No se encontro el pedido
            if (!ordersDto.Any())
            {
                throw new Exception("No se encontro ningun pedido");
            }

            return ordersDto;
        }
        
        public async Task<List<OrderDto>?> GetOrdersAdmin(QueryObjectOrderAdmin queryObject)
        {
            var orders = _context.Orders.Include(o => o.Items).AsQueryable();

            if (queryObject.CustomerId != null)
            {
                orders = orders.Where(o => o.CustomerId == queryObject.CustomerId);
            }

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

            var ordersDto = await orders.Select(o => o.ToDtoFromOrder()).ToListAsync();
            
            //Validacion: No se encontro el pedido
            if (!ordersDto.Any())
            {
                throw new Exception("No se encontro ningun pedido");
            }

            return ordersDto;
        }
    }
}