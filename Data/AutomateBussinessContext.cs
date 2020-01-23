using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutomateBussiness.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutomateBussiness.Data
{
    public class AutomateBussinessContext : IdentityDbContext<FactoryAccount>
    {
        public AutomateBussinessContext(DbContextOptions<AutomateBussinessContext> options)
            : base(options)
        {
        }
         protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<Movie> Movie { get; set; }

        public DbSet<FactoryAccount> FactoryAccounts { get; set; }

        public DbSet<OrganizationModel> Organizations { get; set; }
    }
        

}