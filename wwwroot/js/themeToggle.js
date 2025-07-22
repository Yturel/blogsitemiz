function toggleTheme() {
    const body = document.body;
    const navbar = document.querySelector('.navbar');
    const icon = document.getElementById('theme-icon');

    body.classList.toggle('dark-mode');
    body.classList.toggle('light-mode');
    navbar.classList.toggle('navbar-dark');
    navbar.classList.toggle('navbar-light');

    const isDark = body.classList.contains('dark-mode');
    localStorage.setItem('theme', isDark ? 'dark' : 'light');

    if (icon) {
        icon.src = isDark ? '/resim/kedi.png' : '/resim/light.png';
    }
}

function applySavedTheme() {
    const theme = localStorage.getItem('theme') || 'light';
    const body = document.body;
    const navbar = document.querySelector('.navbar');
    const icon = document.getElementById('theme-icon');

    const isDark = theme === 'dark';

    body.classList.add(isDark ? 'dark-mode' : 'light-mode');
    navbar.classList.add(isDark ? 'navbar-dark' : 'navbar-light');

    if (icon) {
        icon.src = isDark ? '/resim/kedi.png' : '/resim/light.png';
    }
}

document.addEventListener('DOMContentLoaded', applySavedTheme);
