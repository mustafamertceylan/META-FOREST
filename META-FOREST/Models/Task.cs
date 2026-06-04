using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetaForest.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı ID gereklidir")]
        public string? UserId { get; set; }

        [Required(ErrorMessage = "Görev başlığı gereklidir")]
        [StringLength(255, ErrorMessage = "Başlık 255 karakterden fazla olamaz")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Açıklama 1000 karakterden fazla olamaz")]
        public string? Description { get; set; }

        [Required]
        public bool IsCompleted { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Priority: Low, Medium, High
        [StringLength(20)]
        public string Priority { get; set; } = "Medium";

        // Status: Düşük, Orta, Yüksek, Tamamlandı
        [NotMapped]
        public string StatusLabel
        {
            get => IsCompleted ? "Tamamlandı" : Priority;
        }
    }
}
