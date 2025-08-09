const geoCacheKey = 'geoCache_v1';
const geoCache = JSON.parse(localStorage.getItem(geoCacheKey) || '{}');

function saveGeoCache() {
    localStorage.setItem(geoCacheKey, JSON.stringify(geoCache));
}

async function geocode(query) {
    // return cached if present
    if (geoCache[query]) return geoCache[query];

    // Use Nominatim (OpenStreetMap) public API
    const url = `https://nominatim.openstreetmap.org/search?format=json&limit=1&q=${encodeURIComponent(query)}`;

    // Respect Nominatim usage policy — set a short delay between requests.
    const res = await fetch(url, {
        headers: { 'Accept-Language': 'en' } // optional
    });
    if (!res.ok) return null;

    const arr = await res.json();
    if (!Array.isArray(arr) || arr.length === 0) return null;

    const { lat, lon } = arr[0];
    const coords = { lat: parseFloat(lat), lng: parseFloat(lon) };

    // cache and return
    geoCache[query] = coords;
    saveGeoCache();
    return coords;
}

function sleep(ms) { return new Promise(r => setTimeout(r, ms)); }

let map, heatLayer;
function initMap() {
    if (map) return;

    map = L.map('salesHeatmap', { preferCanvas: true }).setView([20, 0], 2);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 18,
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);

    heatLayer = L.heatLayer([], { radius: 25, blur: 15, maxZoom: 10 }).addTo(map);
}

async function buildHeatmapPoints(salesByCity, countryFilter) {
    initMap();
    document.getElementById('heatmapStatus').textContent = 'Geocoding cities... (this may take a few seconds)';
    heatLayer.setLatLngs([]);

    const maxSales = Math.max(...salesByCity.map(s => s.totalSales || 0), 1);

    const points = [];
    for (let i = 0; i < salesByCity.length; i++) {
        const cityRow = salesByCity[i];
        const cityName = cityRow.locationName;
        const qParts = [];
        if (cityRow.zip && cityRow.zip.length) qParts.push(cityRow.zip);
        if (cityName) qParts.push(cityName);
        if (countryFilter) qParts.push(countryFilter);
        const q = qParts.join(', ');

        if (cityRow.latitude && cityRow.longitude) {
            const intensity = (cityRow.totalSales || 0) / maxSales;
            points.push([cityRow.latitude, cityRow.longitude, intensity]);
            continue;
        }

        const cacheKey = q || cityName;
        let coords = geoCache[cacheKey];
        if (!coords) {
            await sleep(900);
            coords = await geocode(q || cityName);
        }
        if (!coords) continue;

        const intensity = (cityRow.totalSales || 0) / maxSales;
        points.push([coords.lat, coords.lng, intensity]);
    }

    if (points.length === 0) {
        document.getElementById('heatmapStatus').textContent = 'No coordinates available to show heatmap.';
        return;
    }

    heatLayer.setLatLngs(points);
    const latlngs = points.map(p => [p[0], p[1]]);
    const bounds = L.latLngBounds(latlngs);
    map.fitBounds(bounds.pad(0.25));
    document.getElementById('heatmapStatus').textContent = `Showing ${points.length} locations on heatmap.`;
}


let countryChart, cityChart;

function renderLocationCharts(data) {
    if (countryChart) countryChart.destroy();
    if (cityChart) cityChart.destroy();

    // Country Pie Chart
    countryChart = new Chart(document.getElementById('countrySalesChart'), {
        type: 'pie',
        data: {
            labels: data.salesByCountry.map(c => c.locationName),
            datasets: [{ data: data.salesByCountry.map(c => c.totalSales) }]
        }
    });

    // City Bar Chart
    cityChart = new Chart(document.getElementById('citySalesChart'), {
        type: 'bar',
        data: {
            labels: data.salesByCity.map(c => c.locationName),
            datasets: [{ label: 'Total Sales', data: data.salesByCity.map(c => c.totalSales) }]
        }
    });

    // Populate Country Table
    const countryRows = data.salesByCountry.map(c =>
        `<tr><td>${c.locationName}</td><td>${c.ordersCount}</td><td>$${c.totalSales.toFixed(2)}</td></tr>`
    ).join('');
    document.getElementById('countrySalesTable').innerHTML = countryRows;

    // Populate City Table
    const cityRows = data.salesByCity.map(c =>
        `<tr><td>${c.locationName}</td><td>${c.ordersCount}</td><td>$${c.totalSales.toFixed(2)}</td></tr>`
    ).join('');
    document.getElementById('citySalesTable').innerHTML = cityRows;
}

async function loadSalesByLocation() {
    const fromDate = document.getElementById('filterFromDate').value;
    const toDate = document.getElementById('filterToDate').value;
    const country = document.getElementById('filterCountry').value;
    const city = document.getElementById('filterCity').value;
    const zip = document.getElementById('filterZip').value;

    const query = new URLSearchParams({ fromDate, toDate, country, city, zip });
    const res = await fetch(`/api/saleapi/sales-by-location?${query}`);
    const data = await res.json();

    renderLocationCharts(data);

    await buildHeatmapPoints(data.salesByCity, country);
}
