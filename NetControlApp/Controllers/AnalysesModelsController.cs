using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NetControlApp.Data;
using NetControlApp.Models;

namespace NetControlApp.Controllers
{
    [Authorize]
    public class AnalysesModelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AnalysesModelsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: AnalysesModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.AnalysesModel.ToListAsync());
        }

        // GET: AnalysesModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analysesModel = await _context.AnalysesModel
                .FirstOrDefaultAsync(m => m.RunId == id);
            if (analysesModel == null)
            {
                return NotFound();
            }

            return View(analysesModel);
        }

        // GET: AnalysesModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AnalysesModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RunId,AnalysisName,NetType,NetNodes,Target,DrugTarget,AlgorithmType,AlgorithmParams,DoContact")] AnalysesModel analysesModel)
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

        // GET: AnalysesModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analysesModel = await _context.AnalysesModel.FindAsync(id);
            if (analysesModel == null)
            {
                return NotFound();
            }

            return View(analysesModel);
        }

        // POST: AnalysesModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RunId,AnalysisName,Time,NetType,NetNodes,Target,DrugTarget,AlgorithmType,AlgorithmParams,DoContact,Network,Progress,BestResult,IsCompleted,ScheduledForDeletion")] AnalysesModel analysesModel)
        {
            if (id != analysesModel.RunId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(analysesModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnalysesModelExists(analysesModel.RunId))
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
            return View(analysesModel);
        }

        // GET: AnalysesModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analysesModel = await _context.AnalysesModel
                .FirstOrDefaultAsync(m => m.RunId == id);
            if (analysesModel == null)
            {
                return NotFound();
            }

            return View(analysesModel);
        }

        // POST: AnalysesModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var analysesModel = await _context.AnalysesModel.FindAsync(id);
            _context.AnalysesModel.Remove(analysesModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnalysesModelExists(int id)
        {
            return _context.AnalysesModel.Any(e => e.RunId == id);
        }
    }
}
