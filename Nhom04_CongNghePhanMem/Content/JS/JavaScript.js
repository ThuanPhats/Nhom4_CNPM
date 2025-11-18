

    // --- Cập nhật trạng thái bằng  AJAX ---
    document.querySelectorAll(".trangthai-dropdown").forEach(drop => {
            drop.addEventListener("change", async (e) => {
                const row = e.target.closest("tr");
                const maDH = row.dataset.id;
                const newStatus = e.target.value;

                const response = await fetch(`/Home/UpdateTrangThaiDonHang`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ MaDH: maDH, TrangThai: newStatus })
                });

                if (response.ok) {
                    alert(" Cập nhật trạng thái thành công!");
                } else {
                    alert("Lỗi khi cập nhật trạng thái!");
                }
            });
    });

