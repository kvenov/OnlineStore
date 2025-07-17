document.addEventListener('DOMContentLoaded', () => {
    // Billing address toggle
    const sameAsShippingCheckbox = document.getElementById('sameAsShipping');
    const billingAddressSection = document.getElementById('billing-address-section');

    if (sameAsShippingCheckbox && billingAddressSection) {
        const toggleBillingAddress = () => {
            if (sameAsShippingCheckbox.checked) {
                billingAddressSection.style.display = 'none';
                // Disable inputs inside billing section so they don't submit
                billingAddressSection.querySelectorAll('input').forEach(input => input.disabled = true);
            } else {
                billingAddressSection.style.display = 'flex'; // match Bootstrap row display
                billingAddressSection.querySelectorAll('input').forEach(input => input.disabled = false);
            }
        };

        toggleBillingAddress(); // initial setup
        sameAsShippingCheckbox.addEventListener('change', toggleBillingAddress);
    }

    // Payment method toggle for Credit Card details
    const paymentOptionRadios = document.querySelectorAll('input[name="SelectedPaymentOption"]');
    const creditCardDetails = document.getElementById('creditCardDetails');

    if (paymentOptionRadios.length && creditCardDetails) {
        const toggleCreditCardDetails = () => {
            const selected = Array.from(paymentOptionRadios).find(r => r.checked);
            // In your razor, Credit Card has value="0"
            if (selected && selected.value === '0') {
                creditCardDetails.classList.remove('d-none');
            } else {
                creditCardDetails.classList.add('d-none');
            }
        };

        toggleCreditCardDetails(); // initial setup
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

        updateAddressHighlight(); // initial highlight
        savedAddressRadios.forEach(radio => radio.addEventListener('change', updateAddressHighlight));
    }
});
