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
        public int RunId { get; set; }

        public ApplicationUser User { get; set; }
        
        [Required]
        [StringLength(100, ErrorMessage = "You must name your analysis", MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Analysis Name")]
        public string AnalysisName { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Time { get; set; }

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
        [DataType(DataType.Text)]
        [Display(Name = "Algorithm parameters")]
        public string AlgorithmParams { get; set; }

        [Required]
        [Display(Name = "Send an e-mail?")]
        public bool DoContact { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Network")]
        public string Network { get; set; }

        [Display(Name = "Progress (%)")]
        public double? Progress { get; set; }

        [Display(Name = "Best result")]
        [DataType(DataType.Text)]
        public string BestResult { get; set; }

        [Display(Name = "Status")]
        public bool? IsCompleted { get; set; }

        [Display(Name = "Scheduled to stop?")]
        public bool? ScheduledToStop { get; set; }
    }
}
