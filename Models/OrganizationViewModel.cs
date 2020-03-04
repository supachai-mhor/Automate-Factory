using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutomateBussiness.Models
{
    public class OrganizationViewModel
    {
        public int id { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string name { get; set; }
        [Required]
        [Display(Name = "Position")]
        public string position { get; set; }

        [Required]
        [Display(Name = "Photo")]
        //[RegularExpression(@"^(0|[1-9]\d*)$")]
        public string photo { get; set; }
        [Required]
        [Phone]
        [Display(Name = "Phone")]
        public string phone { get; set; }
        [Required]
        [Display(Name = "Address")]
        public string address{ get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string email { get; set; }

        //[RegularExpression(@"^(0|[1-9]\d*)$")]
        
        [Display(Name = "Leader Name")]
        public string? parent { get; set; }
        [Range(0, 5)]
        [Display(Name = "Work Quality")]
        public int? work_quality { get; set; }
        [Range(0, 5)]
        [Display(Name = "Initiative")]
        public int? initiative { get; set; }
        [Range(0, 5)]
        [Display(Name = "Cooperative")]
        public int? cooperative { get; set; }

        [Required]
        [Display(Name = "FactoryID")]
        public int factoryID { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "DefaultPassword")]
        public string defaultPassword { get; set; }

        [Display(Name = "NickName")]
        public string nickName { get; set; }

        [Display(Name = "AccessType")]
        public AccessType accessType { get; set; }
    }
    public enum AccessType
    {
        Admin,
        CEO,
        Manager,
        Supervisor,
        Operator
    }
}
