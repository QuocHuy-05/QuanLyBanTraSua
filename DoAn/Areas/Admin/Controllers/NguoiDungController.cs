using System.Linq;
using System.Web.Mvc;
using DACK_LTW_Nhom4.Filters;
using DACK_LTW_Nhom4.Models;

namespace DACK_LTW_Nhom4.Areas.Admin.Controllers
{
    [AdminRequired]
    public class NguoiDungController : Controller
    {
        public ActionResult Index(string vaiTro, string tuKhoa)
        {
            ViewBag.PageTitle = "Quan ly Nguoi dung";
            ViewBag.ActiveMenu = "nguoidung";

            using (var db = new QL_BanTraSuaEntities())
            {
                var q = db.NguoiDung.AsQueryable();
                if (!string.IsNullOrEmpty(vaiTro)) q = q.Where(x => x.VaiTro == vaiTro);
                if (!string.IsNullOrWhiteSpace(tuKhoa))
                {
                    tuKhoa = tuKhoa.Trim();
                    q = q.Where(x => x.HoTen.Contains(tuKhoa) || x.Email.Contains(tuKhoa) || x.SoDienThoai.Contains(tuKhoa));
                }
                var ds = q.OrderByDescending(x => x.MaNguoiDung).ToList();
                ViewBag.VaiTro = vaiTro;
                ViewBag.TuKhoa = tuKhoa;
                return View(ds);
            }
        }

        [HttpPost]
        public JsonResult DoiTrangThai(int id)
        {
            using (var db = new QL_BanTraSuaEntities())
            {
                var nd = db.NguoiDung.FirstOrDefault(x => x.MaNguoiDung == id);
                if (nd == null) return Json(new { thanhCong = false });

                // Bao mat: khong cho khoa admin
                if (nd.VaiTro == "Admin")
                {
                    return Json(new { thanhCong = false, thongBao = "Khong the khoa tai khoan Admin." });
                }

                nd.TrangThai = !nd.TrangThai;
                db.SaveChanges();
                return Json(new { thanhCong = true, trangThai = nd.TrangThai });
            }
        }
    }
}
