using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutomateBussiness.Models;
using AutomateBussiness.Models.ConferenceModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutomateBussiness.Data
{
    public class AutomateBussinessContext : IdentityDbContext<AccountViewModel>
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

        public DbSet<AccountViewModel> AccountsTable { get; set; }

        public DbSet<OrganizationViewModel> OrganizationTable { get; set; }

        public DbSet<FactoryViewModel> FactoryTable { get; set; }
        public DbSet<MachineViewModel> MachineTable { get; set; }
        public DbSet<MachineDailyViewModel> MachineDailyTable { get; set; }
        public DbSet<MachineErrorViewModel> MachineErrorTable { get; set; }

        public DbSet<Relationship> RelationshipsTable { get; set; }
        public DbSet<ChatHistorys> ChatHistorysTable { get; set; }
        public DbSet<ChatGroups> ChatGroupsTable { get; set; }

    }
        

}