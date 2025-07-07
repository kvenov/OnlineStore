document.addEventListener('DOMContentLoaded', () => {
    let selectedSize = null;

    document.querySelectorAll('.size-option').forEach(el => {
        el.addEventListener('click', () => {
            document.querySelectorAll('.size-option').forEach(btn => btn.classList.remove('active'));
            el.classList.add('active');
            selectedSize = el.getAttribute('data-size');
        });
    });

    const addToCartButton = document.getElementById('addToCartBtn');

    addToCartButton.addEventListener('click', () => {
        if (!selectedSize) {
            alert('Please select a size before adding to cart.');
            return;
        }
        else {
            const productId = addToCartButton.dataset.productId;

            addToCart(productId);
            alert('Product added to cart! Size: ' + selectedSize);
        }
    })
})

async function addToCart(productId) {
    try {
        const response = await fetch(`/api/shoppingcartapi/add/${productId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const data = await response.json();

        if (response.ok) {
            await setWishlistCount();
            showAddedToFavorites(productId);
        } else {
            alert(data.message);
        }
    } catch (error) {
        console.error("Error while adding to shoppingCart", error);
        alert("Something went wrong.");
    }
}
