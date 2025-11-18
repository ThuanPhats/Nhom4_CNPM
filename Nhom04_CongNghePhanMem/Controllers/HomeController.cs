using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nhom04_CongNghePhanMem.Models;

namespace Nhom04_CongNghePhanMem.Controllers
{
    public class HomeController : Controller
    {
        QuanLiCuaHangDataContext data = new QuanLiCuaHangDataContext();
        public ActionResult Index()
        {
            var sp = data.SANPHAMs.ToList();
            return View(sp ?? new List<SANPHAM>());

        }




        // Sản phẩm hot
        public PartialViewResult HotProduct()
        {
            var hotProducts = data.SP_TOP10_BANCHAY().Take(4).ToList(); // gọi stored procedure SP_TOP10_BANCHAY
            return PartialView( hotProducts);        // truyền sang partial view
        }

        // Sản phẩm mới
        public PartialViewResult NewProduct()
        {
            var newProducts = data.SANPHAMs
                                 .OrderByDescending(sp => sp.MASP) // lấy sản phẩm mới nhất theo ID
                                 .Take(4)                          // chỉ lấy 5 cái
                                 .ToList();
            return PartialView( newProducts);
        }




    }
}