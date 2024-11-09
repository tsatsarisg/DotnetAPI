using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data;

public class DataContextEF : DbContext
{
    private readonly IConfiguration _config;

    public DataContextEF(IConfiguration config)
    {
        _config = config;
    }
    
    public virtual DbSet<User> Users { get; set; }
    
    public virtual DbSet<UserSalary> UserSalaries { get; set; }
    
    public virtual DbSet<UserJobInfo> UserJobInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder
                .UseSqlServer(_config.GetConnectionString("DefaultConnection"),
                    optionsBuilder => optionsBuilder.EnableRetryOnFailure());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("TutorialAppSchema");
        
        modelBuilder.Entity<User>()
            .ToTable("Users", "TutorialAppSchema")
            .HasKey(x => x.UserId);
        
        modelBuilder.Entity<UserSalary>()
            .HasKey(x => x.UserId);

        modelBuilder.Entity<UserJobInfo>()
            .HasKey(x => x.UserId);
    }
}