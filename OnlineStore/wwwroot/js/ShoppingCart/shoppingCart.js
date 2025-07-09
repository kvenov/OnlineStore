document.addEventListener('DOMContentLoaded', () => {
    let selectedSize = null;

    document.querySelectorAll('.size-option').forEach(el => {
        el.addEventListener('click', () => {
            document.querySelectorAll('.size-option').forEach(btn => btn.classList.remove('active'));
            el.classList.add('active');
            selectedSize = el.getAttribute('data-size');
        });
    });

    //Adding to cart
    const addToCartButton = document.getElementById('addToCartBtn');
    if (addToCartButton) {
        addToCartButton.addEventListener('click', () => {
            if (!selectedSize) {
                alert('Please select a size before adding to cart.');
                return;
            }
            else {
                const productId = addToCartButton.dataset.productId;

                addToCart(productId);
            }
        })
    }

    //Updating Cart Item
    document.querySelectorAll('.update-cartItem-button').forEach(button => {
        const itemId = button.dataset.itemId;

        button.addEventListener('click', () => {
            const quantityInput = document.getElementById(`input-quantity-${itemId}`);
            const itemIdInput = document.getElementById(`input-itemId-${itemId}`);

            if (quantityInput && itemIdInput) {
                const quatity = quantityInput.value;
                const itemId = itemIdInput.value;

                updateCartItem(quatity, itemId)
            }
        })
    })

    //Removing cart Item
    document.querySelectorAll('.remove-cartItem-button').forEach(button => {
        const itemId = button.dataset.itemId;

        button.addEventListener('click', () => {
            const itemIdInput = document.getElementById(`remove-input-itemId-${itemId}`);

            if (itemIdInput) {
                const itemId = itemIdInput.value;

                removeCartItem(itemId)
            }
        })
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
            alert(data.result);

            setCartItemsCount();
            updateCartFlyout();
        } else {
            alert(data.message);
        }
    } catch (error) {
        console.error("Error while adding to shoppingCart", error);
        alert("Something went wrong.");
    }
}

async function updateCartItem(quantity, itemId) {
    try {
        const response = await fetch(`/api/shoppingcartapi/update`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                quantity,
                itemId
            })
        });

        const data = await response.json();

        if (response.ok) {

            document.querySelector(`.item-totalprice-${itemId}`).textContent = `$${data.itemTotalPrice.toFixed(2)}`;
            document.querySelector('.summary-subtotal').textContent = `$${data.subTotal.toFixed(2)}`;
            document.querySelector('.summary-shipping').textContent = `$${data.shipping.toFixed(2)}`;
            document.querySelector('.summary-total').textContent = `$${data.total.toFixed(2)}`;

            updateCartFlyout();
        } else {
            alert(data.message);
        }
    } catch (error) {
        console.error("Error while updating the cart item", error);
        alert("Something went wrong.");
    }
}

async function removeCartItem(itemId) {
    try {
        const response = await fetch(`/api/shoppingcartapi/remove/${itemId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const data = await response.json();

        if (response.ok) {
            if (data.total <= 0) {
                document.querySelector('.checkout-button').disabled = true;
                document.querySelector('.cart-container').innerHTML = `
                    <div class="p-6 bg-white rounded-xl shadow-md border border-dashed border-gray-300 text-center text-gray-600">
                        <p class="text-xl font-medium mb-4">🛒 Your cart is empty</p>
                        <a href="/products" class="text-blue-600 underline hover:text-blue-800 transition">
                            Browse Products
                        </a>
                    </div>
                `;
            }
            else {
                const itemToRemove = document.getElementById(`cart-item-${itemId}`)
                itemToRemove.remove();
            }

            setCartItemsCount();
            updateCartFlyout()

            document.querySelector('.summary-subtotal').textContent = `$${data.subTotal.toFixed(2)}`;
            document.querySelector('.summary-shipping').textContent = `$${data.shipping.toFixed(2)}`;
            document.querySelector('.summary-total').textContent = `$${data.total.toFixed(2)}`;
        } else {
            alert(data.message);
        }
    } catch (error) {
        console.error("Error while removing the cart item", error);
        alert("Something went wrong.");
    }
}

async function setCartItemsCount() {
    try {
        const response = await fetch(`/api/shoppingcartapi/count`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const data = await response.json();

        if (response.ok) {
            const cartItemsCountBadge = document.querySelector('.cart-items-count');

            cartItemsCountBadge.textContent = `${data.count.toFixed()}`;
        } else {
            alert(data.message);
        }
    } catch (error) {
        console.error("Error while adding to shoppingCart", error);
        alert("Something went wrong.");
    }
}

async function updateCartFlyout() {
    try {
        const response = await fetch('/cartFlyout');
        if (response.ok) {
            const html = await response.text();
            document.getElementById('cart-flyout-container').innerHTML = html;
        } else {
            console.error('Failed to refresh cart flyout');
        }
    } catch (error) {
        console.error("Error while updating the cartFlyout", error);
        alert("Something went wrong.");
    }
}
