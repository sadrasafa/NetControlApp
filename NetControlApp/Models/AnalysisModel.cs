using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetControlApp.Models
{
    public class AnalysisModel
    {
        [Key]
        public int RunId { get; set; }

        public ApplicationUser User { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "You must name your analysis", MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Analysis Name")]
        public string AnalysisName { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Time { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Is the network provided as seed nodes?")]
        public bool NetType { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "List of nodes")]
        public string NetNodes { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Target nodes")]
        public string Target { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Drug target nodes")]
        public string DrugTarget { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Algorithm type")]
        public string AlgorithmType { get; set; }

        [Required]
        [Display(Name = "Send an e-mail?")]
        public bool DoContact { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Network")]
        public string Network { get; set; }

        [Display(Name = "Progress (%)")]
        public double? Progress { get; set; } = 0.0;

        [Display(Name = "Best result")]
        [DataType(DataType.Text)]
        public string BestResult { get; set; }

        [Display(Name = "Status")]
        public bool? IsCompleted { get; set; } = false;

        [Display(Name = "Scheduled to stop?")]
        public bool? ScheduledToStop { get; set; } = false;

        [Range(Int32.MinValue, Int32.MaxValue)]
        [Display(Name = "Random seed")]
        public int? RandomSeed { get; set; }

        [Range(1, 20000)]
        [Display(Name = "Maximum iterations")]
        public int? MaxIteration { get; set; }

        [Range(1, 2000)]
        [Display(Name = "Maximum iterations with no improvement")]
        public int? MaxIterationNoImprovement { get; set; }

        [Range(1, 25)]
        [Display(Name = "Maximum path length")]
        public int? MaxPathLength { get; set; }

        [Range(1, 200)]
        [Display(Name = "Population size")]
        public int? GeneticPopulationSize { get; set; }

        [Range(1, 100)]
        [Display(Name = "Random elements per step")]
        public int? GeneticElementsRandom { get; set; }

        [Range(0.00, 1.00)]
        [Display(Name = "Percentage of random chromosomes")]
        public double? GeneticPercentageRandom { get; set; }

        [Range(0.00, 1.00)]
        [Display(Name = "Percentage of elite chromosomes")]
        public double? GeneticPercentageElite { get; set; }

        [Range(0.00, 1.00)]
        [Display(Name = "Probability of mutation")]
        public double? GeneticProbabilityMutation { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Heuristics")]
        public string GreedyHeuristics { get; set; }
    }
}
