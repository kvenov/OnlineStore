async function loadCustomerInsights() {
    const fromDate = document.getElementById('custFromDate').value;
    const toDate = document.getElementById('custToDate').value;

    const res = await fetch(`/api/saleapi/customer-insights?fromDate=${fromDate}&toDate=${toDate}`);
    const data = await res.json();

    // Update KPIs
    document.getElementById('repeatBuyerRate').textContent = data.repeatBuyerRate.toFixed(1) + '%';
    document.getElementById('avgLtv').textContent = '$' + data.averageLtv.toFixed(2);
    document.getElementById('totalCustomers').textContent = data.totalCustomers;

    // Fill Top Customers Table
    const tableBody = document.getElementById('topCustomersTable');
    tableBody.innerHTML = '';
    const select = document.getElementById('customerSelect');
    select.innerHTML = '<option value="">Select Customer</option>';

    data.topCustomers.forEach(cust => {
        tableBody.innerHTML += `
      <tr>
        <td>${cust.name}</td>
        <td>${cust.ordersCount}</td>
        <td>$${cust.totalSpent.toFixed(2)}</td>
      </tr>
    `;
        select.innerHTML += `<option value="${cust.customerId}">${cust.name}</option>`;
    });
}

async function loadOrderHistory() {
    const customerId = document.getElementById('customerSelect').value;
    if (!customerId) {
        document.getElementById('orderHistoryTable').innerHTML = '';
        return;
    }

    const res = await fetch(`/api/saleapi/customer-orders/${customerId}`);
    const orders = await res.json();

    const tableBody = document.getElementById('orderHistoryTable');
    tableBody.innerHTML = '';
    orders.forEach(o => {
        tableBody.innerHTML += `
      <tr>
        <td>${o.orderNumber}</td>
        <td>${new Date(o.date).toLocaleDateString()}</td>
        <td>${o.status}</td>
        <td>$${o.total.toFixed(2)}</td>
      </tr>
    `;
    });
}

document.addEventListener('DOMContentLoaded', loadCustomerInsights());