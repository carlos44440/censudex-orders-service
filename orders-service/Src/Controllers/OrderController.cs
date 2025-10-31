using Microsoft.AspNetCore.Mvc;
using orders_service.Src.DTOs;
using orders_service.Src.Helpers;
using orders_service.Src.Interfaces;

namespace orders_service.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] List<CreateOrderItemDto> createOrderItemsDtos)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = Request.Headers["X-Client-Id"].ToString();
            try
            {
                var order = await _orderRepository.CreateOrder(createOrderItemsDtos, userId);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("checkOrderStatus/{customerId}/{orderId?}")]
        public async Task<IActionResult> CheckOrderStatus([FromRoute] string customerId, [FromRoute] string? orderId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var checkOrdersStatus = await _orderRepository.CheckOrderStatus(customerId, orderId);
                return Ok(checkOrdersStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("updateOrderStatus/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] string orderId, [FromBody] string status)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var order = await _orderRepository.UpdateOrderStatus(orderId, status);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("cancelOrderClient")]
        public async Task<IActionResult> CancelOrderClient([FromBody] CancelOrderClientDto cancelOrderClient)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var order = await _orderRepository.CancelOrderClient(cancelOrderClient);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("cancelOrderAdmin/{orderId}")]
        public async Task<IActionResult> CancelOrderAdmin([FromRoute] string orderId, [FromBody] string? cancellationReason)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var order = await _orderRepository.CancelOrderAdmin(orderId, cancellationReason);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("getOrdersClient/{customerId}")]
        public async Task<IActionResult> GetOrdersClient([FromRoute] string customerId, [FromQuery] QueryObjectOrder queryObject)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var orders = await _orderRepository.GetOrdersClient(customerId, queryObject);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("getOrdersAdmin")]
        public async Task<IActionResult> GetOrdersAdmin([FromQuery] QueryObjectOrderAdmin queryObject)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var orders = await _orderRepository.GetOrdersAdmin(queryObject);
                return Ok(orders);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}