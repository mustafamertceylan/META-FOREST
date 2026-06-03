namespace MetaForest.Models
{
    public class PlantedAsset
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        // İlişkili asset (Hangi ağaç veya karakter dikildi?)
        public int RewardAssetId { get; set; }
        public RewardAsset RewardAsset { get; set; }

        // Izgara (Grid) Sistemindeki Koordinatları (Örn: 5x5 grid için 0-4 arası)
        public int GridX { get; set; }
        public int GridY { get; set; }

        public string ActiveRealm { get; set; } // Hangi tematik adada duruyor?
        public DateTime PlantedAt { get; set; } // Dikilme tarihi
    }
}