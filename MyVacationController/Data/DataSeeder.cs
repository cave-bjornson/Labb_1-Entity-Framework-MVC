using Microsoft.AspNetCore.Identity;
using MyVacationController.Models;

namespace MyVacationController.Data;

public class DataSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        ILogger<DataSeeder> logger
    )
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task Seed()
    {
        if (!_userManager.Users.Any())
        {
            var user = new ApplicationUser()
            {
                UserName = "jon.doe@email.com",
                FirstName = "Jon",
                LastName = "Doe",
                DOB = new DateTime(1980, 1, 1),
            };
            var result = await _userManager.CreateAsync(user, "secret");
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");
                var employee = new Employee() { User = user };
                _dbContext.Employees.Add(employee);
                var leave = new Leave()
                {
                    Employee = employee,
                    Type = LeaveType.Vacation,
                    Start = DateTime.Today,
                    End = DateTime.Today.AddMonths(1),
                    Comment = "Gone Fishin'"
                };
                _dbContext.Leaves.Add(leave);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                _logger.LogError("Failed to create user.");
            }
        }
    }
}
