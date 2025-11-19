using Nhom04_CongNghePhanMem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nhom04_CongNghePhanMem.Controllers
{
    public class AccountController : Controller
    {
        QuanLiCuaHangDataContext data =new QuanLiCuaHangDataContext();


        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult DangNhap(string loginInput, string MATKHAU)
        {
            if (string.IsNullOrWhiteSpace(loginInput) || string.IsNullOrWhiteSpace(MATKHAU))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            // Tìm tài khoản theo TENDANGNHAP hoặc EMAIL
            var tk = data.TAIKHOANs.FirstOrDefault(x =>
                        (x.TENDANGNHAP == loginInput || x.EMAIL == loginInput)
                        && x.MATKHAU == MATKHAU);

            if (tk == null)
            {
                ViewBag.Error = "Tên đăng nhập / Email hoặc mật khẩu không đúng!";
                return View();
            }

            // Lưu session
            Session["MaKH"] = tk.MATK;
            Session["UserName"] = tk.TENDANGNHAP;
            Session["UserRole"] = tk.QUYEN;

            // Phân quyền
            if (Session["UserRole"].ToString() == "QuanTri" || Session["UserRole"].ToString() == "NhanVien")
                return RedirectToAction("QuanTri", "Admin");

            return RedirectToAction("Index", "Home");
        }




        public ActionResult DangXuat()
        {

            Session.Remove("UserName");
            Session.Remove("UserRole");
            Session.Remove("TaiKhoan");
            Session.Remove("IDKhachHang");

            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(TAIKHOAN taiKhoan, KHACHHANG khachHang)
        {
            if (!ModelState.IsValid)
                return View(taiKhoan);

            // Kiểm tra tên đăng nhập tồn tại
            if (data.TAIKHOANs.Any(t => t.TENDANGNHAP == taiKhoan.TENDANGNHAP))
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                return View(taiKhoan);
            }

            // Gán role Khách hàng
            taiKhoan.QUYEN = "KhachHang";

            // Thêm TAIKHOAN và KHACHHANG
            data.TAIKHOANs.InsertOnSubmit(taiKhoan);
            khachHang.MATK = taiKhoan.MATK;
            data.KHACHHANGs.InsertOnSubmit(khachHang);

            data.SubmitChanges();

            return RedirectToAction("DangNhap");
        }
    }
}