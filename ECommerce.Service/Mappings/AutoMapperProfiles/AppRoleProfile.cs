using AutoMapper;
using ECommerce.Core.DTOs.AppRole;
using ECommerce.Entity.Entities.IdentityFrameworkEntities;

namespace ECommerce.Service.Mappings.AutoMapperProfiles
{
    public class AppRoleProfile : Profile
    {
        public AppRoleProfile()
        {
            CreateMap<AppRole, AppRoleListDto>();
            CreateMap<AppRoleUpdateDto, AppRole>();
            CreateMap<AppRoleAddDto, AppRole>();
        }
    }
}
