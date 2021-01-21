using Microsoft.EntityFrameworkCore;

namespace OrderAPI.Model
{
  public class Context : DbContext
  {
    public Context(DbContextOptions options)
    : base(options)
    {
      Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      var data = DataGenerator.Create(0, 500);
      modelBuilder.Entity<Order>().HasData(data.Orders);
      modelBuilder.Entity<OrderFile>().HasData(data.Files);

      base.OnModelCreating(modelBuilder);
    }    

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderFile> OrderFiles { get; set; }
  }
}