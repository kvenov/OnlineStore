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

                addToCart(productId, selectedSize);
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

window.addToCart = async function(productId, productSize) {
    try {
        const response = await fetch(`/api/shoppingcartapi/add/${productId}/${productSize}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const data = await response.json();

        if (response.ok) {
            setCartItemsCount();
            updateCartFlyout();

            showCartToast(data.product);
        } else {
            Swal.fire("Oops", data.message, "error");
        }
    } catch (error) {
        console.error("Error while adding to shoppingCart", error);
        Swal.fire("Error", "Something went wrong while adding to cart.", "error");
    }
}

function showCartToast(product) {
    const toastHtml = `
        <div style="display: flex; align-items: center; gap: 12px;">
            <img src="${product.imageUrl}" alt="${product.name}" style="width: 60px; height: 60px; object-fit: cover; border-radius: 8px;">
            <div style="text-align: left;">
                <div style="font-weight: 600; font-size: 14px;">${product.name}</div>
                <div style="font-size: 13px; color: #555;">${product.price}</div>
                <div style="color: green; font-size: 13px;">Added to Cart</div>
            </div>
        </div>
    `;

    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end', // Top right corner
        showConfirmButton: false,
        timer: 4000,
        timerProgressBar: true,
        background: 'rgba(255, 255, 255, 0.95)',
        customClass: {
            popup: 'shadow-lg rounded-4 border'
        },
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer);
            toast.addEventListener('mouseleave', Swal.resumeTimer);
        }
    });

    Toast.fire({
        html: toastHtml,
        icon: 'success'
    });
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
                const button = document.querySelector('.checkout-button');
                button.disabled = true;

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
        console.error("Error while setting the shoppingCart count", error);
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


(function attachBFCacheAndFocusReconcile() {
    let refreshLock = false;

    function getDomLines() {
        const lines = [];
        document.querySelectorAll('[id^="input-quantity-"]').forEach(qtyInput => {
            const m = qtyInput.id.match(/^input-quantity-(.+)$/);
            if (!m) return;
            const rawKey = m[1];
            const hiddenId = document.getElementById(`input-itemId-${rawKey}`);
            const itemId = hiddenId?.value || rawKey;
            const quantity = qtyInput.value;
            if (itemId && quantity != null) {
                lines.push({ itemId, quantity });
            }
        });
        return lines;
    }

    async function silentUpdateLine(itemId, quantity) {
        try {
            const resp = await fetch(`/api/shoppingcartapi/update`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ quantity, itemId })
            });

            const data = await resp.json();

            if (resp.ok) {
                document.querySelector(`.item-totalprice-${itemId}`).textContent = `$${data.itemTotalPrice.toFixed(2)}`;
                document.querySelector('.summary-subtotal').textContent = `$${data.subTotal.toFixed(2)}`;
                document.querySelector('.summary-shipping').textContent = `$${data.shipping.toFixed(2)}`;
                document.querySelector('.summary-total').textContent = `$${data.total.toFixed(2)}`;

                updateCartFlyout();

                return true;
            } else {
                // Item no longer exists on server – remove stale DOM node if present
                document.getElementById(`cart-item-${itemId}`)?.remove();
                return false;
            }
        } catch {
            // Network or other error; don’t alert during reconcile
            return false;
        }
    }

    async function reconcileCartUi() {
        if (refreshLock) return;
        refreshLock = true;

        try {
            // Always refresh the header badge and flyout
            if (typeof setCartItemsCount === 'function') setCartItemsCount();
            if (typeof updateCartFlyout === 'function') updateCartFlyout();

            // If the page has cart lines, reconcile each line with server
            const lines = getDomLines();
            if (lines.length === 0) return;

            let removedAny = false;

            await Promise.all(lines.map(async ({ itemId, quantity }) => {
                const ok = await silentUpdateLine(itemId, quantity);
                if (!ok) removedAny = true;
            }));

            if (removedAny) {
                if (typeof setCartItemsCount === 'function') setCartItemsCount();
                if (typeof updateCartFlyout === 'function') updateCartFlyout();
            }
        } finally {
            setTimeout(() => { refreshLock = false; }, 800);
        }
    }

    async function refreshCheckoutButtonState() {
        try {
            const response = await fetch(`/api/shoppingcartapi/count`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) return;

            const data = await response.json();

            if (response.ok) {
                if (data.count <= 0) {
                    const button = document.querySelector('.checkout-button');
                    if (button) {
                        button.disabled = true;
                        document.querySelector('.cart-container').innerHTML = `
                        <div class="p-6 bg-white rounded-xl shadow-md border border-dashed border-gray-300 text-center text-gray-600">
                            <p class="text-xl font-medium mb-4">🛒 Your cart is empty</p>
                            <a href="/products" class="text-blue-600 underline hover:text-blue-800 transition">
                                Browse Products
                            </a>
                        </div>
                        `;

                        document.querySelector('.summary-subtotal').textContent = `$0.00`;
                        document.querySelector('.summary-shipping').textContent = `$0.00`;
                        document.querySelector('.summary-total').textContent = `$0.00`;
                    }
                }
            }
        } catch (error) {
            alert("Something went wrong. wdvvevfeeffb");
        }
    }

    window.addEventListener('pageshow', () => {
        reconcileCartUi();
        refreshCheckoutButtonState();
    });

    document.addEventListener('visibilitychange', () => {
        if (document.visibilityState === 'visible') {
            reconcileCartUi();
            refreshCheckoutButtonState();
        }
    });
})();