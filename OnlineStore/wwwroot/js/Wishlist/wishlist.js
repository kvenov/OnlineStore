function toggleNote(id) {
    const editor = document.getElementById(`note-editor-${id}`);
    editor.classList.toggle("d-none");
}

async function setWishlistCount() {
    try {
        const response = await fetch(`/api/wishlistapi/count`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            // If unauthorized, just silently skip
            if (response.status === 401) return;

            // Try to read message if any, otherwise generic message
            let errorMessage = "Something went wrong.";
            try {
                const errorResult = await response.json();
                errorMessage = errorResult.message || errorMessage;
            } catch (_) {
            }

            console.warn(errorMessage);
            return;
        }

        const result = await response.json();

        if (response.ok) {
            const count = result.data;
            const element = document.getElementById('wishlist-count');
            if (element) {
                element.textContent = count;
            }

        } else {
            alert(result.message);
        }
    } catch (error) {
        console.error("Occured error while getting the wishlist count", error);
        alert("Something went wrong.");
    }
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
            await setWishlistCount();
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
        removeButton.removeEventListener('click', handleRemoveClick);
        removeButton.addEventListener('click', handleRemoveClick);
    });
}

async function handleRemoveClick(event) {
    const removeButton = event.currentTarget;
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
                setTimeout(() => {
                    showIsRemovedFromWishlist(itemId);
                    card.remove();
                },500);
            }

            await setWishlistCount();

            window.location.href = "/Wishlist/Index";
        } else {
            alert(data.message);
        }
    } catch (error) {
        console.error("Error while removing from wishlist", error);
        alert("Something went wrong.");
    }
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
        element.style.display = 'block';

        setTimeout(() => {
            element.style.display = 'none';
        }, 2500);
    }
}

document.addEventListener('DOMContentLoaded', (event) => {
    removeFromWishlist();
    setWishlistCount();
})
