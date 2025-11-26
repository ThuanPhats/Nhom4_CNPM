using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nhom04_CongNghePhanMem.Models
{
    public class Register
    {
        // Thông tin tài khoản
        public string TENDANGNHAP { get; set; }
        public string MATKHAU { get; set; }
        public string XACNHANMATKHAU { get; set; }
        public string EMAIL { get; set; }

        // Thông tin khách hàng
        public string HOTENKH { get; set; }
        public string DIACHI { get; set; }
        public string SDT_KH { get; set; }
    }

}