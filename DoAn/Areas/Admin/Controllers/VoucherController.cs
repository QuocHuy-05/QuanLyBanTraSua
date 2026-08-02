using System;
using System.Linq;
using System.Web.Mvc;
using DACK_LTW_Nhom4.Filters;
using DACK_LTW_Nhom4.Models;

namespace DACK_LTW_Nhom4.Areas.Admin.Controllers
{
    [AdminRequired]
    public class VoucherController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.PageTitle = "Quan ly Voucher";
            ViewBag.ActiveMenu = "voucher";
            using (var db = new QL_BanTraSuaEntities())
            {
                var ds = db.Voucher.OrderByDescending(x => x.NgayTao).ToList();
                return View(ds);
            }
        }

        public ActionResult Them()
        {
            ViewBag.PageTitle = "Them Voucher";
            ViewBag.ActiveMenu = "voucher";
            var vc = new Voucher();
            vc.MaCode = "VOUCHER" + DateTime.Now.ToString("yyMMddHHmm");
            vc.KieuGiam = "PhanTram";
            vc.GiaTri = 10;
            vc.DonHangToiThieu = 50000;
            vc.NgayBatDau = DateTime.Today;
            vc.NgayHetHan = DateTime.Today.AddDays(30);
            vc.TrangThai = "ConHieuLuc";
            return View("Form", vc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Them(Voucher model)
        {
            ViewBag.PageTitle = "Them Voucher";
            ViewBag.ActiveMenu = "voucher";
            Validate(model);
            if (!ModelState.IsValid)
            {
                return View("Form", model);
            }
            try
            {
                using (var db = new QL_BanTraSuaEntities())
                {
                    if (db.Voucher.Any(x => x.MaCode == model.MaCode))
                    {
                        ModelState.AddModelError("MaCode", "Ma code voucher da ton tai.");
                        return View("Form", model);
                    }
                    model.NgayTao = DateTime.Now;
                    model.DaDung = 0;
                    db.Voucher.Add(model);
                    db.SaveChanges();
                    TempData["ThongBao"] = "Da them voucher.";
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
            ViewBag.PageTitle = "Sua Voucher";
            ViewBag.ActiveMenu = "voucher";
            using (var db = new QL_BanTraSuaEntities())
            {
                var vc = db.Voucher.FirstOrDefault(x => x.MaVoucher == id);
                if (vc == null) return HttpNotFound();
                return View("Form", vc);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sua(Voucher model)
        {
            ViewBag.PageTitle = "Sua Voucher";
            ViewBag.ActiveMenu = "voucher";
            Validate(model);
            if (!ModelState.IsValid)
            {
                return View("Form", model);
            }
            try
            {
                using (var db = new QL_BanTraSuaEntities())
                {
                    if (db.Voucher.Any(x => x.MaCode == model.MaCode && x.MaVoucher != model.MaVoucher))
                    {
                        ModelState.AddModelError("MaCode", "Ma code voucher da ton tai.");
                        return View("Form", model);
                    }
                    var vc = db.Voucher.FirstOrDefault(x => x.MaVoucher == model.MaVoucher);
                    if (vc == null) return HttpNotFound();

                    vc.MaCode = model.MaCode;
                    vc.MoTa = model.MoTa;
                    vc.KieuGiam = model.KieuGiam;
                    vc.GiaTri = model.GiaTri;
                    vc.DonHangToiThieu = model.DonHangToiThieu;
                    vc.SoLuongPhatHanh = model.SoLuongPhatHanh;
                    vc.NgayBatDau = model.NgayBatDau;
                    vc.NgayHetHan = model.NgayHetHan;
                    vc.TrangThai = model.TrangThai;
                    db.SaveChanges();
                    TempData["ThongBao"] = "Da cap nhat voucher.";
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

        public ActionResult ChiTietSuDung(int id)
        {
            ViewBag.PageTitle = "Lich su su dung voucher";
            ViewBag.ActiveMenu = "voucher";
            using (var db = new QL_BanTraSuaEntities())
            {
                var vc = db.Voucher.FirstOrDefault(x => x.MaVoucher == id);
                if (vc == null) return HttpNotFound();
                var lichSu = db.LichSuVoucher
                    .Include("NguoiDung")
                    .Include("DonHang")
                    .Where(x => x.MaVoucher == id)
                    .OrderByDescending(x => x.NgayDung)
                    .ToList();
                ViewBag.Voucher = vc;
                return View(lichSu);
            }
        }

        private void Validate(Voucher m)
        {
            if (string.IsNullOrWhiteSpace(m.MaCode))
                ModelState.AddModelError("MaCode", "Vui long nhap ma code.");
            if (m.GiaTri < 0)
                ModelState.AddModelError("GiaTri", "Gia tri khong duoc am.");
            if (m.KieuGiam == "PhanTram" && m.GiaTri > 100)
                ModelState.AddModelError("GiaTri", "Phan tram khong duoc lon hon 100.");
            if (m.NgayHetHan < m.NgayBatDau)
                ModelState.AddModelError("NgayHetHan", "Ngay het han phai sau ngay bat dau.");
        }
    }
}
