using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nhom04_CongNghePhanMem.Models
{
    public class CartItemSanPham
    {
        public int vID_SP { get; set; }
        public string vMaSP { get; set; }
        public string vTenSP { get; set; }
        public string vHinhAnh { get; set; }
        public decimal vGiaBan { get; set; }
        public int vSoLuong { get; set; }
        // Thêm màu và size
        public string vMau { get; set; }
        public string vSize { get; set; }


        // Tổng tiền = Giá * Số lượng
        public decimal ThanhTien
        {
            get { return vGiaBan * vSoLuong; }
        }

        // Constructor từ SANPHAM
        public CartItemSanPham(SANPHAM sp, string mau = "", string size = "")
        {
            vID_SP = sp.MASP;
            vMaSP = sp.MASP_MAHOA;
            vTenSP = sp.TENSP;
            vHinhAnh = sp.HINHANH;
            vGiaBan = sp.GIABAN;
            vSoLuong = 1;
            vMau = sp.MAUSAC;
            vSize=sp.KICHTHUOC;

        }

        public CartItemSanPham()
        {
           
        }

        
    }
    
}