using Microsoft.EntityFrameworkCore;

namespace OrderClient.Model
{
  public class Context : DbContext
  {
    public Context()
    {
      Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite("Data Source=Orders.db");
    }

    public DbSet<Order> Orders { get; set; }
  }
}