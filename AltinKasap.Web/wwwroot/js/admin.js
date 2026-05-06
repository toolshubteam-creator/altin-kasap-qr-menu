(function () {
    'use strict';

    var toggle = document.getElementById('akSidebarToggle');
    var sidebar = document.querySelector('.ak-sidebar');
    if (toggle && sidebar) {
        toggle.addEventListener('click', function () {
            sidebar.classList.toggle('is-open');
        });
    }

    document.querySelectorAll('form[data-disable-on-submit="true"]').forEach(function (form) {
        form.addEventListener('submit', function () {
            var btn = form.querySelector('button[type="submit"]');
            if (btn && !btn.disabled) {
                btn.dataset.original = btn.innerHTML;
                btn.disabled = true;
                btn.innerHTML = '<i class="fa-solid fa-spinner fa-spin"></i> Lütfen bekleyin...';
            }
        });
    });

    var imgInput = document.getElementById('ImageFile');
    var imgPreview = document.getElementById('imagePreview');
    if (imgInput && imgPreview) {
        imgInput.addEventListener('change', function () {
            var file = this.files && this.files[0];
            if (file) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    imgPreview.src = e.target.result;
                    imgPreview.style.display = 'block';
                };
                reader.readAsDataURL(file);
            }
        });
    }
})();
