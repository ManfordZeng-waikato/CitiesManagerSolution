using CitiesManager.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.Infrastructure.DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        public virtual DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<City>().HasData(new City()
            {
                CityName = "New York",
                CityId = Guid.Parse("C967D04C-AE5E-431E-BE6F-C17C88B74939")
            });
            modelBuilder.Entity<City>().HasData(new City()
            {
                CityName = "Aukland",
                CityId = Guid.Parse("1C455AA3-0995-4D55-9B3E-6B7E9280BB96")
            });
        }
    }
}
