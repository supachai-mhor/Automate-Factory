using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AutomateBussiness.Models.ConferenceModels
{
    public class ChatGroups
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string id { get; set; }

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
