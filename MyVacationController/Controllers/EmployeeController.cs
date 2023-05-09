using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyVacationController.Data;
using MyVacationController.Models;

namespace MyVacationController.Controllers;

public class EmployeeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<EmployeeController> _logger;
    private readonly IMapper _mapper;

    public EmployeeController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<EmployeeController> logger,
        IMapper mapper
    )
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
        _mapper = mapper;
    }

    // GET
    [Authorize(Policy = "IsAdmin")]
    public async Task<IActionResult> Index(string? searchString)
    {
        IQueryable<Employee> query = _context.Employees
            .Include(employee => employee.User)
            .Include(employee => employee.Leaves);

        if (!string.IsNullOrEmpty(searchString))
        {
            var ss = searchString.ToLower();
            query = query.Where(
                employee =>
                    (employee.User.GivenName + " " + employee.User.SurName).ToLower().Contains(ss)
            );
        }

        var employees = await query.ToListAsync();

        var models = _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeViewModel>>(employees);

        return View(models);
    }
}
