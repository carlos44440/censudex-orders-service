using Grpc.Net.Client;
using ProductService;
using Microsoft.EntityFrameworkCore;
using orders_service.Src.Data;
using orders_service.Src.DTOs;
using orders_service.Src.Helpers;
using orders_service.Src.Interfaces;
using orders_service.Src.Mappers;
using orders_service.Src.Models;
using MassTransit;
using Shared.OrderCreatedMessage;
using orders_service.Src.Messages;

namespace orders_service.Src.Repositories
{
    /// <summary>
    /// Repositorio para la gestión de pedidos.
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        /// <summary>
        /// Contexto de base de datos.
        /// </summary>
        private readonly AppDbContext _context;
        /// <summary>
        /// Cliente gRPC del Product Service.
        /// </summary>
        private readonly Product.ProductClient _productClient;
        /// <summary>
        /// Servicio de envio de correos electrónicos.
        /// </summary>
        private readonly IEmailService _emailService;
        /// <summary>
        /// Endpoint para publicación de eventos de RabbitMQ.
        /// </summary>
        private readonly IPublishEndpoint _publishEndpoint;
        /// <summary>
        /// Instancia del repositorio de pedidos.
        /// </summary>
        /// <param name="context">Contexto de la base de datos.</param>
        /// <param name="emailService">Servicio de envio de correos electrónicos.</param>
        /// <param name="publishEndpoint">Endpoint para publicación de eventos de RabbitMQ.</param>
        public OrderRepository(AppDbContext context, IEmailService emailService, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            var channel = GrpcChannel.ForAddress(Environment.GetEnvironmentVariable("PRODUCT_SERVICE_URL")
                ?? throw new Exception("No se encontro la direccion del servicio de productos"));
            _productClient = new Product.ProductClient(channel);
            _emailService = emailService;
            _publishEndpoint = publishEndpoint;
        }

        /// <summary>
        /// Función para crear pedidos.
        /// </summary>
        /// <param name="createOrderItemsDtos">Artículos del pedido.</param>
        /// <param name="userData">Información del usuario.</param>
        /// <returns>Retorna el pedido creado.</returns>
        /// <exception cref="Exception">
        /// Se lanza cuando:
        /// <list type="bullet">
        /// <item>El pedido contiene productos duplicados.</item>
        /// <item>Algún producto posee un ID inválido.</item>
        /// <item>La cantidad solicitada es menor o igual a cero.</item>
        /// <item>El producto no existe o sus datos no son válidos (nombre o precio).</item>
        /// </list>
        /// </exception>
        public async Task<OrderDto> CreateOrderAsync(List<CreateOrderItemDto> createOrderItemsDtos, UserDataDto userData)
        {
            //Validacion: El pedido no puede tener dos productos duplicados
            if (createOrderItemsDtos.Select(i => i.ProductId).Distinct().Count() != createOrderItemsDtos.Count())
            {
                throw new Exception("El pedido tiene dos productos duplicados");
            }

            var items = new List<OrderItem>();

            foreach (var itemDto in createOrderItemsDtos)
            {
                // Validacion: La id del producto no debe ser nula o vacia
                if (itemDto.ProductId == Guid.Empty) throw new Exception("La id del producto es nula o vacia");

                //Validacion: La cantidad del producto debe ser mayor a 0
                if (itemDto.Quantity <= 0) throw new Exception("La cantidad debe ser mayor a 0");

                // En espera: Se debe contar con el servicio de producto activo
                // var response = _productClient.GetProductById(new GetProductRequest { Id = itemDto.ProductId });

                // Implementacion para pruebas
                var product = new
                {
                    Id = itemDto.ProductId,
                    Name = "Product",
                    Price = 12,
                };

                //Validacion: El producto debe existir
                if (product == null) throw new Exception($"El producto con la Id {itemDto.ProductId} no existe");

                //Validacion: El nombre no puede ser nulo o vacio
                if (string.IsNullOrEmpty(product.Name)) throw new Exception("El nombre del producto no es valido");

                //Validacion: El precio no puede ser menor o igual a 0
                if (product.Price <= 0) throw new Exception("El precio del producto no es valido");

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = itemDto.ProductId,
                    ProductName = product.Name,
                    Quantity = itemDto.Quantity,
                    UnitPrice = (int)product.Price,
                    SubTotal = (int)product.Price * itemDto.Quantity
                };

                items.Add(orderItem);
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderDate = DateTime.UtcNow,
                UserId = userData.Id,
                Items = items,
                Status = "pendiente",
                TotalAmount = items.Sum(item => item.SubTotal),
                TrackingNumber = GenerateNumber.GenerateTrackingNumber(),
                DeliveryDate = DateTime.UtcNow.AddDays(30),
                CancellationReason = null
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderDto = order.ToDtoFromOrder();

            // Enviar correo de confirmacion de creacion del pedido
            string subject = $"Censudex: Confirmación de su compra #{orderDto.Id}";

            var itemsDetails = string.Join("\n", orderDto.Items.Select(item =>
                $"  - Producto: {item.ProductName}\n" +
                $"    Cantidad: {item.Quantity}\n" +
                $"    Precio unitario: ${item.UnitPrice}\n" +
                $"    Subtotal: ${item.SubTotal}\n"
            ));

            string message =
                $"Estimado cliente {userData.Name},\n\n" +
                "Su pedido ha sido creado exitosamente.\n\n" +
                $"Número de pedido: {orderDto.Id}\n\n" +
                "Resumen de compra\n" +
                $"  Items:\n" +
                $"{itemsDetails}\n" +
                $"  Total: ${orderDto.TotalAmount}\n\n" +
                "Detalles de envío\n" +
                $"  Numero de seguimiento: {orderDto.TrackingNumber}\n" +
                $"  Fecha de entrega estimada: {orderDto.DeliveryDate}\n\n" +
                "Gracias por confiar en nosotros.\n" +
                "El equipo de Censudex.";

            var isEmailSent = await _emailService.SendEmailAsync(subject, userData.EmailAddress, message);
            if (!isEmailSent) Console.WriteLine("El email no fue enviado");

            // Mandar mensaje de RabbitMQ: order.created
            var orderItemsMessage = new List<OrderItemMessage> { };

            foreach (var i in createOrderItemsDtos)
            {
                var orderItemM = new OrderItemMessage
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                };
                orderItemsMessage.Add(orderItemM);
            }

            var orderCreatedMessage = new OrderCreatedMessage
            {
                OrderId = orderDto.Id,
                CustomerId = orderDto.UserId,
                Items = orderItemsMessage,
                CreatedAt = orderDto.OrderDate
            };
            await _publishEndpoint.Publish(orderCreatedMessage, context =>
            {
                context.SetRoutingKey("order.created");
            });

            return orderDto;
        }

        /// <summary>
        /// Función para consultar el estado de un pedido.
        /// </summary>
        /// <param name="customerId">Identificador único del cliente.</param>
        /// <param name="orderId">Identificador único del pedido.</param>
        /// <returns>Retorna el estado del pedido.</returns>
        /// <exception cref="Exception">
        /// Se lanza cuando:
        /// <list type="bullet">
        /// <item>La id del cliente es nula o vacia.</item>
        /// <item>La id del pedido es nula o vacia.</item>
        /// <item>La Id del pedido no corresponde a ningun pedido del cliente.</item>
        /// </list>
        /// </exception>
        public async Task<string> CheckOrderStatusAsync(Guid customerId, Guid orderId)
        {
            // Validacion: La id del cliente es nula
            if (customerId == Guid.Empty) throw new Exception("La id del cliente es nula o vacia");

            //Valdacion: La id del pedido es nula
            if (orderId == Guid.Empty) throw new Exception("La id del pedido es nula o vacia");

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == customerId);

            //Validacion: No existe un pedido con el id entregado
            if (order == null) throw new Exception("La Id del pedido no corresponde a ningun pedido del cliente");

            return order.Status;
        }

        /// <summary>
        /// Función para actualizar el estado de un pedido.
        /// </summary>
        /// <param name="orderId">Identificador único del pedido.</param>
        /// <param name="status">Estado del pedido.</param>
        /// <returns>Retorna el pedido creado.</returns>
        /// <exception cref="Exception">
        /// Se lanza cuando:
        /// <list type="bullet">
        /// <item>La id del pedido es nula o vacia.</item>
        /// <item>No existe ningun pedido con el Id ingresado.</item>
        /// <item>El estado no es un valor valido.</item>
        /// </list>
        /// </exception>
        public async Task<OrderDto> UpdateOrderStatusAsync(Guid orderId, string status)
        {
            if (orderId == Guid.Empty) throw new Exception("La id del pedido es nula o vacia");

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
                throw new Exception($"El estado debe ser uno de los siguientes valores. Estados validos: {string.Join(", ", statusList)}");
            }

            order.Status = status.ToLower();
            await _context.SaveChangesAsync();

            // Por implementar: Llamar al serivicio de cliente para obtener los datos del usuario
            // var user = _clientUser.getUser(order.UserId);

            // Por implementar: Si el status se actualiza a "en procesamiento" se notifica al cliente que el pedido esta siendo preparado.
            // if (status.ToLower() == "en procesamiento")
            //{
            //   var subject = $"Censudex: Tu pedido #{order.Id} está siendo preparado";
            //   var message =
            //    $"Hola {user.Name},\n\n" +
            //    $"Queremos informarte que tu pedido #{order.Id} ha pasado al estado 'En procesamiento'.\n" +
            //    $"Nuestro equipo está preparando tus productos para su envío.\n\n" +
            //    "Te notificaremos nuevamente cuando el pedido sea despachado.\n\n" +
            //    "Gracias por tu paciencia y por confiar en nosotros.\n" +
            //    "El equipo de Censudex.";
            //   var isEmailSent = await _emailService.SendEmailAsync(subject, user.Email, message);
            //   if (!isEmailSent) Console.WriteLine("El email no fue enviado");
            //}

            // Por implementar: Si el status se actualiza a "enviado" se envia al cliente el numero de seguimiento y un enlace al transportista. 
            // if (status.ToLower() == "enviado")
            //{
            //   var subject = $"Censudex: Tu pedido #{order.Id} ha sido enviado";
            //   var message = 
            // $"Hola {user.Name},\n\n" +
            // $"Tu pedido #{order.Id} ha sido despachado y se encuentra en camino.\n\n" +
            // $"Número de seguimiento: {order.TrackingNumber}\n\n" +
            // "Gracias por tu preferencia y confianza.\n" +
            // "El equipo de Censudex.";
            //   var isEmailSent = await _emailService.SendEmailAsync(subject, user.Email, message);
            //   if (!isEmailSent) Console.WriteLine("El email no fue enviado");
            //}

            // Por implementar: Si el status se actualiza a "entregado" se envia al cliente una notificacion de confirmacion final.
            // if (status.ToLower() == "entregado")
            //{
            //   var subject = $"Censudex: Tu pedido #{order.Id} ha sido entregado";
            //   var message =
            // $"Hola {user.Name},\n\n" +
            // $"Nos alegra informarte que tu pedido #{order.Id} ha sido entregado exitosamente.\n\n" +
            // "Esperamos que estés satisfecho con tu compra.\n" +
            // "Si tienes algún comentario o deseas evaluar tu experiencia, puedes hacerlo desde tu cuenta en nuestro portal.\n\n" +
            // "Gracias por elegir Censudex.\n" +
            // "El equipo de Censudex.";
            //   var isEmailSent = await _emailService.SendEmailAsync(subject, user.Email, message);
            //   if (!isEmailSent) Console.WriteLine("El email no fue enviado");
            //}

            return order.ToDtoFromOrder();
        }

        /// <summary>
        /// Función para cancelar un pedido.
        /// </summary>
        /// <param name="request">Request para cancelar un pedido.</param>
        /// <param name="userData">Información del usuario.</param>
        /// <returns>Retorna el pedido cancelado.</returns>
        /// <exception cref="Exception">
        /// Se lanza cuando:
        /// <list type="bullet">
        /// <item>No existe ningun pedido con el Id ingresado.</item>
        /// <item>El pedido ya fue entregado o cancelado.</item>
        /// <item>La fecha limite de reembolso fue sobrepasada.</item>
        /// </list>
        /// </exception>
        public async Task<OrderDto> CancelOrderAsync(RequestCancelOrderDto request, UserDataDto userData)
        {
            if (userData.Role == "Admin")
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

                // Por implementar: Llamar al serivicio de cliente para obtener los datos del usuario
                // var user = _clientUser.getUser(order.UserId);

                // En espera: mandar mensaje personalizado en caso de producto sin stock

                // if (request.FailedProducts != null)
                // {
                //     var response = _productClient.GetProducts(new GetProductsRequest { ... });
                //     var subject1 = $"Censudex: Producto sin stock - Recomendaciones para tu pedido";
                //     var recommendedItems = string.Join("\n", response.Select(item =>
                //         $"  - Producto: {item.ProductName}\n" +
                //         $"    Precio unitario: ${item.UnitPrice}\n"
                //     ));
                //     var message1 =
                //         $"Hola {user.Name},\n\n" +
                //         $"Lamentamos informarte que los siguientes productos'{request.FailedProducts}' actualmente no cuenta con stock disponible.\n\n" +
                //         "Sabemos lo importante que es para ti recibir tus productos sin demoras, por lo que te ofrecemos algunas alternativas similares que podrían interesarte:\n\n" +
                //         $"{recommendedItems}\n" +
                //         "Puedes revisar estas opciones y actualizar tu pedido desde tu cuenta en nuestro portal.\n\n" +
                //         "El monto correspondiente será reembolsado automáticamente en las próximas horas.\n\n" +
                //         "Gracias por tu comprensión y por confiar en nosotros.\n" +
                //         "El equipo de Censudex.";
                //     var isEmailSent1 = await _emailService.SendEmailAsync(subject1, user.EmailAddress, message1);
                //     if (!isEmailSent1) Console.WriteLine("El email no fue enviado");
                // }

                // En espera: Se notifica al cliente una confirmacion de la cancelacion de su pedido, con el motivo, y el proceso de reembolso.
                // else {
                // var subject = $"Censudex: Confirmacion de la cancelacion del pedido #{order.Id}";
                // var message =
                //     $"Hola {user.Name},\n\n" +
                //     "Te informamos que tu pedido #" + order.Id + " ha sido cancelado exitosamente.\n\n" +
                //     (string.IsNullOrEmpty(order.CancellationReason) ? "" : $"Motivo de cancelación: {order.CancellationReason}\n\n") +
                //     "En caso de que hayas realizado un pago, el proceso de reembolso se iniciará en las próximas horas. " +
                //     "Dependiendo del método de pago, puede demorar entre 3 a 7 días hábiles.\n\n" +
                //     "Si tienes alguna duda o necesitas más información, puedes contactar a nuestro equipo de soporte " +
                //     "respondiendo este correo o ingresando a tu cuenta en nuestro portal.\n\n" +
                //     "Gracias por confiar en nosotros.\n" +
                //     "El equipo de Censudex";
                // var isEmailSent = await _emailService.SendEmailAsync(subject, user.Email, message);
                // if (!isEmailSent) Console.WriteLine("El email no fue enviado");
                // }
                return order.ToDtoFromOrder();
            }
            else if (userData.Role == "Client")
            {
                var order = _context.Orders.Where(o => o.UserId == userData.Id && o.Id == request.OrderId).ElementAt(0);

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

                // Enviar correo de confirmacion de cancelacion del pedido
                string subject = $"Censudex: Confirmacion de la cancelacion del pedido #{order.Id}";
                string message =
                    $"Hola {userData.Name},\n\n" +
                    "Te informamos que tu pedido #" + order.Id + " ha sido cancelado exitosamente.\n\n" +
                    (string.IsNullOrEmpty(order.CancellationReason) ? "" : $"Motivo de cancelación: {order.CancellationReason}\n\n") +
                    "En caso de que hayas realizado un pago, el proceso de reembolso se iniciará en las próximas horas. " +
                    "Dependiendo del método de pago, puede demorar entre 3 a 7 días hábiles.\n\n" +
                    "Si tienes alguna duda o necesitas más información, puedes contactar a nuestro equipo de soporte " +
                    "respondiendo este correo o ingresando a tu cuenta en nuestro portal.\n\n" +
                    "Gracias por confiar en nosotros.\n" +
                    "El equipo de Censudex";

                var isEmailSent = await _emailService.SendEmailAsync(subject, userData.EmailAddress, message);
                if (!isEmailSent) Console.WriteLine("El email no fue enviado");

                return order.ToDtoFromOrder();
            }
            else
            {
                throw new Exception("Rol de usuario desconocido");
            }
        }

        /// <summary>
        /// Función para obtener pedidos.
        /// </summary>
        /// <param name="queryObject">Filtros para los pedidos.</param>
        /// <param name="userData">Información del usuario.</param>
        /// <returns>Retorna lista de pedidos.</returns>
        /// <exception cref="Exception">
        /// Se lanza cuando:
        /// <list type="bullet">
        /// <item>No se encontro ningun pedido.</item>
        /// </list>
        /// </exception>
        public async Task<List<OrderDto>> GetOrdersAsync(QueryObjectOrder queryObject, UserDataDto userData)
        {
            var orders = _context.Orders.Include(o => o.Items).AsQueryable();

            if (queryObject.OrderId != Guid.Empty)
            {
                orders = orders.Where(o => o.Id == queryObject.OrderId);
            }
            if (queryObject.InitialOrderDate != null && queryObject.FinalOrderDate != null)
            {
                // Validacion: El fecha de inicio no puede ser mayor a la fecha final
                if (queryObject.InitialOrderDate > queryObject.FinalOrderDate) throw new Exception("La fecha de inicio no puede ser mayor a la fecha final");

                orders = orders.Where(o => o.OrderDate > queryObject.InitialOrderDate && o.OrderDate < queryObject.FinalOrderDate);
            }

            if (userData.Role == "Admin")
            {
                if (queryObject.CustomerId != Guid.Empty)
                {
                    orders = orders.Where(o => o.UserId == queryObject.CustomerId);
                }
            }

            if (userData.Role == "Client") orders = orders.Where(o => o.UserId == userData.Id);

            if (userData.Role != "Admin" && userData.Role != "Client")
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