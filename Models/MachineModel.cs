using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutomateBussiness.Models
{
    public class MachineModel
    {
        public int id { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string name { get; set; }

        [Required]
        [Display(Name = "Plant")]
        public string plant { get; set; }

        [Required]
        [Display(Name = "Line")]
        public string line { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string description { get; set; }

        [Required]
        [Display(Name = "Vendor")]
        public string vendor{ get; set; }

        [Required]
        [Display(Name = "Supervisor")]
        public string supervisor { get; set; }

        [Required]
        [Display(Name = "Installed Date")]
        public DateTime? installed_date { get; set; }

    }
    public class MachineDailyModel
    {
        public int id { get; set; }

        [Required]
        [Display(Name = "Date")]
        public DateTime record_date { get; set; }

        [Required]
        [Display(Name = "McName")]
        public int machine_id { get; set; }

        [Required]
        [Display(Name = "JobNumber")]
        public string job_number { get; set; }

        [Required]
        [Display(Name = "Input")]
        public int total_input { get; set; }

        [Required]
        [Display(Name = "Reject")]
        public int total_reject { get; set; }

        [Required]
        [Display(Name = "RunningTime(s)")]
        public int runningtime { get; set; }

        [Required]
        [Display(Name = "DownTime(s)")]
        public int downtime { get; set; }

        [Required]
        [Display(Name = "IdleTime(s)")]
        public int idletime { get; set; }

        [Required]
        [Display(Name = "BreakTime(s)")]
        public int breaktime { get; set; }
        
    }
    public enum ErrorType
    {
        MachineError,
        ToolError,
        ProgramError,
        SetupAndAdjustmentError,
        MaterialError,
        HumanError,
        Etc
    }


    public class MachineErrorRecordModel
    {
        public int id { get; set; }

        [Required]
        [Display(Name = "Date")]
        public DateTime record_date { get; set; }

        [Required]
        [Display(Name = "Name")]
        public int machine_id { get; set; }

        [Required]
        [Display(Name = "JobNumber")]
        public string job_number { get; set; }

        [Required]
        [Display(Name = "Type")]
        public int errorType { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string description { get; set; }

        [Required]
        [Display(Name = "Solution")]
        public int solve { get; set; }

        [Required]
        [Display(Name = "InformBy")]
        public string informby { get; set; }

        [Required]
        [Display(Name = "SolvedBy")]
        public string solvedby { get; set; }

       
    }
}
