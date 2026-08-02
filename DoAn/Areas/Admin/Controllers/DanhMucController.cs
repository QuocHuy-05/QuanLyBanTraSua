using System;
using System.Linq;
using System.Web.Mvc;
using DACK_LTW_Nhom4.Filters;
using DACK_LTW_Nhom4.Models;

namespace DACK_LTW_Nhom4.Areas.Admin.Controllers
{
    [AdminRequired]
    public class DanhMucController : Controller
    {
        public ActionResult Index(string tuKhoa)
        {
            ViewBag.PageTitle = "Quan ly Danh muc";
            ViewBag.ActiveMenu = "danhmuc";

            using (var db = new QL_BanTraSuaEntities())
            {
                var q = db.DanhMuc.AsQueryable();
                if (!string.IsNullOrWhiteSpace(tuKhoa))
                {
                    tuKhoa = tuKhoa.Trim();
                    q = q.Where(x => x.TenDanhMuc.Contains(tuKhoa));
                }
                var ds = q.OrderBy(x => x.ThuTuHienThi).ToList();
                ViewBag.TuKhoa = tuKhoa;
                return View(ds);
            }
        }

        public ActionResult Them()
        {
            ViewBag.PageTitle = "Them Danh muc";
            ViewBag.ActiveMenu = "danhmuc";
            return View("Form", new DanhMuc { TrangThai = true, ThuTuHienThi = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Them(DanhMuc model)
        {
            ViewBag.PageTitle = "Them Danh muc";
            ViewBag.ActiveMenu = "danhmuc";

            if (string.IsNullOrWhiteSpace(model.TenDanhMuc))
            {
                ModelState.AddModelError("TenDanhMuc", "Vui long nhap ten danh muc.");
            }

            try
            {
                using (var db = new QL_BanTraSuaEntities())
                {
                    if (!string.IsNullOrWhiteSpace(model.TenDanhMuc) &&
                        db.DanhMuc.Any(x => x.TenDanhMuc == model.TenDanhMuc))
                    {
                        ModelState.AddModelError("TenDanhMuc", "Ten danh muc nay da ton tai.");
                    }

                    if (!ModelState.IsValid)
                    {
                        return View("Form", model);
                    }

                    db.DanhMuc.Add(model);
                    db.SaveChanges();
                    TempData["ThongBao"] = "Da them danh muc \"" + model.TenDanhMuc + "\".";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ModelState.AddModelError("", "Co loi khi luu. Vui long thu lai.");
                return View("Form", model);
            }
        }

        public ActionResult Sua(int id)
        {
            ViewBag.PageTitle = "Sua Danh muc";
            ViewBag.ActiveMenu = "danhmuc";

            using (var db = new QL_BanTraSuaEntities())
            {
                var dm = db.DanhMuc.FirstOrDefault(x => x.MaDanhMuc == id);
                if (dm == null) return HttpNotFound();
                return View("Form", dm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sua(DanhMuc model)
        {
            ViewBag.PageTitle = "Sua Danh muc";
            ViewBag.ActiveMenu = "danhmuc";

            if (string.IsNullOrWhiteSpace(model.TenDanhMuc))
            {
                ModelState.AddModelError("TenDanhMuc", "Vui long nhap ten danh muc.");
                return View("Form", model);
            }

            try
            {
                using (var db = new QL_BanTraSuaEntities())
                {
                    bool truntTenKhac = db.DanhMuc.Any(x => x.TenDanhMuc == model.TenDanhMuc && x.MaDanhMuc != model.MaDanhMuc);
                    if (truntTenKhac)
                    {
                        ModelState.AddModelError("TenDanhMuc", "Ten danh muc nay da ton tai.");
                        return View("Form", model);
                    }

                    var dm = db.DanhMuc.FirstOrDefault(x => x.MaDanhMuc == model.MaDanhMuc);
                    if (dm == null) return HttpNotFound();

                    dm.TenDanhMuc = model.TenDanhMuc;
                    dm.HinhAnh = model.HinhAnh;
                    dm.ThuTuHienThi = model.ThuTuHienThi;
                    dm.TrangThai = model.TrangThai;
                    db.SaveChanges();

                    TempData["ThongBao"] = "Da cap nhat danh muc.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ModelState.AddModelError("", "Co loi khi luu.");
                return View("Form", model);
            }
        }

        [HttpPost]
        public JsonResult DoiTrangThai(int id)
        {
            using (var db = new QL_BanTraSuaEntities())
            {
                var dm = db.DanhMuc.FirstOrDefault(x => x.MaDanhMuc == id);
                if (dm == null) return Json(new { thanhCong = false });
                dm.TrangThai = !dm.TrangThai;
                db.SaveChanges();
                return Json(new { thanhCong = true, trangThai = dm.TrangThai });
            }
        }
    }
}
