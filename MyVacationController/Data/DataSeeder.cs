using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MyVacationController.Models;
using SimpleBase;

namespace MyVacationController.Data;

public class DataSeeder
{
    public readonly IReadOnlyCollection<ApplicationUser> Users;
    public readonly IReadOnlyCollection<Employee> Employees;
    public readonly IReadOnlyCollection<Leave> Leaves;
    public readonly ApplicationUser AdminUser;

    public DataSeeder()
    {
        AdminUser = GenerateAdminUser();
        Users = GenerateUsers(11);
        Employees = GenerateEmployees(Users);
        Leaves = GenerateLeaves(33, Employees);
    }

    private static readonly string passwordHash =
        new PasswordHasher<ApplicationUser>().HashPassword(null, "secret");

    private static readonly Faker<ApplicationUser> userFaker = new Faker<ApplicationUser>()
        .RuleFor(u => u.Id, f => f.Random.Guid())
        .RuleFor(u => u.EmployeeId, f => f.Random.Guid())
        .RuleFor(u => u.UserName, f => f.Person.Email)
        .RuleFor(u => u.GivenName, f => f.Person.FirstName)
        .RuleFor(u => u.SurName, f => f.Person.LastName)
        .RuleFor(u => u.DateOfBirth, f => DateOnly.FromDateTime(f.Person.DateOfBirth))
        .RuleFor(u => u.IsAdmin, false)
        .RuleFor(u => u.NormalizedUserName, (_, u) => u.UserName!.ToUpper())
        .RuleFor(u => u.Email, (_, u) => u.UserName)
        .RuleFor(u => u.NormalizedEmail, (_, u) => u.UserName!.ToUpper())
        .RuleFor(u => u.EmailConfirmed, false)
        .RuleFor(u => u.PasswordHash, passwordHash)
        .RuleFor(u => u.SecurityStamp, f => Base32.Rfc4648.Encode(f.Random.Bytes(32)))
        .RuleFor(u => u.ConcurrencyStamp, f => f.Random.Guid().ToString())
        .RuleFor(u => u.PhoneNumber, _ => null)
        .RuleFor(u => u.PhoneNumberConfirmed, false)
        .RuleFor(u => u.TwoFactorEnabled, false)
        .RuleFor(u => u.LockoutEnd, _ => null)
        .RuleFor(u => u.LockoutEnabled, true)
        .RuleFor(u => u.AccessFailedCount, 0);

    private static ApplicationUser GenerateUser(
        string FirstName,
        string LastName,
        string UserName,
        bool HasEmployee = true,
        bool IsAdmin = false
    )
    {
        var adminUser = userFaker.Generate();
        adminUser.IsAdmin = IsAdmin;
        adminUser.EmployeeId = HasEmployee ? adminUser.EmployeeId : null;
        adminUser.GivenName = FirstName;
        adminUser.SurName = LastName;
        adminUser.UserName = UserName;
        adminUser.NormalizedUserName = adminUser.UserName.ToUpper();
        adminUser.Email = adminUser.UserName;
        adminUser.NormalizedEmail = adminUser.NormalizedUserName;
        return adminUser;
    }

    private static ApplicationUser GenerateAdminUser()
    {
        return GenerateUser(
            FirstName: "Shodan",
            LastName: "Brosius",
            UserName: "shodan@sky.net",
            HasEmployee: false,
            IsAdmin: true
        );
    }

    private static ApplicationUser GenerateDefaultUser()
    {
        return GenerateUser(FirstName: "Jon", LastName: "Doe", UserName: "jon.doe@email.net");
    }

    private static IReadOnlyCollection<ApplicationUser> GenerateUsers(int amount)
    {
        var users = Enumerable.Range(1, amount).Select(i => SeedRow(userFaker, i)).ToList();
        users.Add(GenerateDefaultUser());
        users.Add(GenerateAdminUser());

        return users;
    }

    private static IReadOnlyCollection<Employee> GenerateEmployees(
        IEnumerable<ApplicationUser> users
    )
    {
        return users
            .Where(u => u.EmployeeId.HasValue)
            .Select(u => new Employee() { Id = u.EmployeeId, UserId = u.Id })
            .ToList();
    }

    private static IReadOnlyCollection<Leave> GenerateLeaves(
        int amount,
        IEnumerable<Employee> employees
    )
    {
        var leaveFaker = new Faker<Leave>()
            .RuleFor(l => l.Id, f => f.Random.Guid())
            .RuleFor(
                l => l.Start,
                f =>
                    f.Date.BetweenDateOnly(
                        DateOnly.FromDateTime(DateTime.Today.AddMonths(-6)),
                        DateOnly.FromDateTime(DateTime.Today.AddMonths(6))
                    )
            )
            .RuleFor(l => l.End, (f, l) => l.Start.AddDays(f.Random.Number(25)))
            .RuleFor(
                l => l.Created,
                (f, l) => l.Start.AddDays(-f.Random.Number(7, 70)).ToDateTime(TimeOnly.MinValue)
            )
            .RuleFor(
                l => l.Comment,
                f =>
                {
                    var lorem = f.Lorem.Text();
                    return lorem.Length <= 500 ? lorem : lorem[..500];
                }
            )
            .RuleFor(l => l.Type, f => f.PickRandom<LeaveType>())
            .RuleFor(l => l.EmployeeId, f => f.PickRandom(employees).Id);

        var leaves = Enumerable.Range(1, amount).Select(i => SeedRow(leaveFaker, i)).ToList();

        return leaves;
    }

    private static T SeedRow<T>(Faker<T> faker, int rowId) where T : class
    {
        var recordRow = faker.UseSeed(rowId).Generate();
        return recordRow;
    }
}
