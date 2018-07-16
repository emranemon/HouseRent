using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HouseRent.Models;

namespace HouseRent.Models
{
    public class HouseRentContext : DbContext
    {
        public HouseRentContext (DbContextOptions<HouseRentContext> options)
            : base(options)
        {
        }

        public DbSet<HouseRent.Models.User> User { get; set; }

        public DbSet<HouseRent.Models.Advertise> Advertise { get; set; }

        public DbSet<HouseRent.Models.Image> Image { get; set; }

        public DbSet<HouseRent.Models.Comment> Comment { get; set; }

        public DbSet<HouseRent.Models.Review> Review { get; set; }

        //for unique constraint Email
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasAlternateKey(c => c.Email)
                .HasName("AlternateKey_Email");
        }
    }
}
