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

function toggleSection(section) {
    const content = section.querySelector('.dropdown-content');
    const arrow = section.querySelector('.arrow');

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
    const isAuthenticated = reviewContainer.dataset.auth === "true";

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
                        <a href="/Identity/Account/Login" class="btn btn-outline-primary mt-2 me-2">Log In</a>
                        <a href="/Identity/Account/Register" class="btn btn-primary mt-2">Register</a>
                    </div>
                `;

                reviewContainer.appendChild(warningBox);
            }
        }
    })
}

document.addEventListener('DOMContentLoaded', () => {
    handleProductRatingWithStars();
    handleUnauthenticatedUser();
})