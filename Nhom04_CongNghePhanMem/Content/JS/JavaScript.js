// ----- Khởi tạo khi trang load -----
window.addEventListener('DOMContentLoaded', function () {

    // ----- Set giá trị mặc định cho Size -----
    const activeSize = document.querySelector('.size-btn.active');
    if (activeSize) {
        document.getElementById('selectedSize').value = activeSize.textContent.trim();
    }

    // ----- Set giá trị mặc định cho Color -----
    const checkedColor = document.querySelector('input[name="colorRadio"]:checked');
    if (checkedColor) {
        document.getElementById('selectedColor').value = checkedColor.value;
    }

    // ----- Chọn Size -----
    document.querySelectorAll('.size-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            // Remove active của các nút khác
            document.querySelectorAll('.size-btn').forEach(b => b.classList.remove('active'));
            // Add active cho nút được click
            this.classList.add('active');
            // Cập nhật hidden input
            document.getElementById('selectedSize').value = this.textContent.trim();
        });
    });

    // ----- Chọn Màu -----
    document.querySelectorAll('input[name="colorRadio"]').forEach(radio => {
        radio.addEventListener('change', function () {
            document.getElementById('selectedColor').value = this.value;
        });
    });

    // ----- Thêm vào LocalStorage trước khi submit -----
    const form = document.getElementById('frmAddCart');
    if (form) {
        form.addEventListener('submit', function (e) {

            let size = document.getElementById('selectedSize').value;
            let color = document.getElementById('selectedColor').value;
            let quantity = parseInt(document.getElementById('soluong').value);

            // Lấy dữ liệu từ form attribute
            let productId = parseInt(this.dataset.id);
            let productName = this.dataset.name;
            let productPrice = parseFloat(this.dataset.price);
            let productImage = this.dataset.image;

            if (!size || !color) {
                alert("Vui lòng chọn size và màu!");
                e.preventDefault(); // chặn submit
                return;
            }

            // Lấy giỏ hàng từ LocalStorage
            let cartJson = localStorage.getItem('localCartItems');
            let cart = cartJson ? JSON.parse(cartJson) : [];

            // Kiểm tra sản phẩm đã tồn tại chưa
            let existIndex = cart.findIndex(x =>
                x.id === productId &&
                x.size === size &&
                x.color === color
            );

            if (existIndex !== -1) {
                cart[existIndex].quantity += quantity;
            } else {
                cart.push({
                    id: productId,
                    name: productName,
                    price: productPrice,
                    image: productImage,
                    size: size,
                    color: color,
                    quantity: quantity
                });
            }

            localStorage.setItem('localCartItems', JSON.stringify(cart));
            console.log("Đã lưu LocalStorage:", cart);
            alert("Đã thêm vào giỏ hàng!");

            // Nếu muốn gửi form lên server, không preventDefault()
        });
    }

});
