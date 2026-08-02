using System;
using System.Linq;
using System.Web.Mvc;
using DACK_LTW_Nhom4.Filters;
using DACK_LTW_Nhom4.Models;

namespace DACK_LTW_Nhom4.Areas.Admin.Controllers
{
    [AdminRequired]
    public class ToppingController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.PageTitle = "Quan ly Topping";
            ViewBag.ActiveMenu = "topping";
            using (var db = new QL_BanTraSuaEntities())
            {
                var ds = db.Topping.OrderBy(x => x.TenTopping).ToList();
                return View(ds);
            }
        }

        public ActionResult Them()
        {
            ViewBag.PageTitle = "Them Topping";
            ViewBag.ActiveMenu = "topping";
            return View("Form", new Topping { TrangThai = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Them(Topping model)
        {
            ViewBag.PageTitle = "Them Topping";
            ViewBag.ActiveMenu = "topping";
            if (string.IsNullOrWhiteSpace(model.TenTopping))
            {
                ModelState.AddModelError("TenTopping", "Vui long nhap ten topping.");
                return View("Form", model);
            }
            try
            {
                using (var db = new QL_BanTraSuaEntities())
                {
                    if (db.Topping.Any(x => x.TenTopping == model.TenTopping))
                    {
                        ModelState.AddModelError("TenTopping", "Ten topping da ton tai.");
                        return View("Form", model);
                    }
                    db.Topping.Add(model);
                    db.SaveChanges();
                    TempData["ThongBao"] = "Da them topping.";
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

        public ActionResult Sua(int id)
        {
            ViewBag.PageTitle = "Sua Topping";
            ViewBag.ActiveMenu = "topping";
            using (var db = new QL_BanTraSuaEntities())
            {
                var tp = db.Topping.FirstOrDefault(x => x.MaTopping == id);
                if (tp == null) return HttpNotFound();
                return View("Form", tp);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sua(Topping model)
        {
            ViewBag.PageTitle = "Sua Topping";
            ViewBag.ActiveMenu = "topping";
            if (string.IsNullOrWhiteSpace(model.TenTopping))
            {
                ModelState.AddModelError("TenTopping", "Vui long nhap ten topping.");
                return View("Form", model);
            }
            try
            {
                using (var db = new QL_BanTraSuaEntities())
                {
                    if (db.Topping.Any(x => x.TenTopping == model.TenTopping && x.MaTopping != model.MaTopping))
                    {
                        ModelState.AddModelError("TenTopping", "Ten topping da ton tai.");
                        return View("Form", model);
                    }
                    var tp = db.Topping.FirstOrDefault(x => x.MaTopping == model.MaTopping);
                    if (tp == null) return HttpNotFound();
                    tp.TenTopping = model.TenTopping;
                    tp.GiaThem = model.GiaThem;
                    tp.TrangThai = model.TrangThai;
                    db.SaveChanges();
                    TempData["ThongBao"] = "Da cap nhat topping.";
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
                var tp = db.Topping.FirstOrDefault(x => x.MaTopping == id);
                if (tp == null) return Json(new { thanhCong = false });
                tp.TrangThai = !tp.TrangThai;
                db.SaveChanges();
                return Json(new { thanhCong = true, trangThai = tp.TrangThai });
            }
        }
    }
}
