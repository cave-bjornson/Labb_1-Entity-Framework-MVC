using System.Security.Claims;
using System.Text.RegularExpressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MyVacationController.Data;
using MyVacationController.Models;

namespace MyVacationController.Controllers
{
    [Authorize]
    public class LeaveController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LeaveController> _logger;
        private readonly IMapper _mapper;

        public LeaveController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<LeaveController> logger,
            IMapper mapper
        )
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: Leaves
        public async Task<IActionResult> Index(Guid id)
        {
            var leaves = _context.Leaves
                .Include(leave => leave.Employee)
                .Where(leave => leave.Employee.Id.ToString() == User.FindFirstValue("EmployeeId"));

            ViewBag.TitlePrefix = "My";

            return View(await _mapper.ProjectTo<LeaveViewModel>(leaves, null).ToListAsync());
        }

        [Authorize(Policy = "IsAdmin")]
        public async Task<IActionResult> AdminIndex(Guid id)
        {
            var leaves = _context.Leaves
                .Include(leave => leave.Employee)
                .Where(leave => leave.Employee.Id == id);

            return View(await _mapper.ProjectTo<LeaveViewModel>(leaves, null).ToListAsync());
        }

        // GET: Leaves/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == null || _context.Leaves == null)
            {
                return NotFound();
            }

            var model = await _mapper
                .ProjectTo<LeaveViewModel>(_context.Leaves.Where(leave => leave.Id == id))
                .SingleOrDefaultAsync();

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        public async Task<IActionResult> AdminDetails(Guid id)
        {
            if (id == null || _context.Leaves == null)
            {
                return NotFound();
            }

            var model = await _mapper
                .ProjectTo<LeaveViewModel>(_context.Leaves.Where(leave => leave.Id == id))
                .SingleOrDefaultAsync();

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: Leaves/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Title = "Apply for Leave";
            ViewBag.Action = "Create";

            return View();
        }

        // POST: Leaves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Start,End,Type,Comment")] LeaveViewModel model
        )
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.User == user);

                var leave = _mapper.Map<Leave>(
                    model,
                    options => options.AfterMap((_, dest) => dest.Employee = employee)
                );

                _context.Add(leave);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Title = "Apply for Leave";
            ViewBag.Action = "Create";

            return View(model);
        }

        // GET: Leaves/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Leaves == null)
            {
                return NotFound();
            }

            var model = await _mapper
                .ProjectTo<LeaveViewModel>(_context.Leaves.Where(leave => leave.Id == id))
                .SingleOrDefaultAsync();

            if (model == null)
            {
                return NotFound();
            }

            ViewBag.Title = "Edit Leave Application";
            ViewBag.Action = "Edit";

            return View("Create", model);
        }

        // POST: Leaves/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            Guid id,
            [Bind("Id,Start,End,Type,Comment")] LeaveViewModel model
        )
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var leave = await _context.Leaves.FindAsync(id);
                    _mapper.Map<LeaveViewModel, Leave>(model, leave);
                    _context.Update(leave);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Title = "Edit Leave Application";
            ViewBag.Action = "Edit";

            return View("Create", model);
        }

        // GET: Leaves/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == null || _context.Leaves == null)
            {
                return NotFound();
            }

            var model = await _mapper
                .ProjectTo<LeaveViewModel>(_context.Leaves.Where(leave => leave.Id == id))
                .SingleOrDefaultAsync();

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // POST: Leaves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Leaves == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Leaves'  is null.");
            }
            var leave = await _context.Leaves.FindAsync(id);
            if (leave != null)
            {
                _context.Leaves.Remove(leave);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeaveExists(Guid? id)
        {
            return (_context.Leaves?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [Authorize(Policy = "IsAdmin")]
        public async Task<IActionResult> Monthly(DateTime? month)
        {
            _logger.LogInformation("Month: {Month}", month);

            var leaves = _context.Leaves.Include(leave => leave.Employee).AsQueryable();

            if (month is not null)
            {
                // var ymArray = month.Split('-');
                // var yyyy = int.Parse(ymArray.First());
                // var mm = int.Parse(ymArray.Last());
                int yyyy = month.GetValueOrDefault().Year;
                int mm = month.GetValueOrDefault().Month;

                leaves = leaves.Where(
                    leave => leave.Created.Year == yyyy && leave.Created.Month == mm
                );

                ViewBag.Month = month.GetValueOrDefault().ToString("yyyy-MM");
            }

            return View(await _mapper.ProjectTo<LeaveViewModel>(leaves, null).ToListAsync());
        }
    }
}
