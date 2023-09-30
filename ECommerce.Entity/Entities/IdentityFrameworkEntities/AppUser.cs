using Microsoft.AspNetCore.Identity;

namespace ECommerce.Entity.Entities.IdentityFrameworkEntities
{
    public class AppUser : IdentityUser<Guid>
    {
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedBy { get; set; }
    }
}
