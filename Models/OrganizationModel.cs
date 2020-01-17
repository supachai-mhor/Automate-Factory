using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomateBussiness.Models
{
    public class OrganizationModel
    {
        public int id { get; set; }
        public string ids { get; set; }
        public string name { get; set; }
        public string position { get; set; }
        public string photo { get; set; }
        public string phone { get; set; }
        public string address{ get; set; }
        public string email { get; set; }
        public string? parent { get; set; }
        public int? work_quality { get; set; }
        public int? initiative { get; set; }
        public int? cooperative { get; set; }

    }
}
