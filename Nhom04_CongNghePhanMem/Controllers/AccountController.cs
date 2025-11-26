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

            // Phân quyền admin/nhân viên
            if (tk.QUYEN == "QuanTri" || tk.QUYEN == "NhanVien")
            {
                Session["UserId"] = tk.MATK;        // Dùng MATK của tài khoản admin
                Session["UserName"] = tk.TENDANGNHAP;
                Session["UserRole"] = tk.QUYEN;

                return RedirectToAction("Index", "Home"); // Vào dashboard admin
            }

            // Chỉ tìm khách hàng nếu là Khách hàng
            KHACHHANG kh = data.KHACHHANGs.FirstOrDefault(x => x.MATK == tk.MATK);

            if (kh == null)
            {
                ViewBag.Error = "Tài khoản chưa có thông tin khách hàng.";
                return View();
            }

            // Lưu session cho khách hàng
            Session["UserId"] = kh.MAKH;
            Session["UserName"] = kh.HOTENKH;
            Session["UserRole"] = tk.QUYEN;

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
            return View(new Register()); // gửi model trống
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult DangKy(Register model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (string.IsNullOrWhiteSpace(model.TENDANGNHAP))
            {
                ModelState.AddModelError("TENDANGNHAP", "Tên đăng nhập không được để trống");
                return View(model);
            }

            if (data.TAIKHOANs.Any(t => t.TENDANGNHAP == model.TENDANGNHAP))
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                return View(model);
            }

            // Tạo tài khoản và khách hàng
            TAIKHOAN taiKhoan = new TAIKHOAN
            {
                TENDANGNHAP = model.TENDANGNHAP,
                MATKHAU = model.MATKHAU,
                EMAIL = model.EMAIL,
                QUYEN = "KhachHang"
            };
            data.TAIKHOANs.InsertOnSubmit(taiKhoan);
            data.SubmitChanges();

            KHACHHANG khachHang = new KHACHHANG
            {
                MATK = taiKhoan.MATK,
                HOTENKH = model.HOTENKH,
                SDT_KH = model.SDT_KH,
                EMAIL_KH = model.EMAIL,
                DIACHI_KH = ""
            };
            data.KHACHHANGs.InsertOnSubmit(khachHang);
            data.SubmitChanges();

            // Gửi thông báo và chuyển view
            TempData["SuccessMessage"] = "Đăng ký thành công! Đang chuyển sang đăng nhập...";
            return RedirectToAction("DangNhap", "Account");
        }


    }
}