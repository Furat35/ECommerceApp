using AutoMapper;
using ECommerce.Core.DTOs.CartLine;
using ECommerce.Entity.Entities;

namespace ECommerce.Service.Mappings.AutoMapperProfiles
{
    public class CartLineProfile : Profile
    {
        public CartLineProfile()
        {
            CreateMap<CartLine, CartLineListDto>();
        }
    }
}
