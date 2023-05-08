using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyVacationController.Models;

namespace MyVacationController.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MonthlyOverView(DateTime? month)
        {
            return View();
        }

        // [HttpPost, ValidateAntiForgeryToken]
        // public IActionResult MonthlyOverView(DateTime? month) //[Bind("Month")] MonthViewModel model)
        // {
        //     return View(model);
        // }
    }
}
