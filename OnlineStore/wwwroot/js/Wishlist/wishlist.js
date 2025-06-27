function toggleNote(id) {
    const editor = document.getElementById(`note-editor-${id}`);
    editor.classList.toggle("d-none");
}

async function addToWishlist(productId) {
    try {
        const response = await fetch(`/api/wishlistapi/add/${productId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const data = await response.json();

        if (response.ok) {
            showAddedToFavorites(productId);
        } else {
            alert(data.message);
        }
    } catch (error) {
        console.error("Error adding to wishlist", error);
        alert("Something went wrong.");
    }
}

function removeFromWishlist() {
    document.querySelectorAll('.remove-btn').forEach((removeButton) => {
        removeButton.addEventListener('click', async () => {
            const itemId = removeButton.getAttribute('data-id');

            try {
                const response = await fetch(`/api/wishlistapi/remove/${itemId}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });

                const data = await response.json();

                if (response.ok) {
                    const card = document.getElementById(`wishlist-item-${itemId}`);
                    if (card) {
                        card.classList.add('fade-out');
                        setTimeout(() => card.remove(), 500);
                    }

                    showIsRemovedFromWishlist(itemId);
                } else {
                    alert(data.message);
                }
            } catch (error) {
                console.error("Error while removing from wishlist", error);
                alert("Something went wrong.");
            }
        })
    })
}

function showAddedToFavorites(productId) {
    const element = document.querySelector(`.js-added-to-cart-${productId}`);
    if (element) {
        element.classList.add('show');

        setTimeout(() => {
            element.classList.remove('show');
        }, 2000);
    }
}

function showIsRemovedFromWishlist(itemId) {
    const element = document.querySelector(`.js-removed-from-wishlist-${itemId}`);
    if (element) {
        element.classList.add('show');

        setTimeout(() => {
            element.classList.remove('show');
        }, 2000);
    }
}

document.addEventListener('DOMContentLoaded', (event) => {
    removeFromWishlist();
})
