using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;

namespace Cerium.AspNetDemo.IdentityImplementations
{
    public class ExtendedUserDbContext : IdentityDbContext<ExtendedUser>
    {
        public ExtendedUserDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var address = modelBuilder.Entity<Address>();
            address.ToTable("AspNetUserAddresses");
            address.HasKey(x => x.Id);

            var user = modelBuilder.Entity<ExtendedUser>();
            user.Property(x => x.FullName).IsRequired().HasMaxLength(256)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("FullNameIndex")));
            user.HasMany(x => x.Addresses).WithRequired().HasForeignKey(x => x.UserId);

        }
    }
}