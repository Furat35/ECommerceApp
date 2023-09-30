namespace ECommerce.Core.Exceptions.AppRole
{
    public class AppRoleAlreadyExistsException : BadRequestException
    {
        public AppRoleAlreadyExistsException(string message) : base(message)
        {

        }

        public AppRoleAlreadyExistsException() : base("Role already exists exception!")
        {

        }
    }
}
