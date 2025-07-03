document.addEventListener('DOMContentLoaded', () => {
    const table = $('#promotionsTable').DataTable({
        pageLength: 10,
        lengthMenu: [5, 10, 25, 50],
        columnDefs: [
            { orderable: false, targets: 7 } // disable ordering on Actions col
        ],
        language: {
            searchPlaceholder: "Search promotions..."
        }
    });

    flatpickr('.flatpickr', {
        dateFormat: "Y-m-d",
        minDate: "today"
    });

    // Fetch products list dynamically for the product dropdown
    async function fetchProducts() {
        try {
            const response = await fetch('/api/productapi/get'); // Adjust your endpoint
            const products = await response.json();
            const productSelect = document.getElementById('productId');
            productSelect.innerHTML = `<option value="" disabled selected>Select a product</option>`;
            products.forEach(p => {
                productSelect.insertAdjacentHTML('beforeend', `<option value="${p.id}">${p.name}</option>`);
            });
        } catch (err) {
            console.error('Failed to load products', err);
        }
    }
    fetchProducts();

    // Open modal for adding or editing promotion
    const promotionModal = new bootstrap.Modal(document.getElementById('addPromotionModal'));
    const form = document.getElementById('promotionForm');
    const saveBtn = document.getElementById('savePromotionBtn');

    // Handle Edit button click
    $('#promotionsTable').on('click', '.edit-btn', async function () {
        const promotionId = $(this).data('id');
        // Fetch promotion by id (or use data from row)
        try {
            const response = await fetch(`/api/productpromotionapi/get/${promotionId}`);
            if (!response.ok) throw new Error("Failed to load promotion");
            const promo = await response.json();

            console.log("StartDate:", promo.startDate); // Debug

            form.reset();
            form.querySelector('#promotionId').value = promo.id;
            form.querySelector('#productId').value = promo.productId;
            form.querySelector('#label').value = promo.label;
            form.querySelector('#promotionPrice').value = promo.promotionPrice;
            form.querySelector('#startDate')._flatpickr.setDate(new Date(promo.startDate));
            form.querySelector('#expDate')._flatpickr.setDate(new Date(promo.expDate));
            form.querySelector('#isDeleted').checked = promo.isDeleted;

            document.getElementById('addPromotionLabel').textContent = "Edit Product Promotion";
            saveBtn.innerHTML = `<i class="fas fa-save me-2"></i> Update Promotion`;

            promotionModal.show();
        } catch (error) {
            alert("Could not load promotion details.");
        }
    });

    // Handle Delete button click
    $('#promotionsTable').on('click', '.delete-btn', function () {
        const promoId = $(this).data('id');
        if (confirm('Are you sure you want to delete this promotion?')) {
            fetch(`/api/productpromotions/delete/${promoId}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            }).then(async res => {
                if (res.ok) {
                    alert('Promotion deleted successfully.');
                    location.reload();
                } else {
                    const err = await res.json();
                    alert('Error: ' + err.message);
                }
            }).catch(() => alert('Failed to delete promotion.'));
        }
    });

    // Handle Add Promotion button click to reset form
    document.querySelector('button[data-bs-target="#addPromotionModal"]').addEventListener('click', () => {
        form.reset();
        document.getElementById('promotionId').value = 0;
        document.getElementById('addPromotionLabel').textContent = "Add Product Promotion";
        saveBtn.innerHTML = `<i class="fas fa-plus me-2"></i> Add Promotion`;
    });

    // Form validation & submit
    form.addEventListener('submit', async e => {
        e.preventDefault();
        if (!form.checkValidity()) {
            form.classList.add('was-validated');
            return;
        }

        const isEdit = Number(form.promotionId.value) > 0;

        const formData = {
            ProductId: Number(form.productId.value),
            Label: form.label.value.trim(),
            PromotionPrice: form.promotionPrice.value,
            StartDate: form.startDate.value,
            ExpDate: form.expDate.value,
            IsDeleted: form.isDeleted.checked.toString()
        };

        if (isEdit) {
            formData.Id = Number(form.promotionId.value);
        }

        try {
            const url = isEdit ? `/api/productpromotionapi/edit` : '/api/productpromotionapi/create';
            const method = 'POST';

            const response = await fetch(url, {
                method,
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(formData)
            });

            if (!response.ok) {
                const errorMessage = result.message || (result.errors?.join(', ') ?? 'Unknown error');
                alert('Error: ' + errorMessage);
                return;
            }

            alert(`Promotion ${isEdit ? 'updated' : 'added'} successfully.`);
            promotionModal.hide();
            location.reload();

        } catch (error) {
            alert('Failed to save promotion.');
            console.error(error);
        }
    });
});