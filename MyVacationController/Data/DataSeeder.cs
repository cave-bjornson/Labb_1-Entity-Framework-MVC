using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MyVacationController.Models;

namespace MyVacationController.Data;

public class DataSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    // private readonly IUserStore<ApplicationUser> _userStore;
    // private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        ILogger<DataSeeder> logger
    // IUserStore<ApplicationUser> userStore,
    // IUserEmailStore<ApplicationUser> emailStore
    )
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _logger = logger;
        // _userStore = userStore;
        // _emailStore = emailStore;
    }

    public async Task Seed()
    {
        if (!_userManager.Users.Any())
        {
            var user = new ApplicationUser()
            {
                UserName = "jon.doe@email.com",
                GivenName = "Jon",
                SurName = "Doe",
                DateOfBirth = new DateOnly(1980, 1, 1),
                IsAdmin = false,
            };

            // await _userStore.SetUserNameAsync(user, user.UserName, CancellationToken.None);
            // await _emailStore.SetEmailAsync(user, user.UserName, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, "secret");

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password");
                var employee = new Employee() { User = user };
                var employeeEntity = _dbContext.Employees.Add(employee);
                var saveResult = await _dbContext.SaveChangesAsync();
                if (saveResult > 0)
                {
                    user.EmployeeId = employeeEntity.Entity.Id;
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (updateResult.Succeeded)
                    {
                        _logger.LogInformation("Employee created and associated with User Account");
                    }
                    else
                    {
                        _logger.LogError("Error when associating employee with ApplicationUser");
                    }

                    var leave = new Leave()
                    {
                        Employee = employee,
                        Type = LeaveType.Vacation,
                        Start = DateOnly.FromDateTime(DateTime.Today),
                        End = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
                        Comment = "Gone Fishin'"
                    };
                    _dbContext.Leaves.Add(leave);
                    await _dbContext.SaveChangesAsync();
                }
            }
            else
            {
                _logger.LogError("Failed to create user");
            }
        }
    }
}
