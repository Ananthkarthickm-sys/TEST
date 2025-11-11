using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoanManagementSystem.Data;
using LoanManagementSystem.Models;
using LoanManagementSystem.DTOs;
using LoanManagementSystem.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LoanManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILoanCalculationService _calculationService;

        public LoansController(ApplicationDbContext context, ILoanCalculationService calculationService)
        {
            _context = context;
            _calculationService = calculationService;
        }

        // GET: api/Loans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetLoans()
        {
            var loans = await _context.Loans
                .Include(l => l.FrequencyNavigation)
                .OrderBy(l => l.Ordering)
                .Select(l => new LoanDto
                {
                    AutoId = l.AutoId,
                    CompanyId = l.CompanyId,
                    AssociatedAccountId = l.AssociatedAccountId,
                    IsActive = l.IsActive,
                    Ordering = l.Ordering,
                    Title = l.Title,
                    LoanNumber = l.LoanNumber,
                    PaymentDate = l.PaymentDate,
                    PurchasePrice = l.PurchasePrice,
                    CashDown = l.CashDown,
                    FinancedAmount = l.FinancedAmount,
                    Frequency = l.Frequency,
                    FrequencyType = l.FrequencyNavigation != null ? l.FrequencyNavigation.FrequencyType : null,
                    NumberYears = l.NumberYears,
                    InterestRate = l.InterestRate,
                    CorrectionRate = l.CorrectionRate,
                    FinalInterestRate = l.FinalInterestRate,
                    PeriodInterestRate = l.PeriodInterestRate,
                    Notes = l.Notes,
                    SignedTermYears = l.SignedTermYears,
                    SignedTermType = l.SignedTermType,
                    SignedMaturityDate = l.SignedMaturityDate,
                    SignedTermNumberOfPayments = l.SignedTermNumberOfPayments,
                    SignedTermRemaining = l.SignedTermRemaining,
                    ExtraPaymentTrigger = l.ExtraPaymentTrigger,
                    LoanExtraPayment = l.LoanExtraPayment,
                    NumberOfPayments = l.NumberOfPayments,
                    PaymentAmount = l.PaymentAmount,
                    FinalPaymentAmount = l.FinalPaymentAmount,
                    AnnualTaxAmount = l.AnnualTaxAmount,
                    NewTermLength = l.NewTermLength,
                    NewNumberOfPayments = l.NewNumberOfPayments,
                    TotalInterest = l.TotalInterest,
                    TotalPrincipal = l.TotalPrincipal,
                    TotalExtraPayment = l.TotalExtraPayment,
                    MonthlyTaxAmount = l.MonthlyTaxAmount
                })
                .ToListAsync();

            return Ok(loans);
        }

        // GET: api/Loans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoanDto>> GetLoan(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.FrequencyNavigation)
                .Where(l => l.AutoId == id)
                .Select(l => new LoanDto
                {
                    AutoId = l.AutoId,
                    CompanyId = l.CompanyId,
                    AssociatedAccountId = l.AssociatedAccountId,
                    IsActive = l.IsActive,
                    Ordering = l.Ordering,
                    Title = l.Title,
                    LoanNumber = l.LoanNumber,
                    PaymentDate = l.PaymentDate,
                    PurchasePrice = l.PurchasePrice,
                    CashDown = l.CashDown,
                    FinancedAmount = l.FinancedAmount,
                    Frequency = l.Frequency,
                    FrequencyType = l.FrequencyNavigation != null ? l.FrequencyNavigation.FrequencyType : null,
                    NumberYears = l.NumberYears,
                    InterestRate = l.InterestRate,
                    CorrectionRate = l.CorrectionRate,
                    FinalInterestRate = l.FinalInterestRate,
                    PeriodInterestRate = l.PeriodInterestRate,
                    Notes = l.Notes,
                    SignedTermYears = l.SignedTermYears,
                    SignedTermType = l.SignedTermType,
                    SignedMaturityDate = l.SignedMaturityDate,
                    SignedTermNumberOfPayments = l.SignedTermNumberOfPayments,
                    SignedTermRemaining = l.SignedTermRemaining,
                    ExtraPaymentTrigger = l.ExtraPaymentTrigger,
                    LoanExtraPayment = l.LoanExtraPayment,
                    NumberOfPayments = l.NumberOfPayments,
                    PaymentAmount = l.PaymentAmount,
                    FinalPaymentAmount = l.FinalPaymentAmount,
                    AnnualTaxAmount = l.AnnualTaxAmount,
                    PeriodTaxAmount = l.PeriodTaxAmount,
                    NewTermLength = l.NewTermLength,
                    NewNumberOfPayments = l.NewNumberOfPayments,
                    TotalInterest = l.TotalInterest,
                    TotalPrincipal = l.TotalPrincipal,
                    TotalExtraPayment = l.TotalExtraPayment,
                    MonthlyTaxAmount = l.MonthlyTaxAmount,
                    CorrectionRateStr = l.CorrectionRate.ToString() ?? string.Empty,
                    PeriodInterestRateStr = l.PeriodInterestRate.ToString() ?? string.Empty,
                    finalInterestRateStr = l.FinalInterestRate.ToString() ?? string.Empty
                })
                .FirstOrDefaultAsync();

            if (loan == null)
            {
                return NotFound(new { message = "Loan not found" });
            }

            return Ok(loan);
        }

        // POST: api/Loans
        [HttpPost]
        public async Task<ActionResult<LoanDto>> CreateLoan(CreateLoanDto createDto)
        {
            var loan = new Loan
            {
                CompanyId = createDto.CompanyId,
                AssociatedAccountId = createDto.AssociatedAccountId,
                IsActive = createDto.IsActive,
                Ordering = createDto.Ordering,
                Title = createDto.Title,
                LoanNumber = createDto.LoanNumber,
                PaymentDate = createDto.PaymentDate,
                PurchasePrice = createDto.PurchasePrice,
                CashDown = createDto.CashDown,
                FinancedAmount = createDto.PurchasePrice - createDto.CashDown,
                Frequency = createDto.Frequency,
                NumberYears = createDto.NumberYears,
                InterestRate = createDto.InterestRate,
                CorrectionRate = createDto.CorrectionRate,
                Notes = createDto.Notes,
                SignedTermYears = createDto.SignedTermYears,
                SignedTermType = createDto.SignedTermType,
                SignedMaturityDate = createDto.SignedMaturityDate,
                ExtraPaymentTrigger = createDto.ExtraPaymentTrigger,
                LoanExtraPayment = createDto.LoanExtraPayment,
                AnnualTaxAmount = createDto.AnnualTaxAmount,
                PeriodTaxAmount = createDto.PeriodTaxAmount,                
            };

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            // Reload with frequency
            await _context.Entry(loan).Reference(l => l.FrequencyNavigation).LoadAsync();

            var loanDto = new LoanDto
            {
                AutoId = loan.AutoId,
                CompanyId = loan.CompanyId,
                AssociatedAccountId = loan.AssociatedAccountId,
                IsActive = loan.IsActive,
                Ordering = loan.Ordering,
                Title = loan.Title,
                LoanNumber = loan.LoanNumber,
                PaymentDate = loan.PaymentDate,
                PurchasePrice = loan.PurchasePrice,
                CashDown = loan.CashDown,
                FinancedAmount = loan.FinancedAmount,
                Frequency = loan.Frequency,
                FrequencyType = loan.FrequencyNavigation?.FrequencyType,
                NumberYears = loan.NumberYears,
                InterestRate = loan.InterestRate,
                CorrectionRate = loan.CorrectionRate,
                Notes = loan.Notes,
                SignedTermYears = loan.SignedTermYears,
                SignedTermType = loan.SignedTermType,
                SignedMaturityDate = loan.SignedMaturityDate,
                ExtraPaymentTrigger = loan.ExtraPaymentTrigger,
                LoanExtraPayment = loan.LoanExtraPayment,
                AnnualTaxAmount = loan.AnnualTaxAmount,
                PeriodTaxAmount = loan.PeriodTaxAmount,
            };

            return CreatedAtAction(nameof(GetLoan), new { id = loan.AutoId }, loanDto);
        }

        // PUT: api/Loans/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLoan(int id, UpdateLoanDto updateDto)
        {
            var loan = await _context.Loans.FindAsync(id);

            if (loan == null)
            {
                return NotFound(new { message = "Loan not found" });
            }

            loan.CompanyId = updateDto.CompanyId;
            loan.AssociatedAccountId = updateDto.AssociatedAccountId;
            loan.IsActive = updateDto.IsActive;
            loan.Ordering = updateDto.Ordering;
            loan.Title = updateDto.Title;
            loan.LoanNumber = updateDto.LoanNumber;
            loan.PaymentDate = updateDto.PaymentDate;
            loan.PurchasePrice = updateDto.PurchasePrice;
            loan.CashDown = updateDto.CashDown;
            loan.FinancedAmount = updateDto.PurchasePrice - updateDto.CashDown;
            loan.Frequency = updateDto.Frequency;
            loan.NumberYears = updateDto.NumberYears;
            loan.InterestRate = updateDto.InterestRate;
            loan.CorrectionRate = updateDto.CorrectionRate;
            loan.Notes = updateDto.Notes;
            loan.SignedTermYears = updateDto.SignedTermYears;
            loan.SignedTermType = updateDto.SignedTermType;
            loan.SignedMaturityDate = updateDto.SignedMaturityDate;
            loan.ExtraPaymentTrigger = updateDto.ExtraPaymentTrigger;
            loan.LoanExtraPayment = updateDto.LoanExtraPayment;
            loan.AnnualTaxAmount = updateDto.AnnualTaxAmount;
            loan.PeriodTaxAmount = updateDto.PeriodTaxAmount;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Loans.AnyAsync(l => l.AutoId == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Loans/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoan(int id)
        {
            var loan = await _context.Loans.FindAsync(id);

            if (loan == null)
            {
                return NotFound(new { message = "Loan not found" });
            }

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Loans/calculate
        [HttpPost("calculate")]
        public async Task<ActionResult<LoanCalculationResponse>> CalculateLoan(LoanCalculationRequest request)
        {
            try
            {
                var frequency = await _context.Frequencies.FindAsync(request.FrequencyId);
                if (frequency == null)
                {
                    return BadRequest(new { message = "Invalid frequency" });
                }
                var result = _calculationService.CalculateLoan(request, frequency);
                result.finalInterestRateStr = result.FinalInterestRate.ToString();
                result.PeriodInterestRateStr = result.PeriodInterestRate.ToString();
                return Ok(result);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        // POST: api/Loans/5/calculate-full
        [HttpPost("{id}/calculate-full")]
        public async Task<ActionResult<LoanDto>> CalculateFullLoan(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.FrequencyNavigation)
                .FirstOrDefaultAsync(l => l.AutoId == id);

            if (loan == null)
            {
                return NotFound(new { message = "Loan not found" });
            }

            if (loan.FrequencyNavigation == null)
            {
                return BadRequest(new { message = "Loan must have a frequency set" });
            }

            // Perform calculations
            var request = new LoanCalculationRequest
            {
                PurchasePrice = loan.PurchasePrice ?? 0,
                CashDown = loan.CashDown ?? 0,
                FrequencyId = loan.Frequency ?? 0,
                NumberYears = loan.NumberYears ?? 0,
                InterestRate = loan.InterestRate ?? 0,
                CorrectionRate = loan.CorrectionRate ?? 0,
                LoanExtraPayment = loan.LoanExtraPayment ?? 0,
                AnnualTaxAmount = loan.AnnualTaxAmount ?? 0,
                ExtraPaymentTrigger = loan.ExtraPaymentTrigger
            };

            var calculation = _calculationService.CalculateLoan(request, loan.FrequencyNavigation);

            // Update loan with calculated values
            loan.FinancedAmount = calculation.FinancedAmount;
            loan.NumberOfPayments = calculation.NumberOfPayments;
            loan.PeriodInterestRate = calculation.PeriodInterestRate;
            loan.FinalInterestRate = calculation.FinalInterestRate;
            loan.PaymentAmount = calculation.PaymentAmount;
            loan.FinalPaymentAmount = calculation.FinalPaymentAmount;
            if (loan.AnnualTaxAmount.HasValue && loan.AnnualTaxAmount > 0)
                loan.PeriodTaxAmount = loan.AnnualTaxAmount / loan.FrequencyNavigation.PaymentsPerYear;
            else
                loan.PeriodTaxAmount = loan.AnnualTaxAmount;
            if (loan.SignedTermYears.HasValue)
            {
                loan.SignedTermNumberOfPayments = (int)(loan.SignedTermYears.Value * loan.FrequencyNavigation.PaymentsPerYear);
            }
            else
            {
                loan.SignedTermNumberOfPayments = 0;
            }

            await _context.SaveChangesAsync();

            return await GetLoan(id);
        }

        // POST: api/Loans/5/generate-schedule
        [HttpPost("{id}/generate-schedule")]
        public async Task<ActionResult> GenerateSchedule(int id)
        {
            try
            {


                var loan = await _context.Loans
                    .Include(l => l.FrequencyNavigation)
                    .FirstOrDefaultAsync(l => l.AutoId == id);

                if (loan == null)
                {
                    return NotFound(new { message = "Loan not found" });
                }

                if (loan.FrequencyNavigation == null)
                {
                    return BadRequest(new { message = "Loan must have a frequency set" });
                }

                // Delete existing schedule
                var existingSchedule = await _context.PaymentSchedules
                    .Where(ps => ps.LoanId == id)
                    .ToListAsync();
                _context.PaymentSchedules.RemoveRange(existingSchedule);

                // Generate new schedule
                var schedule = await _calculationService.GeneratePaymentSchedule(id, loan, loan.FrequencyNavigation);
                
                //var schedules = _calculationService.GeneratePaymentSchedule(loan);

                // Calculate totals
                decimal totalInterest = 0;
                decimal totalPrincipal = 0;
                decimal totalExtraPayment = 0;
                int actualPaymentCount = 0;
                int loops = 0;
                foreach (var payment in schedule.Skip(1))
                {

                    if (payment.LoanExtraPaymentPerPeriod.HasValue) totalExtraPayment += payment.LoanExtraPaymentPerPeriod.Value;
                    actualPaymentCount++;
                    if (loops > 0)
                    {
                        if (payment.Interest.HasValue && payment.Interest.Value > 0) totalInterest += payment.Interest.Value;

                        if (payment.Principal.HasValue) totalPrincipal += payment.Principal.Value;
                    }
                    loops++;
                }
                //foreach (var payment in schedule.Skip(1))
                //{
                //    if ((payment.StartingBalance ?? 0) > 0)
                //    {
                //        if (payment.Interest.HasValue) totalInterest += payment.Interest.Value;
                //        if (payment.LoanExtraPaymentPerPeriod.HasValue) totalExtraPayment += payment.LoanExtraPaymentPerPeriod.Value;
                //        actualPaymentCount++;
                //    }
                //    if ((payment.EndingBalance ?? 0) > 0)
                //    {
                //        if (payment.Principal.HasValue) totalPrincipal += payment.Principal.Value;
                //    }
                //}
                // Update loan totals
                if (loan.ExtraPaymentTrigger && loan.LoanExtraPayment > 0)
                    totalExtraPayment = totalExtraPayment - loan.LoanExtraPayment ?? 0;
                loan.TotalInterest = Math.Round(totalInterest, 4);
                loan.TotalPrincipal = Math.Round(totalPrincipal, 4);
                loan.TotalExtraPayment = Math.Round(totalExtraPayment, 4);

                loan.NewNumberOfPayments = actualPaymentCount;
                loan.NewTermLength = (decimal)actualPaymentCount / loan.FrequencyNavigation.PaymentsPerYear;
                DateTime date = DateTime.Today; // your start date
                if (loan.SignedMaturityDate.HasValue && loan.SignedMaturityDate.Value != DateOnly.MinValue)
                {
                    DateTime signedMaturityDate = loan.SignedMaturityDate.Value.ToDateTime(TimeOnly.MinValue);
                    // Only calculate if maturity date is after today
                    if (signedMaturityDate > date)
                    {
                        // Calculate difference in days
                        double totalDays = (signedMaturityDate - date).TotalDays;
                        // Convert to years
                        double? signedTermRemaining = totalDays / 365;
                        // Convert to decimal and round
                        decimal value = (decimal)(signedTermRemaining ?? 0);
                        decimal roundedValue = Math.Round(value, 0, MidpointRounding.AwayFromZero);
                        loan.SignedTermRemaining = roundedValue;
                    }
                    else
                    {
                        // Maturity date is in the past or today
                        loan.SignedTermRemaining = 0m;
                    }
                }
                else
                {
                    // Null or default date
                    loan.SignedTermRemaining = 0m;
                }



                foreach (var payment in schedule)
                {
                    if (payment.ScheduleDate.HasValue)
                        payment.ScheduleDate = payment.ScheduleDate.Value;

                    payment.StartingBalance = Math.Abs(payment.StartingBalance ?? 0);
                    payment.Interest = Math.Abs(payment.Interest ?? 0);
                    payment.Principal = Math.Abs(payment.Principal ?? 0);
                    payment.EndingBalanceExtra = Math.Abs(payment.EndingBalanceExtra ?? 0);
                    payment.EndingBalance = Math.Abs(payment.EndingBalance ?? 0);
                }
                // Add schedule to database
                _context.PaymentSchedules.AddRange(schedule);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = $"Generated {schedule.Count} payment schedule entries using {loan.FrequencyNavigation.PaymentsPerYear} payments per year (every {loan.FrequencyNavigation.DaysBetweenPayments} days).",
                    scheduleCount = schedule.Count,
                    totalInterest = totalInterest,
                    totalPrincipal = totalPrincipal,
                    totalExtraPayment = totalExtraPayment,
                    actualPayments = actualPaymentCount
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //// POST: api/Loans/5/generate-schedule
        //[HttpPost("{id}/generate-schedule")]
        //public async Task<ActionResult> GenerateSchedule(int id)
        //{
        //    try
        //    {
        //        var loan = await _context.Loans
        //            .Include(l => l.FrequencyNavigation)
        //            .FirstOrDefaultAsync(l => l.AutoId == id);

        //        if (loan == null)
        //            return NotFound(new { message = "Loan not found" });

        //        if (loan.FrequencyNavigation == null)
        //            return BadRequest(new { message = "Loan must have a frequency set" });

        //        // Delete existing schedule
        //        var existingSchedule = await _context.PaymentSchedules
        //            .Where(ps => ps.LoanId == id)
        //            .ToListAsync();

        //        _context.PaymentSchedules.RemoveRange(existingSchedule);

        //        // Generate new schedule
        //        var schedule = await _calculationService.GeneratePaymentSchedule(id, loan, loan.FrequencyNavigation);

        //        // Calculate totals
        //        decimal totalInterest = 0;
        //        decimal totalPrincipal = 0;
        //        decimal totalExtraPayment = 0;
        //        int actualPaymentCount = 0;

        //        foreach (var payment in schedule.Skip(1)) // Skip initial "Payment 0"
        //        {
        //            if ((payment.StartingBalance ?? 0) > 0 && (payment.EndingBalance ?? 0) >= 0)
        //            {
        //                if (payment.Interest.HasValue) totalInterest += payment.Interest.Value;
        //            }
        //            if ((payment.StartingBalance ?? 0) > 0)
        //            {
        //                if (payment.Principal.HasValue) totalPrincipal += payment.Principal.Value;
        //                if (payment.LoanExtraPaymentPerPeriod.HasValue) totalExtraPayment += payment.LoanExtraPaymentPerPeriod.Value;
        //                actualPaymentCount++;
        //            }
        //        }

        //        // Round totals
        //        loan.TotalInterest = Math.Round(totalInterest, 5);
        //        loan.TotalPrincipal = Math.Round(totalPrincipal, 5);
        //        loan.TotalExtraPayment = Math.Round(totalExtraPayment, 5);

        //        // Update term details
        //        loan.NewNumberOfPayments = actualPaymentCount;
        //        loan.NewTermLength = (decimal)actualPaymentCount / loan.FrequencyNavigation.PaymentsPerYear;

        //        // Normalize schedule dates to UTC (optional)
        //        foreach (var payment in schedule)
        //        {
        //            if (payment.ScheduleDate.HasValue)
        //                payment.ScheduleDate = payment.ScheduleDate.Value;
        //        }

        //        // Add schedule to database
        //        _context.PaymentSchedules.AddRange(schedule);

        //        // Update loan record explicitly
        //        _context.Loans.Update(loan);

        //        await _context.SaveChangesAsync();

        //        return Ok(new
        //        {
        //            message = $"Generated {schedule.Count} payment schedule entries using {loan.FrequencyNavigation.PaymentsPerYear} payments per year (every {loan.FrequencyNavigation.DaysBetweenPayments} days).",
        //            scheduleCount = schedule.Count,
        //            totalInterest = loan.TotalInterest,
        //            totalPrincipal = loan.TotalPrincipal,
        //            totalExtraPayment = loan.TotalExtraPayment,
        //            actualPayments = actualPaymentCount
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
        //    }
        //}
    }
}
