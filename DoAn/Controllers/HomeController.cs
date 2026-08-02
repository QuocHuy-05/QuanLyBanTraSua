using System.Linq;
using System.Web.Mvc;
using DACK_LTW_Nhom4.Models;

namespace DACK_LTW_Nhom4.Controllers
{
    /// <summary>
    /// Trang chu — hien thi san pham ban chay va danh muc.
    /// </summary>
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var db = new QL_BanTraSuaEntities())
            {
                // San pham ban chay (toi da 8 san pham)
                var banChay = db.SanPham
                    .Where(x => x.TrangThai && x.LaBanChay)
                    .OrderByDescending(x => x.TongLuotMua)
                    .Take(8)
                    .ToList();

                // Danh muc dang hien thi
                var danhMuc = db.DanhMuc
                    .Where(x => x.TrangThai)
                    .OrderBy(x => x.ThuTuHienThi)
                    .ToList();

                ViewBag.SanPhamBanChay = banChay.Count > 0 ? (object)banChay : null;
                ViewBag.DanhMuc = danhMuc.Count > 0 ? (object)danhMuc : null;
            }

            return View();
        }
    }
}
