using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SampleWebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace SampleWebApp.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        this.SeedRoles(builder);
        this.SeedUsers(builder);
        this.SeedUserRoles(builder);
    }

    private void SeedUsers(ModelBuilder builder)
    {
        PasswordHasher<IdentityUser> passwordHasher = new PasswordHasher<IdentityUser>();
        IdentityUser user = new IdentityUser()
        {
            Id = "b74ddd14-6340-4840-95c2-db12554843e5",
            UserName = "admin@samplewebapp",
            NormalizedUserName = "ADMIN@SAMPLEWEBAPP.COM",
            Email = "admin@samplewebapp.com",
            NormalizedEmail = "ADMIN@SAMPLEWEBAPP.COM",
            LockoutEnabled = false,
            SecurityStamp = new Guid().ToString("D"),
            EmailConfirmed = true,
            TwoFactorEnabled = false
        };

        user.PasswordHash = passwordHasher.HashPassword(user, "Admunto_123#");
        builder.Entity<IdentityUser>().HasData(user);
    }

    private void SeedRoles(ModelBuilder builder)
    {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() {
                    Id = "fab4fac1-c546-41de-aebc-a14da6895711",
                    Name = "Adminstrator",
                    ConcurrencyStamp = "1",
                    NormalizedName = "ADMINISTRATOR"
                }
            );
    }

    private void SeedUserRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>(){
                RoleId = "fab4fac1-c546-41de-aebc-a14da6895711",
                UserId = "b74ddd14-6340-4840-95c2-db12554843e5"
            }
        );
    }
    public DbSet<SampleWebApp.Models.Sample> Sample { get;set; }
    public DbSet<SampleWebApp.Models.SampleProgram> SampleProgram { get;set; }
    public DbSet<SampleWebApp.Models.SocialPost> SocialPost { get;set; }
}