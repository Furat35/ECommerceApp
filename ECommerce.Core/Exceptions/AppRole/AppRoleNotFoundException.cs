namespace ECommerce.Core.Exceptions.AppRole
{
    public class AppRoleNotFoundException : NotFoundException
    {
        public AppRoleNotFoundException() : base("Role not found exception!")
        {

        }

        public AppRoleNotFoundException(string message) : base(message)
        {

        }
    }
}
