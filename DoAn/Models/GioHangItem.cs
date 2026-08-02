namespace DACK_LTW_Nhom4.Models
{
    /// Dai dien 1 dong san pham trong gio hang (luu qua Session).
    public class GioHangItem
    {
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public string HinhAnh { get; set; }

        public string Size { get; set; }

        public string MucDuong { get; set; }

        public string MucDa { get; set; }

        public string DanhSachTopping { get; set; }

        public decimal GiaTopping { get; set; }

        public decimal DonGia { get; set; }

        public int SoLuong { get; set; }

        public decimal ThanhTien
        {
            get { return (DonGia + GiaTopping) * SoLuong; }
        }
    }
}
