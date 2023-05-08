using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using MyVacationController.Models;

namespace MyVacationController.Data;

public class ApplicationDbContext : IdentityUserContext<ApplicationUser, Guid>
{
    private string dateTimeSQLFunction;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Employee> Employees { get; set; } = default!;
    public DbSet<Leave> Leaves { get; set; } = default!;

    /// <inheritdoc />
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<Guid>().HaveConversion<GuidToStringConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>(user =>
        {
            user.HasKey(u => u.Id);
            user.Property(e => e.Id).ValueGeneratedOnAdd();
            user.Property(u => u.DateOfBirth).IsRequired();
            user.Property(u => u.GivenName).HasMaxLength(50);
            user.Property(u => u.SurName).HasMaxLength(50);
            user.HasOne<Employee>().WithOne(e => e.User).HasForeignKey<Employee>("UserId");
        });

        builder.Entity<Employee>(employee =>
        {
            employee.HasKey(e => e.Id);
            employee.Property(e => e.Id).ValueGeneratedOnAdd();
            employee.Ignore(e => e.FirstName);
            employee.Ignore(e => e.LastName);
            employee.Ignore(e => e.Email);
            employee.Ignore(e => e.DOB);
        });

        dateTimeSQLFunction = Database.IsSqlite() ? "datetime('now', 'localtime')" : "getdate()";

        builder.Entity<Leave>(leave =>
        {
            leave.HasKey(l => l.Id);
            leave.Property(l => l.Id).ValueGeneratedOnAdd();
            leave.Property(l => l.Comment).HasMaxLength(500);
            leave.HasOne(l => l.Employee).WithMany(e => e.Leaves).HasForeignKey("EmployeeId");
            leave.Property(l => l.Created).HasDefaultValueSql(dateTimeSQLFunction);
        });
    }

    public DbSet<MyVacationController.Models.LeaveViewModel> LeaveViewModel { get; set; } = default!;
}
