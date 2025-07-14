const megaMenu = document.getElementById('megaMenu');
let timeout;

function showMegaMenu(gender) {
    localStorage.setItem("selectedGender", gender);
    clearTimeout(timeout);
    megaMenu.classList.add('show');
}

function hideMegaMenu() {
    timeout = setTimeout(() => {
        megaMenu.classList.remove('show');
    }, 200); // slight delay for smoother UX
}

function navigateToCategory(category, subcategory) {
    const gender = localStorage.getItem("selectedGender") || "unisex";
    window.location.href = `/products/${gender}/${category}/${subcategory}`;
}

// Optional: Keep menu visible when hovering it
megaMenu.addEventListener('mouseover', () => clearTimeout(timeout));
megaMenu.addEventListener('mouseout', hideMegaMenu);

//Searchbar
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

//User account
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