using AutoMapper;
using ECommerce.Core.DTOs.Cart;
using ECommerce.Entity.Entities;

namespace ECommerce.Service.Mappings.AutoMapperProfiles
{
    public class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<Cart, CartListDto>();
        }
    }
}
