using CaseApp.Models.Access;
using CaseApp.Models.Invoices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CaseApp.Data;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext() { }

    #region DBSets
    public DbSet<ApplicationUser> ApplicationUser { get; set; }

    public DbSet<Reconciliation> Reconciliation { get; set; }
    #endregion DBSets

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        #region Identity
        builder.Entity<User>(b =>
        {
            b.HasPartitionKey(u => u.Id);
            b.HasKey(u => u.Id);
            b.Property(u => u.Id).ToJsonProperty("id");
            b.Property<string>("_etag").IsETagConcurrency();

            b.Property(u => u.UserName).IsRequired().ToJsonProperty("userName");
            b.Property(u => u.PasswordHash).IsRequired().ToJsonProperty("passwordHash");

            b.Ignore(u => u.ConcurrencyStamp);
            b.Ignore(u => u.Email);
            b.Ignore(u => u.NormalizedEmail);
            b.Ignore(u => u.EmailConfirmed);
            b.Ignore(u => u.NormalizedUserName);
            b.Ignore(u => u.SecurityStamp);
            b.Ignore(u => u.PhoneNumber);
            b.Ignore(u => u.PhoneNumberConfirmed);
            b.Ignore(u => u.TwoFactorEnabled);
            b.Ignore(u => u.LockoutEnabled);
            b.Ignore(u => u.LockoutEnd);
            b.Ignore(u => u.AccessFailedCount);

            b.HasNoDiscriminator();

            b.ToContainer("users");
        });

        builder.Ignore<IdentityRole<Guid>>();
        builder.Ignore<IdentityRoleClaim<Guid>>();
        builder.Ignore<IdentityUserClaim<Guid>>();
        builder.Ignore<IdentityUserLogin<Guid>>();
        builder.Ignore<IdentityUserRole<Guid>>();
        builder.Ignore<IdentityUserToken<Guid>>();
        #endregion Identity

        builder.Entity<ApplicationUser>(b =>
        {
            b.HasPartitionKey(au => au.AppId);
            b.HasKey(au => au.AppId);
            b.Property(au => au.AppId).ToJsonProperty("id");

            b.Property(au => au.AppName).IsRequired().HasMaxLength(250).ToJsonProperty("appName");
            b.Property(au => au.AppPasswordHash).IsRequired().HasMaxLength(250).ToJsonProperty("appPasswordHash");
            b.Property(au => au.CreatedAt).IsRequired().ToJsonProperty("createdAt");

            b.HasNoDiscriminator();

            b.ToContainer("applicationUsers");
        });

        builder.Entity<Reconciliation>(b =>
        {
            b.HasPartitionKey(r => r.Id);
            b.HasKey(r => r.Id);
            b.Property(r => r.Id).ValueGeneratedOnAdd();
            b.Property(r => r.Id).ToJsonProperty("id");

            b.Property(r => r.CustomerId).IsRequired().ToJsonProperty("customerId");
            b.Property(r => r.InvoiceId).IsRequired().HasMaxLength(20).ToJsonProperty("invoiceId");
            b.Property(r => r.Amount).IsRequired().HasPrecision(15, 2).ToJsonProperty("amount");
            b.Property(r => r.ReconciliationStatus).IsRequired().ToJsonProperty("reconciliationStatus");
            b.Property(r => r.DueDate).IsRequired().ToJsonProperty("dueDate");
            b.Property(r => r.ReconciliationAt).IsRequired().ToJsonProperty("reconciliationAt");

            b.HasNoDiscriminator();

            b.ToContainer("reconciliations");
        });
    }
}