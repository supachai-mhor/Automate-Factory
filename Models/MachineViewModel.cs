using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AutomateBussiness.Models
{
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
    public class MachineViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
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
        public string vendor{ get; set; }

        [Required]
        [Display(Name = "SupervisorEmail")]
        public string supervisor { get; set; }

        [Required]
        [Display(Name = "Installed Date")]
        public DateTime installed_date { get; set; }

        [Required]
        [Display(Name = "FactoryID")]
        public int factoryID { get; set; }

        [Required]
        [Display(Name = "Image")]
        public string machineImage { get; set; }

        public string machineHashID { get; set; }

    }
    public class MachineDailyViewModel
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

        [Required]
        [Display(Name = "FactoryID")]
        public int factoryID { get; set; }
        }
    public class MachineErrorViewModel
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

        [Required]
        [Display(Name = "FactoryID")]
        public int factoryID { get; set; }
    }
    public class PlaningViewModel
    {
        public int id { get; set; }
        public string job_number { get; set; }
        public int planQty { get; set; }
        public int expectRatio { get; set; }
        public int qtyPerInput { get; set; }
        public string job_detail { get; set; }

    }
    public enum MachineState
    {
        isRunning,
        isDowntime,
        isIdle,
        isSetting
    }
    public class MachineData
    {
        public MachineState machineState;
        public double runningtimes;
        public double downTimetimes;
        public double settingtimes;
        public double idletimes;

        public TimeSpan RunningTimeSpan;
        public TimeSpan DownTimeSpan;
        public TimeSpan SettingTimeSpan;
        public TimeSpan IdleTimeSpan;

        public int totalInput;
        public int totalPass;

        public int input;
        public int pass;
        public double yield;
        public double oee;

        public string machineName;
        public string jobNumber;
        public string supervisorName;
        public string operatorName;
        public TimeSpan startTime;
        public TimeSpan endTime;

    }
}
