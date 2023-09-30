namespace ECommerce.Core.Entities
{
    public class HttpResponseBody
    {
        public HttpResponseBody()
        {

        }

        public HttpResponseBody(int statusCode, string responseBody)
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }

        public int StatusCode { get; set; }
        public string ResponseBody { get; set; }
    }
}
