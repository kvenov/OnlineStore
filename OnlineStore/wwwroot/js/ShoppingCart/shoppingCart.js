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


    document.querySelectorAll('.update-cartItem-button').forEach(button => {
        const itemId = button.dataset.itemId;

        button.addEventListener('click', () => {
            const quantityInput = document.getElementById(`input-quantity-${itemId}`);
            const itemIdInput = document.getElementById(`input-itemId-${itemId}`);

            if (quantityInput && itemIdInput) {
                const quatity = quantityInput.value;
                const itemId = itemIdInput.value;

                UpdateCartItem(quatity, itemId)
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
        } else {
            alert(data.message);
        }
    } catch (error) {
        console.error("Error while adding to shoppingCart", error);
        alert("Something went wrong.");
    }
}

async function UpdateCartItem(quantity, itemId) {
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
            alert(data.result)
            location.reload();
        } else {
            alert(data.message);
        }
    } catch (error) {
        console.error("Error while updating the cart item", error);
        alert("Something went wrong.");
    }
}
