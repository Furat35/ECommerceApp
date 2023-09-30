using ECommerce.Core.Consts;
using ECommerce.Core.DTOs.Order;
using ECommerce.Core.DTOs.Product;
using ECommerce.Entity.Entities;
using ECommerce.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> CompleteOrder(Cart cart)
        {
            await _orderService.CompleteOrderAsync(cart);
            return Ok();
        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = "Admin", Roles = $"{RoleConsts.Admin},{RoleConsts.Moderator}")]
        public async Task<IActionResult> GetAllCompletedOrders()
        {
            List<OrderListDto> orders = await _orderService.GetAllCompletedOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("[action]/{id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> GetAllCompletedOrders([FromRoute] string id)
        {
            List<OrderListDto> orders = await _orderService.GetAllCompletedOrdersByUserIdAsync(Guid.Parse(id));
            return Ok(orders);
        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = "Admin", Roles = $"{RoleConsts.Admin},{RoleConsts.Moderator}")]
        public async Task<IActionResult> GetAllNotCompletedOrders()
        {
            List<OrderListDto> orders = await _orderService.GetAllNotCompletedOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("[action]/{id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> GetAllNotCompletedOrders([FromRoute] string id)
        {
            List<OrderListDto> orders = await _orderService.GetAllNotCompletedOrdersByUserIdAsync(Guid.Parse(id));
            return Ok(orders);
        }

        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = "Admin", Roles = $"{RoleConsts.Admin},{RoleConsts.Moderator}")]
        public async Task<IActionResult> AddProduct([FromBody] ProductAddDto productDto)
        {
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
