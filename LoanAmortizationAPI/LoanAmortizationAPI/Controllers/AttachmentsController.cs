using LoanManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using LoanManagementSystem.Models;
using LoanManagementSystem.DTOs;
using LoanManagementSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttachmentsController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public AttachmentsController(IWebHostEnvironment env, ApplicationDbContext context)
        {
            _env = env;
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] int loanId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected");

            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

            var uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadDir, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            var attachment = new Models.Attachment
            {
                LoanId = loanId,
                FileName = file.FileName,
                FilePath = $"/uploads/{uniqueName}",
                FileSize = file.Length,
                UploadedAt = DateTime.UtcNow
            };

            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();

            return Ok(attachment);
        }

        [HttpGet("GetByLoan/{loanId}")]
        public IActionResult GetByLoan(int loanId)
        {
            var attachments = _context.Attachments.Where(a => a.LoanId == loanId).ToList();
            return Ok(attachments);
        }

        [HttpGet("download/{id}")]
        public IActionResult DownloadAttachment(int id)
        {
            var attachment = _context.Attachments.FirstOrDefault(a => a.AttachmentId == id);
            if (attachment == null)
                return NotFound("Attachment not found");

            // Build the full path to the uploaded file
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            var fileName = Path.GetFileName(attachment.FilePath); // remove /uploads/
            var filePath = Path.Combine(uploadFolder, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File does not exist on server");

            // Optional: detect content type from file extension
            var contentType = "application/octet-stream";

            var fileBytes = System.IO.File.ReadAllBytes(filePath); // <-- use filePath
            return File(fileBytes, contentType, fileName);
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteAttachment(int id)
        {
            var attachment = _context.Attachments.FirstOrDefault(a => a.AttachmentId == id);
            if (attachment == null) return NotFound("Attachment not found");

            var fileName = Path.GetFileName(attachment.FilePath);
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            var filePath = Path.Combine(uploadFolder, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Attachments.Remove(attachment);
            _context.SaveChanges();

            return Ok(new { message = "Attachment deleted successfully" });
        }

    }
}
