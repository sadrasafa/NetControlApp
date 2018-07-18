using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetControlApp.Models.DashboardViewModels;

namespace NetControlApp.Controllers
{
    public class DashboardController : Controller
    {
        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }

        [Authorize]
        public IActionResult ViewRecent()
        {
            return View();
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

        [HttpGet]
        public async Task<IActionResult> NewAnalysis2()
        {
            var model = new NewAnalysis2ViewModel { };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewAnalysis2(NewAnalysis2ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToAction(nameof(NewAnalysis2));
        }

    }
}
