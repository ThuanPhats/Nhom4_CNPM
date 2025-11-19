using Nhom04_CongNghePhanMem.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Data.Linq.Mapping;
using System.Web;
using System.Web.Mvc;


namespace Nhom04_CongNghePhanMem.Controllers
{
    public class HomeController : Controller
    {
        QuanLiCuaHangDataContext data = new QuanLiCuaHangDataContext();
        public ActionResult Index(string search)
        {
            var spQuery = data.SANPHAMs.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                string searchLower = search.ToLower();
                spQuery = spQuery.Where(sp => sp.TENSP.ToLower().Contains(searchLower));
            }

            var spList = spQuery.ToList();

            ViewBag.Search = search;

            return View(spList ?? new List<SANPHAM>());
        }





        // Sản phẩm hot
        public PartialViewResult HotProduct()
        {
            var hotProducts = data.SANPHAMs
                       .OrderByDescending(sp => sp.DIEMTB>3) 
                       .Take(4)                          
                       .ToList();
            return PartialView( hotProducts);       
        }

        // Sản phẩm mới
        public PartialViewResult NewProduct()
        {
            // Lấy tên sản phẩm từ HOT
            var newProducts = data.SANPHAMs
                       .OrderByDescending(sp => sp.MASP) // lấy sản phẩm mới nhất theo ID
                       .Take(4)                          // chỉ lấy 5 cái
                       .ToList();

            return PartialView(newProducts);
        }

        public ActionResult MenuDMLoai(int? categoryId)
        {
            List<LOAISANPHAM> DM = data.LOAISANPHAMs.ToList();
            var vayCuoiQuery = data.SANPHAMs.AsQueryable();

            if (categoryId.HasValue)
            {

                vayCuoiQuery = vayCuoiQuery.Where(v => v.MALOAI == categoryId);
            }
            ViewBag.Categories = new SelectList(data.LOAISANPHAMs, "MALOAI", "TENLOAI", categoryId);
            return PartialView(DM);
        }






        public ActionResult SanPham(string search, int? categoryId, string price, int page = 1)
        {
            int pageSize = 15;
            var query = data.SANPHAMs.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(sp => sp.TENSP.ToLower().Contains(search.ToLower()));

            if (categoryId.HasValue)
                query = query.Where(sp => sp.MALOAI == categoryId);

            if (!string.IsNullOrEmpty(price))
            {
                switch (price)
                {
                    case "duoi-1": query = query.Where(sp => sp.GIABAN < 100000); break;
                    case "2-5": query = query.Where(sp => sp.GIABAN >= 200000 && sp.GIABAN <=500000); break;
                    case "tren-15": query = query.Where(sp => sp.GIABAN > 500000); break;
                }
            }

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var products = query
                .OrderBy(sp => sp.TENSP)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Count = totalItems;
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Categories = new SelectList(data.LOAISANPHAMs.ToList(), "MALOAI", "TENLOAI");
            ViewBag.SelectedCategory = categoryId;
            ViewBag.CurrentPrice = price;

            return View(products);
        }
        //chitiet
        public ActionResult XemChiTiet(int id)
        {
            if (id < 0)
                return RedirectToAction("Index");

            var sp = data.SANPHAMs.FirstOrDefault(item => item.MASP == id);
            if (sp == null)
                return HttpNotFound();

            var ds1 = data.SANPHAMs
                          .Where(x => x.MASP != id)
                          .OrderBy(x => Guid.NewGuid())
                          .Take(4)
                          .ToList();

            ViewBag.dsCD = ds1;
            return View(sp);
        }

        // Partial view đánh giá
        public ActionResult PartialDanhGia(int masp)
        {
            // Lấy danh sách đánh giá của sản phẩm theo ngày mới nhất
            var danhGia = data.DANHGIAs
                              .Where(d => d.MASP == masp)
                              .OrderByDescending(d => d.NGAYDG)
                              .ToList();

            return PartialView("PartialDanhGia", danhGia); 
        }
        // Hiển thị danh sách yêu cầu của khách hàng
        public ActionResult HoTroKhachHang()
        {
            if (Session["MaKH"] == null)
                return RedirectToAction("DangNhap", "Account");

            int makh = (int)Session["MaKH"];

            var list = data.YEUCAUHOTROs
               .Where(x => x.MAKH == makh)
               .OrderByDescending(x => x.NGAYGUI)
               .ToList();  // OK


            return View(list);
        }

        // POST gửi yêu cầu mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HoTroKhachHang(string noiDungMoi)
        {
            if (Session["MaKH"] == null)
                return RedirectToAction("DangNhap", "Account");

            int makh = (int)Session["MaKH"];

            if (string.IsNullOrWhiteSpace(noiDungMoi))
            {
                TempData["Error"] = "Nội dung yêu cầu không được để trống.";
                return RedirectToAction("HoTroKhachHang");
            }

            var yeuCau = new YEUCAUHOTRO
            {
                MAYC_MAHOA = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                NGAYGUI = DateTime.Now,
                NOIDUNG = noiDungMoi,
                TRANGTHAI = "Chưa xử lý",
                MAKH = makh
            };

            data.YEUCAUHOTROs.InsertOnSubmit(yeuCau);
            data.SubmitChanges();


            TempData["Success"] = "Yêu cầu của bạn đã được gửi thành công!";
            return RedirectToAction("HoTroKhachHang");
        }

        public ActionResult GioiThieu()
        {
            return View();
        }

        public ActionResult BoSuuTap(string phanLoai, string search, int? categoryId, string price, int page = 1)
        {
            int pageSize = 15;

            // Lọc theo phanLoai
            var query = data.SANPHAMs.AsQueryable();
            if (!string.IsNullOrEmpty(phanLoai))
                query = query.Where(sp => sp.PHANLOAI == phanLoai);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(sp => sp.TENSP.ToLower().Contains(search.ToLower()));

            if (categoryId.HasValue)
                query = query.Where(sp => sp.MALOAI == categoryId);

            if (!string.IsNullOrEmpty(price))
            {
                switch (price)
                {
                    case "duoi-1": query = query.Where(sp => sp.GIABAN < 100000); break;
                    case "2-5": query = query.Where(sp => sp.GIABAN >= 200000 && sp.GIABAN <= 500000); break;
                    case "tren-15": query = query.Where(sp => sp.GIABAN > 500000); break;
                }
            }

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var products = query
                .OrderBy(sp => sp.TENSP)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Gán ViewBag giống SanPham
            ViewBag.Count = totalItems;
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Categories = new SelectList(data.LOAISANPHAMs.ToList(), "MALOAI", "TENLOAI");
            ViewBag.SelectedCategory = categoryId;
            ViewBag.CurrentPrice = price;
            ViewBag.PhanLoai = phanLoai;



           
            return View("SanPham", products);
        }

        // Hiển thị giỏ hàng
        public ActionResult GioHang()
        {
            GioHangSanPham gh = Session["ghSP"] as GioHangSanPham;
            if (gh == null) gh = new GioHangSanPham();
            return View(gh);
        }

        // Thêm sản phẩm vào giỏ

        public ActionResult ThemVaoGioHang(int id, int soLuong = 1, string size = "", string mau = "")
        {
            if (Session["UserName"] == null)
                return RedirectToAction("DangNhap", "Account");

            GioHangSanPham gh = Session["ghSP"] as GioHangSanPham;
            if (gh == null)
                gh = new GioHangSanPham();

            gh.Them(id, soLuong, mau, size);

            Session["ghSP"] = gh;
            Session["SoLuongGioHang"] = gh.TongSoLuong();

            return RedirectToAction("GioHang");
        }



        // Xóa sản phẩm khỏi giỏ
        public ActionResult XoaKhoiGio(int id)
        {
            GioHangSanPham gh = Session["ghSP"] as GioHangSanPham;
            if (gh != null)
            {
                gh.Xoa(id);
                Session["ghSP"] = gh;
                Session["SoLuongGioHang"] = gh.TongSoLuong();
            }
            return RedirectToAction("GioHang");
        }

        // Cập nhật số lượng
        [HttpPost]
        public ActionResult CapNhatGioHang(int id, int soLuong)
        {
            GioHangSanPham gh = Session["ghSP"] as GioHangSanPham;
            if (gh != null)
            {
                gh.CapNhat(id, soLuong);
                Session["ghSP"] = gh;
                Session["SoLuongGioHang"] = gh.TongSoLuong();
            }
            return RedirectToAction("GioHang");
        }

        // Thanh toán (GET)
        public ActionResult DatHang()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("DangNhap", "Account");

            GioHangSanPham gh = Session["ghSP"] as GioHangSanPham;
            if (gh == null || gh.lst.Count == 0)
                return RedirectToAction("GioHang");

            return View(gh);
        }

        // Thanh toán (POST)
        [HttpPost]
        public ActionResult DatHang(string HoTen, string Email, string SDT,
                               string DiaChi, string GhiChu, string payment)
        {
            // Kiểm tra đăng nhập
            if (Session["UserName"] == null)
                return RedirectToAction("DangNhap", "Account");

            // Lấy giỏ hàng
            GioHangSanPham gh = Session["ghSP"] as GioHangSanPham;
            if (gh == null || gh.lst.Count == 0)
                return RedirectToAction("GioHang");

            int makh = (int)Session["MaKH"];

            // Tạo mã hóa đơn duy nhất cho hiển thị
            string maHoaDon = "HD" + DateTime.Now.ToString("yyyyMMddHHmmss");

            // Tạo đơn hàng
            DONHANG dh = new DONHANG
            {
                NGAYDAT = DateTime.Now,
                TONGTIEN = gh.TongThanhTien(),
                MAKH = makh,
                TRANGTHAI = "Chờ xác nhận",
                MADH_MAHOA = maHoaDon
            };

            try
            {
                // Insert đơn hàng -> MADH sẽ được sinh
                data.DONHANGs.InsertOnSubmit(dh);
                data.SubmitChanges(); // MADH now populated

                // Insert chi tiết đơn hàng, set MADH foreign key
                foreach (var item in gh.lst)
                {
                    CHITIETDONHANG ct = new CHITIETDONHANG
                    {
                        MADH = dh.MADH,
                        MASP = item.vID_SP,
                        SOLUONG = item.vSoLuong,
                        DONGIA = item.vGiaBan
                    };
                    data.CHITIETDONHANGs.InsertOnSubmit(ct);
                }

                data.SubmitChanges();

                // Xóa giỏ hàng
                Session["ghSP"] = null;

                // Chuyển đến trang thành công
                return RedirectToAction("DatHangThanhCong", new { id = dh.MADH_MAHOA });
            }
            catch (Exception ex)
            {
                // Log or show error (use TempData to show to user)
                TempData["Error"] = "Lỗi khi lưu đơn hàng: " + ex.Message;
                // Optionally keep the cart and redirect back to checkout
                return RedirectToAction("ThanhToan");
            }
        }
        public ActionResult ThanhToan()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("DangNhap", "Account");

            GioHangSanPham gh = Session["ghSP"] as GioHangSanPham;
            if (gh == null || gh.lst.Count == 0)
                return RedirectToAction("GioHang");

            return View(gh);
        }
        // Trang thành công
        public ActionResult DatHangThanhCong(string id)
        {
            ViewBag.MaHoaDon = id;
            return View();
        }
        public ActionResult QuanLy()
        {
            // Giả sử bạn lưu UserId trong Session
            int userId = (int)Session["UserId"];
            var user = data.KHACHHANGs.FirstOrDefault(u => u.MAKH == userId);
            return View(user);
        }

        [HttpPost]
        public ActionResult QuanLy(KHACHHANG model)
        {
            if (ModelState.IsValid)
            {
                var user = data.KHACHHANGs.FirstOrDefault(u => u.MAKH == model.MAKH);
                if (user != null)
                {
                    user.HOTENKH = model.HOTENKH;
                    user.EMAIL_KH = model.EMAIL_KH;
                    user.SDT_KH = model.SDT_KH;
                    user.DIACHI_KH = model.DIACHI_KH;
                   data.SubmitChanges();
                    ViewBag.ThongBao = "Cập nhật thành công!";
                }
            }
            return View(model);
        }

        // GET: Theo dõi đơn hàng
        public ActionResult DonHang()
        {
            int userId = (int)Session["UserId"];
            var donHang = data.DONHANGs
                           .Where(d => d.MAKH == userId)
                           .OrderByDescending(d => d.NGAYDAT)
                           .ToList();
            return View(donHang);
        }

        // GET: Chi tiết đơn hàng

        public ActionResult ChiTietDonHang(int id)
        {
            // Lấy đơn hàng theo id
            var donHang = data.DONHANGs
                              .FirstOrDefault(d => d.MADH == id);

            if (donHang == null)
                return HttpNotFound();

            // Lấy chi tiết đơn hàng
            var chiTiet = data.CHITIETDONHANGs
                              .Where(ct => ct.MADH == donHang.MADH)
                              .ToList();

            // Gán chi tiết vào ViewBag hoặc tạo ViewModel
            ViewBag.ChiTietDonHang = chiTiet;

            return View(donHang);
        }

        [HttpPost]
        public ActionResult HuyDonHang(int id)
        {
            // Kiểm tra đăng nhập (nếu cần)
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            // Lấy đơn hàng từ database
            DONHANG donHang = data.DONHANGs.FirstOrDefault(d => d.MADH == id);

            if (donHang == null)
            {
                // Nếu không tìm thấy đơn, trả về trang lỗi hoặc quay lại danh sách đơn hàng
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("DanhSachDonHang"); // Thay bằng action danh sách đơn hàng của bạn
            }

            // Chỉ cho phép hủy khi đơn chưa xác nhận
            if (donHang.TRANGTHAI == "Chờ xác nhận")
            {
                donHang.TRANGTHAI = "Đã hủy";
                data.SubmitChanges();

                TempData["SuccessMessage"] = "Đơn hàng đã được hủy thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Đơn hàng không thể hủy vì đã được xác nhận hoặc đang xử lý.";
            }

            // Quay về trang chi tiết đơn hàng
            return RedirectToAction("ChiTietDonHang", new { id = id });
        }

    }
}