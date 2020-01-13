using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomateBussiness.Models
{
    public class OrganizationModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public int? ReportingManager { get; set; }
    }
}
