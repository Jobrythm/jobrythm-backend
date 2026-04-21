using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Jobrythm.Infrastructure.Persistence;

/// <summary>
/// Used only by dotnet-ef at design time (migrations). Not used at runtime.
/// </summary>
public class JobrythmDbContextFactory : IDesignTimeDbContextFactory<JobrythmDbContext>
{
    public JobrythmDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<JobrythmDbContext>()
            .UseNpgsql(
                "Host=localhost;Database=jobrythm;Username=postgres;Password=postgres",
                b => b.MigrationsAssembly(typeof(JobrythmDbContext).Assembly.FullName))
            .Options;

        return new JobrythmDbContext(options);
    }
}
