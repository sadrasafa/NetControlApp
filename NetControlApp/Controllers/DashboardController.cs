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
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> ViewRecent()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            var runs = _context.AnalysesModel.Where(run => run.User == user).OrderByDescending(run => run.Time);
            return View(runs);
        }

        [Authorize]
        public IActionResult NewAnalysis()
        {
            return View();
        }

        // POST: Dashboard/NewAnalysis
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewAnalysis([Bind("RunId,AnalysisName,NetType,NetNodes,Target,DrugTarget,AlgorithmType,AlgorithmParams,DoContact")] AnalysesModel analysesModel)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            analysesModel.User = user;
            analysesModel.Time = DateTime.Now;
            analysesModel.IsCompleted = false;
            analysesModel.ScheduledToStop = false;

            if (ModelState.IsValid && user != null)
            {
                _context.Add(analysesModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(analysesModel);
        }

        [Authorize]
        public IActionResult TestAlgorithmRun()
        {
            return View();
        }

    }
}
