using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Transportation_Management_System_Tracking_Service_REST_API.Models.Enums;

namespace Transportation_Management_System_Tracking_Service_REST_API.Models
{
    public class Document
    {
        [Key]
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey("OrderId")]
        public Guid OrderId { get; set; }

        public Order Order { get; set; } = default!;

        public DocumentType Type { get; set; }

        [Required]
        public string FileName { get; set; } = default!;

        [Required]
        public string StorageUrl { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}