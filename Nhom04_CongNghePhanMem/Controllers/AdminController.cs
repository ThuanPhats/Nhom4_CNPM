using Nhom04_CongNghePhanMem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nhom04_CongNghePhanMem.Controllers
{
    public class AdminController : Controller
    {
        QuanLiCuaHangDataContext data = new QuanLiCuaHangDataContext();


        // GET: Admin
        // ================== HÀM CHECK QUYỀN ==================
        private ActionResult CheckAdmin()
        {
            if (Session["UserRole"] == null)
                return RedirectToAction("DangNhap", "Account");

            var role = Session["UserRole"].ToString();
            if (role != "QuanTri" && role != "NhanVien")
                return RedirectToAction("Index", "Home");

            return null; // hợp lệ
        }

        // ================== TRANG CHÍNH ADMIN ==================
        public ActionResult QuanTriAdmin()
        {
            var r = CheckAdmin();
            if (r != null) return r;

            // Tổng số sản phẩm
            var tongSanPham = data.SANPHAMs.Count();

            // Sản phẩm còn hàng
            var sanPhamConHang = data.SANPHAMs.Count(s => s.SOLUONGTON > 0);

            // Sản phẩm hết hàng
            var sanPhamHetHang = data.SANPHAMs.Count(s => s.SOLUONGTON == 0);

            // Top 5 sản phẩm có điểm trung bình cao nhất (DIEMTB)
            var topSanPham = data.SANPHAMs
                                .OrderByDescending(s => s.DIEMTB)
                                .Take(5)
                                .ToList();

            // Truyền dữ liệu sang View
            ViewBag.TongSanPham = tongSanPham;
            ViewBag.SanPhamConHang = sanPhamConHang;
            ViewBag.SanPhamHetHang = sanPhamHetHang;
            ViewBag.TopSanPham = topSanPham;

            return View();
        }
        public PartialViewResult Sidebar()
        {
            

            return PartialView();
        }

        // =====================================================================
        // 1. QUẢN LÝ SẢN PHẨM
        // =====================================================================

        // DANH SÁCH + TÌM KIẾM SẢN PHẨM
        public ActionResult QuanLySanPham(string search)
        {
            var r = CheckAdmin();
            if (r != null) return r;

            // 🔥 CHỈ LẤY SẢN PHẨM CHƯA BỊ ẨN
            var query = data.SANPHAMs
                            .Where(s => s.IsDeleted == false)
                            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                int maSo;
                if (int.TryParse(search, out maSo))
                {
                    query = query.Where(s => s.MASP == maSo);
                }
                else
                {
                    var upper = search.ToUpper();
                    if (upper.StartsWith("SP"))
                    {
                        var numPart = upper.Substring(2);
                        if (int.TryParse(numPart, out maSo))
                        {
                            query = query.Where(s => s.MASP == maSo);
                        }
                        else
                        {
                            query = query.Where(s => false);
                        }
                    }
                    else
                    {
                        query = query.Where(s => false);
                    }
                }
            }

            var lst = query.OrderBy(s => s.MASP).ToList();
            ViewBag.Search = search;
            return View(lst);
        }


        // GET: Thêm sản phẩm
        public ActionResult ThemSanPham()
        {
            var r = CheckAdmin();
            if (r != null) return r;

            ViewBag.MALOAI = new SelectList(
                data.LOAISANPHAMs.OrderBy(l => l.TENLOAI),
                "MALOAI", "TENLOAI"
            );
            ViewBag.MANCC = new SelectList(
                data.NHACUNGCAPs.OrderBy(n => n.TENNCC),
                "MANCC", "TENNCC"
            );
            return View();
        }

        // POST: Thêm sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemSanPham(SANPHAM model, HttpPostedFileBase fileAnh)
        {
            var r = CheckAdmin();
            if (r != null) return r;

            if (ModelState.IsValid)
            {
                // upload ảnh
                if (fileAnh != null && fileAnh.ContentLength > 0)
                {
                    var fileName = System.IO.Path.GetFileName(fileAnh.FileName);
                    var path = Server.MapPath("~/Content/Images/" + fileName);
                    fileAnh.SaveAs(path);
                    model.HINHANH = fileName;
                }
                else
                {
                    // nếu không chọn file thì gán ảnh mặc định (đảm bảo file này có trong thư mục)
                    model.HINHANH = "no-image.png";
                }

                data.SANPHAMs.InsertOnSubmit(model);
                data.SubmitChanges();

                TempData["Success"] = "Thêm sản phẩm thành công";
                return RedirectToAction("QuanLySanPham");
            }

            ViewBag.MALOAI = new SelectList(data.LOAISANPHAMs, "MALOAI", "TENLOAI", model.MALOAI);
            ViewBag.MANCC = new SelectList(data.NHACUNGCAPs, "MANCC", "TENNCC", model.MANCC);
            return View(model);
        }



        // GET: Sửa sản phẩm
        public ActionResult SuaSanPham(int id)
        {
            var r = CheckAdmin();
            if (r != null) return r;

            var sp = data.SANPHAMs.SingleOrDefault(s => s.MASP == id);
            if (sp == null) return HttpNotFound();

            ViewBag.MALOAI = new SelectList(
                data.LOAISANPHAMs, "MALOAI", "TENLOAI", sp.MALOAI
            );
            ViewBag.MANCC = new SelectList(
                data.NHACUNGCAPs, "MANCC", "TENNCC", sp.MANCC
            );

            return View(sp);
        }

        // POST: Sửa sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaSanPham(SANPHAM model, HttpPostedFileBase fileAnh, int SoLuongNhap = 0)
        {
            var r = CheckAdmin();
            if (r != null) return r;

            var sp = data.SANPHAMs.SingleOrDefault(s => s.MASP == model.MASP);
            if (sp == null) return HttpNotFound();

            if (ModelState.IsValid)
            {
                sp.TENSP = model.TENSP;
                sp.GIABAN = model.GIABAN;
                sp.MAUSAC = model.MAUSAC;
                sp.KICHTHUOC = model.KICHTHUOC;
                sp.MOTA = model.MOTA;
                sp.MALOAI = model.MALOAI;
                sp.MANCC = model.MANCC;
                sp.PHANLOAI = model.PHANLOAI;

                // ⭐ Tính số lượng tồn mới
                sp.SOLUONGTON = model.SOLUONGTON + SoLuongNhap;


                // Cập nhật ảnh
                if (fileAnh != null && fileAnh.ContentLength > 0)
                {
                    var fileName = System.IO.Path.GetFileName(fileAnh.FileName);
                    var path = Server.MapPath("~/Content/Images/" + fileName);
                    fileAnh.SaveAs(path);
                    sp.HINHANH = fileName;
                }

                data.SubmitChanges();
                TempData["Success"] = "Cập nhật sản phẩm thành công.";
                return RedirectToAction("QuanLySanPham");
            }

            ViewBag.MALOAI = new SelectList(data.LOAISANPHAMs, "MALOAI", "TENLOAI", model.MALOAI);
            ViewBag.MANCC = new SelectList(data.NHACUNGCAPs, "MANCC", "TENNCC", model.MANCC);

            return View(model);
        }


        // XÓA SẢN PHẨM
        public ActionResult XoaSanPham(int id)
        {
            var r = CheckAdmin();
            if (r != null) return r;

            var sp = data.SANPHAMs.SingleOrDefault(s => s.MASP == id);
            if (sp == null) return HttpNotFound();

            // Nếu có đơn => chỉ được ẩn
            bool daBan = data.CHITIETDONHANGs.Any(c => c.MASP == id);

            if (daBan)
            {
                // Chỉ ẩn
                sp.IsDeleted = true;
                data.SubmitChanges();
                TempData["Success"] = "Sản phẩm đã có đơn hàng → chỉ được ẩn.";
            }
            else
            {
                // Chưa bán lần nào => có thể xóa
                data.SANPHAMs.DeleteOnSubmit(sp);
                data.SubmitChanges();
                TempData["Success"] = "Xóa sản phẩm thành công.";
            }

            return RedirectToAction("QuanLySanPham");
        }




        // =====================================================================
        // 2. QUẢN LÝ KHÁCH HÀNG
        // =====================================================================

        // QUẢN LÝ KHÁCH HÀNG (DANH SÁCH + TÌM KIẾM)
        public ActionResult QuanLyKhachHang(string search)
        {
            var r = CheckAdmin();
            if (r != null) return r;

            // Lấy tất cả khách hàng
            var query = data.KHACHHANGs.AsQueryable();

            // Tìm kiếm theo: Họ tên / Email / SĐT
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(k => k.HOTENKH.Contains(search)
                                      || k.EMAIL_KH.Contains(search)
                                      || k.SDT_KH.Contains(search));
            }

            // Có thể sắp xếp theo mã KH hoặc ngày tạo
            var lst = query.OrderBy(k => k.MAKH).ToList();

            return View(lst);   // View: QuanLyKhachHang.cshtml (IEnumerable<KHACHHANG>)
        }

        // XEM CHI TIẾT 1 KHÁCH HÀNG + CÁC ĐƠN HÀNG CỦA HỌ
        public ActionResult ChiTietKhachHang(int id)
        {
            var r = CheckAdmin();
            if (r != null) return r;

            var kh = data.KHACHHANGs.SingleOrDefault(k => k.MAKH == id);
            if (kh == null) return HttpNotFound();

            var donHangs = data.DONHANGs
                               .Where(d => d.MAKH == id)
                               .OrderByDescending(d => d.NGAYDAT)
                               .ToList();

            var vm = new ChiTietKH_Admin
            {
                KhachHang = kh,
                DonHangs = donHangs
            };

            return View(vm);   // View: ChiTietKhachHang.cshtml
        }







        // Có thể thêm khóa/mở tài khoản nếu muốn

        // =====================================================================
        // 3. QUẢN LÝ ĐƠN HÀNG
        // =====================================================================

        // QUẢN LÝ ĐƠN HÀNG
        public ActionResult QuanLyDonHang(string trangThai, string maDon)
        {
            var r = CheckAdmin();
            if (r != null) return r;

            var query = data.DONHANGs.AsQueryable();

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(d => d.TRANGTHAI == trangThai);
            }

            // Tìm theo mã đơn (Mã hiển thị hoặc MADH)
            if (!string.IsNullOrEmpty(maDon))
            {
                query = query.Where(d =>
                    (d.MADH_MAHOA ?? d.MADH.ToString()).Contains(maDon));
            }

            var lst = query
                .OrderByDescending(d => d.NGAYDAT)
                .ToList();

            return View(lst);
        }


        // XEM CHI TIẾT ĐƠN HÀNG
        public ActionResult ChiTietDonHangAdmin(int id)
        {
            var r = CheckAdmin();
            if (r != null) return r;

            var don = data.DONHANGs.SingleOrDefault(d => d.MADH == id);
            if (don == null) return HttpNotFound();

            var ctdh = data.CHITIETDONHANGs
                          .Where(c => c.MADH == id)
                          .ToList();

            var vm = new DonHang_Admin
            {
                DonHang = don,
                ChiTiet = ctdh
            };

            return View(vm);
        }

        // CẬP NHẬT TRẠNG THÁI ĐƠN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatTrangThaiDon(int id, string trangThai)
        {
            var r = CheckAdmin();
            if (r != null) return r;

            if (string.IsNullOrEmpty(trangThai))
            {
                TempData["Error"] = "Vui lòng chọn trạng thái đơn hàng.";
                return RedirectToAction("ChiTietDonHangAdmin", new { id = id });
            }

            var don = data.DONHANGs.SingleOrDefault(d => d.MADH == id);
            if (don == null) return HttpNotFound();

            // (Nếu muốn kiểm soát luồng nghiệp vụ, có thể check chuyển trạng thái hợp lệ ở đây)
            don.TRANGTHAI = trangThai;

            data.SubmitChanges();

            TempData["Success"] = "Cập nhật trạng thái đơn hàng thành công.";
            return RedirectToAction("ChiTietDonHangAdmin", new { id = id });
        }


        // =====================================================================
        // 4. QUẢN LÝ DANH MỤC (LOAISANPHAM)
        // =====================================================================

        public ActionResult QuanLyDanhMuc(string search)
        {
            var ds = data.LOAISANPHAMs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim(); // bỏ khoảng trắng đầu/cuối

                // không phân biệt hoa/thường
                ds = ds.Where(x => x.TENLOAI.ToLower().Contains(search.ToLower()));
            }

            ds = ds.OrderBy(x => x.TENLOAI);

            ViewBag.Search = search; // để hiển thị lại trên ô tìm kiếm

            return View(ds.ToList());
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemDanhMuc(LOAISANPHAM model)
        {
            var r = CheckAdmin();
            if (r != null) return r;

            if (ModelState.IsValid)
            {
                data.LOAISANPHAMs.InsertOnSubmit(model);
                data.SubmitChanges();
                TempData["Success"] = "Thêm danh mục thành công.";
            }
            return RedirectToAction("QuanLyDanhMuc");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaDanhMuc(LOAISANPHAM model)
        {
            var r = CheckAdmin();
            if (r != null) return r;

            var dm = data.LOAISANPHAMs.SingleOrDefault(l => l.MALOAI == model.MALOAI);
            if (dm == null) return HttpNotFound();

            dm.TENLOAI = model.TENLOAI;
            data.SubmitChanges();
            TempData["Success"] = "Cập nhật danh mục thành công.";
            return RedirectToAction("QuanLyDanhMuc");
        }

        public ActionResult XoaDanhMuc(int id)
        {
            var r = CheckAdmin();
            if (r != null) return r;

            var dm = data.LOAISANPHAMs.SingleOrDefault(l => l.MALOAI == id);
            if (dm == null) return HttpNotFound();

            bool coSanPham = data.SANPHAMs.Any(s => s.MALOAI == id);
            if (coSanPham)
            {
                TempData["Error"] = "Danh mục đang có sản phẩm, không thể xóa.";
            }
            else
            {
                data.LOAISANPHAMs.DeleteOnSubmit(dm);
                data.SubmitChanges();
                TempData["Success"] = "Xóa danh mục thành công.";
            }
            return RedirectToAction("QuanLyDanhMuc");
        }

        // =====================================================================
        // 5. BÁO CÁO – THỐNG KÊ
        // =====================================================================

        public ActionResult BaoCaoThongKe(DateTime? tuNgay, DateTime? denNgay)
        {
            var r = CheckAdmin();
            if (r != null) return r;

            // Nếu không nhập, lấy mặc định
            if (!tuNgay.HasValue)
                tuNgay = data.DONHANGs.Min(d => d.NGAYDAT);

            if (!denNgay.HasValue)
                denNgay = DateTime.Today;

            // NGĂN NGƯỜI DÙNG NHẬP NGÀY LỚN HƠN HÔM NAY
            if (denNgay > DateTime.Today)
                denNgay = DateTime.Today;

            // Lấy đơn hàng trong khoảng
            var donTheoNgay = data.DONHANGs
                .Where(d => d.NGAYDAT >= tuNgay.Value && d.NGAYDAT <= denNgay.Value);

            var model = new ThongKe_Admin();
            model.TuNgay = tuNgay.Value;
            model.DenNgay = denNgay.Value;

            model.TongDonHang = donTheoNgay.Count();
            model.TongDoanhThu = donTheoNgay.Sum(d => d.TONGTIEN);

            model.DonHangChoXacNhan = donTheoNgay.Count(d => d.TRANGTHAI == "CHỜ XÁC NHẬN");
            model.DonHangDangGiao = donTheoNgay.Count(d => d.TRANGTHAI.Contains("ĐANG GIAO"));
            model.DonHangDaGiao = donTheoNgay.Count(d => d.TRANGTHAI.Contains("ĐÃ GIAO"));
            model.DonHangDaHuy = donTheoNgay.Count(d => d.TRANGTHAI.Contains("HỦY"));

            // Top 10 sản phẩm bán chạy
            model.TopSanPhamBanChay = (from sp in data.SANPHAMs
                                       join ctdh in data.CHITIETDONHANGs on sp.MASP equals ctdh.MASP
                                       join dh in donTheoNgay on ctdh.MADH equals dh.MADH
                                       group new { sp, ctdh } by new { sp.MASP, sp.TENSP } into g
                                       orderby g.Sum(x => x.ctdh.SOLUONG) descending
                                       select new TopSanPham
                                       {
                                           TenSanPham = g.Key.TENSP,
                                           TongSoLuongBan = g.Sum(x => x.ctdh.SOLUONG),
                                           TongDoanhThu = g.Sum(x => x.ctdh.THANHTIEN)
                                       }).Take(10).ToList();

            return View(model);
        }


        // Xem danh sách yêu cầu
        public ActionResult YeuCauHotro()
        {
            var list = data.YEUCAUHOTROs
                           .OrderByDescending(x => x.NGAYGUI)
                           .ToList();
            return View(list);
        }

        // Trả lời yêu cầu
        [HttpPost]
        public ActionResult TraLoiHotro(int idYC, string phanHoi)
        {
            var yc = data.YEUCAUHOTROs.FirstOrDefault(x => x.MAYC == idYC);
            if (yc != null)
            {
                yc.PHANHOI = phanHoi;
                yc.TRANGTHAI = "Đã xử lý";
                data.SubmitChanges();
            }
            return RedirectToAction("YeuCauHotro");
        }
    }
}