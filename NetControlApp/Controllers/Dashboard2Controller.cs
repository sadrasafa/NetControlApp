using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NetControlApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetControlApp.Models.Dashboard2ViewModels;

namespace NetControlApp.Controllers
{
    public class Dashboard2Controller : Controller
    {
        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ViewRecent()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> NewAnalysis()
        {
            var model = new NewAnalysisViewModel { };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewAnalysis(NewAnalysisViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToAction(nameof(NewAnalysis));
        }
    }
}
