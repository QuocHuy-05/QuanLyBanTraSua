using System.ComponentModel.DataAnnotations;

namespace DACK_LTW_Nhom4.ViewModels
{
    /// <summary>
    /// ViewModel cho form Dang nhap.
    /// Khong dung Entity NguoiDung vi chi can 2-3 truong + validation rieng.
    /// </summary>
    public class DangNhapVM
    {
        [Required(ErrorMessage = "Vui long nhap email")]
        [EmailAddress(ErrorMessage = "Email khong dung dinh dang")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui long nhap mat khau")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mat khau toi thieu 6 ky tu")]
        [Display(Name = "Mat khau")]
        public string MatKhau { get; set; }

        [Display(Name = "Ghi nho dang nhap")]
        public bool GhiNhoToi { get; set; }
    }
}
