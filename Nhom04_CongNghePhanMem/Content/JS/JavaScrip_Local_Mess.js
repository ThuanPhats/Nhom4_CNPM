// --- Lưu localStorage ---
var cartJson = localStorage.getItem('localCartItems');
var cart = cartJson ? JSON.parse(cartJson) : [];
var existingIndex = cart.findIndex(item => item.id === newItem.id && item.size === newItem.size);

if (existingIndex !== -1) {
    cart[existingIndex].quantity += 1;
} else {
    cart.push(newItem);
}

localStorage.setItem('localCartItems', JSON.stringify(cart));
console.log('Đã lưu vào localStorage:', cart);

// --- Gửi AJAX cập nhật session server ---
// alerts.js
function ShowSuccess(message) {
    Swal.fire({
        icon: 'success',
        title: message,
        showConfirmButton: false,
        timer: 1800,
        position: 'top-end',
        toast: true
    });
}

function ShowError(message) {
    Swal.fire({
        icon: 'error',
        title: message,
        showConfirmButton: false,
        timer: 1800,
        position: 'top-end',
        toast: true
    });
}

