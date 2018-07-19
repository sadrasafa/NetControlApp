using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetControlApp.Models.Dashboard2ViewModels
{
    public class NewAnalysisViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "You must name your analysis", MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Name of the analysis")]
        public string AnalysisName { get; set; }

        [Required]
        [Display(Name = "Type of the provided nodes")]
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

        [Display(Name = "Algorithm type")]
        public int AlgorithmType { get; set; }

        [Display(Name = "Send an e-mail?")]
        public bool DoContact { get; set; }

        public string StatusMessage { get; set; }
    }
}
