using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using TestMvc.Models;

namespace TestMvc.Data
{
    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions<TestContext> options) : base(options) { }

        public DbSet<Image> Images { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Pour renommer la table Images en Image
            modelBuilder.Entity<Image>().ToTable("Image");
        }
    }
}