using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nhom04_CongNghePhanMem.Models
{
    public class DonHang_Admin
    {
        public DONHANG DonHang { get; set; }
        public List<CHITIETDONHANG> ChiTiet { get; set; }
    }

}