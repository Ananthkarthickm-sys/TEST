namespace LoanManagementSystem.DTOs
{
    // Frequency DTOs
    public class FrequencyDto
    {
        public int AutoId { get; set; }
        public string FrequencyType { get; set; } = string.Empty;
        public int PaymentsPerYear { get; set; }
        public int DaysBetweenPayments { get; set; }
    }

    public class CreateFrequencyDto
    {
        public string FrequencyType { get; set; } = string.Empty;
        public int PaymentsPerYear { get; set; }
        public int DaysBetweenPayments { get; set; }
    }

    public class UpdateFrequencyDto
    {
        public string FrequencyType { get; set; } = string.Empty;
        public int PaymentsPerYear { get; set; }
        public int DaysBetweenPayments { get; set; }
    }

    // Loan DTOs
    public class LoanDto
    {
        public int AutoId { get; set; }
        public string? CompanyId { get; set; }
        public string? AssociatedAccountId { get; set; }
        public bool IsActive { get; set; }
        public int? Ordering { get; set; }
        public string? Title { get; set; }
        public string? LoanNumber { get; set; }
        public DateOnly? PaymentDate { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CashDown { get; set; }
        public decimal? FinancedAmount { get; set; }
        public int? Frequency { get; set; }
        public string? FrequencyType { get; set; }
        public decimal? NumberYears { get; set; }
        public decimal? InterestRate { get; set; }
        public decimal? CorrectionRate { get; set; }
        public decimal? FinalInterestRate { get; set; }
        public decimal? PeriodInterestRate { get; set; }
        public string? Notes { get; set; }
        public decimal? SignedTermYears { get; set; }
        public string? SignedTermType { get; set; }
        public DateOnly? SignedMaturityDate { get; set; }
        public int? SignedTermNumberOfPayments { get; set; }
        public decimal? SignedTermRemaining { get; set; }
        public bool ExtraPaymentTrigger { get; set; }
        public decimal? LoanExtraPayment { get; set; }
        public int? NumberOfPayments { get; set; }
        public decimal? PaymentAmount { get; set; }
        public decimal? FinalPaymentAmount { get; set; }
        public decimal? AnnualTaxAmount { get; set; }
        public decimal? PeriodTaxAmount { get; set; }
        public decimal? NewTermLength { get; set; }
        public int? NewNumberOfPayments { get; set; }
        public decimal? TotalInterest { get; set; }
        public decimal? TotalPrincipal { get; set; }
        public decimal? TotalExtraPayment { get; set; }
        public decimal MonthlyTaxAmount { get; set; }
        public string CorrectionRateStr { get; set; }
        public string PeriodInterestRateStr { get; set; }
        public string finalInterestRateStr { get; set; }
    }

    public class CreateLoanDto
    {
        public string? CompanyId { get; set; }
        public string? AssociatedAccountId { get; set; }
        public bool IsActive { get; set; } = true;
        public int? Ordering { get; set; }
        public string? Title { get; set; }
        public string? LoanNumber { get; set; }
        public DateOnly? PaymentDate { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CashDown { get; set; }
        public int? Frequency { get; set; }
        public decimal? NumberYears { get; set; }
        public decimal? InterestRate { get; set; }
        public decimal? CorrectionRate { get; set; }
        public string? Notes { get; set; }
        public decimal? SignedTermYears { get; set; }
        public string? SignedTermType { get; set; }
        public DateOnly? SignedMaturityDate { get; set; }
        public bool ExtraPaymentTrigger { get; set; }
        public decimal? LoanExtraPayment { get; set; }
        public decimal? AnnualTaxAmount { get; set; }
        public decimal? PeriodTaxAmount { get; set; }
    }

    public class UpdateLoanDto
    {
        public string? CompanyId { get; set; }
        public string? AssociatedAccountId { get; set; }
        public bool IsActive { get; set; }
        public int? Ordering { get; set; }
        public string? Title { get; set; }
        public string? LoanNumber { get; set; }
        public DateOnly? PaymentDate { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CashDown { get; set; }
        public int? Frequency { get; set; }
        public decimal? NumberYears { get; set; }
        public decimal? InterestRate { get; set; }
        public decimal? CorrectionRate { get; set; }
        public string? Notes { get; set; }
        public decimal? SignedTermYears { get; set; }
        public string? SignedTermType { get; set; }
        public DateOnly? SignedMaturityDate { get; set; }
        public bool ExtraPaymentTrigger { get; set; }
        public decimal? LoanExtraPayment { get; set; }
        public decimal? AnnualTaxAmount { get; set; }
        public decimal? PeriodTaxAmount { get; set; }

    }

    // Payment Schedule DTOs
    public class PaymentScheduleDto
    {
        public int AutoId { get; set; }
        public int LoanId { get; set; }
        public int PaymentNumber { get; set; }
        public DateOnly? ScheduleDate { get; set; }
        public decimal? PaymentAmount { get; set; }
        public decimal? Principal { get; set; }
        public decimal? Interest { get; set; }
        public decimal? InterestRateAnnual { get; set; }
        public decimal? InterestRatePerPeriod { get; set; }
        public decimal? StartingBalance { get; set; }
        public decimal? EndingBalance { get; set; }
        public decimal? EndingBalanceExtra { get; set; }
        public decimal? LoanExtraPaymentPerPeriod { get; set; }
    }

    // Loan Calculation Request
    public class LoanCalculationRequest
    {
        public decimal PurchasePrice { get; set; }
        public decimal CashDown { get; set; }
        public int FrequencyId { get; set; }
        public decimal NumberYears { get; set; }
        public decimal InterestRate { get; set; }
        public decimal CorrectionRate { get; set; }
        public decimal LoanExtraPayment { get; set; }
        public decimal AnnualTaxAmount { get; set; }
        public bool ExtraPaymentTrigger { get; set; }      
    }

    // Loan Calculation Response
    public class LoanCalculationResponse
    {
        public decimal FinancedAmount { get; set; }
        public int NumberOfPayments { get; set; }
        public decimal PeriodInterestRate { get; set; }
        public decimal FinalInterestRate { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal FinalPaymentAmount { get; set; }
        public decimal MonthlyTaxAmount { get; set; }
        public decimal TotalPaymentWithTax { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal SignedTermRemaining { get; set; }
        public string PeriodInterestRateStr { get; set; }
        public string finalInterestRateStr { get; set; }

    }

    // Chart Data DTOs
    public class LoanChartDataDto
    {
        public List<ChartPoint> PrincipalData { get; set; } = new();
        public List<ChartPoint> InterestData { get; set; } = new();
    }

    public class ChartPoint
    {
        public int PaymentNumber { get; set; }
        public decimal Value { get; set; }
    }
}
