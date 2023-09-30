using AutoMapper;
using ECommerce.Core.BackgroundServices.DTOs;
using ECommerce.Core.DTOs.Order;
using ECommerce.Core.Exceptions;
using ECommerce.Core.RabbitMqMessageBroker;
using ECommerce.Core.RabbitMqMessageBroker.Abstract;
using ECommerce.Data.Repositories;
using ECommerce.Data.UnitOfWorks;
using ECommerce.Entity.Entities;
using ECommerce.Service.Abstract;
using ECommerce.Service.Consts;
using ECommerce.Service.Extensions;
using ECommerce.Service.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace ECommerce.Service.Concrete
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;
        private readonly Lazy<DatabaseSaveChanges> _saveChanges;
        private static readonly IRabbitMqMessagePublisher _messagePublisherService;
        private IRepositoryBase<Product> Products
            => _unitOfWork.GetRepository<Product>();
        private IRepositoryBase<Order> Orders
            => _unitOfWork.GetRepository<Order>();

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContext = httpContext;
            _saveChanges = new Lazy<DatabaseSaveChanges>(() => new DatabaseSaveChanges(unitOfWork));
        }

        static OrderService()
        {
            _messagePublisherService = new RabbitMqMessagePublisher();
            _messagePublisherService.Connect("E.Commerce.Orders.Completed", "E_Commerce_Orders", ExchangeType.Direct, "E.Commerce.Orders.Completed.Key");
        }

        public async Task CompleteOrderAsync(Cart cart)
        {
            if (cart is null)
                throw new Exception("invalid cart");
            foreach (CartLine cartLine in cart.CartLines)
            {
                Product product = await Products.GetByIdAsync(cartLine.ProductId.ToString());
                if (product is null)
                    cart.CartLines.Remove(cartLine);
            }
            if (!cart.CartLines.Any())
                throw new Exception("Empty cart");
            Order order = new Order
            {
                Cart = cart,
                TotalPrice = cart.CartLines.Sum(_ => _.Quantity * _.QuantityPrice),
                OrderStatusType = OrderStatusTypes.OrderPlaced
            };
            await Orders.AddAsync(order);
            bool result = await _saveChanges.Value.SaveChangesAsync();
            if (!result)
                throw new InternalServerException();

            if (!string.IsNullOrEmpty(_httpContext.HttpContext.User.GetUserEmail()))
                _messagePublisherService.PublishMessage(new OrderCompleted
                {
                    Email = _httpContext.HttpContext.User.GetUserEmail(),
                    OrderId = order.Id.ToString()
                });
        }

        public async Task<float> ReturnOrderAsync(string orderId)
        {
            Order order = await Orders.GetByIdAsync(orderId);
            float returnAmount = order.TotalPrice;
            order.OrderStatusType = OrderStatusTypes.OrderReturning;
            Orders.Update(order);
            bool result = await _saveChanges.Value.SaveChangesAsync();
            if (!result)
                throw new InternalServerException();

            return returnAmount;
        }

        public async Task ConfirmOrderReturnedAsync(string orderId)
        {
            Order order = await Orders.GetByIdAsync(orderId);
            order.OrderStatusType = OrderStatusTypes.OrderReturned;
            Orders.Update(order);
            bool result = await _saveChanges.Value.SaveChangesAsync();
            if (!result)
                throw new InternalServerException();
        }

        public async Task<List<OrderListDto>> GetAllCompletedOrdersAsync()
        {
            List<Order> orders = await Orders
                .GetAll(_ => _.OrderStatusType == OrderStatusTypes.OrderCompleted)
                .Include(_ => _.Cart)
                .ThenInclude(_ => _.CartLines)
                .ToListAsync();
            return MapOrderToOrderListDto(orders);
        }

        public async Task<List<OrderListDto>> GetAllNotCompletedOrdersAsync()
        {
            List<Order> orders = await Orders
                .GetAll(_ => _.OrderStatusType != OrderStatusTypes.OrderCompleted)
                .Include(_ => _.Cart)
                .ThenInclude(_ => _.CartLines)
                .ToListAsync();
            return MapOrderToOrderListDto(orders);
        }

        public async Task<List<OrderListDto>> GetAllCompletedOrdersByUserIdAsync(Guid id)
        {
            List<Order> orders = await Orders
                .GetAll(_ => _.Cart.AppUserId == id && _.OrderStatusType == OrderStatusTypes.OrderCompleted)
                .Include(_ => _.Cart)
                .ThenInclude(_ => _.CartLines)
                .ToListAsync();
            return MapOrderToOrderListDto(orders);
        }

        public async Task<List<OrderListDto>> GetAllNotCompletedOrdersByUserIdAsync(Guid id)
        {
            List<Order> orders = await Orders
                .GetAll(_ => _.Cart.AppUserId == id && _.OrderStatusType != OrderStatusTypes.OrderCompleted)
                .Include(_ => _.Cart)
                .ThenInclude(_ => _.CartLines)
                .ToListAsync();
            return MapOrderToOrderListDto(orders);
        }

        private List<OrderListDto> MapOrderToOrderListDto(List<Order> orders)
            => _mapper.Map<List<OrderListDto>>(orders);
    }
}
