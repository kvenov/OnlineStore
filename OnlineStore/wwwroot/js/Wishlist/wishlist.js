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

        // If unauthorized, silently return
        if (response.status === 401) return;

        if (!response.ok) {
            let errorMessage = "Something went wrong.";

            const contentType = response.headers.get("content-type");
            if (contentType && contentType.includes("application/json")) {
                try {
                    const errorResult = await response.json();
                    errorMessage = errorResult.message || errorMessage;
                } catch (_) {
                   // Ignore parsing error
                }
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

window.addToWishlist = async function (productId) {
    try {
        const response = await fetch(`/api/authorizationapi/auth`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'include'
        });

        if (response.ok) {
            const data = await response.json();

            if (data.isAuthenticated) {
                try {
                    const wishlistResponse = await fetch(`/api/wishlistapi/add/${productId}`, {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    });

                    const wishlistData = await wishlistResponse.json();

                    if (wishlistResponse.ok) {
                        await setWishlistCount();
                        showAddedToFavorites(productId);
                    } else {
                        Swal.fire("Oops", wishlistData.message, "error");
                    }
                } catch (error) {
                    console.error("Error adding to wishlist", error);
                    Swal.fire("Error", "Something went wrong while adding to wishlist.", "error");
                }
            } else {
                showSignInModal();
            }
        } else {
            showSignInModal();
        }
    } catch (error) {
        console.error("Error while getting user credentials", error);
        Swal.fire("Error", "Something went wrong while checking authentication.", "error");
    }
};

function showSignInModal() {
    Swal.fire({
        title: 'Sign in Required',
        text: 'To use the wishlist, you need to be signed in.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#0d6efd',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Sign In',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = '/Account/Login';
        }
    });
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

document.addEventListener('DOMContentLoaded', async () => {
    try {
        const response = await fetch(`/api/authorizationapi/auth`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'include'
        });

        if (response.ok) {
            const data = await response.json();

            if (data.isAuthenticated) {
                removeFromWishlist();
                setWishlistCount();
            }
        } else {
            console.warn("Unathorized access");
        }
    } catch (error) {
        console.error("Error while getting the user credentials", error);
        alert("Something went wrong.");
    }
})
