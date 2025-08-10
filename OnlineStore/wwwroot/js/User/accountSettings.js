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

document.addEventListener("DOMContentLoaded", () => {
    const forms = document.querySelectorAll(".settings-form");

    forms.forEach(form => {
        form.addEventListener("submit", async (e) => {
            e.preventDefault();

            const url = form.dataset.apiUrl;
            const method = form.getAttribute("method") || "post";

            const formData = new FormData(form);
            const jsonData = Object.fromEntries(formData.entries());

            try {
                const response = await fetch(url, {
                    method: method.toUpperCase(),
                    headers: {
                        "Content-Type": "application/json",
                        "RequestVerificationToken": getAntiForgeryToken()
                    },
                    body: JSON.stringify(jsonData)
                });

                if (!response.ok) {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: "Something went wrong!"
                    });
                    return;
                }

                const data = await response.json();

                if (data.result) {
                    Swal.fire({
                        icon: "success",
                        title: "Success",
                        text: data.message || "Your changes have been saved.",
                        timer: 2200,
                        showConfirmButton: false
                    });

                    setTimeout(() => {
                        location.reload();
                    }, 2300)
                } else {
                    Swal.fire({
                        icon: "error",
                        title: "Error",
                        text: data.message || "Could not save your changes."
                    });
                }

            } catch (err) {
                console.error("Request failed:", err);
                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: "Network error. Please try again."
                });
            }
        });
    });

    function getAntiForgeryToken() {
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenInput ? tokenInput.value : "";
    }
});