using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace Mvc3QA.Models.Mapping
{
    public class Pts_ProblemHistoryMap : EntityTypeConfiguration<Pts_ProblemHistory>
    {
        public Pts_ProblemHistoryMap()
        {
            // Primary Key
            this.HasKey(t => t.ProblemHistoryID);

            // Properties
            this.Property(t => t.UserID)
                .HasMaxLength(50);

            this.Property(t => t.Content)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Pts_ProblemHistory");
            this.Property(t => t.ProblemHistoryID).HasColumnName("ProblemHistoryID");
            this.Property(t => t.ProblemID).HasColumnName("ProblemID");
            this.Property(t => t.ChangeDate).HasColumnName("ChangeDate");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.Content).HasColumnName("Content");

            // Relationships
            this.HasRequired(t => t.Pts_Problems)
                .WithMany(t => t.Pts_ProblemHistory)
                .HasForeignKey(d => d.ProblemID);

        }
    }
}
