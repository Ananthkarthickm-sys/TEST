using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanManagementSystem.Models
{
    [Table("tbl_attachments")]
    public class Attachment
    {
        [Key]
        [Column("attachment_id")]
        public int AttachmentId { get; set; }

        [Column("file_name")]
        [StringLength(100)]
        public string FileName { get; set; } = "";

        [Column("file_type")]
        [StringLength(10)]
        public string FileType { get; set; } = "";

        [Column("file_size")]
        public long FileSize { get; set; }

        [Column("file_path")]
        public string FilePath { get; set; } = "";

        [Column("uploaded_at")]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [Column("uploaded_by")]
        public string? UploadedBy { get; set; }

        [Column("loan_id")]
        public int? LoanId { get; set; }

        [Column("description")]
        public string? Description { get; set; }
    }
}
