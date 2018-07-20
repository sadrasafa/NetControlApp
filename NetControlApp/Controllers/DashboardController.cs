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
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Dash
        public IActionResult Index()
        {
            return View();
        }

        // GET: Dash
        public async Task<IActionResult> ViewAll()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            var runs = _context.AnalysisModel.Where(run => run.User == user).OrderByDescending(run => run.Time);
            return View(runs);
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

            if (ModelState.IsValid && user != null)
            {
                // Remove from the model the parameters of the unused algorithm type.
                if (analysisModel.RandomSeed == null)
                {
                    analysisModel.RandomSeed = (new Random()).Next();
                }
                if (analysisModel.MaxIteration == null)
                {
                    analysisModel.MaxIteration = 10000;
                }
                if (analysisModel.MaxIterationNoImprovement == null)
                {
                    analysisModel.MaxIterationNoImprovement = null;
                }
                if (analysisModel.MaxPathLength == null)
                {
                    analysisModel.MaxPathLength = 5;
                }
                if (analysisModel.AlgorithmType == "greedy")
                {
                    if (analysisModel.GreedyHeuristics == null)
                    {
                        analysisModel.GreedyHeuristics = "T F 1 (->@CA)(->@PA)(->D)(->CA)(->PA)(->N)(->T)";
                    }
                }
                else
                {
                    if (analysisModel.GeneticPopulationSize == null)
                    {
                        analysisModel.GeneticPopulationSize = 80;
                    }
                    if (analysisModel.GeneticElementsRandom == null)
                    {
                        analysisModel.GeneticElementsRandom = 15;
                    }
                    if (analysisModel.GeneticPercentageRandom == null)
                    {
                        analysisModel.GeneticPercentageRandom = 0.25;
                    }
                    if (analysisModel.GeneticPercentageElite == null)
                    {
                        analysisModel.GeneticPercentageElite = 0.25;
                    }
                    if (analysisModel.GeneticProbabilityMutation == null)
                    {
                        analysisModel.GeneticProbabilityMutation = 0.001;
                    }
                }
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
