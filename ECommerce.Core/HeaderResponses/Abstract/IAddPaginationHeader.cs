using ECommerce.Core.Filters;

namespace ECommerce.Core.HeaderResponses.Abstract
{
    public interface IAddPaginationHeader
    {
        Dictionary<string, string> AddPaginationHeader(Metadata metadata);
    }
}
