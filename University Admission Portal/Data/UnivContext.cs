using Microsoft.EntityFrameworkCore;
using University_Admission_Portal.Models;

namespace University_Admission_Portal.Data
{
    public class UnivContext : DbContext
    {
        public UnivContext(DbContextOptions<UnivContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Application> Applications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Application>()
                .Property(a => a.Status)
                .HasConversion<string>();

            modelBuilder.Entity<User>()
                .HasOne(u => u.StudentProfile)
                .WithOne(s => s.User)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.StaffProfile)
                .WithOne(st => st.User)
                .HasForeignKey<Staff>(st => st.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // STUDENT -> APPLICATIONS : NO ACTION (prevent cascade)
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Applications)
                .WithOne(a => a.Student)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            // COURSE -> APPLICATIONS : RESTRICT (no cascade)
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Applications)
                .WithOne(a => a.Course)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // STAFF -> APPLICATIONS : SET NULL (staff removal doesn't delete applications)
            modelBuilder.Entity<Staff>()
                .HasMany(st => st.ApprovedApplications)
                .WithOne(a => a.ApprovedByStaff)
                .HasForeignKey(a => a.ApprovedByStaffId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
