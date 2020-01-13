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
    public class AutomateBussinessContext : IdentityDbContext
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

        public DbSet<UserAccount> UserAccounts { get; set; }

        public DbSet<OrganizationModel> Organizations { get; set; }
    }
    //public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    //{
    //    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    //        : base(options)
    //    {
    //    }
    //    protected override void OnModelCreating(ModelBuilder builder)
    //    {
    //        base.OnModelCreating(builder);
    //    }
        
    //}

}