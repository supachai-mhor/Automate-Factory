using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutomateBussiness.Models
{
    public class MachineCreateViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string name { get; set; }

        [Required]
        [Display(Name = "Plant")]
        public string plant { get; set; }

        [Required]
        [Display(Name = "Process")]
        public string process { get; set; }

        [Required]
        [Display(Name = "Line")]
        public string line { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string description { get; set; }

        [Required]
        [Display(Name = "Vendor")]
        public string vendor { get; set; }

        [Required]
        [Display(Name = "SupervisorEmail")]
        public string supervisor { get; set; }

        [Required]
        [Display(Name = "Installed Date")]
        public DateTime installed_date { get; set; }

        [Display(Name = "Image")]
        public IFormFile machineImage { get; set; }

    }
}
