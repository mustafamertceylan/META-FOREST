namespace MetaForest.Models
{
    public class RewardAsset
    {
        public int Id { get; set; }
        public string Name { get; set; } // Örn: "Siber Devre Ağacı", "Ejderha Yumurtası"
        public string RealmType { get; set; } // Hangi adaya ait? (cyber, dragon, cartoon)
        public int PhaseLevel { get; set; } // Kaçıncı evrim fazı? (1: Bebek/Fide, 2: Orta, 3: Savaşçı/Ulu Ağaç)
        public string WebmFileName { get; set; } // wwwroot/assets/animations/ altındaki dosya adı (Örn: cyber_lvl1.webm)
    }
}