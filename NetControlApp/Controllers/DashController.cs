using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NetControlApp.Data;
using NetControlApp.Models;

namespace NetControlApp.Controllers
{
    public class DashController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Dash
        public async Task<IActionResult> Index()
        {
            return View(await _context.AnalysisModel.ToListAsync());
        }

        // GET: Dash
        public async Task<IActionResult> ViewAll()
        {
            return View(await _context.AnalysisModel.ToListAsync());
        }

        // GET: Dash/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analysisModel = await _context.AnalysisModel
                .FirstOrDefaultAsync(m => m.RunId == id);
            if (analysisModel == null)
            {
                return NotFound();
            }

            return View(analysisModel);
        }

        // GET: Dash/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dash/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RunId,AnalysisName,NetType,NetNodes,Target,DrugTarget,AlgorithmType,DoContact,RandomSeed,MaxIteration,MaxIterationNoImprovement,MaxPathLength,GeneticPopulationSize,GeneticElementsRandom,GeneticPercentageRandom,GeneticPercentageElite,GeneticProbabilityMutation,GreedyHeuristics")] AnalysisModel analysisModel)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            analysisModel.User = user;
            analysisModel.Time = DateTime.Now;
            analysisModel.IsCompleted = false;
            analysisModel.ScheduledToStop = false;

            if (ModelState.IsValid && user != null)
            {
                _context.Add(analysisModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(analysisModel);
        }

        // GET: Dash/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analysisModel = await _context.AnalysisModel.FindAsync(id);
            if (analysisModel == null)
            {
                return NotFound();
            }
            return View(analysisModel);
        }

        // POST: Dash/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RunId,AnalysisName,Time,NetType,NetNodes,Target,DrugTarget,AlgorithmType,DoContact,Network,Progress,BestResult,IsCompleted,ScheduledToStop,RandomSeed,MaxIteration,MaxIterationNoImprovement,MaxPathLength,GeneticPopulationSize,GeneticElementsRandom,GeneticPercentageRandom,GeneticPercentageElite,GeneticProbabilityMutation,GreedyHeuristics")] AnalysisModel analysisModel)
        {
            if (id != analysisModel.RunId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(analysisModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnalysisModelExists(analysisModel.RunId))
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
            return View(analysisModel);
        }

        // GET: Dash/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analysisModel = await _context.AnalysisModel
                .FirstOrDefaultAsync(m => m.RunId == id);
            if (analysisModel == null)
            {
                return NotFound();
            }

            return View(analysisModel);
        }

        // POST: Dash/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var analysisModel = await _context.AnalysisModel.FindAsync(id);
            _context.AnalysisModel.Remove(analysisModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnalysisModelExists(int id)
        {
            return _context.AnalysisModel.Any(e => e.RunId == id);
        }
    }
}
