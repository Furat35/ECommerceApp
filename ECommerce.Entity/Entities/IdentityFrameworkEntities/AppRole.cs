using Microsoft.AspNetCore.Identity;

namespace ECommerce.Entity.Entities.IdentityFrameworkEntities
{
    public class AppRole : IdentityRole<Guid>
    {
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual DateTime? DeletedDate { get; set; }
        public virtual string? DeletedBy { get; set; }
    }
}
