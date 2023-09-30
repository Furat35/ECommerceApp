using AutoMapper;
using ECommerce.Core.DTOs.Product;
using ECommerce.Core.Filters;
using ECommerce.Core.Filters.Product;
using ECommerce.Core.HeaderResponses;
using ECommerce.Entity.Entities;

namespace ECommerce.Service.Helpers.FilterServices
{
    public class ProductFilterService
    {
        private readonly IMapper _mapper;

        public ProductFilterService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public ProductResponseFilter<List<ProductListDto>> FilterProducts(IQueryable<Product> products, ProductRequestFilter filters)
        {
            if (filters.NameStartsWith != null)
                products = products.Where(_ => _.Name.ToLower().Contains(filters.NameStartsWith.ToLower()));

            if (!(filters.MaxPrice == null && filters.MinPrice == null))
                if (filters.MinPrice != null && filters.MaxPrice != null)
                    products = products.Where(_ => _.Price > filters.MinPrice && _.Price < filters.MaxPrice);
                else if (filters.MinPrice != null)
                    products = products.Where(_ => _.Price >= filters.MinPrice);
                else
                    products = products.Where(_ => _.Price <= filters.MaxPrice);

            Metadata metadata = new()
            {
                PageSize = filters.Size,
                TotalEntities = products.Count(),
                TotalPages = (int)Math.Ceiling(products.Count() / Convert.ToDouble(filters.Size)),
                CurrentPage = filters.Page
            };

            List<Product> filteredProducts = products.Skip(metadata.PageSize * metadata.CurrentPage)
              .Take(filters.Size)
              .ToList();


            return new()
            {
                ResponseValue = _mapper.Map<List<ProductListDto>>(filteredProducts),
                Headers = new CustomHeaderResponse().AddPaginationHeader(metadata)
            };
        }
    }
}
