using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyVacationController.Models;

namespace MyVacationController.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Employee> Employees { get; set; } = default!;
    public DbSet<Leave> Leaves { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>(user =>
        {
            user.Property(u => u.Id).ValueGeneratedOnAdd();
            user.Property(u => u.DOB).IsRequired();
            user.Property(u => u.FirstName).HasMaxLength(50);
            user.Property(u => u.LastName).HasMaxLength(50);
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

        builder.Entity<Leave>(leave =>
        {
            leave.HasKey(l => l.Id);
            leave.Property(l => l.Id).ValueGeneratedOnAdd();
            leave.Property(l => l.Comment).HasMaxLength(500);
            leave.HasOne(l => l.Employee).WithMany(e => e.Leaves).HasForeignKey("EmployeeId");
        });
    }
}
