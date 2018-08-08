﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NetControlApp.Data;
using NetControlApp.Models;
using NetControlApp.Algorithms;

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
            var analyses = _context.AnalysisModel.Where(a => a.User == user).OrderByDescending(a => a.StartTime);
            return View(analyses);
        }

        // GET: Dash/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analysisModel = await _context.AnalysisModel
                .FirstOrDefaultAsync(m => m.AnalysisId == id);
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
        public async Task<IActionResult> Create([Bind("AnalysisName,UserGivenNetworkType,UserGivenNetworkGeneration,UserGivenNodes,UserGivenTarget,UserGivenDrugTarget,DoContact,AlgorithmType,GeneticRandomSeed,GeneticMaxIteration,GeneticMaxIterationNoImprovement,GeneticMaxPathLength,GeneticPopulationSize,GeneticElementsRandom,GeneticPercentageRandom,GeneticPercentageElite,GeneticProbabilityMutation,GreedyRandomSeed,GreedyMaxIteration,GreedyMaxIterationNoImprovement,GreedyMaxPathLength,GreedyCutToDriven,GreedyCutNonBranching,GreedyHeuristics")] AnalysisModel analysisModel)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            analysisModel.User = user;

            if (ModelState.IsValid && user != null)
            {
                if (Algorithms.Algorithms.GenerateNetwork(analysisModel))
                {
                    _context.Add(analysisModel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View(analysisModel);
                }
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
        public async Task<IActionResult> Edit(int id, [Bind("AnalysisId,StartTime,EndTime,AnalysisName,UserGivenNetworkType,UserGivenNodes,UserGivenTarget,UserGivenDrugTarget,NetworkNodeCount,NetworkNodes,NetworkEdgeCount,NetworkEdges,NetworkTargetCount,NetworkTargets,NetworkDrugTargetCount,NetworkDrugTargets,NetworkBestResultCount,NetworkBestResultNodes,DoContact,Status,ScheduledToStop,AlgorithmType,GeneticRandomSeed,GeneticMaxIteration,GeneticIterationNoImprovement,GeneticMaxPathLength,GeneticPopulationSize,GeneticElementsRandom,GeneticPercentageRandom,GeneticPercentageElite,GeneticProbabilityMutation,GreedyRandomSeed,GreedyMaxIteration,GreedyMaxIterationNoImprovement,GreedyMaxPathLength,GreedyHeuristics")] AnalysisModel analysisModel)
        {
            if (id != analysisModel.AnalysisId)
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
                    if (!AnalysisModelExists(analysisModel.AnalysisId))
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
                .FirstOrDefaultAsync(m => m.AnalysisId == id);
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
            return _context.AnalysisModel.Any(e => e.AnalysisId == id);
        }
    }
}
