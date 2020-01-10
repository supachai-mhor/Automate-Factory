using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMovie.Models
{
    
    public class UserAccount
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string UserName { get; set; }
        public string HashedPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
