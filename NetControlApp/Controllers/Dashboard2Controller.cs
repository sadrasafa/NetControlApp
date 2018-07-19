using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NetControlApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return View();
        }
    }
}
