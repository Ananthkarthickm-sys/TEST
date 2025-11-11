using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanManagementSystem.Models
{
    [Table("tbl_loan")]
    public class Loan
    {
        [Key]
        [Column("autoid")]
        public int AutoId { get; set; }

        [Column("company_id")]
        [StringLength(100)]
        public string? CompanyId { get; set; }

        [Column("associated_account_id")]
        [StringLength(100)]
        public string? AssociatedAccountId { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("ordering")]
        public int? Ordering { get; set; }

        [Column("title")]
        [StringLength(200)]
        public string? Title { get; set; }

        [Column("loan_number")]
        [StringLength(100)]
        public string? LoanNumber { get; set; }

        [Column("payment_date")]
        public DateOnly? PaymentDate { get; set; }

        [Column("purchase_price", TypeName = "decimal(18,2)")]
        public decimal? PurchasePrice { get; set; }

        [Column("cash_down",TypeName = "decimal(18,2)")]
        public decimal? CashDown { get; set; }

        [Column("financed_amount",TypeName = "decimal(18,2)")]
        public decimal? FinancedAmount { get; set; }

        [Column("frequency")]
        public int? Frequency { get; set; }

        [Column("number_years")]
        public decimal? NumberYears { get; set; }

        [Column("interest_rate")]
        public decimal? InterestRate { get; set; }

        [Column("correction_rate")]
        public decimal? CorrectionRate { get; set; }

        [Column("final_interest_rate")]
        public decimal? FinalInterestRate { get; set; }

        [Column("period_interest_rate")]
        public decimal? PeriodInterestRate { get; set; }

        [Column("notes", TypeName = "text")]
        public string? Notes { get; set; }

        [Column("signed_term_years")]
        public decimal? SignedTermYears { get; set; }

        [Column("signed_term_type")]
        [StringLength(50)]
        public string? SignedTermType { get; set; }

        [Column("signed_maturity_date")]
        public DateOnly? SignedMaturityDate { get; set; }

        [Column("signed_term_number_of_payments")]
        public int? SignedTermNumberOfPayments { get; set; }

        [Column("signed_term_remaining")]
        public decimal? SignedTermRemaining { get; set; }

        [Column("extra_payment_trigger")]
        public bool ExtraPaymentTrigger { get; set; } = false;

        [Column("loan_extra_payment", TypeName = "decimal(18,4)")]
        public decimal? LoanExtraPayment { get; set; }

        [Column("number_of_payments")]
        public int? NumberOfPayments { get; set; }

        [Column("payment_amount",TypeName = "decimal(18,4)")]
        public decimal? PaymentAmount { get; set; }

        [Column("final_payment_amount",TypeName = "decimal(18,4)")]
        public decimal? FinalPaymentAmount { get; set; }

        [Column("annual_tax_amount", TypeName = "decimal(18,4)")]
        public decimal? AnnualTaxAmount { get; set; }

        [Column("period_tax_amount", TypeName = "decimal(18,4)")]
        public decimal? PeriodTaxAmount { get; set; }

        [Column("new_term_length")]
        public decimal? NewTermLength { get; set; }

        [Column("new_number_of_payments")]
        public int? NewNumberOfPayments { get; set; }

        [Column("total_interest", TypeName = "decimal(18,2)")]
        public decimal? TotalInterest { get; set; }

        [Column("total_principal", TypeName = "decimal(18,2)")]
        public decimal? TotalPrincipal { get; set; }

        [Column("total_extra_payment", TypeName = "decimal(18,2)")]
        public decimal? TotalExtraPayment { get; set; }

        [Column("attachment")]
        public byte[]? Attachment { get; set; }

        // Navigation properties
        [ForeignKey("Frequency")]
        public virtual Frequency? FrequencyNavigation { get; set; }

        public virtual ICollection<PaymentSchedule> PaymentSchedules { get; set; } = new List<PaymentSchedule>();

        // Computed properties
        [NotMapped]
        public decimal DecInterestRate => InterestRate.HasValue ? InterestRate.Value / 100 : 0;

        [NotMapped]
        public decimal MonthlyTaxAmount => AnnualTaxAmount.HasValue && FrequencyNavigation != null 
            ? AnnualTaxAmount.Value / FrequencyNavigation.PaymentsPerYear 
            : 0;
    }
}
