document.addEventListener("DOMContentLoaded", () => {
    const newPassword = document.querySelector("#NewPassword");
    const strengthIndicator = document.createElement("small");
    if (newPassword) {
        newPassword.parentElement.appendChild(strengthIndicator);

        newPassword.addEventListener("input", () => {
            const val = newPassword.value;
            let strength = "Weak";
            if (val.length > 8 && /[A-Z]/.test(val) && /\d/.test(val) && /[\W]/.test(val)) {
                strength = "Strong";
                strengthIndicator.style.color = "green";
            } else if (val.length >= 6) {
                strength = "Medium";
                strengthIndicator.style.color = "orange";
            } else {
                strengthIndicator.style.color = "red";
            }
            strengthIndicator.textContent = `Password strength: ${strength}`;
        });
    }
});