$(document).ready(function () {
    $("body").on("click", ".btnAddToCart", function (e) {
        e.preventDefault();

        var productId = $(this).data("id");
        var quantity = 1; // Số lượng mặc định là 1
        var tquantity = $("#quantity_value").text();
        if (tquantity !== '') {
            quantity = parseInt(tquantity);
        }

        console.log("Product ID:", productId);
        console.log("Quantity:", quantity);

        // Gửi yêu cầu AJAX đến controller
        $.ajax({
            url: "/ShoppingCart/AddToCart",
            type: "POST",
            data: { productId: productId, quantity: quantity },
            success: function (response) {
                console.log(response);
                $("#checkout_items").html(response.count)
                alert(response.message);

                // Cập nhật giỏ hàng trên UI nếu cần

            },
            error: function (xhr, status, error) {
                // Kiểm tra mã trạng thái HTTP
                if (xhr.status === 401) {
                    // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                    window.location.href = "/Account/Login?ReturnUrl=" + encodeURIComponent(window.location.href);
                } else {
                    alert("Có lỗi xảy ra khi thêm vào giỏ hàng.");
                }
            }
        });
    });
});