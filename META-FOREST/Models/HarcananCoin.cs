using System;
using System.ComponentModel.DataAnnotations;

namespace MetaForest.Models
{
    /// <summary>
    /// Kullanıcıların mağazada harcadıkları coinleri takip etmek için model
    /// </summary>
    public class HarcananCoin
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string ItemName { get; set; } = string.Empty;

        public int SpentAmount { get; set; }

        public DateTime PurchaseDate { get; set; }
    }
}
