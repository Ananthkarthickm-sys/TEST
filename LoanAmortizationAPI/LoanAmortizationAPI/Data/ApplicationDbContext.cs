using Microsoft.EntityFrameworkCore;
using LoanManagementSystem.Models;

namespace LoanManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Frequency> Frequencies { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<PaymentSchedule> PaymentSchedules { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Frequency
            modelBuilder.Entity<Frequency>(entity =>
            {
                entity.HasKey(e => e.AutoId);
                entity.HasIndex(e => e.FrequencyType).IsUnique();
            });

            // Configure Loan
            modelBuilder.Entity<Loan>(entity =>
            {
                entity.HasKey(e => e.AutoId);

                entity.HasOne(e => e.FrequencyNavigation)
                      .WithMany(f => f.Loans)
                      .HasForeignKey(e => e.Frequency)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.LoanNumber);
                entity.HasIndex(e => e.IsActive);
            });

            // Configure PaymentSchedule
            modelBuilder.Entity<PaymentSchedule>(entity =>
            {
                entity.HasKey(e => e.AutoId);

                entity.HasOne(e => e.Loan)
                      .WithMany(l => l.PaymentSchedules)
                      .HasForeignKey(e => e.LoanId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.LoanId, e.PaymentNumber });
            });

            // Seed initial frequency data
            modelBuilder.Entity<Frequency>().HasData(
                new Frequency { AutoId = 1, FrequencyType = "Weekly", PaymentsPerYear = 52, DaysBetweenPayments = 7 },
                new Frequency { AutoId = 2, FrequencyType = "Bi-Weekly", PaymentsPerYear = 26, DaysBetweenPayments = 14 },
                new Frequency { AutoId = 3, FrequencyType = "Semi-Monthly", PaymentsPerYear = 24, DaysBetweenPayments = 15 },
                new Frequency { AutoId = 4, FrequencyType = "Monthly", PaymentsPerYear = 12, DaysBetweenPayments = 30 },
                new Frequency { AutoId = 5, FrequencyType = "Quarterly", PaymentsPerYear = 4, DaysBetweenPayments = 91 },
                new Frequency { AutoId = 6, FrequencyType = "Semi-Annually", PaymentsPerYear = 2, DaysBetweenPayments = 182 },
                new Frequency { AutoId = 7, FrequencyType = "Annually", PaymentsPerYear = 1, DaysBetweenPayments = 365 }
            );
        }
    }
}
