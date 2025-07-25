﻿let selectedSize = null;
let currentProductId = null;

function showAddToCartModal(productId) {
    currentProductId = productId;
    selectedSize = null;

    // Find product info from DOM (or fetch via Ajax if you prefer)
    const card = $(`button[data-product-id="${productId}"]`).closest('.card');
    const name = card.find('.card-title').text().trim();
    const price = card.find('.card-text').text().trim();
    const imageUrl = card.find('img').attr('src');

    // Fill modal with product info
    $('#modalProductImage').attr('src', imageUrl);
    $('#modalProductName').text(name);
    $('#modalProductPrice').text(price);
    $('#confirmAddToCartBtn').prop('disabled', true);

    // Fetch sizes dynamically (mocked here)
    $.ajax({
        url: `/api/productapi/sizes/${productId}`,
        method: 'GET',
        success: function (sizes) {
            $('#sizeOptions').empty();
            sizes.forEach(size => {
                const btn = $(`<button class="btn btn-outline-primary size-btn">${size}</button>`);
                btn.on('click', function () {
                    $('.size-btn').removeClass('active');
                    $(this).addClass('active');
                    selectedSize = size;
                    $('#confirmAddToCartBtn').prop('disabled', false);
                });
                $('#sizeOptions').append(btn);
            });
        },
        error: function () {
            $('#sizeOptions').html('<span class="text-danger">Failed to load sizes.</span>');
        }
    });

    // Show modal
    const modal = new bootstrap.Modal(document.getElementById('addToCartModal'));
    modal.show();
}

$('#confirmAddToCartBtn').on('click', function () {
    if (!selectedSize || !currentProductId) return;

    addToCart(currentProductId, selectedSize);

    bootstrap.Modal.getInstance(document.getElementById('addToCartModal')).hide();
});

// Add event binding to all "Add to Cart" buttons
$(document).ready(function () {
    $('.btn-add-to-cart').on('click', function () {
        const productId = $(this).data('product-id');
        showAddToCartModal(productId);
    });
});