using System;
using System.ComponentModel.DataAnnotations;

namespace MetaForest.Models
{
    public class FocusSession
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int DurationMinutes { get; set; }

        // Hatayı çözen kısım burası!
        public DateTime SessionDate { get; set; }

        public int GainedCoin { get; set; }

        public string? SelectedRealm { get; set; }
    }
}