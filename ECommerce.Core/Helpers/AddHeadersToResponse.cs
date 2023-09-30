using Microsoft.AspNetCore.Http;

namespace ECommerce.Core.Helpers
{
    public class AddHeadersToResponse
    {
        private readonly HttpContext _httpContext;

        public AddHeadersToResponse(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public void AddToResponse(Dictionary<string, string> headers)
        {
            foreach (var header in headers)
                _httpContext.Response.Headers.Add(header.Key, header.Value);
        }
    }
}
