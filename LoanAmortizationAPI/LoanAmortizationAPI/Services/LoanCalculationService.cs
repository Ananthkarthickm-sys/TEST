using LoanManagementSystem.Models;
using LoanManagementSystem.DTOs;

namespace LoanManagementSystem.Services
{
    public interface ILoanCalculationService
    {
        LoanCalculationResponse CalculateLoan(LoanCalculationRequest request, Frequency frequency);
        Task<List<PaymentSchedule>> GeneratePaymentSchedule(int loanId, Loan loan, Frequency frequency);

        List<PaymentSchedule> GeneratePaymentSchedule(Loan loan);

    }

    public class LoanCalculationService : ILoanCalculationService
    {
        public LoanCalculationResponse CalculateLoan(LoanCalculationRequest request, Frequency frequency)
        {
            var response = new LoanCalculationResponse();

            // Calculate financed amount
            response.FinancedAmount = request.PurchasePrice - request.CashDown;

            // Calculate number of payments
            response.NumberOfPayments = (int)(request.NumberYears * frequency.PaymentsPerYear);

            ////// Calculate period interest rate
            //response.PeriodInterestRate = (request.InterestRate / 100) / frequency.PaymentsPerYear;

            ////// Calculate final interest rate with correction
            //response.FinalInterestRate = response.PeriodInterestRate + (request.CorrectionRate / 100);

            decimal interestRate = request.InterestRate / 100m;
            decimal correctionRate = request.CorrectionRate;
            decimal paymentsPerYear = frequency.PaymentsPerYear;

            response.PeriodInterestRate = Math.Round(interestRate / paymentsPerYear, 10);
            response.FinalInterestRate = Math.Round(response.PeriodInterestRate + correctionRate, 10);


            // Calculate payment amount using PMT formula
            // PMT = (PV * rate) / (1 - (1 + rate)^(-nper))
            var rate = (double)response.FinalInterestRate;
            var nper = response.NumberOfPayments;
            var pv = (double)response.FinancedAmount;

            if (rate != 0)
            {
                // Payment at beginning of period (Type = 1)
                var temp = Math.Pow(1 + rate, nper);
                var _paymentAmount = (decimal)((pv * rate * temp) / ((temp - 1) * (1 + rate)));
                response.PaymentAmount = _paymentAmount;
            }
            else
            {
                response.PaymentAmount = (decimal)(pv / nper);
            }
            // Round to cents
            response.PaymentAmount = Math.Round(response.PaymentAmount, 4);

            // Calculate final payment amount with extra payment
            response.FinalPaymentAmount = response.PaymentAmount + request.LoanExtraPayment;

            // Calculate monthly tax amount
            response.MonthlyTaxAmount = request.AnnualTaxAmount / frequency.PaymentsPerYear;

            // Total payment with tax
            response.TotalPaymentWithTax = response.FinalPaymentAmount + response.MonthlyTaxAmount;

            // Generate message
            if (request.ExtraPaymentTrigger && request.LoanExtraPayment > 0)
            {
                response.Message = $"You'll pay ${response.TotalPaymentWithTax:N2}, for {response.NumberOfPayments} times. " +
                    $"This includes ${response.PaymentAmount:N2} loan payment, ${request.LoanExtraPayment:N2} extra payment " +
                    $"and ${response.MonthlyTaxAmount:N2} of taxes.";
            }
            else
            {
                response.Message = $"You'll pay ${response.TotalPaymentWithTax:N2}, for {response.NumberOfPayments} times. " +
                    $"This includes ${response.PaymentAmount:N2} loan payment, and ${response.MonthlyTaxAmount:N2} of taxes.";
            }

            return response;
        }
        //public async Task<List<PaymentSchedule>> GeneratePaymentSchedule(int loanId, Loan loan, Frequency frequency)
        //{
        //    var schedule = new List<PaymentSchedule>();

        //    if (loan == null || frequency == null) return schedule;
        //    if (!loan.FinancedAmount.HasValue || !loan.PaymentAmount.HasValue || !loan.NumberOfPayments.HasValue)
        //        return schedule;

        //    decimal balance = loan.FinancedAmount.Value;
        //    decimal rate = loan.FinalInterestRate ?? 0; // per period already
        //    decimal payment = loan.PaymentAmount.Value;
        //    int nper = loan.NumberOfPayments.Value;
        //    decimal extraPayment = loan.ExtraPaymentTrigger ? (loan.LoanExtraPayment ?? 0) : 0;

        //    DateOnly payDate = loan.PaymentDate ?? DateOnly.FromDateTime(DateTime.Today);

        //    // Payment 0 row
        //    schedule.Add(new PaymentSchedule
        //    {
        //        LoanId = loanId,
        //        PaymentNumber = 0,
        //        ScheduleDate = payDate,
        //        StartingBalance = 0,
        //        EndingBalance = balance,
        //        PaymentAmount = 0,
        //        Principal = 0,
        //        Interest = 0
        //    });

        //    for (int i = 1; i <= nper && balance > 0; i++)
        //    {
        //        payDate = payDate.AddDays(frequency.DaysBetweenPayments);

        //        decimal interest = balance * rate;
        //        decimal principal = payment - interest;
        //        if (principal < 0) principal = 0;

        //        decimal appliedExtra = Math.Min(extraPayment, balance - principal);
        //        decimal newBalance = balance - principal - appliedExtra;
        //        if (newBalance < 0) newBalance = 0;

        //        schedule.Add(new PaymentSchedule
        //        {
        //            LoanId = loanId,
        //            PaymentNumber = i,
        //            ScheduleDate = payDate,
        //            PaymentAmount = payment,
        //            Principal = principal,
        //            Interest = interest,
        //            StartingBalance = balance,
        //            EndingBalance = newBalance,
        //            LoanExtraPaymentPerPeriod = appliedExtra
        //        });

        //        balance = newBalance;
        //        if (balance <= 0.01m) break;
        //    }


        //    return await Task.FromResult(schedule);
        //}


        public async Task<List<PaymentSchedule>> GeneratePaymentSchedule(int loanId, Loan loan, Frequency frequency)
        {
            var schedule = new List<PaymentSchedule>();

            if (!loan.PaymentDate.HasValue || !loan.FinancedAmount.HasValue ||
                !loan.PaymentAmount.HasValue || !loan.NumberOfPayments.HasValue)
            {
                return schedule;
            }
            bool chkBit = false;
            var paymentNumber = 0;
            var scheduleDate = loan.PaymentDate.Value;
            var pv = loan.FinancedAmount.Value;
            var rate = loan.FinalInterestRate ?? 0;
            var paymentAmount = loan.PaymentAmount.Value;
            var extraPayment = loan.ExtraPaymentTrigger ? (loan.LoanExtraPayment ?? 0) : 0;
            var numberOfPayments = loan.NumberOfPayments.Value;

            // Add initial payment (Payment 0)
            schedule.Add(new PaymentSchedule
            {
                LoanId = loanId,
                PaymentNumber = paymentNumber,
                ScheduleDate = scheduleDate,
                PaymentAmount = paymentAmount,
                Principal = null,
                Interest = null,
                StartingBalance = null,
                EndingBalance = pv,
                InterestRateAnnual = loan.InterestRate,
                InterestRatePerPeriod = loan.FinalInterestRate,
                EndingBalanceExtra = null,
                LoanExtraPaymentPerPeriod = null
            });

            paymentNumber++;
            scheduleDate = scheduleDate.AddDays(frequency.DaysBetweenPayments);

            // Generate payment schedule
            for (int i = 1; i <= numberOfPayments + 4; i++)
            {
                if (chkBit) break;

                var startingBalance = pv;
                var interest = pv * rate;
                var principal = paymentAmount - interest;

                if (principal < 0) principal = 0;

                var endingBalance = pv - principal;
                var endingBalanceExtra = endingBalance - extraPayment;

                schedule.Add(new PaymentSchedule
                {
                    LoanId = loanId,
                    PaymentNumber = paymentNumber,
                    ScheduleDate = scheduleDate,
                    PaymentAmount = paymentAmount,
                    Principal = principal,
                    Interest = interest,
                    StartingBalance = startingBalance,
                    EndingBalance = endingBalance,
                    InterestRateAnnual = loan.InterestRate,
                    InterestRatePerPeriod = loan.FinalInterestRate,
                    EndingBalanceExtra = endingBalanceExtra,
                    LoanExtraPaymentPerPeriod = extraPayment
                });

                // Update PV for next iteration
                if (loan.ExtraPaymentTrigger)
                {
                    pv = endingBalanceExtra;
                }
                else
                {
                    pv = endingBalance;
                }
                if (startingBalance < 0 && chkBit == false)
                {
                    chkBit = true;
                }
                paymentNumber++;
                scheduleDate = scheduleDate.AddDays(frequency.DaysBetweenPayments);
            }
            int count = schedule.Count();
            return await Task.FromResult(schedule);
        }

        /// <summary>
        /// Generate complete payment schedule for a loan
        /// </summary>
        public List<PaymentSchedule> GeneratePaymentSchedule(Loan loan)
        {
            if (loan.Frequency == null)
            {
                throw new ArgumentException("Frequency information is required");
            }

            var schedules = new List<PaymentSchedule>();
            var paymentDate = loan.PaymentDate ?? DateOnly.MinValue;

            decimal pv = loan.FinancedAmount ?? 0;
            decimal sumInterest = 0;
            decimal sumPrincipal = 0;
            decimal sumExtraPayment = 0;

            int maxPayments = loan.NumberOfPayments ?? 0 + 4;
            decimal startingBalance = 0;
            decimal endingBalance = pv;

            // Create payment 0 - Initial balance
            schedules.Add(new PaymentSchedule
            {
                LoanId = loan.AutoId,
                PaymentNumber = 0,
                ScheduleDate = paymentDate,
                PaymentAmount = loan.PaymentAmount,
                Principal = null,
                Interest = null,
                StartingBalance = null,
                EndingBalance = pv,
                InterestRateAnnual = loan.InterestRate * 100,
                InterestRatePerPeriod = loan.FinalInterestRate,
                EndingBalanceExtra = null,
                LoanExtraPaymentPerPeriod = null
            });

            // Generate payment schedule
            for (int paymentNumber = 1; paymentNumber <= maxPayments; paymentNumber++)
            {
                if (startingBalance < 0)
                {
                    // Update loan with new term information
                    if (!loan.NewNumberOfPayments.HasValue)
                    {
                        loan.NewTermLength = (decimal)(paymentNumber - 1) / loan.FrequencyNavigation.PaymentsPerYear;
                        loan.NewNumberOfPayments = paymentNumber - 1;
                    }
                    break;
                }

                paymentDate = paymentDate.AddDays(loan.FrequencyNavigation.DaysBetweenPayments);

                // Calculate interest for this payment
                decimal interest = pv * (decimal)loan.FinalInterestRate;

                if (interest > 0 && startingBalance > 0)
                {
                    sumInterest += interest;
                }

                // Calculate principal for this payment
                decimal principal = loan.PaymentAmount - interest ?? 0;

                if (principal > 0 && startingBalance > 0)
                {
                    sumPrincipal += principal;
                }

                // Sum extra payments
                if (loan.ExtraPaymentTrigger && startingBalance > 0)
                {
                    sumExtraPayment += loan.LoanExtraPayment ?? 0;
                }

                startingBalance = pv;
                endingBalance = pv - principal;
                decimal extraEndingBalance = endingBalance - loan.LoanExtraPayment ?? 0;

                schedules.Add(new PaymentSchedule
                {
                    LoanId = loan.AutoId,
                    PaymentNumber = paymentNumber,
                    ScheduleDate = paymentDate,
                    PaymentAmount = loan.PaymentAmount,
                    Principal = principal,
                    Interest = interest,
                    StartingBalance = startingBalance,
                    EndingBalance = endingBalance,
                    InterestRateAnnual = loan.InterestRate * 100,
                    InterestRatePerPeriod = loan.FinalInterestRate,
                    EndingBalanceExtra = extraEndingBalance,
                    LoanExtraPaymentPerPeriod = loan.LoanExtraPayment
                });

                // Update PV for next iteration
                if (loan.ExtraPaymentTrigger)
                {
                    pv = extraEndingBalance;
                }
                else
                {
                    pv = endingBalance;
                }
            }

            // Update loan totals
            loan.TotalInterest = sumInterest;
            loan.TotalPrincipal = sumPrincipal;
            loan.TotalExtraPayment = sumExtraPayment;

            return schedules;
        }
    }
}
