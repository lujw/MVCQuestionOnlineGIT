using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace Mvc3QA.Models.Mapping
{
    public class Pts_RecordsMap : EntityTypeConfiguration<Pts_Records>
    {
        public Pts_RecordsMap()
        {
            // Primary Key
            this.HasKey(t => t.RecordID);

            // Properties
            this.Property(t => t.CreateUser)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Content)
                .IsRequired();

            this.Property(t => t.SrcUserID)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.AssignToObjectID)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.AssignTo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ResponseTotalTime)
                .HasMaxLength(50);

            this.Property(t => t.Course)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Pts_Records");
            this.Property(t => t.RecordID).HasColumnName("RecordID");
            this.Property(t => t.ProblemID).HasColumnName("ProblemID");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
            this.Property(t => t.CreateTime).HasColumnName("CreateTime");
            this.Property(t => t.Content).HasColumnName("Content");
            this.Property(t => t.ProblemStateID).HasColumnName("ProblemStateID");
            this.Property(t => t.RecordClass).HasColumnName("RecordClass");
            this.Property(t => t.SrcStateID).HasColumnName("SrcStateID");
            this.Property(t => t.SrcUserID).HasColumnName("SrcUserID");
            this.Property(t => t.AssignType).HasColumnName("AssignType");
            this.Property(t => t.AssignStateID).HasColumnName("AssignStateID");
            this.Property(t => t.AssignToObjectID).HasColumnName("AssignToObjectID");
            this.Property(t => t.AssignTo).HasColumnName("AssignTo");
            this.Property(t => t.ResponseWorkTime).HasColumnName("ResponseWorkTime");
            this.Property(t => t.ResponseTotalTime).HasColumnName("ResponseTotalTime");
            this.Property(t => t.RealTimeCost).HasColumnName("RealTimeCost");
            this.Property(t => t.Course).HasColumnName("Course");
            this.Property(t => t.Describe).HasColumnName("Describe");
            this.Property(t => t.ListOrder).HasColumnName("ListOrder");
            this.Property(t => t.YFTime).HasColumnName("YFTime");
            this.Property(t => t.YFWay).HasColumnName("YFWay");

            // Relationships
            this.HasRequired(t => t.Pts_Problems)
                .WithMany(t => t.Pts_Records)
                .HasForeignKey(d => d.ProblemID);
            this.HasRequired(t => t.Pts_ProblemState)
                .WithMany(t => t.Pts_Records)
                .HasForeignKey(d => d.ProblemStateID);

        }
    }
}
