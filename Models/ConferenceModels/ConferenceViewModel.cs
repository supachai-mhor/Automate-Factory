using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomateBussiness.Models.ConferenceModels
{
    public class ConferenceViewModel
    {
        public OrganizationViewModel user { get; set; }
        public IEnumerable<MachineViewModel> machines { get; set; }
        public IEnumerable<ChatHistorys> chatHistorys { get; set; }
        public IEnumerable<ChatGroups> chatGroups { get; set; }
        public IEnumerable<Relationships> relationships { get; set; }
    }
}
