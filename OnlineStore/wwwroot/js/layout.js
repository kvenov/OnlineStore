const megaMenu = document.getElementById('megaMenu');
let timeout;

function showMegaMenu(category) {
    clearTimeout(timeout);
    megaMenu.classList.add('show');
}

function hideMegaMenu() {
    timeout = setTimeout(() => {
        megaMenu.classList.remove('show');
    }, 200); // slight delay for smoother UX
}

// Optional: Keep menu visible when hovering it
megaMenu.addEventListener('mouseover', () => clearTimeout(timeout));
megaMenu.addEventListener('mouseout', hideMegaMenu);

document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.querySelector('.search-input');
    const searchExpanded = document.querySelector('.search-expanded');
    const cancelBtn = document.querySelector('.search-cancel-btn');

    searchInput.addEventListener('focus', () => {
        searchExpanded.classList.add('active');
    });

    cancelBtn?.addEventListener('click', () => {
        searchExpanded.classList.remove('active');
        searchInput.blur();
    });

    document.addEventListener('click', (e) => {
        if (!searchExpanded.contains(e.target) && !searchInput.contains(e.target)) {
            searchExpanded.classList.remove('active');
        }
    });
});

document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll('.account-menu-toggle').forEach(toggle => {
        toggle.addEventListener('click', function (e) {
            e.preventDefault();
            const container = this.closest('.account-menu-container');
            container.classList.toggle('show');

            // Optional: Close others
            document.querySelectorAll('.account-menu-container').forEach(other => {
                if (other !== container) other.classList.remove('show');
            });
        });
    });

    // Close on outside click
    document.addEventListener('click', function (e) {
            if (!e.target.closest('.account-menu-container')) {
                document.querySelectorAll('.account-menu-container').forEach(container => {
                container.classList.remove('show');
                });
            }
    });
});