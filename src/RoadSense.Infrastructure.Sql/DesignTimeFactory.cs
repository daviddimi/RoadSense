using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RoadSense.Infrastructure.Sql;

public static class DbDefaults
{
    public const string ConnectionString =
        @"Server=(localdb)\\MSSQLLocalDB;Database=RoadSense;Trusted_Connection=True;TrustServerCertificate=True";

    public static DbContextOptions<RoadSenseDbContext> Options(string? cs = null) =>
        new DbContextOptionsBuilder<RoadSenseDbContext>().UseSqlServer(cs ?? ConnectionString).Options;
}

/// <summary>Използва се от „dotnet ef“ при създаване на миграции.</summary>
public class DesignTimeFactory : IDesignTimeDbContextFactory<RoadSenseDbContext>
{
    public RoadSenseDbContext CreateDbContext(string[] args) =>
        new(DbDefaults.Options(Environment.GetEnvironmentVariable("ROADSENSE_CONNECTION")));
}
