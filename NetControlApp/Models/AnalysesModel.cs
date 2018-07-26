using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetControlApp.Models
{
    public class AnalysesModel
    {
        [Key]
        public int AnalysisId { get; set; }

        public ApplicationUser User { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        public DateTime? EndTime { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Analysis Name")]
        public string AnalysisName { get; set; }

        [Required]
        [Display(Name = "Is the network provided as seed nodes?")]
        public bool UserGivenNetworkType { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "List of nodes")]
        public string UserGivenNodes { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Target nodes")]
        public string UserGivenTarget { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Drug target nodes")]
        public string UserGivenDrugTarget { get; set; }

        [Display(Name = "Number of nodes in the network")]
        public int? NetworkNodeCount { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Nodes in the network")]
        public string NetworkNodes { get; set; }

        [Display(Name = "Number of edges")]
        public int? NetworkEdgeCount { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Edges")]
        public string NetworkEdges { get; set; }

        [Display(Name = "Number of target nodes")]
        public int? NetworkTargetCount { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Target nodes")]
        public string NetworkTargets { get; set; }

        [Display(Name = "Number of drug target nodes")]
        public int? NetworkDrugTargetCount { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Drug target nodes")]
        public string NetworkDrugTargets { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Number of nodes of the best result")]
        public int? NetworkBestResultCount { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Best result")]
        public string NetworkBestResultNodes { get; set; }

        [Required]
        [Display(Name = "Send an e-mail?")]
        public bool DoContact { get; set; }
        
        [Display(Name = "Current status")]
        public string Status { get; set; }

        [Display(Name = "Scheduled to stop?")]
        public bool? ScheduledToStop { get; set; } = false;

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Algorithm type")]
        public string AlgorithmType { get; set; }

        [Range(Int32.MinValue, Int32.MaxValue)]
        [Display(Name = "Genetic random seed")]
        public int? GeneticRandomSeed { get; set; }

        [Range(1, 20000)]
        [Display(Name = "Genetic maximum iterations")]
        public int? GeneticMaxIteration { get; set; }

        [Range(1, 2000)]
        [Display(Name = "Genetic maximum iterations with no improvement")]
        public int? GeneticMaxIterationNoImprovement { get; set; }

        [Range(1, 25)]
        [Display(Name = "Genetic maximum path length")]
        public int? GeneticMaxPathLength { get; set; }

        [Range(1, 200)]
        [Display(Name = "Genetic population size")]
        public int? GeneticPopulationSize { get; set; }

        [Range(1, 100)]
        [Display(Name = "Genetic random elements per step")]
        public int? GeneticElementsRandom { get; set; }

        [Range(0.00, 1.00)]
        [Display(Name = "Genetic percentage of random chromosomes")]
        public double? GeneticPercentageRandom { get; set; }

        [Range(0.00, 1.00)]
        [Display(Name = "Genetic percentage of elite chromosomes")]
        public double? GeneticPercentageElite { get; set; }

        [Range(0.00, 1.00)]
        [Display(Name = "Genetic probability of mutation")]
        public double? GeneticProbabilityMutation { get; set; }

        [Range(Int32.MinValue, Int32.MaxValue)]
        [Display(Name = "Greedy random seed")]
        public int? GreedyRandomSeed { get; set; }

        [Range(1, 20000)]
        [Display(Name = "Greedy maximum iterations")]
        public int? GreedyMaxIteration { get; set; }

        [Range(1, 2000)]
        [Display(Name = "Greedy maximum iterations with no improvement")]
        public int? GreedyMaxIterationNoImprovement { get; set; }

        [Range(1, 25)]
        [Display(Name = "Greedy maximum path length")]
        public int? GreedyMaxPathLength { get; set; }

        [Display(Name = "Greedy cut the paths to driven nodes")]
        public bool? GreedyCutToDriven { get; set; }

        [Display(Name = "Greedy cut the non branching paths")]
        public bool? GreedyCutNonBranching { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Greedy search heuristics")]
        public string GreedyHeuristics { get; set; }
    }
}
