using System.ComponentModel.DataAnnotations;

namespace DACK_LTW_Nhom4.ViewModels
{
    /// <summary>
    /// ViewModel cho form Dang ky tai khoan khach hang.
    /// </summary>
    public class DangKyVM
    {
        [Required(ErrorMessage = "Vui long nhap ho ten")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ho ten tu 2 den 100 ky tu")]
        [Display(Name = "Ho va ten")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Vui long nhap email")]
        [EmailAddress(ErrorMessage = "Email khong dung dinh dang")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui long nhap so dien thoai")]
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "So dien thoai khong hop le (vd: 0901234567)")]
        [Display(Name = "So dien thoai")]
        public string SoDienThoai { get; set; }

        [Display(Name = "Dia chi")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Vui long nhap mat khau")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mat khau toi thieu 6 ky tu")]
        [Display(Name = "Mat khau")]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Vui long xac nhan mat khau")]
        [DataType(DataType.Password)]
        [Compare("MatKhau", ErrorMessage = "Mat khau xac nhan khong trung khop")]
        [Display(Name = "Xac nhan mat khau")]
        public string XacNhanMatKhau { get; set; }
    }
}
