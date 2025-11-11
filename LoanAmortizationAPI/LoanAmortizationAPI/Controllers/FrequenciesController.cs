using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoanManagementSystem.Data;
using LoanManagementSystem.Models;
using LoanManagementSystem.DTOs;

namespace LoanManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FrequenciesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FrequenciesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Frequencies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FrequencyDto>>> GetFrequencies()
        {
            var frequencies = await _context.Frequencies
                .Select(f => new FrequencyDto
                {
                    AutoId = f.AutoId,
                    FrequencyType = f.FrequencyType,
                    PaymentsPerYear = f.PaymentsPerYear,
                    DaysBetweenPayments = f.DaysBetweenPayments
                })
                .ToListAsync();

            return Ok(frequencies);
        }

        // GET: api/Frequencies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FrequencyDto>> GetFrequency(int id)
        {
            var frequency = await _context.Frequencies
                .Where(f => f.AutoId == id)
                .Select(f => new FrequencyDto
                {
                    AutoId = f.AutoId,
                    FrequencyType = f.FrequencyType,
                    PaymentsPerYear = f.PaymentsPerYear,
                    DaysBetweenPayments = f.DaysBetweenPayments
                })
                .FirstOrDefaultAsync();

            if (frequency == null)
            {
                return NotFound(new { message = "Frequency not found" });
            }

            return Ok(frequency);
        }

        // POST: api/Frequencies
        [HttpPost]
        public async Task<ActionResult<FrequencyDto>> CreateFrequency(CreateFrequencyDto createDto)
        {
            if (await _context.Frequencies.AnyAsync(f => f.FrequencyType == createDto.FrequencyType))
            {
                return BadRequest(new { message = "Frequency type already exists" });
            }

            var frequency = new Frequency
            {
                FrequencyType = createDto.FrequencyType,
                PaymentsPerYear = createDto.PaymentsPerYear,
                DaysBetweenPayments = createDto.DaysBetweenPayments
            };

            _context.Frequencies.Add(frequency);
            await _context.SaveChangesAsync();

            var frequencyDto = new FrequencyDto
            {
                AutoId = frequency.AutoId,
                FrequencyType = frequency.FrequencyType,
                PaymentsPerYear = frequency.PaymentsPerYear,
                DaysBetweenPayments = frequency.DaysBetweenPayments
            };

            return CreatedAtAction(nameof(GetFrequency), new { id = frequency.AutoId }, frequencyDto);
        }

        // PUT: api/Frequencies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFrequency(int id, UpdateFrequencyDto updateDto)
        {
            var frequency = await _context.Frequencies.FindAsync(id);

            if (frequency == null)
            {
                return NotFound(new { message = "Frequency not found" });
            }

            // Check for duplicate frequency type
            if (await _context.Frequencies.AnyAsync(f => 
                f.FrequencyType == updateDto.FrequencyType && f.AutoId != id))
            {
                return BadRequest(new { message = "Frequency type already exists" });
            }

            frequency.FrequencyType = updateDto.FrequencyType;
            frequency.PaymentsPerYear = updateDto.PaymentsPerYear;
            frequency.DaysBetweenPayments = updateDto.DaysBetweenPayments;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Frequencies.AnyAsync(f => f.AutoId == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Frequencies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFrequency(int id)
        {
            var frequency = await _context.Frequencies.FindAsync(id);

            if (frequency == null)
            {
                return NotFound(new { message = "Frequency not found" });
            }

            // Check if frequency has loans
            var hasLoans = await _context.Loans.AnyAsync(l => l.Frequency == id);
            if (hasLoans)
            {
                return BadRequest(new { message = "Cannot delete frequency with existing loans" });
            }

            _context.Frequencies.Remove(frequency);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Frequencies/first
        [HttpGet("first")]
        public async Task<ActionResult<FrequencyDto>> GetFirstFrequency()
        {
            var frequency = await _context.Frequencies
                .OrderBy(f => f.AutoId)
                .Select(f => new FrequencyDto
                {
                    AutoId = f.AutoId,
                    FrequencyType = f.FrequencyType,
                    PaymentsPerYear = f.PaymentsPerYear,
                    DaysBetweenPayments = f.DaysBetweenPayments
                })
                .FirstOrDefaultAsync();

            if (frequency == null)
            {
                return NotFound(new { message = "No frequencies found" });
            }

            return Ok(frequency);
        }

        // GET: api/Frequencies/last
        [HttpGet("last")]
        public async Task<ActionResult<FrequencyDto>> GetLastFrequency()
        {
            var frequency = await _context.Frequencies
                .OrderByDescending(f => f.AutoId)
                .Select(f => new FrequencyDto
                {
                    AutoId = f.AutoId,
                    FrequencyType = f.FrequencyType,
                    PaymentsPerYear = f.PaymentsPerYear,
                    DaysBetweenPayments = f.DaysBetweenPayments
                })
                .FirstOrDefaultAsync();

            if (frequency == null)
            {
                return NotFound(new { message = "No frequencies found" });
            }

            return Ok(frequency);
        }

        // GET: api/Frequencies/5/previous
        [HttpGet("{id}/previous")]
        public async Task<ActionResult<FrequencyDto>> GetPreviousFrequency(int id)
        {
            var frequency = await _context.Frequencies
                .Where(f => f.AutoId < id)
                .OrderByDescending(f => f.AutoId)
                .Select(f => new FrequencyDto
                {
                    AutoId = f.AutoId,
                    FrequencyType = f.FrequencyType,
                    PaymentsPerYear = f.PaymentsPerYear,
                    DaysBetweenPayments = f.DaysBetweenPayments
                })
                .FirstOrDefaultAsync();

            if (frequency == null)
            {
                return NotFound(new { message = "No previous frequency found" });
            }

            return Ok(frequency);
        }

        // GET: api/Frequencies/5/next
        [HttpGet("{id}/next")]
        public async Task<ActionResult<FrequencyDto>> GetNextFrequency(int id)
        {
            var frequency = await _context.Frequencies
                .Where(f => f.AutoId > id)
                .OrderBy(f => f.AutoId)
                .Select(f => new FrequencyDto
                {
                    AutoId = f.AutoId,
                    FrequencyType = f.FrequencyType,
                    PaymentsPerYear = f.PaymentsPerYear,
                    DaysBetweenPayments = f.DaysBetweenPayments
                })
                .FirstOrDefaultAsync();

            if (frequency == null)
            {
                return NotFound(new { message = "No next frequency found" });
            }

            return Ok(frequency);
        }

        // GET: api/Frequencies/count
        [HttpGet("count")]
        public async Task<ActionResult<object>> GetFrequencyCount()
        {
            var count = await _context.Frequencies.CountAsync();
            return Ok(new { count });
        }

        // POST: api/Frequencies/5/refresh
        [HttpPost("{id}/refresh")]
        public async Task<ActionResult<FrequencyDto>> RefreshFrequency(int id)
        {
            // This mimics the RefreshData_Click from VBA
            // Simply returns the current frequency data (requery simulation)
            return await GetFrequency(id);
        }
    }
}
