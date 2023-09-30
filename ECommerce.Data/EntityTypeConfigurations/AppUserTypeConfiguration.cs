using ECommerce.Entity.Entities.IdentityFrameworkEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.EntityTypeConfigurations
{
    public class AppUserTypeConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            //builder
            //    .HasOne(_ => _.Cart)
            //    .WithOne(_ => _.AppUser)
            //    .HasForeignKey<Cart>(_ => _.Id);
        }
    }
}
