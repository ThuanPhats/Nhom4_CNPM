using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nhom04_CongNghePhanMem.Models
{
    public class ThongKe_Admin
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }

        public int TongDonHang { get; set; }
        public decimal TongDoanhThu { get; set; }

        public int DonHangChoXacNhan { get; set; }
        public int DonHangDangGiao { get; set; }
        public int DonHangDaGiao { get; set; }
        public int DonHangDaHuy { get; set; }

        public List<TopSanPham> TopSanPhamBanChay { get; set; }
    }
}