/**
 * DYNAMIC ATMOSPHERE SYSTEM
 * Hava durumuna göre dinamik atmosfer efektleri uygular
 */

class AtmosphereManager {
    constructor() {
        this.currentWeather = null;
        this.weatherContainer = null;
        this.particles = [];
        this.particleCount = 50; // Yağmur/kar tanesi sayısı
        this.updateInterval = null;
        this.init();
    }

    /**
     * Initialize the atmosphere manager
     */
    init() {
        // Sayfaya CSS dosyasını ekle (eğer henüz eklenmemişse)
        this.ensureAtmosphereCSSLoaded();

        // Weather container'ı bul veya oluştur
        this.weatherContainer = document.body;

        // İlk hava durumunu oku
        this.updateWeather();

        // Her 30 saniyede hava durumunu güncelle
        this.updateInterval = setInterval(() => this.updateWeather(), 30000);

        // Sayfa kapatılırken interval'ı temizle
        window.addEventListener('beforeunload', () => clearInterval(this.updateInterval));
    }

    /**
     * CSS dosyasının yüklenmiş olduğundan emin ol
     */
    ensureAtmosphereCSSLoaded() {
        const linkExists = Array.from(document.getElementsByTagName('link'))
            .some(link => link.href.includes('atmosphere.css'));

        if (!linkExists) {
            const link = document.createElement('link');
            link.rel = 'stylesheet';
            link.href = '/css/atmosphere.css';
            document.head.appendChild(link);
        }
    }

    /**
     * Hava durumunu oku (navbar'dan veya API'den)
     */
    async updateWeather() {
        try {
            // Navbar'daki hava durumu bilgisini oku
            const weatherFromDOM = this.extractWeatherFromDOM();

            if (weatherFromDOM) {
                this.applyWeatherEffect(weatherFromDOM);
            } else {
                // Fallback: API'den çek
                const weatherFromAPI = await this.fetchWeatherFromAPI();
                if (weatherFromAPI) {
                    this.applyWeatherEffect(weatherFromAPI);
                }
            }
        } catch (error) {
            console.warn('Hava durumu alınamadı:', error);
        }
    }

    /**
     * DOM'dan hava durumunu çıkar (navbar'dan)
     */
    extractWeatherFromDOM() {
        try {
            // Navbar'daki hava durumu bilgisini bul
            const weatherElements = document.querySelectorAll('[class*="weather"]');

            for (let element of weatherElements) {
                const text = element.textContent || '';

                // Emoji'ye göre hava durumunu belirle
                if (text.includes('🌧️') || text.includes('🌦️')) {
                    return 'rain';
                } else if (text.includes('❄️') || text.includes('🌨️')) {
                    return 'snow';
                } else if (text.includes('⛈️')) {
                    return 'thunderstorm';
                } else if (text.includes('☁️') || text.includes('🌫️')) {
                    return 'cloudy';
                } else if (text.includes('☀️') || text.includes('🌤️') || text.includes('🌞')) {
                    return 'clear';
                }
            }

            // Hava durumu açıklamasını navbar'dan bul
            const navbarText = document.querySelector('.navbar')?.textContent || '';

            if (navbarText.toLowerCase().includes('rain') || navbarText.toLowerCase().includes('yağmur')) {
                return 'rain';
            } else if (navbarText.toLowerCase().includes('snow') || navbarText.toLowerCase().includes('kar')) {
                return 'snow';
            } else if (navbarText.toLowerCase().includes('thunder') || navbarText.toLowerCase().includes('şimşek')) {
                return 'thunderstorm';
            } else if (navbarText.toLowerCase().includes('cloud') || navbarText.toLowerCase().includes('bulut')) {
                return 'cloudy';
            } else if (navbarText.toLowerCase().includes('clear') || navbarText.toLowerCase().includes('berrak')) {
                return 'clear';
            }

            return null;
        } catch (error) {
            console.warn('DOM\'dan hava durumu çıkarılamadı:', error);
            return null;
        }
    }

    /**
     * API'den hava durumunu çek (fallback)
     */
    async fetchWeatherFromAPI() {
        try {
            // Eğer bir endpoint varsa buradan çekilebilir
            // Şu an için DOM parsing kullanıyoruz
            return null;
        } catch (error) {
            console.warn('API\'den hava durumu alınamadı:', error);
            return null;
        }
    }

    /**
     * Hava durumuna göre efekt uygula
     */
    applyWeatherEffect(weather) {
        if (this.currentWeather === weather) {
            return; // Aynı hava durumu, değişiklik yok
        }

        this.currentWeather = weather;

        // Önceki efektleri temizle
        this.clearWeatherEffects();

        // Yeni efekti uygula
        switch (weather) {
            case 'rain':
                this.applyRainEffect();
                break;
            case 'snow':
                this.applySnowEffect();
                break;
            case 'clear':
                this.applyClearEffect();
                break;
            case 'cloudy':
                this.applyCloudyEffect();
                break;
            case 'thunderstorm':
                this.applyThunderstormEffect();
                break;
            default:
                this.applyClearEffect();
        }

        console.log('Hava durumu efekti uygulandı:', weather);
    }

    /**
     * Önceki hava efektlerini temizle
     */
    clearWeatherEffects() {
        this.weatherContainer.classList.remove(
            'weather-rain',
            'weather-snow',
            'weather-clear',
            'weather-cloudy',
            'weather-thunderstorm'
        );

        // Partikülleri temizle
        this.particles.forEach(particle => particle.remove?.());
        this.particles = [];

        // Sun glow'u temizle
        const sunGlow = document.querySelector('.sun-glow');
        sunGlow?.remove();

        // Sun core'u temizle
        const sunCore = document.querySelector('.sun-core');
        sunCore?.remove();

        // Yıldızları temizle
        document.querySelectorAll('.star').forEach(star => star.remove());

        // Bulutları temizle
        document.querySelectorAll('.cloud').forEach(cloud => cloud.remove());

        // Şimşekleri temizle
        document.querySelectorAll('.lightning').forEach(lightning => lightning.remove());
    }

    /**
     * Yağmur efekti uygula
     */
    applyRainEffect() {
        this.weatherContainer.classList.add('weather-rain');

        // Yağmur partikülleri oluştur
        for (let i = 0; i < this.particleCount; i++) {
            const drop = document.createElement('div');
            drop.className = 'rain-particle';
            drop.style.setProperty('--left-pos', Math.random() * 100 + '%');
            drop.style.setProperty('--top-pos', Math.random() * 100 + '%');
            drop.style.animationDuration = (Math.random() * 0.5 + 0.5) + 's';
            drop.style.animationDelay = Math.random() * 2 + 's';

            this.weatherContainer.appendChild(drop);
            this.particles.push(drop);
        }

        // Rain drops background ekleme
        const rainBg = document.createElement('div');
        rainBg.className = 'rain-drops';
        this.weatherContainer.appendChild(rainBg);
        this.particles.push(rainBg);
    }

    /**
     * Kar efekti uygula
     */
    applySnowEffect() {
        this.weatherContainer.classList.add('weather-snow');

        // Kar taneleri oluştur
        const snowflakes = ['❄️', '✨', '🌨️', '⛄'];
        for (let i = 0; i < this.particleCount; i++) {
            const snowflake = document.createElement('div');
            snowflake.className = 'snowflake';
            snowflake.textContent = snowflakes[Math.floor(Math.random() * snowflakes.length)];
            snowflake.style.left = Math.random() * 100 + '%';
            snowflake.style.animationDuration = (Math.random() * 10 + 10) + 's';
            snowflake.style.animationDelay = Math.random() * 5 + 's';
            snowflake.style.fontSize = (Math.random() * 1.5 + 1) + 'em';

            this.weatherContainer.appendChild(snowflake);
            this.particles.push(snowflake);
        }
    }

    /**
     * Berrak hava efekti uygula (siber güneş)
     */
    applyClearEffect() {
        this.weatherContainer.classList.add('weather-clear');

        // Sun glow ekleme
        const sunGlow = document.createElement('div');
        sunGlow.className = 'sun-glow';
        this.weatherContainer.appendChild(sunGlow);
        this.particles.push(sunGlow);

        // Sun core ekleme
        const sunCore = document.createElement('div');
        sunCore.className = 'sun-core';
        this.weatherContainer.appendChild(sunCore);
        this.particles.push(sunCore);

        // Yıldızlar ekleme
        for (let i = 0; i < 20; i++) {
            const star = document.createElement('div');
            star.className = 'star';
            star.style.left = Math.random() * 100 + '%';
            star.style.top = Math.random() * 40 + '%';
            star.style.animationDuration = (Math.random() * 3 + 2) + 's';
            star.style.animationDelay = Math.random() * 5 + 's';

            this.weatherContainer.appendChild(star);
            this.particles.push(star);
        }
    }

    /**
     * Bulutlu hava efekti uygula
     */
    applyCloudyEffect() {
        this.weatherContainer.classList.add('weather-cloudy');

        // Bulutlar ekleme
        for (let i = 0; i < 5; i++) {
            const cloud = document.createElement('div');
            cloud.className = 'cloud';
            cloud.style.top = Math.random() * 30 + '%';
            cloud.style.animationDuration = (Math.random() * 10 + 20) + 's';
            cloud.style.animationDelay = i * 5 + 's';

            this.weatherContainer.appendChild(cloud);
            this.particles.push(cloud);
        }
    }

    /**
     * Fırtına efekti uygula (yağmur + şimşek)
     */
    applyThunderstormEffect() {
        this.weatherContainer.classList.add('weather-thunderstorm');

        // Önce yağmur efektini uygula
        this.applyRainEffect();

        // Şimşek efekti başlat (periyodik olarak)
        this.startLightningEffect();
    }

    /**
     * Şimşek efekti başlat
     */
    startLightningEffect() {
        const createLightning = () => {
            const lightning = document.createElement('div');
            lightning.className = 'lightning';

            this.weatherContainer.appendChild(lightning);
            this.particles.push(lightning);

            // Animasyon bitince sil
            setTimeout(() => {
                lightning.remove();
                this.particles = this.particles.filter(p => p !== lightning);
            }, 200);

            // Sonraki şimşeği 3-6 saniye sonra oluştur
            setTimeout(createLightning, Math.random() * 3000 + 3000);
        };

        createLightning();
    }

    /**
     * Atmosferi durdur (cleanup)
     */
    destroy() {
        clearInterval(this.updateInterval);
        this.clearWeatherEffects();
    }
}

// Sayfa yüklendiğinde atmosfer sistemini başlat
document.addEventListener('DOMContentLoaded', () => {
    window.atmosphereManager = new AtmosphereManager();
});

// Eğer sayfa zaten yüklüyse hemen başlat
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        window.atmosphereManager = new AtmosphereManager();
    });
} else {
    window.atmosphereManager = new AtmosphereManager();
}
