$(document).ready(function () {
    let originalHeader = $('#searchResultsHeader').text();
    let queryMatch = originalHeader.match(/Search results for "(.*)"/);
    let queryText = queryMatch ? queryMatch[1] : '';

    $('#applyFiltersBtn').on('click', function () {
        const selectedGenders = $('.filter-section input[type="checkbox"]:checked')
            .filter(function () { return $(this).attr('id').startsWith('gender'); })
            .map(function () { return $(this).val().toLowerCase(); })
            .get();

        const selectedColors = $('.filter-section input[type="checkbox"]:checked')
            .filter(function () { return $(this).attr('id').startsWith('color'); })
            .map(function () { return $(this).val().toLowerCase(); })
            .get();

        const selectedPrice = $('input[name="price"]:checked').val();

        let visibleCount = 0;

        $('.col[data-gender]').each(function () {
            const gender = $(this).data('gender').toString().toLowerCase();
            const color = $(this).data('color').toString().toLowerCase();
            const price = parseFloat($(this).data('price'));

            let matchesGender = selectedGenders.length === 0 || selectedGenders.includes(gender);
            let matchesColor = selectedColors.length === 0 || selectedColors.includes(color);
            let matchesPrice = true;

            if (selectedPrice === 'under100') {
                matchesPrice = price < 100;
            } else if (selectedPrice === 'over100') {
                matchesPrice = price >= 100;
            }

            if (matchesGender && matchesColor && matchesPrice) {
                $(this).show();
                visibleCount++;
            } else {
                $(this).hide();
            }
        });

        $('#searchResultsHeader').html(`Search results for "<strong>${queryText}</strong>" (${visibleCount})`);

    });
});