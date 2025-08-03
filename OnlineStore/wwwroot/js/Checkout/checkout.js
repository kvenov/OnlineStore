document.addEventListener('DOMContentLoaded', () => {
    // Billing address toggle
    const sameAsShippingCheckbox = document.getElementById('sameAsShipping');
    const billingAddressSection = document.getElementById('billing-address-section');

    if (sameAsShippingCheckbox && billingAddressSection) {
        const toggleBillingAddress = () => {
            if (sameAsShippingCheckbox.checked) {
                billingAddressSection.style.display = 'none';
                billingAddressSection.querySelectorAll('input').forEach(input => input.disabled = true);
            } else {
                billingAddressSection.style.display = 'flex';
                billingAddressSection.querySelectorAll('input').forEach(input => input.disabled = false);
            }
        };

        toggleBillingAddress();
        sameAsShippingCheckbox.addEventListener('change', toggleBillingAddress);
    }

    // Payment method toggle for Credit Card details
    const paymentOptionRadios = document.querySelectorAll('input[name="Payment.SelectedPaymentOption"]');
    const creditCardDetails = document.getElementById('creditCardDetails');

    if (paymentOptionRadios.length && creditCardDetails) {
        const toggleCreditCardDetails = () => {
            const selected = Array.from(paymentOptionRadios).find(r => r.checked);
            if (selected && selected.value === 'CreditCard') {
                creditCardDetails.classList.remove('d-none');
            } else {
                creditCardDetails.classList.add('d-none');
            }
        };

        toggleCreditCardDetails();
        paymentOptionRadios.forEach(radio => radio.addEventListener('change', toggleCreditCardDetails));
    }

    // Saved Shipping Address cards highlight
    const savedAddressRadios = document.querySelectorAll('input[name="SelectedShippingAddressId"]');

    if (savedAddressRadios.length) {
        const updateAddressHighlight = () => {
            savedAddressRadios.forEach(radio => {
                const cardLabel = radio.closest('label.card');
                if (cardLabel) {
                    if (radio.checked) {
                        cardLabel.classList.add('border-primary', 'shadow');
                    } else {
                        cardLabel.classList.remove('border-primary', 'shadow');
                    }
                }
            });
        };

        updateAddressHighlight();
        savedAddressRadios.forEach(radio => radio.addEventListener('change', updateAddressHighlight));
    }

    const radios = document.querySelectorAll('input[name="shippingOption"]');
    radios.forEach(radio => {
        radio.addEventListener('change', function () {
            document.getElementById('selectedOptionName').value = this.dataset.name;
            document.getElementById('selectedOptionDateRange').value = this.dataset.daterange;
            document.getElementById('selectedOptionPrice').value = this.dataset.price;
        });
    });


    document.addEventListener('input', function (e) {
        if (e.target.classList.contains('masked-card-input')) {
            e.target.value = e.target.value.replace(/[^0-9\*]/g, '');
        }
    });
});
