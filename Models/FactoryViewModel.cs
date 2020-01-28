using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutomateBussiness.Models
{
    public class FactoryViewModel
    {
        public int id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string factoryName { get; set; }

        [Display(Name = "Founder")]
        public string founder { get; set; }

        [Display(Name = "FoundDate")]
        public DateTime foundDate { get; set; }

        [Display(Name = "CapitalRegister")]
        public string capitalRegister { get; set; }

        [Display(Name = "Description")]
        public string factoryDescription { get; set; }

        [Display(Name = "BusinessArea")]
        public string businessArea { get; set; }

        [Display(Name = "Address")]
        public string address { get; set; }

        [Phone]
        [Display(Name = "Phone")]
        public string phone { get; set; }

        [Phone]
        [Display(Name = "Mobile")]
        public string mobile { get; set; }

        [Phone]
        [Display(Name = "Fax")]
        public string fax { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string email { get; set; }

        [Url]
        [Display(Name = "Website")]
        public string website { get; set; }

        [RegularExpression(@"^(0|[1-9]\d*)$")]
        [Display(Name = "TaxID")]
        public string taxId { get; set; }

    }
}
