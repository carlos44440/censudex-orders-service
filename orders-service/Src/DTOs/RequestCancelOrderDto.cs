using System.ComponentModel.DataAnnotations;
using orders_service.Src.Messages;

namespace orders_service.Src.DTOs
{
    /// <summary>
    /// DTO utilizado para solicitar la cancelación de un pedido.
    /// </summary>
    public class RequestCancelOrderDto
    {
        /// <summary>
        /// Identificador único del pedido que se desea cancelar.
        /// </summary>
        [Required]
        public Guid OrderId { get; set; }

        /// <summary>
        /// Motivo de la cancelación del pedido.
        /// Este campo es opcional.
        /// </summary>
        public string? CancellationReason { get; set; }

        /// <summary>
        /// Lista de productos que fallaron por falta de stock,
        /// en caso de que la cancelación se origine por inventario insuficiente.
        /// </summary>
        public List<StockFailure>? FailedProducts { get; set; }
    }
}
