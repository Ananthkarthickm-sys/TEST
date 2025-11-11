using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanManagementSystem.Models
{
    [Table("tbl_payment_schedule")]
    public class PaymentSchedule
    {
        [Key]
        [Column("autoid")]
        public int AutoId { get; set; }

        [Column("loan_id")]
        [Required]
        public int LoanId { get; set; }

        [Column("payment_number")]
        [Required]
        public int PaymentNumber { get; set; }

        [Column("schedule_date")]
        public DateOnly? ScheduleDate { get; set; }

        [Column("payment_amount", TypeName = "decimal(18,2)")]
        public decimal? PaymentAmount { get; set; }

        [Column("principal", TypeName = "decimal(18,2)")]
        public decimal? Principal { get; set; }

        [Column("interest", TypeName = "decimal(18,2)")]
        public decimal? Interest { get; set; }

        [Column("interest_rate_annual")]
        public decimal? InterestRateAnnual { get; set; }

        [Column("interest_rate_per_period")]
        public decimal? InterestRatePerPeriod { get; set; }

        [Column("starting_balance", TypeName = "decimal(18,2)")]
        public decimal? StartingBalance { get; set; }

        [Column("ending_balance", TypeName = "decimal(18,2)")]
        public decimal? EndingBalance { get; set; }

        [Column("ending_balance_extra", TypeName = "decimal(18,2)")]
        public decimal? EndingBalanceExtra { get; set; }

        [Column("loan_extra_payment_per_period", TypeName = "decimal(18,2)")]
        public decimal? LoanExtraPaymentPerPeriod { get; set; }

        // Navigation property
        [ForeignKey("LoanId")]
        public virtual Loan? Loan { get; set; }
    }
}
