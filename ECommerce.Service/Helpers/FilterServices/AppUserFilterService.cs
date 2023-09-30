using AutoMapper;
using ECommerce.Core.DTOs.AppUser;
using ECommerce.Core.Filters;
using ECommerce.Core.Filters.AppUser;
using ECommerce.Core.HeaderResponses;
using ECommerce.Entity.Entities.IdentityFrameworkEntities;

namespace ECommerce.Service.Helpers.FilterServices
{
    internal class AppUserFilterService
    {
        private readonly IMapper _mapper;

        public AppUserFilterService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public AppUserResponseFilter<List<AppUserListDto>> FilterUsers(IQueryable<AppUser> users, AppUserRequestFilter filters)
        {
            //if (filters.Includes != null)
            //    categories = categories.Where(_ => _.CategoryName.ToLower().Contains(filters.Includes.ToLower()));

            Metadata metadata = new()
            {
                PageSize = filters.Size,
                TotalEntities = users.Count(),
                TotalPages = users.Count() / filters.Size,
                CurrentPage = filters.Page
            };

            List<AppUser> filteredUsers = users.Skip(metadata.PageSize * metadata.CurrentPage)
                .Take(filters.Size)
                .ToList();

            return new()
            {
                ResponseValue = _mapper.Map<List<AppUserListDto>>(filteredUsers),
                Headers = new CustomHeaderResponse().AddPaginationHeader(metadata)
            };
        }
    }
}
