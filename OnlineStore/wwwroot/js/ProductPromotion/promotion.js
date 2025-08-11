document.addEventListener('DOMContentLoaded', () => {
    function getAntiForgeryToken() {
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenElement ? tokenElement.value : '';
    }

    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 2000,
        timerProgressBar: true,
        background: '#fff',
        color: '#333',
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer);
            toast.addEventListener('mouseleave', Swal.resumeTimer);
        }
    });

    function showSuccess(message) {
        Toast.fire({
            icon: 'success',
            title: message
        });
    }

    function showError(message) {
        Toast.fire({
            icon: 'error',
            title: message
        });
    }

    const table = $('#promotionsTable').DataTable({
        pageLength: 10,
        lengthMenu: [5, 10, 25, 50],
        columnDefs: [
            { orderable: false, targets: 7 }
        ],
        language: {
            searchPlaceholder: "Search promotions..."
        }
    });

    flatpickr('.flatpickr', {
        dateFormat: "Y-m-d",
        minDate: "today"
    });

    async function fetchProducts() {
        try {
            const response = await fetch('/api/productapi/get', {
                headers: {
                    'RequestVerificationToken': getAntiForgeryToken()
                }
            });
            const products = await response.json();
            const productSelect = document.getElementById('productId');
            productSelect.innerHTML = `<option value="" disabled selected>Select a product</option>`;
            products.forEach(p => {
                productSelect.insertAdjacentHTML('beforeend', `<option value="${p.id}">${p.name}</option>`);
            });
        } catch (err) {
            console.error('Failed to load products', err);
            showError('Failed to load products list.');
        }
    }
    fetchProducts();

    const promotionModal = new bootstrap.Modal(document.getElementById('addPromotionModal'));
    const form = document.getElementById('promotionForm');
    const saveBtn = document.getElementById('savePromotionBtn');

    $('#promotionsTable').on('click', '.edit-btn', async function () {
        const promotionId = $(this).data('id');
        try {
            const response = await fetch(`/api/productpromotionapi/get/${promotionId}`, {
                headers: {
                    'RequestVerificationToken': getAntiForgeryToken()
                }
            });
            if (!response.ok) throw new Error("Failed to load promotion");
            const promo = await response.json();

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
            showError("Could not load promotion details.");
        }
    });

    $('#promotionsTable').on('click', '.delete-btn', function () {
        const promotionId = $(this).data('id');
        Swal.fire({
            title: 'Are you sure?',
            text: "This will set the promotion as soft deleted.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(`/api/productpromotionapi/delete/${promotionId}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': getAntiForgeryToken()
                    }
                }).then(async res => {
                    if (res.ok) {
                        showSuccess('Promotion deleted successfully.');
                        setTimeout(() => location.reload(), 2100);
                    } else {
                        const err = await res.json();
                        showError(err.message || 'Failed to delete promotion.');
                    }
                }).catch(() => {
                    showError('Failed to delete promotion.');
                });
            }
        });
    });

    document.querySelector('button[data-bs-target="#addPromotionModal"]').addEventListener('click', () => {
        form.reset();
        document.getElementById('promotionId').value = 0;
        document.getElementById('addPromotionLabel').textContent = "Add Product Promotion";
        saveBtn.innerHTML = `<i class="fas fa-plus me-2"></i> Add Promotion`;
    });

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
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getAntiForgeryToken()
                },
                body: JSON.stringify(formData)
            });

            if (!response.ok) {
                const result = await response.json();
                const errorMessage = result.message || (result.errors?.join(', ') ?? 'Unknown error');
                showError(errorMessage);
                return;
            }

            showSuccess(`Promotion ${isEdit ? 'updated' : 'added'} successfully.`);
            promotionModal.hide();
            setTimeout(() => location.reload(), 2100);

        } catch (error) {
            showError('Failed to save promotion.');
            console.error(error);
        }
    });
});