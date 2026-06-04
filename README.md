# 🌳 MetaForest - Oyunlaştırılmış Üretkenlik Platformu

MetaForest, kullanıcıların odaklanma (Pomodoro) seanslarını tamamlayarak dijital varlıklar (fidanlar, karakterler) kazandığı ve bu varlıkları kendi dijital adalarındaki 5x5'lik ızgaralara (grid) yerleştirebildiği ASP.NET Core MVC tabanlı bir verimlilik uygulamasıdır.

## 🚀 Git & GitHub Çalışma Kuralları (Çakışmaları Önlemek İçin)

Projede 3 kişi eşzamanlı çalıştığımız için **kesinlikle doğrudan `main` dalına (branch) kod yazmıyoruz.** Her göreve başlarken şu adımları izlemeliyiz:

1. `git checkout main` (Ana dala geçin)
2. `git pull origin main` (En güncel kodları bilgisayarınıza çekin)
3. `git checkout -b feature/kendi-adiniz-gorev-adi` (Yeni bir dal oluşturup ona geçin)
4. Kodlamanızı yapın, test edin.
5. `git add .` ve `git commit -m "Geliştirme açıklaması"` ile kaydedin.
6. `git push origin feature/kendi-adiniz-gorev-adi` ile GitHub'a gönderin.
7. GitHub üzerinden bir **Pull Request (PR)** açın. Ekip arkadaşlarınız onayladıktan sonra `main` dalına birleştirin (Merge).

---

## 🎯 Sprint 1: Görev Dağılımı ve Yol Haritası

Bu sprintin amacı uygulamanın temel arayüz etkileşimlerini ve yönetim panelini ayağa kaldırmaktır.

### 🧑‍💻 1. ENES: Yönetici (Admin) Paneli Tasarımı
* **Çalışılacak Dal (Branch):** `feature/enes-admin-panel`
* **Görev:** Yönetici paneli arayüzünü oluşturmak.
* **Detaylar:** `Views/Home/AdminPanel.cshtml` sayfasını tasarlamak. Veritabanındaki `RewardAssets` tablosundaki verileri çekip (Kazanılabilen varlıklar/fazlar) bu sayfada şık bir Bootstrap tablosu içerisinde listelemek.

### 🧑‍💻 2. MERT: Izgara Sürükle-Bırak (Drag & Drop) Mekaniği
* **Çalışılacak Dal (Branch):** `feature/mert-drag-drop`
* **Görev:** Kullanıcının kazandığı varlıkların haritada yerini değiştirebilmesini sağlamak.
* **Detaylar:** `Views/Home/Index.cshtml` içerisindeki HTML5 karelerine (grid-cell) Drag & Drop özelliği kazandırmak. Bir varlık yeni bir kareye bırakıldığında arka plandaki `HomeController`'a AJAX isteği atarak yeni `GridX` ve `GridY` koordinatlarını veritabanında güncellemek.

### 🧑‍💻 3. ARDA: Atmosfer ve Hava Durumu API Entegrasyonu
* **Çalışılacak Dal (Branch):** `feature/arda-weather-api`
* **Görev:** Uygulamaya gerçek zamanlı hava durumu atmosferi katmak.
* **Detaylar:** `Services` klasörü oluşturup içine basit bir hava durumu API'sinden (örn. OpenWeatherMap) veri çeken servis yazmak. Bu veriyi (Örn: 🌤️ İstanbul 24°C) `_Layout.cshtml` içindeki üst menüde (Navbar) kullanıcılara göstermek.

---
**Not:** Takıldığınız yerlerde ekip arkadaşlarınızla iletişime geçmeyi ve küçük parçalar halinde commit atmayı unutmayın. Başarılar! 🚀
