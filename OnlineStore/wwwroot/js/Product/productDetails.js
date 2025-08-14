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

function handleNotValidUsers() {
    const reviewContainer = document.getElementById('review-container');

    if (reviewContainer) {
        const reviewFormButton = document.getElementById('review-submit-button');
        reviewFormButton.addEventListener('click', (event) => {
            console.log(reviewContainer.dataset.isPurchased);
            const isPurchased = reviewContainer.dataset.isPurchased === "True";

            if (!isPurchased) {
                event.preventDefault();
                reviewFormButton.disabled = true;

                Swal.fire({
                    title: 'You must purchase this product first 🛒',
                    text: 'Only customers who have bought this product can leave a review.',
                    icon: 'warning',
                    confirmButtonText: 'Ok',
                    reverseButtons: true,
                    customClass: {
                        confirmButton: 'swal2-confirm btn btn-primary me-2',
                    },
                    buttonsStyling: false
                });
            }
        });
    }
}

document.addEventListener('DOMContentLoaded', () => {
    handleProductRatingWithStars();
    handleNotValidUsers();
    updateProductRating();
})