using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Mvc3QA.Models.Mapping;

namespace Mvc3QA.Models
{
    public class questiononlineContext : DbContext
    {
        static questiononlineContext()
        {
            Database.SetInitializer<questiononlineContext>(null);
        }

		public questiononlineContext()
			: base("Name=questiononlineContext")
		{
		}

        public DbSet<Accounts_Department> Accounts_Department { get; set; }     
        public DbSet<Kb_Articles> Kb_Articles { get; set; }
        public DbSet<Kb_Categories> Kb_Categories { get; set; }
        public DbSet<Pts_ProblemCategory> Pts_ProblemCategory { get; set; }
        public DbSet<Pts_ProblemHistory> Pts_ProblemHistory { get; set; }
        public DbSet<Pts_Problems> Pts_Problems { get; set; }
        public DbSet<Pts_ProblemState> Pts_ProblemState { get; set; }
        public DbSet<Pts_ProblemStateRecord> Pts_ProblemStateRecord { get; set; }
        public DbSet<Pts_Records> Pts_Records { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }   

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new Accounts_DepartmentMap());         
            modelBuilder.Configurations.Add(new Kb_ArticlesMap());
            modelBuilder.Configurations.Add(new Kb_CategoriesMap());
            modelBuilder.Configurations.Add(new Pts_ProblemCategoryMap());
            modelBuilder.Configurations.Add(new Pts_ProblemHistoryMap());
            modelBuilder.Configurations.Add(new Pts_ProblemsMap());
            modelBuilder.Configurations.Add(new Pts_ProblemStateMap());
            modelBuilder.Configurations.Add(new Pts_ProblemStateRecordMap());
            modelBuilder.Configurations.Add(new Pts_RecordsMap());
            modelBuilder.Configurations.Add(new sysdiagramMap());
           
        }
    }
}
