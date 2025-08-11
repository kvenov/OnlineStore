document.addEventListener("DOMContentLoaded", () => {
    const searchInput = document.getElementById("searchInput");
    const roleFilter = document.getElementById("roleFilter");
    const userCards = document.querySelectorAll(".user-card");

    function normalize(text) {
        return text.toLowerCase().trim();
    }

    function searchUsers() {
        const searchTerm = normalize(searchInput.value);
        const selectedRole = roleFilter.value;

        userCards.forEach(card => {
            const username = normalize(card.dataset.userName);
            const email = normalize(card.dataset.userEmail);
            const roles = card.dataset.userRoles.split(',');

            const matchesSearch =
                username.includes(searchTerm) || email.includes(searchTerm);

            const matchesRole =
                selectedRole === "all" || roles.includes(selectedRole);

            if (matchesSearch && matchesRole) {
                card.style.display = "block";
            } else {
                card.style.display = "none";
            }
        });
    }

    function filterUsersByRole() {
        const selectedRole = roleFilter.value;

        userCards.forEach(card => {
            const roles = card.dataset.userRoles.split(',');

            const matchesRole =
                selectedRole === "all" || roles.includes(selectedRole);

            if (matchesRole) {
                card.style.display = "block";
            } else {
                card.style.display = "none";
            }
        });
    }

    searchInput.addEventListener("input", searchUsers);
    roleFilter.addEventListener("change", filterUsersByRole);

});

function isUserAdmin(user) {
    return user.roles.includes('Admin');
}

function getUserData(element) {
    const card = element.closest('.user-card');
    return {
        id: card.dataset.userId,
        name: card.dataset.userName,
        email: card.dataset.userEmail,
        roles: card.dataset.userRoles.split(',')
    };
}

function viewDetails(button) {
    const user = getUserData(button);
    Swal.fire({
        title: `${user.name} Details`,
        html: `<p><strong>Email:</strong> ${user.email}</p><p><strong>Roles:</strong> ${user.roles.join(', ')}</p>`,
        icon: 'info'
    });
}

function assignRole(button) {
    const user = getUserData(button);
    if (isUserAdmin(user)) {
        Swal.fire('Permission Denied', 'You cannot modify an existing Admin!', 'warning');
        return;
    }

    const allRoles = ['Admin', 'Manager', 'User'];

    const availableRoles = allRoles.filter(role => !user.roles.includes(role));

    if (availableRoles.length === 0) {
        Swal.fire('No Roles to Assign', `${user.name} already has all assignable roles.`, 'info');
        return;
    }

    const inputOptions = {};
    availableRoles.forEach(role => inputOptions[role] = role);

    Swal.fire({
        title: `Assign Role to ${user.name}`,
        input: 'select',
        inputOptions: inputOptions,
        inputPlaceholder: 'Select a role',
        showCancelButton: true,
        preConfirm: async (role) => {
            const response = await fetch(`/api/usermanagementapi/assign/${user.id}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    "RequestVerificationToken": getAntiForgeryToken()
                },
                body: JSON.stringify({ role })
            });

            if (!response.ok) Swal.fire('Error', 'Failed to assign role', 'error');

            const data = await response.json();

            if (data.result) {
                Swal.fire({ toast: true, icon: 'success', title: `${role} assigned`, position: 'top-end', timer: 2000, showConfirmButton: false });

                setTimeout(() => {
                    location.reload();
                }, 2060);
            } else {
                Swal.fire('Error', 'Failed to assign role', 'error');
            }
        }
    });
}

function removeRole(button) {
    const user = getUserData(button);
    if (isUserAdmin(user)) {
        Swal.fire('Permission Denied', 'You cannot modify an existing Admin!', 'warning');
        return;
    }

    const removableRoles = user.roles;
    if (removableRoles.length === 0) {
        Swal.fire('Not Allowed', 'No removable roles available.', 'info');
        return;
    }

    const options = removableRoles.reduce((obj, r) => { obj[r] = r; return obj; }, {});

    Swal.fire({
        title: `Remove Role from ${user.name}`,
        input: 'select',
        inputOptions: options,
        inputPlaceholder: 'Select a role to remove',
        showCancelButton: true,
        preConfirm: async (role) => {
            const response = await fetch(`/api/usermanagementapi/remove/${user.id}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    "RequestVerificationToken": getAntiForgeryToken()
                },
                body: JSON.stringify({ role })
            });

            if (!response.ok) Swal.fire('Error', 'Failed to remove role', 'error'); 

            const data = await response.json();

            if (data.result) {
                Swal.fire({ toast: true, icon: 'info', title: `${role} removed`, position: 'top-end', timer: 2000, showConfirmButton: false });

                setTimeout(() => {
                    location.reload();
                }, 2060);
            } else {
                Swal.fire('Error', 'Failed to remove role', 'error');
            }
        }
    });
}

function softDelete(button) {
    const user = getUserData(button);
    if (isUserAdmin(user)) {
        Swal.fire('Permission Denied', 'You cannot modify an existing Admin!', 'warning');
        return;
    }

    Swal.fire({
        title: `Soft delete ${user.name}?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, soft delete',
        preConfirm: async () => {
            const response = await fetch(`/api/usermanagementapi/delete/${user.id}`, {
                method: 'POST',
                headers: {
                    "RequestVerificationToken": getAntiForgeryToken()
                }
            });

            if (!response.ok) Swal.fire('Error', 'Failed to soft delete user', 'error');

            const data = await response.json();

            if (data.result) {
                Swal.fire({ toast: true, icon: 'warning', title: 'User soft-deleted', position: 'top-end', timer: 2000, showConfirmButton: false });

                setTimeout(() => {
                    location.reload();
                }, 2060);
            } else {
                Swal.fire('Error', 'Failed to soft delete user', 'error');
            }
        }
    });
}

function renew(button) {
    const user = getUserData(button);
    if (isUserAdmin(user)) {
        Swal.fire('Permission Denied', 'You cannot modify an existing Admin!', 'warning');
        return;
    }

    Swal.fire({
        title: `Renew ${user.name}?`,
        icon: 'info',
        showCancelButton: true,
        confirmButtonText: 'Yes, renew user',
        preConfirm: async () => {
            const response = await fetch(`/api/usermanagementapi/renew/${user.id}`, {
                method: 'POST',
                headers: {
                    "RequestVerificationToken": getAntiForgeryToken()
                }
            });

            if (!response.ok) Swal.fire('Error', 'Failed to renew user', 'error');

            const data = await response.json();

            if (data.result) {
                Swal.fire({ toast: true, icon: 'success', title: 'User renewed', position: 'top-end', timer: 2000, showConfirmButton: false });

                setTimeout(() => {
                    location.reload();
                }, 2060);
            } else {
                Swal.fire('Error', 'Failed to renew user', 'error');
            }
        }
    });
}


function getAntiForgeryToken() {
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    return tokenInput ? tokenInput.value : "";
}