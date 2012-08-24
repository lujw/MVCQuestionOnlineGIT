using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace Mvc3QA.Models.Mapping
{
    public class Pts_ProblemStateMap : EntityTypeConfiguration<Pts_ProblemState>
    {
        public Pts_ProblemStateMap()
        {
            // Primary Key
            this.HasKey(t => t.ProblemStateID);

            // Properties
            this.Property(t => t.StateName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("Pts_ProblemState");
            this.Property(t => t.ProblemStateID).HasColumnName("ProblemStateID");
            this.Property(t => t.StateName).HasColumnName("StateName");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.ListOrder).HasColumnName("ListOrder");
            this.Property(t => t.IsFinalState).HasColumnName("IsFinalState");
            this.Property(t => t.IsCustomerShow).HasColumnName("IsCustomerShow");
            this.Property(t => t.DeptID).HasColumnName("DeptID");
            this.Property(t => t.IsState).HasColumnName("IsState");
        }
    }
}
