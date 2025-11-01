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
            if (userId == null) return Unauthorized(new { message = "No se encontro el id del usuario"});
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

        [HttpGet("checkOrderStatus/{orderId}")]
        public async Task<IActionResult> CheckOrderStatus([FromRoute] string orderId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var customerId = Request.Headers["X-Client-Id"].ToString();
            if (customerId == null) return Unauthorized(new { message = "No se encontro el id del usuario"});
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

        [HttpPatch("cancelOrder/{orderId}")]
        public async Task<IActionResult> CancelOrderClient([FromRoute] string orderId, [FromBody] string? cancellationReason)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = Request.Headers["X-Client-Id"].ToString();
            if (userId == null) return Unauthorized(new { message = "No se encontro el id del usuario"});
            var role = Request.Headers["X-Client-Role"].ToString();
            if (role == null) return Unauthorized(new { message = "No se encontro el rol del usuario" });

            var cancelOrderDto = new RequestCancelOrderDto
            {
                UserId = userId,
                UserRole = role,
                OrderId = orderId,
                CancellationReason = cancellationReason
            };

            try
            {
                var order = await _orderRepository.CancelOrder(cancelOrderDto);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("getOrders")]
        public async Task<IActionResult> GetOrders([FromQuery] QueryObjectOrder queryObject)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = Request.Headers["X-Client-Id"].ToString();
            if (userId == null) return Unauthorized(new { message = "No se encontro el id del usuario"});
            var role = Request.Headers["X-Client-Role"].ToString();
            if (role == null) return Unauthorized(new { message = "No se encontro el rol del usuario" });

            try
            {
                var orders = await _orderRepository.GetOrders(userId, role, queryObject);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}