using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace Mvc3QA.Models.Mapping
{
    public class Pts_ProblemsMap : EntityTypeConfiguration<Pts_Problems>
    {
        public Pts_ProblemsMap()
        {
            // Primary Key
            this.HasKey(t => t.ProblemID);

            // Properties
            this.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Description)
                .IsRequired();

            this.Property(t => t.Content)
                .IsRequired();

            this.Property(t => t.AssignedTo)
                .HasMaxLength(50);

            this.Property(t => t.AssignedToUser)
                .HasMaxLength(50);

            this.Property(t => t.CreateUser)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CreatUserName)
                .HasMaxLength(50);

            this.Property(t => t.HandlingUser)
                .HasMaxLength(50);        

            // Table & Column Mappings
            this.ToTable("Pts_Problems");
            this.Property(t => t.ProblemID).HasColumnName("ProblemID");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Content).HasColumnName("Content");
            this.Property(t => t.AssignedTo).HasColumnName("AssignedTo");
            this.Property(t => t.AssignedToUser).HasColumnName("AssignedToUser");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
            this.Property(t => t.CreateTime).HasColumnName("CreateTime");
            this.Property(t => t.StartTime).HasColumnName("StartTime");
            this.Property(t => t.CloseTime).HasColumnName("CloseTime");
            this.Property(t => t.RealStartTime).HasColumnName("RealStartTime");
            this.Property(t => t.RealEndTime).HasColumnName("RealEndTime");
            this.Property(t => t.IsClosed).HasColumnName("IsClosed");
            this.Property(t => t.IsStart).HasColumnName("IsStart");
            this.Property(t => t.CreatUserName).HasColumnName("CreatUserName");
            this.Property(t => t.HandlingUser).HasColumnName("HandlingUser");
            this.Property(t => t.ProblemStateID).HasColumnName("ProblemStateID");

            //Relationships
            //this.HasOptional(t => t.Pts_ProblemState)
            //    .WithMany(t => t.Pts_Problems)
            //    .HasForeignKey(d => d.ProblemStateID);

        }
    }
}
