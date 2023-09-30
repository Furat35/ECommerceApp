using AutoMapper;
using ECommerce.Core.DTOs.Order;
using ECommerce.Entity.Entities;

namespace ECommerce.Service.Mappings.AutoMapperProfiles
{
    public class OrderMapProfile : Profile
    {
        public OrderMapProfile()
        {
            CreateMap<Order, OrderListDto>();
        }
    }
}
