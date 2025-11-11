using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanManagementSystem.Models
{
    [Table("tbl_frequency")]
    public class Frequency
    {
        [Key]
        [Column("autoid")]
        public int AutoId { get; set; }

        [Column("frequency_type")]
        [StringLength(100)]
        [Required]
        public string FrequencyType { get; set; } = string.Empty;

        [Column("payments_per_year")]
        [Required]
        public int PaymentsPerYear { get; set; }

        [Column("days_between_payments")]
        [Required]
        public int DaysBetweenPayments { get; set; }

        // Navigation property
        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
