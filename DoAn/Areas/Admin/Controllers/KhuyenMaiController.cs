using System;
using System.Linq;
using System.Web.Mvc;
using DACK_LTW_Nhom4.Filters;
using DACK_LTW_Nhom4.Models;

namespace DACK_LTW_Nhom4.Areas.Admin.Controllers
{
    [AdminRequired]
    public class KhuyenMaiController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.PageTitle = "Quan ly Khuyen mai";
            ViewBag.ActiveMenu = "khuyenmai";
            using (var db = new QL_BanTraSuaEntities())
            {
                var ds = db.KhuyenMai.OrderByDescending(x => x.NgayTao).ToList();
                return View(ds);
            }
        }

        public ActionResult Them()
        {
            ViewBag.PageTitle = "Them Khuyen mai";
            ViewBag.ActiveMenu = "khuyenmai";
            LoadSanPhamViewBag(new int[0]);
            var km = new KhuyenMai();
            km.NgayBatDau = DateTime.Today;
            km.NgayKetThuc = DateTime.Today.AddDays(7);
            km.TrangThai = "DangChay";
            km.KieuGiam = "PhanTram";
            return View("Form", km);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Them(KhuyenMai model, int[] sanPhamIds)
        {
            ViewBag.PageTitle = "Them Khuyen mai";
            ViewBag.ActiveMenu = "khuyenmai";
            ValidateKhuyenMai(model);
            if (!ModelState.IsValid)
            {
                LoadSanPhamViewBag(sanPhamIds);
                return View("Form", model);
            }

            try
            {
                using (var db = new QL_BanTraSuaEntities())
                {
                    model.NgayTao = DateTime.Now;
                    if (sanPhamIds != null && sanPhamIds.Length > 0)
                    {
                        var dsSP = db.SanPham.Where(s => sanPhamIds.Contains(s.MaSanPham)).ToList();
                        foreach (var sp in dsSP) model.SanPham.Add(sp);
                    }
                    db.KhuyenMai.Add(model);
                    db.SaveChanges();
                    TempData["ThongBao"] = "Da them chuong trinh khuyen mai.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ModelState.AddModelError("", "Co loi khi luu.");
                LoadSanPhamViewBag(sanPhamIds);
                return View("Form", model);
            }
        }

        public ActionResult Sua(int id)
        {
            ViewBag.PageTitle = "Sua Khuyen mai";
            ViewBag.ActiveMenu = "khuyenmai";
            using (var db = new QL_BanTraSuaEntities())
            {
                var km = db.KhuyenMai.Include("SanPham").FirstOrDefault(x => x.MaKhuyenMai == id);
                if (km == null) return HttpNotFound();
                LoadSanPhamViewBag(km.SanPham.Select(s => s.MaSanPham).ToArray());
                return View("Form", km);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sua(KhuyenMai model, int[] sanPhamIds)
        {
            ViewBag.PageTitle = "Sua Khuyen mai";
            ViewBag.ActiveMenu = "khuyenmai";
            ValidateKhuyenMai(model);
            if (!ModelState.IsValid)
            {
                LoadSanPhamViewBag(sanPhamIds);
                return View("Form", model);
            }

            try
            {
                using (var db = new QL_BanTraSuaEntities())
                {
                    var km = db.KhuyenMai.Include("SanPham").FirstOrDefault(x => x.MaKhuyenMai == model.MaKhuyenMai);
                    if (km == null) return HttpNotFound();

                    km.TenChuongTrinh = model.TenChuongTrinh;
                    km.MoTa = model.MoTa != null ? model.MoTa : "";
                    km.KieuGiam = model.KieuGiam;
                    km.GiaTri = model.GiaTri;
                    km.NgayBatDau = model.NgayBatDau;
                    km.NgayKetThuc = model.NgayKetThuc;
                    km.TrangThai = model.TrangThai;

                    km.SanPham.Clear();
                    if (sanPhamIds != null && sanPhamIds.Length > 0)
                    {
                        var dsSP = db.SanPham.Where(s => sanPhamIds.Contains(s.MaSanPham)).ToList();
                        foreach (var sp in dsSP) km.SanPham.Add(sp);
                    }

                    db.SaveChanges();
                    TempData["ThongBao"] = "Da cap nhat khuyen mai.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ModelState.AddModelError("", "Co loi khi luu.");
                LoadSanPhamViewBag(sanPhamIds);
                return View("Form", model);
            }
        }

        [HttpPost]
        public JsonResult DoiTrangThai(int id, string trangThaiMoi)
        {
            using (var db = new QL_BanTraSuaEntities())
            {
                var km = db.KhuyenMai.FirstOrDefault(x => x.MaKhuyenMai == id);
                if (km == null) return Json(new { thanhCong = false });
                if (trangThaiMoi != "NhapNhay" && trangThaiMoi != "DangChay" && trangThaiMoi != "KetThuc")
                {
                    return Json(new { thanhCong = false, thongBao = "Trang thai khong hop le." });
                }
                km.TrangThai = trangThaiMoi;
                db.SaveChanges();
                return Json(new { thanhCong = true, trangThai = km.TrangThai });
            }
        }

        private void ValidateKhuyenMai(KhuyenMai m)
        {
            if (string.IsNullOrWhiteSpace(m.TenChuongTrinh))
                ModelState.AddModelError("TenChuongTrinh", "Vui long nhap ten chuong trinh.");
            if (m.GiaTri < 0)
                ModelState.AddModelError("GiaTri", "Gia tri khong duoc am.");
            if (m.KieuGiam == "PhanTram" && m.GiaTri > 100)
                ModelState.AddModelError("GiaTri", "Phan tram khong duoc lon hon 100.");
            if (m.NgayKetThuc < m.NgayBatDau)
                ModelState.AddModelError("NgayKetThuc", "Ngay ket thuc phai sau ngay bat dau.");
        }

        private void LoadSanPhamViewBag(int[] chon)
        {
            if (chon == null) chon = new int[0];
            using (var db = new QL_BanTraSuaEntities())
            {
                ViewBag.SanPham = db.SanPham.Where(x => x.TrangThai).OrderBy(x => x.TenSanPham).ToList();
                ViewBag.SanPhamChon = chon;
            }
        }
    }
}
