using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DACK_LTW_Nhom4.Filters;
using DACK_LTW_Nhom4.Models;

namespace DACK_LTW_Nhom4.Areas.Admin.Controllers
{
    [AdminRequired]
    public class SanPhamController : Controller
    {
        private static readonly string[] CHO_PHEP_EXT = new string[] { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MAX_KICH_THUOC = 2 * 1024 * 1024; // 2MB

        public ActionResult Index(int? maDanhMuc, string tuKhoa)
        {
            ViewBag.PageTitle = "Quan ly San pham";
            ViewBag.ActiveMenu = "sanpham";

            using (var db = new QL_BanTraSuaEntities())
            {
                var q = db.SanPham.Include("DanhMuc").AsQueryable();
                if (maDanhMuc.HasValue) q = q.Where(x => x.MaDanhMuc == maDanhMuc.Value);
                if (!string.IsNullOrWhiteSpace(tuKhoa))
                {
                    tuKhoa = tuKhoa.Trim();
                    q = q.Where(x => x.TenSanPham.Contains(tuKhoa));
                }
                var ds = q.OrderByDescending(x => x.MaSanPham).ToList();

                ViewBag.DanhMuc = db.DanhMuc.OrderBy(x => x.ThuTuHienThi).ToList();
                ViewBag.MaDanhMuc = maDanhMuc;
                ViewBag.TuKhoa = tuKhoa;
                return View(ds);
            }
        }

        public ActionResult Them()
        {
            ViewBag.PageTitle = "Them San pham";
            ViewBag.ActiveMenu = "sanpham";
            LoadDanhMucToppingViewBag();
            var sp = new SanPham();
            sp.TrangThai = true;
            sp.NgayTao = DateTime.Now;
            return View("Form", sp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Them(SanPham model, HttpPostedFileBase fileAnh, int[] toppingIds)
        {
            ViewBag.PageTitle = "Them San pham";
            ViewBag.ActiveMenu = "sanpham";

            if (string.IsNullOrWhiteSpace(model.TenSanPham))
            {
                ModelState.AddModelError("TenSanPham", "Vui long nhap ten san pham.");
            }

            string duongDanAnh = "";
            string loiUpload = LuuAnh(fileAnh, out duongDanAnh);
            if (loiUpload != null)
            {
                ModelState.AddModelError("HinhAnh", loiUpload);
            }

            if (!ModelState.IsValid)
            {
                LoadDanhMucToppingViewBag();
                ViewBag.ToppingChon = toppingIds;
                return View("Form", model);
            }

            try
            {
                using (var db = new QL_BanTraSuaEntities())
                {
                    model.HinhAnh = !string.IsNullOrEmpty(duongDanAnh) ? duongDanAnh : (model.HinhAnh != null ? model.HinhAnh : "");
                    model.NgayTao = DateTime.Now;

                    // Gan topping
                    if (toppingIds != null && toppingIds.Length > 0)
                    {
                        var dsTopping = db.Topping.Where(t => toppingIds.Contains(t.MaTopping)).ToList();
                        foreach (var tp in dsTopping)
                        {
                            model.Topping.Add(tp);
                        }
                    }

                    db.SanPham.Add(model);
                    db.SaveChanges();
                    TempData["ThongBao"] = "Da them san pham \"" + model.TenSanPham + "\".";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ModelState.AddModelError("", "Co loi khi luu.");
                LoadDanhMucToppingViewBag();
                ViewBag.ToppingChon = toppingIds;
                return View("Form", model);
            }
        }

        public ActionResult Sua(int id)
        {
            ViewBag.PageTitle = "Sua San pham";
            ViewBag.ActiveMenu = "sanpham";
            using (var db = new QL_BanTraSuaEntities())
            {
                var sp = db.SanPham.Include("Topping").FirstOrDefault(x => x.MaSanPham == id);
                if (sp == null) return HttpNotFound();
                LoadDanhMucToppingViewBag();
                ViewBag.ToppingChon = sp.Topping.Select(t => t.MaTopping).ToArray();
                return View("Form", sp);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sua(SanPham model, HttpPostedFileBase fileAnh, int[] toppingIds)
        {
            ViewBag.PageTitle = "Sua San pham";
            ViewBag.ActiveMenu = "sanpham";

            if (string.IsNullOrWhiteSpace(model.TenSanPham))
            {
                ModelState.AddModelError("TenSanPham", "Vui long nhap ten san pham.");
            }

            string duongDanAnhMoi = null;
            string loiUpload = LuuAnh(fileAnh, out duongDanAnhMoi);
            if (loiUpload != null)
            {
                ModelState.AddModelError("HinhAnh", loiUpload);
            }

            if (!ModelState.IsValid)
            {
                LoadDanhMucToppingViewBag();
                ViewBag.ToppingChon = toppingIds;
                return View("Form", model);
            }

            try
            {
                using (var db = new QL_BanTraSuaEntities())
                {
                    var sp = db.SanPham.Include("Topping").FirstOrDefault(x => x.MaSanPham == model.MaSanPham);
                    if (sp == null) return HttpNotFound();

                    sp.MaDanhMuc = model.MaDanhMuc;
                    sp.TenSanPham = model.TenSanPham;
                    sp.MoTa = model.MoTa != null ? model.MoTa : "";
                    sp.GiaS = model.GiaS;
                    sp.GiaM = model.GiaM;
                    sp.GiaL = model.GiaL;
                    sp.LaBanChay = model.LaBanChay;
                    sp.TrangThai = model.TrangThai;
                    if (!string.IsNullOrEmpty(duongDanAnhMoi))
                    {
                        sp.HinhAnh = duongDanAnhMoi;
                    }

                    // Cap nhat topping (xoa cu, them moi)
                    sp.Topping.Clear();
                    if (toppingIds != null && toppingIds.Length > 0)
                    {
                        var dsTopping = db.Topping.Where(t => toppingIds.Contains(t.MaTopping)).ToList();
                        foreach (var tp in dsTopping)
                        {
                            sp.Topping.Add(tp);
                        }
                    }

                    db.SaveChanges();
                    TempData["ThongBao"] = "Da cap nhat san pham.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ModelState.AddModelError("", "Co loi khi luu.");
                LoadDanhMucToppingViewBag();
                ViewBag.ToppingChon = toppingIds;
                return View("Form", model);
            }
        }

        [HttpPost]
        public JsonResult DoiTrangThai(int id)
        {
            using (var db = new QL_BanTraSuaEntities())
            {
                var sp = db.SanPham.FirstOrDefault(x => x.MaSanPham == id);
                if (sp == null) return Json(new { thanhCong = false });
                sp.TrangThai = !sp.TrangThai;
                db.SaveChanges();
                return Json(new { thanhCong = true, trangThai = sp.TrangThai });
            }
        }

        [HttpPost]
        public JsonResult DanhDauBanChay(int id)
        {
            using (var db = new QL_BanTraSuaEntities())
            {
                var sp = db.SanPham.FirstOrDefault(x => x.MaSanPham == id);
                if (sp == null) return Json(new { thanhCong = false });
                sp.LaBanChay = !sp.LaBanChay;
                db.SaveChanges();
                return Json(new { thanhCong = true, laBanChay = sp.LaBanChay });
            }
        }

        // ===== Helpers =====

        private void LoadDanhMucToppingViewBag()
        {
            using (var db = new QL_BanTraSuaEntities())
            {
                ViewBag.DanhMuc = db.DanhMuc.OrderBy(x => x.ThuTuHienThi).ToList();
                ViewBag.Topping = db.Topping.Where(x => x.TrangThai).OrderBy(x => x.TenTopping).ToList();
            }
        }

        /// <summary>Tra ve null neu OK, tra ve text loi neu sai. Set duongDan ra.</summary>
        private string LuuAnh(HttpPostedFileBase file, out string duongDan)
        {
            duongDan = null;
            if (file == null || file.ContentLength == 0) return null;

            if (file.ContentLength > MAX_KICH_THUOC)
            {
                return "Anh vuot qua 2MB.";
            }

            string ext = Path.GetExtension(file.FileName).ToLower();
            if (!CHO_PHEP_EXT.Contains(ext))
            {
                return "Chi cho phep dinh dang .jpg, .png, .webp.";
            }

            string tenFile = Guid.NewGuid().ToString("N") + ext;
            string thuMuc = Server.MapPath("~/Content/Images/Products/");
            if (!Directory.Exists(thuMuc)) Directory.CreateDirectory(thuMuc);
            string duongDanVatLy = Path.Combine(thuMuc, tenFile);
            file.SaveAs(duongDanVatLy);

            duongDan = "/Content/Images/Products/" + tenFile;
            return null;
        }
    }
}
