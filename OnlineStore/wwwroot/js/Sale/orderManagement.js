function fetchOrders() {
    const orderNumber = document.getElementById('filterOrderNumber').value;
    const customer = document.getElementById('filterCustomer').value;
    const dateFrom = document.getElementById('filterDateFrom').value;
    const dateTo = document.getElementById('filterDateTo').value;
    const status = document.getElementById('filterStatus').value;

    const query = new URLSearchParams({
        orderNumber,
        customer,
        dateFrom,
        dateTo,
        status
    });

    fetch(`/api/saleapi/filter?${query}`)
        .then(res => res.json())
        .then(data => {
            const tbody = document.getElementById('ordersTableBody');
            tbody.innerHTML = '';

            if (!data.length) {
                tbody.innerHTML = '<tr><td colspan="7" class="text-center">No orders found</td></tr>';
                return;
            }

            data.forEach(order => {
                const row = `<tr>
                            <td>${order.orderNumber}</td>
                            <td>${order.customerName} <br/><span class="text-xs">${order.customerEmail}</span></td>
                            <td>${new Date(order.date).toLocaleDateString()}</td>
                            <td><span class="btn btn-warning">${order.status}</span></td>
                            <td>$${order.total.toFixed(2)}</td>
                            <td>
                                <button onclick="viewOrder('${order.id}')" class="btn btn-xs btn-outline">View Order Details</button>
                            </td>
                        </tr>`;
                tbody.insertAdjacentHTML('beforeend', row);
            });
        });
}

fetchOrders();

async function viewOrder(orderId) {
    const res = await fetch(`/api/saleapi/details/${orderId}`);
    const data = await res.json();

    // Fill content
    document.getElementById("modalOrderNumber").textContent = data.orderNumber;
    document.getElementById("modalCustomerName").textContent = data.customerName;
    document.getElementById("modalCustomerEmail").textContent = data.customerEmail;
    document.getElementById("modalOrderDate").textContent = new Date(data.orderDate).toLocaleString();
    document.getElementById("modalPaymentMethod").textContent = data.paymentMethod;
    document.getElementById("modalBillingAddress").textContent = data.billingAddress;
    document.getElementById("modalShippingAddress").textContent = data.shippingAddress;

    document.getElementById("modalShippingOption").textContent = data.shippingOption;
    document.getElementById("modalEstimateDeliveryStart").textContent = data.estimateDeliveryStart;
    document.getElementById("modalEstimateDeliveryEnd").textContent = data.estimateDeliveryEnd;


    document.getElementById("modalSubtotal").textContent = data.subtotal.toFixed(2);
    document.getElementById("modalShipping").textContent = data.shippingCost.toFixed(2);
    document.getElementById("modalTotal").textContent = data.total.toFixed(2);

    // Status dropdown
    const statusSelect = document.getElementById("modalOrderStatus");
    statusSelect.innerHTML = '';
    data.availableStatuses.forEach(s => {
        const opt = document.createElement("option");
        opt.value = s;
        opt.textContent = s;
        if (s === data.status) opt.selected = true;
        statusSelect.appendChild(opt);
    });

    // Order items
    const tbody = document.getElementById("modalOrderItems");
    tbody.innerHTML = '';
    data.items.forEach(item => {
        const row = `
            <tr>
                <td>${item.productName}</td>
                <td>${item.quantity}</td>
                <td>${item.productSize}</td>
                <td>$${item.price.toFixed(2)}</td>
                <td>$${(item.price * item.quantity).toFixed(2)}</td>
            </tr>
        `;
        tbody.insertAdjacentHTML("beforeend", row);
    });

    // Show modal
    const modalElement = document.getElementById('orderDetailsModal');
    modalElement.dataset.orderId = orderId;

    const modal = new bootstrap.Modal(modalElement);
    modal.show();
}

document.getElementById("btnCancelOrder").addEventListener("click", async () => {
    const orderId = document.getElementById("orderDetailsModal").dataset.orderId;

    Swal.fire({
        title: `Cancel order with ID: ${orderId}`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, cancel it',
        cancelButtonText: 'No, keep it',
        preConfirm: async () => {
            const response = await fetch(`/api/saleapi/cancel/${orderId}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            });

            if (!response.ok) {
                Swal.fire('Error', 'Failed to cancel order', 'error');
                return;
            }

            const data = await response.json();

            if (data.result) {
                Swal.fire({ toast: true, icon: 'info', title: `Order cancelled`, position: 'top-end', timer: 2000, showConfirmButton: false });
                setTimeout(() => { location.reload(); }, 2060);
            } else {
                Swal.fire('Error', data.message || 'Failed to cancel order', 'error');
            }
        }
    });
});


document.getElementById("btnFinishOrder").addEventListener("click", async () => {
    const orderId = document.getElementById("orderDetailsModal").dataset.orderId;

    Swal.fire({
        title: `Finish order with ID: ${orderId}`,
        icon: 'info',
        showCancelButton: true,
        confirmButtonText: 'Yes, finish it',
        cancelButtonText: 'No, do not',
        preConfirm: async () => {
            const response = await fetch(`/api/saleapi/finish/${orderId}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            });

            if (!response.ok) {
                Swal.fire('Error', 'Failed to finish order', 'error');
                return;
            }

            const data = await response.json();

            if (data.result) {
                Swal.fire({ toast: true, icon: 'success', title: `Order finished`, position: 'top-end', timer: 2000, showConfirmButton: false });
                setTimeout(() => { location.reload(); }, 2060);
            } else {
                Swal.fire('Error', data.message || 'Failed to finish order', 'error');
            }
        }
    });
});