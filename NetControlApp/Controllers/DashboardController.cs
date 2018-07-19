using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetControlApp.Data;
using NetControlApp.Models;
using Microsoft.AspNetCore.Identity;

namespace NetControlApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> ViewRecent()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            var runs = _context.Runs.Where(run => run.User == user).OrderByDescending(run => run.Time).Take(3) ;
            return View(runs);
        }

        [Authorize]
        public IActionResult NewAnalysis()
        {
            return View();
        }

        [Authorize]
        public IActionResult TestAlgorithmRun()
        {
            return View();
        }

    }
}
