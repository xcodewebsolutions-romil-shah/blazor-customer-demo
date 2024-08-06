using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Customers.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Customers.Data;
using Customers.Data.Domains;
using Customers.Data.Views;

namespace Customers.Data
{
    public partial class CustomersDBContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public CustomersDBContext()
        {
        }

        public CustomersDBContext(DbContextOptions<CustomersDBContext> options) : base(options)
        {
        }

        public DbSet<Collection> Collections { get; set; }
        public DbSet<CollectionSOW> CollectionSOWs { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerLicense> CustomerLicenses { get; set; }
        public DbSet<CustomerUsers> CustomerUsers { get; set; }
        public DbSet<KnownWord> KnownWords { get; set; }
        public DbSet<LicenseDefinition> LicenseDefinitions { get; set; }
        public DbSet<CollectionAnalysis> CollectionAnalyses { get; set; }
        public DbSet<AnalysisDetail> AnalysisDetails { get; set; }
        public DbSet<CollectionProposal> CollectionProposals { get; set; }
        public DbSet<CollectionUsers> CollectionUsers { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<AnalysisReportParameter> AnalysisReportParameters { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        /// <summary>
        /// View and Stored Procedure
        /// </summary>
        public DbSet<vCollectionSOWList> vCollectionSOWList { get; set; }
        public DbSet<vCollectionProposalList> vCollectionProposalList { get; set; }
        public DbSet<vCollectionDashboard> vCollectionDashboard { get; set; }
        public DbSet<vCollectionAnalysisDetails> vcollectionanalysisdetail { get; set; }


        partial void OnModelBuilding(ModelBuilder builder);

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                   .HasMany(u => u.Roles)
                   .WithMany(r => r.Users)
                   .UsingEntity<IdentityUserRole<int>>();

            builder.Entity<CollectionSOW>()
                .HasOne(ss => ss.Collection)
                .WithMany(ss => ss.CollectionSOWs)
                .HasForeignKey(p => p.CollectionId);

            builder.Entity<vCollectionSOWList>(e =>
            {
                e.HasNoKey();
                e.ToView("vCollectionSOWList");
            });

            builder.Entity<vCollectionProposalList>(e =>
            {
                e.HasNoKey();
                e.ToView("vCollectionProposalList");
            });

            builder.Entity<vCollectionDashboard>(e =>
            {
                e.HasNoKey();
                e.ToView("vcollection_dashboard");
            });

            builder.Entity<vCollectionAnalysisDetails>(e =>
            {
                e.HasNoKey();
                e.ToView("vcollectionanalysisdetail");
            });

            this.OnModelBuilding(builder);
        }

    }
}