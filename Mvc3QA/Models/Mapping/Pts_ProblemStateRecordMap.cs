using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace Mvc3QA.Models.Mapping
{
    public class Pts_ProblemStateRecordMap : EntityTypeConfiguration<Pts_ProblemStateRecord>
    {
        public Pts_ProblemStateRecordMap()
        {
            // Primary Key
            this.HasKey(t => t.RecordHistoryID);

            // Properties
            this.Property(t => t.UserID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Pts_ProblemStateRecord");
            this.Property(t => t.RecordHistoryID).HasColumnName("RecordHistoryID");
            this.Property(t => t.RecordID).HasColumnName("RecordID");
            this.Property(t => t.StateID).HasColumnName("StateID");
            this.Property(t => t.StartTime).HasColumnName("StartTime");
            this.Property(t => t.MaxEndTime).HasColumnName("MaxEndTime");
            this.Property(t => t.EndTime).HasColumnName("EndTime");
            this.Property(t => t.KeepTime).HasColumnName("KeepTime");
            this.Property(t => t.WorkgroupID).HasColumnName("WorkgroupID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.BizKeepTime).HasColumnName("BizKeepTime");

            // Relationships
            this.HasRequired(t => t.Pts_Records)
                .WithMany(t => t.Pts_ProblemStateRecord)
                .HasForeignKey(d => d.RecordID);

        }
    }
}
