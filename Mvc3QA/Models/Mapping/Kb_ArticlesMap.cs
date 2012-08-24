using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace Mvc3QA.Models.Mapping
{
    public class Kb_ArticlesMap : EntityTypeConfiguration<Kb_Articles>
    {
        public Kb_ArticlesMap()
        {
            // Primary Key
            this.HasKey(t => t.ArticleID);

            // Properties
            this.Property(t => t.Title)
                .HasMaxLength(255);

            this.Property(t => t.KeyWords)
                .HasMaxLength(1024);

            this.Property(t => t.Summary)
                .HasMaxLength(2048);

            this.Property(t => t.CreateUser)
                .HasMaxLength(50);

            this.Property(t => t.UpdateUser)
                .HasMaxLength(50);

            this.Property(t => t.Source)
                .HasMaxLength(255);

            this.Property(t => t.SourceUrl)
                .HasMaxLength(255);

            this.Property(t => t.OriginAuthor)
                .HasMaxLength(50);

            this.Property(t => t.ReviewedBy)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Kb_Articles");
            this.Property(t => t.ArticleID).HasColumnName("ArticleID");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.KeyWords).HasColumnName("KeyWords");
            this.Property(t => t.Summary).HasColumnName("Summary");
            this.Property(t => t.Content).HasColumnName("Content");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
            this.Property(t => t.UpdateUser).HasColumnName("UpdateUser");
            this.Property(t => t.NumRead).HasColumnName("NumRead");
            this.Property(t => t.IsRepost).HasColumnName("IsRepost");
            this.Property(t => t.NumRatings).HasColumnName("NumRatings");
            this.Property(t => t.RateValue).HasColumnName("RateValue");
            this.Property(t => t.ArticleStatus).HasColumnName("ArticleStatus");
            this.Property(t => t.Source).HasColumnName("Source");
            this.Property(t => t.SourceUrl).HasColumnName("SourceUrl");
            this.Property(t => t.ValueLevel).HasColumnName("ValueLevel");
            this.Property(t => t.Difficulty).HasColumnName("Difficulty");
            this.Property(t => t.RelateProblemID).HasColumnName("RelateProblemID");
            this.Property(t => t.OriginAuthor).HasColumnName("OriginAuthor");
            this.Property(t => t.CreateUserID).HasColumnName("CreateUserID");
            this.Property(t => t.NumComment).HasColumnName("NumComment");
            this.Property(t => t.IsReviewed).HasColumnName("IsReviewed");
            this.Property(t => t.ReviewedBy).HasColumnName("ReviewedBy");
            this.Property(t => t.IsPublic).HasColumnName("IsPublic");
            this.Property(t => t.Note1).HasColumnName("Note1");
            this.Property(t => t.Note2).HasColumnName("Note2");
            this.Property(t => t.Note3).HasColumnName("Note3");
            this.Property(t => t.Note4).HasColumnName("Note4");
            this.Property(t => t.Note5).HasColumnName("Note5");

            // Relationships
            this.HasRequired(t => t.Kb_Categories)
                .WithMany(t => t.Kb_Articles)
                .HasForeignKey(d => d.CategoryID);

        }
    }
}
