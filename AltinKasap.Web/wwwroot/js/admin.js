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

    function initSortable() {
        var el = document.getElementById('sortable-list');
        if (!el || typeof Sortable === 'undefined') return;
        var sortType = el.dataset.sortType || 'category';

        Sortable.create(el, {
            handle: '.drag-handle',
            animation: 150,
            ghostClass: 'sortable-ghost',
            onEnd: function () {
                var ids = Array.prototype.map.call(
                    el.querySelectorAll('tr[data-id]'),
                    function (r) { return parseInt(r.dataset.id, 10); }
                );
                var token = document.querySelector('input[name="__RequestVerificationToken"]');
                fetch('/api/sort', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token ? token.value : ''
                    },
                    body: JSON.stringify({ type: sortType, orderedIds: ids })
                }).then(function (r) { return r.ok ? r.json() : Promise.reject(r); })
                  .then(function (d) {
                      if (d && d.success) {
                          var t = document.createElement('div');
                          t.className = 'alert alert-success position-fixed top-0 end-0 m-3 shadow';
                          t.style.zIndex = 1080;
                          t.innerHTML = '<i class="fa-solid fa-check"></i> Sıralama güncellendi';
                          document.body.appendChild(t);
                          setTimeout(function () { t.remove(); }, 2000);
                          var rows = el.querySelectorAll('tr[data-id] .ak-sort-order');
                          rows.forEach(function (cell, i) { cell.textContent = (i + 1); });
                      }
                  })
                  .catch(function () {
                      alert('Sıralama güncellenirken hata oluştu.');
                  });
            }
        });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initSortable);
    } else {
        initSortable();
    }
})();
