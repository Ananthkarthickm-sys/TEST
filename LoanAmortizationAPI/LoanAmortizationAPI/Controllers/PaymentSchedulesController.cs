using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoanManagementSystem.Data;
using LoanManagementSystem.DTOs;

namespace LoanManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentSchedulesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentSchedulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PaymentSchedules/loan/5
        [HttpGet("loan/{loanId}")]
        public async Task<ActionResult<IEnumerable<PaymentScheduleDto>>> GetPaymentSchedulesByLoan(int loanId)
        {
            var schedules = await _context.PaymentSchedules
                .Where(ps => ps.LoanId == loanId)
                .OrderBy(ps => ps.PaymentNumber)
                .Select(ps => new PaymentScheduleDto
                {
                    AutoId = ps.AutoId,
                    LoanId = ps.LoanId,
                    PaymentNumber = ps.PaymentNumber,
                    ScheduleDate = ps.ScheduleDate,
                    PaymentAmount = ps.PaymentAmount,
                    Principal = ps.Principal,
                    Interest = ps.Interest,
                    InterestRateAnnual = ps.InterestRateAnnual,
                    InterestRatePerPeriod = ps.InterestRateAnnual,
                    StartingBalance = ps.StartingBalance,
                    EndingBalance = ps.EndingBalance,
                    EndingBalanceExtra = ps.EndingBalanceExtra,
                    LoanExtraPaymentPerPeriod = ps.LoanExtraPaymentPerPeriod
                })
                .ToListAsync();

            return Ok(schedules);
        }

        // GET: api/PaymentSchedules/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentScheduleDto>> GetPaymentSchedule(int id)
        {
            var schedule = await _context.PaymentSchedules
                .Where(ps => ps.AutoId == id)
                .Select(ps => new PaymentScheduleDto
                {
                    AutoId = ps.AutoId,
                    LoanId = ps.LoanId,
                    PaymentNumber = ps.PaymentNumber,
                    ScheduleDate = ps.ScheduleDate,
                    PaymentAmount = ps.PaymentAmount,
                    Principal = ps.Principal,
                    Interest = ps.Interest,
                    InterestRateAnnual = ps.InterestRateAnnual,
                    InterestRatePerPeriod = ps.InterestRatePerPeriod,
                    StartingBalance = ps.StartingBalance,
                    EndingBalance = ps.EndingBalance,
                    EndingBalanceExtra = ps.EndingBalanceExtra,
                    LoanExtraPaymentPerPeriod = ps.LoanExtraPaymentPerPeriod
                })
                .FirstOrDefaultAsync();

            if (schedule == null)
            {
                return NotFound(new { message = "Payment schedule not found" });
            }

            return Ok(schedule);
        }

        // GET: api/PaymentSchedules/loan/5/chart-data
        [HttpGet("loan/{loanId}/chart-data")]
        public async Task<ActionResult<LoanChartDataDto>> GetChartData(int loanId)
        {
            var schedules = await _context.PaymentSchedules
                .Where(ps => ps.LoanId == loanId && ps.PaymentNumber > 0)
                .OrderBy(ps => ps.PaymentNumber)
                .Select(ps => new
                {
                    ps.PaymentNumber,
                    ps.Principal,
                    ps.Interest
                })
                .ToListAsync();

            var chartData = new LoanChartDataDto
            {
                PrincipalData = schedules
                    .Where(s => s.Principal.HasValue)
                    .Select(s => new ChartPoint
                    {
                        PaymentNumber = s.PaymentNumber,
                        Value = s.Principal.Value
                    })
                    .ToList(),
                InterestData = schedules
                    .Where(s => s.Interest.HasValue)
                    .Select(s => new ChartPoint
                    {
                        PaymentNumber = s.PaymentNumber,
                        Value = s.Interest.Value
                    })
                    .ToList()
            };

            return Ok(chartData);
        }

        // DELETE: api/PaymentSchedules/loan/5
        [HttpDelete("loan/{loanId}")]
        public async Task<IActionResult> DeleteScheduleByLoan(int loanId)
        {
            var schedules = await _context.PaymentSchedules
                .Where(ps => ps.LoanId == loanId)
                .ToListAsync();

            if (!schedules.Any())
            {
                return NotFound(new { message = "No payment schedules found for this loan" });
            }

            _context.PaymentSchedules.RemoveRange(schedules);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
