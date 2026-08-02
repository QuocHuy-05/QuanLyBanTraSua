using System.ComponentModel.DataAnnotations;

namespace DACK_LTW_Nhom4.ViewModels
{
    /// <summary>
    /// ViewModel cho form Xac nhan don hang.
    /// </summary>
    public class CheckoutVM
    {
        [Required(ErrorMessage = "Vui long nhap ho ten nguoi nhan")]
        [StringLength(100)]
        [Display(Name = "Ten nguoi nhan")]
        public string TenNguoiNhan { get; set; }

        [Required(ErrorMessage = "Vui long nhap so dien thoai")]
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "So dien thoai khong hop le")]
        [Display(Name = "So dien thoai")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Vui long nhap dia chi giao hang")]
        [StringLength(500)]
        [Display(Name = "Dia chi giao hang")]
        public string DiaChiGiaoHang { get; set; }

        [Display(Name = "Ghi chu")]
        public string GhiChu { get; set; }
    }
}
