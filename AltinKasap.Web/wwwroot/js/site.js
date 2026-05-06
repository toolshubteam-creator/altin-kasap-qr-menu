(function () {
    'use strict';

    var toggleBtn = document.getElementById('akThemeToggle');
    if (toggleBtn) {
        var icon = toggleBtn.querySelector('i');
        var setIcon = function (theme) {
            if (icon) {
                icon.className = theme === 'dark'
                    ? 'fa-solid fa-sun'
                    : 'fa-solid fa-moon';
            }
        };
        setIcon(document.documentElement.getAttribute('data-bs-theme'));

        toggleBtn.addEventListener('click', function () {
            var current = document.documentElement.getAttribute('data-bs-theme');
            var next = current === 'dark' ? 'light' : 'dark';
            document.documentElement.setAttribute('data-bs-theme', next);
            localStorage.setItem('ak-theme', next);
            setIcon(next);
        });
    }

    var pills = document.querySelectorAll('.ak-cat-pill');
    pills.forEach(function (pill) {
        pill.addEventListener('click', function (e) {
            e.preventDefault();
            var slug = pill.getAttribute('data-cat');
            var target = document.getElementById('cat-' + slug);
            if (target) {
                var offset = 70;
                var top = target.getBoundingClientRect().top + window.pageYOffset - offset;
                window.scrollTo({ top: top, behavior: 'smooth' });
            }
        });
    });

    if ('IntersectionObserver' in window) {
        var sections = document.querySelectorAll('.ak-category');
        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    var id = entry.target.id.replace('cat-', '');
                    pills.forEach(function (p) {
                        p.classList.toggle('is-active', p.getAttribute('data-cat') === id);
                    });
                    var activePill = document.querySelector('.ak-cat-pill.is-active');
                    if (activePill) {
                        activePill.scrollIntoView({ behavior: 'smooth', block: 'nearest', inline: 'center' });
                    }
                }
            });
        }, { rootMargin: '-30% 0px -60% 0px', threshold: 0 });

        sections.forEach(function (s) { observer.observe(s); });
    }
})();
