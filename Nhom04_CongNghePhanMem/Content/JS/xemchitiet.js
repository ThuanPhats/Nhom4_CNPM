$(document).ready(function () {

    var $addToCartButton = $('#addToCartButton');
    var $sizeInput = $('#selectedSizeHiddenInput');

    // --- 1. Xử lý chọn Size ---
    $addToCartButton.prop('disabled', true).text('Vui lòng chọn kích cỡ');
    $sizeInput.val('');

    $('.size-option').on('click', function (e) {
        if ($(this).hasClass('disabled')) return;

        var selectedSize = $(this).data('size');
        $sizeInput.val(selectedSize);

        $('.size-option').removeClass('active');
        $(this).addClass('active');

        $addToCartButton.prop('disabled', false)
            .removeClass('btn-secondary')
            .addClass('btn-pink')
            .html('<i class="bi bi-cart-plus"></i> Thêm vào giỏ hàng');
    });

    // --- 2. Add to cart ---
    $('#addToCartForm').on('submit', function (e) {
        e.preventDefault();

        if ($sizeInput.val() === '') {
            alert('Vui lòng chọn kích cỡ trước khi thêm vào giỏ hàng.');
            return;
        }

        // Lấy dữ liệu từ data-* attributes
        var productId = $addToCartButton.data('product-id');
        var productName = $addToCartButton.data('product-name');
        var productPrice = $addToCartButton.data('product-price');
        var productImage = $addToCartButton.data('product-image');
        var selectedSize = $sizeInput.val();

        var newItem = {
            id: productId,
            name: productName,
            price: productPrice,
            image: productImage,
            size: selectedSize,
            quantity: 1
        };

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
        $.ajax({
            url: '/Home/ThemVaoGioHang',
            type: 'GET',
            data: { id: productId, size: selectedSize },
            success: function () {
                alert('Đã thêm sản phẩm vào giỏ hàng!');
                window.location.href = '/Home/GioHang';
            },
            error: function () {
                alert('Lỗi khi thêm sản phẩm vào giỏ hàng trên server.');
            }
        });
    });

    // --- 3. Trường hợp không có size ---
    if ($('#size-options-container .size-option').length === 0) {
        $addToCartButton.prop('disabled', true)
            .removeClass('btn-pink')
            .addClass('btn-secondary')
            .text('Không có hàng');
    }

});
