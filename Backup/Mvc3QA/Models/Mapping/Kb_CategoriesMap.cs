using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace Mvc3QA.Models.Mapping
{
    public class Kb_CategoriesMap : EntityTypeConfiguration<Kb_Categories>
    {
        public Kb_CategoriesMap()
        {
            // Primary Key
            this.HasKey(t => t.CategoryID);

            // Properties
            this.Property(t => t.CategoryName)
                .IsRequired()
                .HasMaxLength(1024);

            this.Property(t => t.Path)
                .HasMaxLength(255);

            this.Property(t => t.AllowedUserGroups)
                .HasMaxLength(1000);

            // Table & Column Mappings
            this.ToTable("Kb_Categories");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.CategoryName).HasColumnName("CategoryName");
            this.Property(t => t.ParentID).HasColumnName("ParentID");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ArticleCount).HasColumnName("ArticleCount");
            this.Property(t => t.LastUpdate).HasColumnName("LastUpdate");
            this.Property(t => t.Path).HasColumnName("Path");
            this.Property(t => t.AllowedUserGroups).HasColumnName("AllowedUserGroups");
            this.Property(t => t.IsPublic).HasColumnName("IsPublic");
            this.Property(t => t.RequireReview).HasColumnName("RequireReview");
            this.Property(t => t.ListOrder).HasColumnName("ListOrder");
        }
    }
}
