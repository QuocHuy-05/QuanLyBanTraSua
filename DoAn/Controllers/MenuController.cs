using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DACK_LTW_Nhom4.Models;

namespace DACK_LTW_Nhom4.Controllers
{
    /// <summary>
    /// Trang menu cho khach hang xem san pham.
    /// </summary>
    public class MenuController : Controller
    {
        // GET: /Menu
        public ActionResult Index()
        {
            using (var db = new QL_BanTraSuaEntities())
            {
                var danhMuc = db.DanhMuc
                    .Where(x => x.TrangThai)
                    .OrderBy(x => x.ThuTuHienThi)
                    .ToList();

                var sanPham = db.SanPham
                    .Include("DanhMuc")
                    .Where(x => x.TrangThai)
                    .OrderByDescending(x => x.LaBanChay)
                    .ThenByDescending(x => x.TongLuotMua)
                    .ToList();

                ViewBag.DanhMuc = danhMuc;
                return View(sanPham);
            }
        }

        // GET: /Menu/TheoLoai/3
        public ActionResult TheoLoai(int id)
        {
            using (var db = new QL_BanTraSuaEntities())
            {
                var dm = db.DanhMuc.FirstOrDefault(x => x.MaDanhMuc == id);
                if (dm == null || !dm.TrangThai)
                {
                    return HttpNotFound();
                }

                var sanPham = db.SanPham
                    .Where(x => x.MaDanhMuc == id && x.TrangThai)
                    .OrderByDescending(x => x.LaBanChay)
                    .ThenByDescending(x => x.TongLuotMua)
                    .ToList();

                ViewBag.DanhMucHienTai = dm;

                var dsDanhMuc = db.DanhMuc
                    .Where(x => x.TrangThai)
                    .OrderBy(x => x.ThuTuHienThi)
                    .ToList();
                ViewBag.DanhMuc = dsDanhMuc;

                return View(sanPham);
            }
        }

        // GET: /Menu/ChiTiet/12
        public ActionResult ChiTiet(int id)
        {
            using (var db = new QL_BanTraSuaEntities())
            {
                var sp = db.SanPham
                    .Include("DanhMuc")
                    .Include("Topping")
                    .FirstOrDefault(x => x.MaSanPham == id);

                if (sp == null || !sp.TrangThai)
                {
                    return HttpNotFound();
                }

                return View(sp);
            }
        }
    }
}
