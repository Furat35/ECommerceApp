namespace ECommerce.Core.DTOs.AppUser
{
    public class AppUserAddDto
    {
        public string UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public List<string>? RoleIds { get; set; }
    }
}
