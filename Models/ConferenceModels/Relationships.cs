using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AutomateBussiness.Models.ConferenceModels
{
    public enum RelationStatus
    {
        unaccept,
        accepted
    }
    public enum RelationType
    {
        Contacts,
        groups,
        vendors,
        customers,
        machines
    }
    public class Relationship
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string id { get; set; }

        [Required]
        public DateTime relationDate { get; set; }

        [Required]
        public string requestId { get; set; }

        [Required]
        public string responedId { get; set; }

        public string requestById { get; set; }

        [Required]
        public RelationType relationType { get; set; }
        
        [Required]
        public RelationStatus relationStatus { get; set; }

        [Required]
        public bool isFavorites { get; set; }

    }
}
