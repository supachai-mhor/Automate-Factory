using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutomateBussiness.Models.ConferenceModels
{
    public class ChatGroups
    {
        public uint id { get; set; }

        [Required]
        public string groupID { get; set; }

        [Required]
        [Display(Name = "GroupName")]
        public string groupName { get; set; }

        [Display(Name = "Image")]
        public string groupImage { get; set; }

        public DateTime createDate { get; set; }

    }
}
