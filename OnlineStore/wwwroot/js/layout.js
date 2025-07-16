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
    }, 200);
}

function navigateToCategory(category, subcategory) {
    const gender = localStorage.getItem("selectedGender") || "unisex";
    const safeCategory = encodeURIComponent(category.replace(/\//g, "@"));
    const safeSubcategory = encodeURIComponent(subcategory.replace(/\//g, "@"));

    window.location.href = `/products/${gender}/${safeCategory}/${safeSubcategory}`;
}

// Optional: Keep menu visible when hovering it
megaMenu.addEventListener('mouseover', () => clearTimeout(timeout));
megaMenu.addEventListener('mouseout', hideMegaMenu);

//Searchbar
const debounce = (func, delay) => {
    let timer;
    return function (...args) {
        clearTimeout(timer);
        timer = setTimeout(() => func.apply(this, args), delay);
    };
};

$(document).ready(function () {
    const $input = $('#searchQueryBox');
    const $results = $('#searchResults');
    const $expanded = $('#searchExpanded');

    // Show panel when main input is focused
    $('#searchInput').on('focus', function () {
        $expanded.show();
        $('#searchQueryBox').focus();
    });

    // Debounced AJAX search
    $input.on('input', debounce(function () {
        const query = $(this).val().trim();

        if (!query) {
            $results.empty();
            return;
        }

        $.ajax({
            url: '/api/productapi/search',
            type: 'GET',
            data: { query: query },
            success: function (data) {
                $results.empty();
                if (data.length === 0) {
                    $results.append('<p class="text-muted">No results found.</p>');
                    return;
                }

                data.forEach(product => {
                    const card = `
                            <div class="search-card">
                                <img src="${product.imageUrl}" data-product-id="${product.id}" alt="${product.name}" />
                                <div>
                                    <h6>${product.name}</h6>
                                    <small>${product.price}</small>
                                </div>
                            </div>`;
                    $results.append(card);
                });
            },
            error: function () {
                $results.html('<p class="text-danger">Search failed.</p>');
            }
        });
    }, 300));

    // Handle popular searches
    $('.popular-searches span').on('click', function () {
        const term = $(this).text();
        $('#searchQueryBox').val(term).trigger('input');
    });

    // Submit on Enter key
    $input.on('keypress', function (e) {
        if (e.key === 'Enter') {
            performFullSearch();
        }
    });

    $(document).on('click', '.search-card img', function () {
        const productId = $(this).data('product-id');
        window.location.href = `/product/details/${productId}`;
    });


    // Optional search button
    $('.search-btn').on('click', performFullSearch);

    // Close the panel if clicking outside
    $(document).on('click', function (e) {
        const $target = $(e.target);
        const $expanded = $('#searchExpanded');
        const $input = $('#searchInput');

        if (!$target.closest('#searchExpanded').length && !$target.closest('#searchInput').length) {
            $expanded.hide();
            $('#searchResults').empty();
        }
    });

});

function performFullSearch() {
    const query = $('#searchQueryBox').val().trim();
    if (!query) {
        alert("Please enter a search term.");
        return;
    }

    window.location.href = `/products/search?query=${encodeURIComponent(query)}`;
}

function hideSearch() {
    $('#searchExpanded').hide();
    $('#searchResults').empty();
}

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