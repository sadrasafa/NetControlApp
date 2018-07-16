using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetControlApp.Controllers
{
    public class DashboardController : Controller
    {
        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult ViewRecent()
        {
            return View();
        }
    }
}
