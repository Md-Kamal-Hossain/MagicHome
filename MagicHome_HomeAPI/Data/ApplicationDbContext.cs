using MagicHome_HomeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicHome_HomeAPI.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {


        }
        public DbSet<House> Houses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.Entity<House>().HasData(new House()
           { Id = 1,
             Name = "Riverview",
             Details = "At bank of Rhein river",
             ImageUrl = "",
             Rate = 200,
             Sqft = 120,
             Occupancy = 4 ,
             Amenity= ""
             
           }
           );
        }
    }
}
