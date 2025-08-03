document.addEventListener('DOMContentLoaded', function () {
    const expMonthInput = document.getElementById('Payment_CreditCardDetails_ExpMonth');
    const expYearInput = document.getElementById('Payment_CreditCardDetails_ExpYear');

    const errorMonthSpan = document.getElementById('ExpMonthError');
    const errorYearSpan = document.getElementById('ExpYearError');

    function validateExpiration() {
        const month = parseInt(expMonthInput.value, 10);
        const year = parseInt(expYearInput.value, 10);
        let fullYear = year;

        if (isNaN(month) || isNaN(year)) {
            showExpError(false);
            return;
        }

        if (year < 100) {
            fullYear += 2000;
        }

        const now = new Date();
        const current = new Date(now.getFullYear(), now.getMonth(), 1);
        const expiration = new Date(fullYear, month - 1, 1);

        const isValid = expiration >= current;
        showExpError(!isValid);
    }

    function showExpError(show) {
        if (show) {
            errorMonthSpan.classList.remove('d-none');
            errorYearSpan.classList.remove('d-none');
        } else {
            errorMonthSpan.classList.add('d-none');
            errorYearSpan.classList.add('d-none');
        }
    }

    expYearInput.addEventListener('input', () => {
        validateExpiration();
    });

    expMonthInput.addEventListener('input', () => {
        validateExpiration();
    });
});