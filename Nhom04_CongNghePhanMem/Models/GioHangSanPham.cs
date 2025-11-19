using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nhom04_CongNghePhanMem.Models
{
    public class GioHangSanPham
    {
        public List<CartItemSanPham> lst;
        private QuanLiCuaHangDataContext data;

        public GioHangSanPham()
        {
            lst = new List<CartItemSanPham>();
            data = new QuanLiCuaHangDataContext();
        }

        public GioHangSanPham(List<CartItemSanPham> cartList, QuanLiCuaHangDataContext db)
        {
            lst = cartList;
            data = db;
        }

        // Tổng số lượng mặt hàng
        public int TongSoLuong()
        {
            return lst.Sum(n => n.vSoLuong);
        }

        // Tổng thành tiền
        public decimal TongThanhTien()
        {
            return lst.Sum(n => n.ThanhTien);
        }

        // Trong GioHangSanPham
        public void Them(int id, int soLuong, string mau, string size)
        {
            var sp = lst.FirstOrDefault(x => x.vID_SP == id && x.vSize == size && x.vMau == mau);
            if (sp != null)
            {
                sp.vSoLuong += soLuong;
            }
            else
            {
                var sanpham = data.SANPHAMs.FirstOrDefault(x => x.MASP == id);
                if (sanpham != null)
                {
                    lst.Add(new CartItemSanPham
                    {
                        vID_SP = id,
                        vTenSP = sanpham.TENSP,
                        vHinhAnh = sanpham.HINHANH,
                        vGiaBan = sanpham.GIABAN,
                        vSoLuong = soLuong,
                        vSize = size,
                        vMau = mau
                    });
                }
            }

        }

        // Cập nhật số lượng
        public void CapNhat(int vID_SP, int soLuong)
        {
            var item = lst.Find(n => n.vID_SP == vID_SP);
            if (item != null)
            {
                item.vSoLuong = (soLuong > 0) ? soLuong : 1;
            }
        }

        // Xóa sản phẩm
        public void Xoa(int vID_SP)
        {
            var item = lst.Find(n => n.vID_SP == vID_SP);
            if (item != null)
            {
                lst.Remove(item);
            }
        }

        // Lấy tổng số mặt hàng
        public int TongMatHang()
        {
            return lst.Count;
        }
    }
}