using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using eTickets.Data; // Make sure to include the namespace for ApplicationDbContext
using Microsoft.EntityFrameworkCore;

namespace eTickets
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var host = CreateHostBuilder(args).Build();

      // Create and seed the database if needed during design time
      using (var scope = host.Services.CreateScope())
      {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
      }

      host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>();
            });

    // Method to create ApplicationDbContext for EF Core tools at design time
    public static AppDbContext CreateDbContext(string[] args)
    {
      var builder = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json")
          .AddEnvironmentVariables();

      if (args != null)
      {
        builder.AddCommandLine(args);
      }

      var config = builder.Build();
      var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
      optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnectionString"));

      return new AppDbContext(optionsBuilder.Options);
    }
  }
}