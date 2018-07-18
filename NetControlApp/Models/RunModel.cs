using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetControlApp.Models
{
    public class RunModel
    {
        [Key]
        public int RunId { get; set; }

        //[Required]
        public ApplicationUser User { get; set; }

        //[Required]
        public string RunName { get; set; }

        //[Required]
        public DateTime? Time { get; set; }

        //[Required]
        public string NetType { get; set; }

        //[Required]
        public string NetNodes { get; set; }

        //[Required]
        public bool? DoContact { get; set; }

        //[Required]
        public string Network { get; set; }

        //[Required]
        public string Target { get; set; }

        //[Required]
        public string DrugTarget { get; set; }

        //[Required]
        public string AlgorithmType { get; set; }

        //[Required]
        public string AlgorithmParams { get; set; }

        //[Required]
        public double? Progress { get; set; }

        //[Required]
        public string BestResult { get; set; }

        //[Required]
        public bool? IsCompleted { get; set; }
    }
}
