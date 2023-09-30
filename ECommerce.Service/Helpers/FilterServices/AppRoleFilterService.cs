using AutoMapper;
using ECommerce.Core.DTOs.AppRole;
using ECommerce.Core.Filters;
using ECommerce.Core.Filters.AppRole;
using ECommerce.Core.HeaderResponses;
using ECommerce.Entity.Entities.IdentityFrameworkEntities;

namespace ECommerce.Service.Helpers.FilterServices
{
    internal class AppRoleFilterService
    {
        private readonly IMapper _mapper;

        public AppRoleFilterService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public AppRoleResponseFilter<List<AppRoleListDto>> FilterRoles(IQueryable<AppRole> roles, AppRoleRequestFilter filters)
        {
            //if (filters.Includes != null)
            //    categories = categories.Where(_ => _.CategoryName.ToLower().Contains(filters.Includes.ToLower()));

            Metadata metadata = new()
            {
                PageSize = filters.Size,
                TotalEntities = roles.Count(),
                TotalPages = roles.Count() / filters.Size,
                CurrentPage = filters.Page
            };

            List<AppRole> filteredRoles = roles.Skip(metadata.PageSize * metadata.CurrentPage)
                .Take(filters.Size)
                .ToList();

            return new()
            {
                ResponseValue = filteredRoles.Count > 0 ? _mapper.Map<List<AppRoleListDto>>(filteredRoles) : null,
                Headers = new CustomHeaderResponse().AddPaginationHeader(metadata)
            };
        }
    }
}
