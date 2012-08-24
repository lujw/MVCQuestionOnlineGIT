using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace Mvc3QA.Models.Mapping
{
    public class Pts_ProblemCategoryMap : EntityTypeConfiguration<Pts_ProblemCategory>
    {
        public Pts_ProblemCategoryMap()
        {
            // Primary Key
            this.HasKey(t => t.CategoryID);

            // Properties
            this.Property(t => t.TypeName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Description)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("Pts_ProblemCategory");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.ParentID).HasColumnName("ParentID");
            this.Property(t => t.TypeName).HasColumnName("TypeName");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.ListOrder).HasColumnName("ListOrder");
        }
    }
}
