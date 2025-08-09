let topSellingChart, leastSellingChart, salesBySizeChart, salesByPriceChart;

function renderCharts(data) {
    if (topSellingChart) topSellingChart.destroy();
    if (leastSellingChart) leastSellingChart.destroy();
    if (salesBySizeChart) salesBySizeChart.destroy();
    if (salesByPriceChart) salesByPriceChart.destroy();

    topSellingChart = new Chart(document.getElementById('topSellingChart'), {
        type: 'bar',
        data: {
            labels: data.topSellingProducts.map(p => p.productName),
            datasets: [{ label: 'Units Sold', data: data.topSellingProducts.map(p => p.unitsSold) }]
        }
    });

    leastSellingChart = new Chart(document.getElementById('leastSellingChart'), {
        type: 'bar',
        data: {
            labels: data.leastSellingProducts.map(p => p.productName),
            datasets: [{ label: 'Units Sold', data: data.leastSellingProducts.map(p => p.unitsSold) }]
        }
    });

    salesBySizeChart = new Chart(document.getElementById('salesBySizeChart'), {
        type: 'pie',
        data: {
            labels: data.salesBySize.map(s => s.size),
            datasets: [{ data: data.salesBySize.map(s => s.unitsSold) }]
        }
    });

    salesByPriceChart = new Chart(document.getElementById('salesByPriceChart'), {
        type: 'doughnut',
        data: {
            labels: data.salesByPriceRange.map(p => p.priceRange),
            datasets: [{ data: data.salesByPriceRange.map(p => p.unitsSold) }]
        }
    });
}

async function loadAnalytics() {
    const fromDate = document.getElementById('filterFromDate').value;
    const toDate = document.getElementById('filterToDate').value;
    const priceRange = document.getElementById('filterPriceRange').value;

    const query = new URLSearchParams({ fromDate, toDate, priceRange });

    const res = await fetch(`/api/saleapi/product-analytics?${query}`);
    const data = await res.json();

    renderCharts(data);
}

document.addEventListener('DOMContentLoaded', loadAnalytics);