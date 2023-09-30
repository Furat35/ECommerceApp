namespace ECommerce.Core.Exceptions.AppRole
{
    internal class AppRoleBadRequestException : BadRequestException
    {
        public AppRoleBadRequestException() : base("Role bad request exception!")
        {

        }
        public AppRoleBadRequestException(string message) : base(message)
        {
        }
    }
}
