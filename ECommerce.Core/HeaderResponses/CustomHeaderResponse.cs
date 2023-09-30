using ECommerce.Core.Exceptions;
using ECommerce.Core.Filters;
using ECommerce.Core.HeaderResponses.Abstract;

namespace ECommerce.Core.HeaderResponses
{
    public class CustomHeaderResponse : IAddPaginationHeader
    {
        public Dictionary<string, string> AddPaginationHeader(Metadata metadata)
             => metadata.IsValidPage
            ? new Dictionary<string, string>()
                {
                    {"X-Pagination",   $"CurrentPage:{metadata.CurrentPage};TotalPages:{metadata.TotalPages};" +
                    $"TotalEntities: {metadata.TotalEntities};PageSize: {metadata.PageSize};HasNext:{metadata.HasNext};HasPrevious;{metadata.HasPrevious};" }
                }
            : throw new BadRequestException("Invalid filters!");
    }
}
