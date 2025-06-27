let selectedSize = null;

document.querySelectorAll('.size-option').forEach(el => {
    el.addEventListener('click', () => {
        document.querySelectorAll('.size-option').forEach(btn => btn.classList.remove('active'));
        el.classList.add('active');
        selectedSize = el.getAttribute('data-size');
    });
});

document.getElementById('addToCartBtn').addEventListener('click', () => {
    if (!selectedSize) {
        alert('Please select a size before adding to cart.');
        return;
    }
    // Submit logic here (form submission or JS fetch)
    alert('Product added to cart! Size: ' + selectedSize);
});

function toggleSection(section) {
    section.classList.toggle('active');
}

function toggleSection(headerElement) {
    const section = headerElement.closest('.dropdown-section');
    const content = section.querySelector('.dropdown-content');
    const arrow = headerElement.querySelector('.arrow');

    const isVisible = content.style.display === "block";

    content.style.display = isVisible ? "none" : "block";
    arrow.textContent = isVisible ? "▶" : "▼";
}


function highlightStars(rating, stars) {
    stars.forEach(star => {
        const value = parseInt(star.dataset.value);
        if (value <= rating) {
            star.classList.add('selected');
        } else {
            star.classList.remove('selected');
        }
    });
}

function toggleEditReview(reviewId) {
    const form = document.getElementById(`edit-review-form-${reviewId}`);
    form.style.display = form.style.display === "none" ? "block" : "none";
}

function updateProductRating() {
    document.querySelectorAll('.star-rating').forEach(starContainer => {
        const stars = starContainer.querySelectorAll('.star-input');
        const input = starContainer.querySelector('input[type="hidden"]');

        stars.forEach(star => {
            star.addEventListener('click', () => {
                const value = parseInt(star.dataset.value);
                input.value = value;

                stars.forEach((s, index) => {
                    s.classList.toggle('fas', index < value);
                    s.classList.toggle('far', index >= value);
                });
            });
        });
    });
}

function handleProductRatingWithStars() {
    const stars = document.querySelectorAll('.star');
    const ratingInput = document.getElementById('rating-input');

    stars.forEach(star => {
        star.addEventListener('mouseover', () => {
            const value = parseInt(star.dataset.value);
            highlightStars(value, stars);
        });

        star.addEventListener('mouseout', () => {
            const value = parseInt(ratingInput.value);
            highlightStars(value, stars);
        });

        star.addEventListener('click', () => {
            const starValue = parseInt(star.dataset.value);
            ratingInput.value = starValue;
            highlightStars(starValue, stars);
        });
    });
}

function handleUnauthenticatedUser() {
    const reviewContainer = document.getElementById('review-container');

    if (reviewContainer) {
        const isAuthenticated = reviewContainer.dataset.auth === "true";
        const productId = reviewContainer.dataset.productId;
        const returnUrl = `/Product/Details/${productId}`;

        const reviewFormButton = document.getElementById('review-submit-button');
        reviewFormButton.addEventListener('click', (event) => {
            if (!isAuthenticated) {
                event.preventDefault();
                reviewFormButton.disabled = true;

                if (!document.querySelector('.alert')) {

                    const warningBox = document.createElement("div");
                    warningBox.className = 'alert alert-warning mt-5 mb-5 text-center fade-in';
                    warningBox.innerHTML = `
                    <div class="alert alert-warning mt-5 mb-5 text-center">
                          <strong>👋 Want to leave a review?</strong><br />
                          <a href="/Identity/Account/Login?returnUrl=${returnUrl}" 
                            class="btn btn-outline-primary mt-2 me-2">Log In</a>
                          <a href="/Identity/Account/Register?returnUrl=${returnUrl}" 
                            class="btn btn-primary mt-2">Register</a>
                    </div>
                `;

                    reviewContainer.appendChild(warningBox);
                }
            }
        })
    }
}

document.addEventListener('DOMContentLoaded', () => {
    handleProductRatingWithStars();
    handleUnauthenticatedUser();
    updateProductRating();
})