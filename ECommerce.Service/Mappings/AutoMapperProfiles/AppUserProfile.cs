using AutoMapper;
using ECommerce.Core.DTOs.AppUser;
using ECommerce.Entity.Entities.IdentityFrameworkEntities;

namespace ECommerce.Service.Mappings.AutoMapperProfiles
{
    public class AppUserProfile : Profile
    {
        public AppUserProfile()
        {
            CreateMap<AppUser, AppUserListDto>();
            CreateMap<AppUserUpdateDto, AppUser>();
            CreateMap<AppUserAddDto, AppUser>();
        }
    }
}
