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
            document.getElementById('selectedOptionEstimatedDeliveryStart').value = this.dataset.estimatedDeliveryStart;
            document.getElementById('selectedOptionEstimatedDeliveryEnd').value = this.dataset.estimatedDeliveryEnd;
            document.getElementById('selectedOptionPrice').value = this.dataset.price;
        });
    });


    document.addEventListener('input', function (e) {
        if (e.target.classList.contains('masked-card-input')) {
            e.target.value = e.target.value.replace(/[^0-9\*]/g, '');
        }
    });

    // SHIPPING: toggle new shipping address form
    const addNewShippingBtn = document.getElementById('addNewShippingAddressBtn');
    const newShippingSection = document.getElementById('new-shipping-address-section');
    const savedShippingRadios = document.querySelectorAll('input[name="MemberAddress.SelectedShippingAddressId"]');

    if (addNewShippingBtn && newShippingSection) {
        const toggleNewShippingSection = (show) => {
            newShippingSection.style.display = show ? 'block' : 'none';
            newShippingSection.querySelectorAll('input').forEach(input => input.disabled = !show);
        };

        // Default state based on selected radio
        const hasAnySavedShipping = savedShippingRadios.length > 0;
        const hasSelectedShipping = Array.from(savedShippingRadios).some(r => r.checked);
        toggleNewShippingSection(!hasAnySavedShipping || !hasSelectedShipping);


        savedShippingRadios.forEach(radio => {
            radio.addEventListener('change', () => {
                toggleNewShippingSection(false);
            });
        });

        addNewShippingBtn.addEventListener('click', () => {
            toggleNewShippingSection(true);
            savedShippingRadios.forEach(r => r.checked = false);
        });
    }

    // BILLING: toggle new billing address form
    const addNewBillingBtn = document.getElementById('addNewBillingAddressBtn');
    const newBillingSection = document.getElementById('new-billing-address-section');
    const savedBillingRadios = document.querySelectorAll('input[name="MemberAddress.SelectedBillingAddressId"]');

    if (addNewBillingBtn && newBillingSection) {
        const toggleNewBillingSection = (show) => {
            newBillingSection.style.display = show ? 'block' : 'none';
            newBillingSection.querySelectorAll('input').forEach(input => input.disabled = !show);
        };

        const hasAnySavedBilling = savedBillingRadios.length > 0;
        const hasSelectedBilling = Array.from(savedBillingRadios).some(r => r.checked);
        toggleNewBillingSection(!hasSelectedBilling || !hasAnySavedBilling);

        savedBillingRadios.forEach(radio => {
            radio.addEventListener('change', () => {
                toggleNewBillingSection(false);
            });
        });

        addNewBillingBtn.addEventListener('click', () => {
            toggleNewBillingSection(true);
            savedBillingRadios.forEach(r => r.checked = false);
        });
    }

    function parseCurrency(value) {
        if (value.trim().toLowerCase() === 'free') return 0;

        return parseFloat(value.replace(/[^0-9.-]+/g, ""));
    }

    function formatCurrency(num) {
        if (num == 0) return "Free";

        return num.toLocaleString('en-US', { style: 'currency', currency: 'USD' });
    }

    document.querySelectorAll('input[name="shippingOption"]').forEach(so => {
        so.addEventListener('change', function () {
            const shippingPrice = parseFloat(so.dataset.price) || 0;

            let subtotal = parseCurrency(document.getElementById('summary-subtotal-span').textContent);
            let delivery = parseCurrency(document.getElementById('summary-deliveryCost-span').textContent);

            let total;
            if (shippingPrice === 0) {
                delivery = 0;
                total = subtotal;
            } else if (shippingPrice === 20) {
                delivery = subtotal >= 300 ? 0 : shippingPrice;
                total = subtotal + delivery;
            } else {
                delivery = shippingPrice;
                total = subtotal + delivery;
            }

            document.getElementById('summary-subtotal-span').textContent = formatCurrency(subtotal);
            document.getElementById('summary-deliveryCost-span').textContent = delivery === 0 ? 'Free' : formatCurrency(delivery);
            document.getElementById('summary-total-span').textContent = formatCurrency(total);
        });
    });
    
});
