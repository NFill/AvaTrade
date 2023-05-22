using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using AvaTrade.Domain.Entities;

namespace AvaTrade.Data
{
  public class AvaTradeDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
  {
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
    public DbSet<News> News { get; set; }
    public AvaTradeDbContext(DbContextOptions<AvaTradeDbContext> options)
        : base(options)
    {
    }
    // TODO: OnModelCreating configure Entities. Remove [NotMapped] Atribute
  }




  public class DbContextFactory : IDesignTimeDbContextFactory<AvaTradeDbContext>
  {
    public AvaTradeDbContext CreateDbContext(string[] args)
    {
      var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json").Build();

      var dbContextBuilder = new DbContextOptionsBuilder<AvaTradeDbContext>();
      string connectionString = configuration["ConnectionStrings:AvaTradeConnection"];
      dbContextBuilder.UseSqlServer(connectionString);
      return new AvaTradeDbContext(dbContextBuilder.Options);
    }
  }
}