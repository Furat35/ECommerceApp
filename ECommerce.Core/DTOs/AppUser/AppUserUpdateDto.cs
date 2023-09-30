namespace ECommerce.Core.DTOs.AppUser
{
    public class AppUserUpdateDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CurrentPassword { get; set; }
        public string Password { get; set; }
        public List<string> RoleIds { get; set; }
    }
}
