using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetControlApp.Models.DashboardViewModels
{
    public class NewAnalysis2ViewModel
    {
        [Required]
        [StringLength(100,ErrorMessage ="You must name your analysis", MinimumLength =3)]
        [DataType(DataType.Text)]
        [Display(Name ="Nume")]
        public string AnalysisName { get; set; }

    }
}
