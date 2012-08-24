using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace Mvc3QA.Models.Mapping
{
    public class Accounts_DepartmentMap : EntityTypeConfiguration<Accounts_Department>
    {
        public Accounts_DepartmentMap()
        {
            // Primary Key
            this.HasKey(t => t.DeptID);

            // Properties
            this.Property(t => t.DeptName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Accounts_Department");
            this.Property(t => t.DeptID).HasColumnName("DeptID");
            this.Property(t => t.DeptName).HasColumnName("DeptName");
            this.Property(t => t.ListOrder).HasColumnName("ListOrder");
            this.Property(t => t.IsSubmit).HasColumnName("IsSubmit");
        }
    }
}
