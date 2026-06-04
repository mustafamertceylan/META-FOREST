// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// ===== TEMA SİSTEMİ =====
function toggleTheme() {
    const body = document.body;
    const btn = document.getElementById('btnTheme');

    if (body.classList.contains('light-theme')) {
        body.classList.remove('light-theme');
        btn.textContent = '🌙';
        localStorage.setItem('theme', 'dark');
    } else {
        body.classList.add('light-theme');
        btn.textContent = '☀️';
        localStorage.setItem('theme', 'light');
    }
}

// Sayfa yüklenince kaydedilen temayı uygula
(function () {
    const saved = localStorage.getItem('theme');
    if (saved === 'light') {
        document.body.classList.add('light-theme');
        const btn = document.getElementById('btnTheme');
        if (btn) btn.textContent = '☀️';
    }
})();