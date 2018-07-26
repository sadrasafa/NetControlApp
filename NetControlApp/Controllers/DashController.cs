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
        public IActionResult Index()
        {
            return View();
        }

        // GET: Dash
        public async Task<IActionResult> ViewAll()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            var analyses = _context.AnalysesModel.Where(a => a.User == user).OrderByDescending(a => a.StartTime);
            return View(analyses);
        }

        // GET: Dash/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analysesModel = await _context.AnalysesModel
                .FirstOrDefaultAsync(m => m.AnalysisId == id);
            if (analysesModel == null)
            {
                return NotFound();
            }

            return View(analysesModel);
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
        public async Task<IActionResult> Create([Bind("AnalysisName,UserGivenNetworkType,UserGivenNodes,UserGivenTarget,UserGivenDrugTarget,DoContact,AlgorithmType,GeneticRandomSeed,GeneticMaxIteration,GeneticMaxIterationNoImprovement,GeneticMaxPathLength,GeneticPopulationSize,GeneticElementsRandom,GeneticPercentageRandom,GeneticPercentageElite,GeneticProbabilityMutation,GreedyRandomSeed,GreedyMaxIteration,GreedyMaxIterationNoImprovement,GreedyMaxPathLength,GreedyCutToDriven,GreedyCutNonBranching,GreedyHeuristics")] AnalysesModel analysesModel)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            analysesModel.User = user;

            if (ModelState.IsValid && user != null)
            {
                // Remove the parameters of the unused algorithm, and set the empty
                // parameters of the used algorithm to the default values.
                if (analysesModel.AlgorithmType == "genetic")
                {
                    if (analysesModel.GeneticRandomSeed == null)
                    {
                        analysesModel.GeneticRandomSeed = (new Random()).Next();
                    }
                    if (analysesModel.GeneticMaxIteration == null)
                    {
                        analysesModel.GeneticMaxIteration = 10000;
                    }
                    if (analysesModel.GeneticMaxIterationNoImprovement == null)
                    {
                        analysesModel.GeneticMaxIterationNoImprovement = null;
                    }
                    if (analysesModel.GeneticMaxPathLength == null)
                    {
                        analysesModel.GeneticMaxPathLength = 5;
                    }
                    if (analysesModel.GeneticPopulationSize == null)
                    {
                        analysesModel.GeneticPopulationSize = 80;
                    }
                    if (analysesModel.GeneticElementsRandom == null)
                    {
                        analysesModel.GeneticElementsRandom = 25;
                    }
                    if (analysesModel.GeneticPercentageRandom == null)
                    {
                        analysesModel.GeneticPercentageRandom = 0.25;
                    }
                    if (analysesModel.GeneticPercentageElite == null)
                    {
                        analysesModel.GeneticPercentageElite = 0.25;
                    }
                    if (analysesModel.GeneticProbabilityMutation == null)
                    {
                        analysesModel.GeneticProbabilityMutation = 0.001;
                    }
                }
                // Remove from the model the parameters of the unused algorithm type.
                else if (analysesModel.AlgorithmType == "greedy")
                {
                    if (analysesModel.GreedyRandomSeed == null)
                    {
                        analysesModel.GreedyRandomSeed = (new Random()).Next();
                    }
                    if (analysesModel.GreedyMaxIteration == null)
                    {
                        analysesModel.GreedyMaxIteration = 10000;
                    }
                    if (analysesModel.GreedyMaxIterationNoImprovement == null)
                    {
                        analysesModel.GreedyMaxIterationNoImprovement = null;
                    }
                    if (analysesModel.GreedyMaxPathLength == null)
                    {
                        analysesModel.GreedyMaxPathLength = 0;
                    }
                    if (analysesModel.GreedyCutToDriven == null)
                    {
                        analysesModel.GreedyCutToDriven = true;
                    }
                    if (analysesModel.GreedyCutNonBranching == null)
                    {
                        analysesModel.GreedyCutNonBranching = false;
                    }
                    if (analysesModel.GreedyHeuristics == null)
                    {
                        analysesModel.GreedyHeuristics = "(->@CA)(->@PA)(->D)(->CA)(->PA)(->N)(->T)";
                    }
                }
                _context.Add(analysesModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(analysesModel);
        }

        // GET: Dash/Edit/5
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

        // POST: Dash/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AnalysisId,StartTime,EndTime,AnalysisName,UserGivenNetworkType,UserGivenNodes,UserGivenTarget,UserGivenDrugTarget,NetworkNodeCount,NetworkNodes,NetworkEdgeCount,NetworkEdges,NetworkTargetCount,NetworkTargets,NetworkDrugTargetCount,NetworkDrugTargets,NetworkBestResultCount,NetworkBestResultNodes,DoContact,Status,ScheduledToStop,AlgorithmType,GeneticRandomSeed,GeneticMaxIteration,GeneticIterationNoImprovement,GeneticMaxPathLength,GeneticPopulationSize,GeneticElementsRandom,GeneticPercentageRandom,GeneticPercentageElite,GeneticProbabilityMutation,GreedyRandomSeed,GreedyMaxIteration,GreedyMaxIterationNoImprovement,GreedyMaxPathLength,GreedyHeuristics")] AnalysesModel analysesModel)
        {
            if (id != analysesModel.AnalysisId)
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
                    if (!AnalysesModelExists(analysesModel.AnalysisId))
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

        // GET: Dash/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analysesModel = await _context.AnalysesModel
                .FirstOrDefaultAsync(m => m.AnalysisId == id);
            if (analysesModel == null)
            {
                return NotFound();
            }

            return View(analysesModel);
        }

        // POST: Dash/Delete/5
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
            return _context.AnalysesModel.Any(e => e.AnalysisId == id);
        }
    }
}
