using MassTransit;
using Microsoft.OpenApi.Extensions;
using orders_service.Src.DTOs;
using orders_service.Src.Interfaces;
using Shared.OrderFailedStockMessage;

namespace ConsumerApi.Consumers
{
    /// <summary>
    /// Consumidor de eventos <c>order.failed.stock</c> enviados por el servicio de inventario.
    /// Este consumidor se encarga de gestionar la cancelación automática de un pedido cuando
    /// no existe stock suficiente para uno o más productos.
    /// </summary>
    public class OrderFailedStockConsumer : IConsumer<OrderFailedStockMessage>
    {
        /// <summary>
        /// Repositorio encargado de la lógica de negocio relacionada con pedidos.
        /// </summary>
        private readonly IOrderRepository _orderRepository;

        /// <summary>
        /// Constructor del consumidor que recibe las dependencias necesarias
        /// mediante inyección de dependencias.
        /// </summary>
        /// <param name="orderRepository">Repositorio de órdenes utilizado para cancelar el pedido.</param>
        public OrderFailedStockConsumer(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        
        /// <summary>
        /// Método que se ejecuta cuando se recibe un mensaje <see cref="OrderFailedStockMessage"/>.
        /// Procesa el fallo de stock, registra el evento y envía la cancelación de la orden al repositorio.
        /// </summary>
        /// <param name="context">Contexto del mensaje recibido desde RabbitMQ.</param>
        public async Task Consume(ConsumeContext<OrderFailedStockMessage> context)
        {
            var message = context.Message;

            // Log básico para depuración: muestra hora, ID del pedido y razón de fallo.
            Console.WriteLine($"[{message.FailedAt:HH:mm:ss}] {message.OrderId} - {message.FailedProducts}: {message.Reason}");

            // Crear DTO para cancelar la orden automáticamente.
            var requestCancelOrderDto = new RequestCancelOrderDto
            {
                OrderId = message.OrderId,
                CancellationReason = message.Reason,
                FailedProducts = message.FailedProducts
            };

            // Datos del "usuario" que ejecuta la cancelación del sistema.
            var userData = new UserDataDto
            {
                Id = Guid.Empty,
                Name = "System",
                Role = "Admin",
                EmailAddress = "admin@gmail.com"
            };

            // Ejecutar cancelación del pedido en el repositorio.
            await _orderRepository.CancelOrderAsync(requestCancelOrderDto, userData);
        }
    }
}
