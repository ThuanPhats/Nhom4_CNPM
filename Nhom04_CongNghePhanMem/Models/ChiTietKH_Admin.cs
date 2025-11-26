using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nhom04_CongNghePhanMem.Models
{
    public class ChiTietKH_Admin
    {
        public KHACHHANG KhachHang { get; set; }
        public List<DONHANG> DonHangs { get; set; }
    }
}