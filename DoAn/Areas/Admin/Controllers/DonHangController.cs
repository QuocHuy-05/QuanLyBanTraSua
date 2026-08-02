using System;
using System.Linq;
using System.Web.Mvc;
using DACK_LTW_Nhom4.Filters;
using DACK_LTW_Nhom4.Models;

namespace DACK_LTW_Nhom4.Areas.Admin.Controllers
{
    [AdminRequired]
    public class DonHangController : Controller
    {
        public ActionResult Index(string trangThai, DateTime? tuNgay, DateTime? denNgay)
        {
            ViewBag.PageTitle = "Quan ly Don hang";
            ViewBag.ActiveMenu = "donhang";

            using (var db = new QL_BanTraSuaEntities())
            {
                var q = db.DonHang.Include("NguoiDung").AsQueryable();
                if (!string.IsNullOrEmpty(trangThai)) q = q.Where(x => x.TrangThai == trangThai);
                if (tuNgay.HasValue) q = q.Where(x => x.NgayDat >= tuNgay.Value);
                if (denNgay.HasValue)
                {
                    DateTime denCong1 = denNgay.Value.AddDays(1);
                    q = q.Where(x => x.NgayDat < denCong1);
                }
                var ds = q.OrderByDescending(x => x.NgayDat).ToList();
                ViewBag.TrangThai = trangThai;
                ViewBag.TuNgay = tuNgay;
                ViewBag.DenNgay = denNgay;
                return View(ds);
            }
        }

        public ActionResult ChiTiet(int id)
        {
            ViewBag.PageTitle = "Chi tiet don hang";
            ViewBag.ActiveMenu = "donhang";
            using (var db = new QL_BanTraSuaEntities())
            {
                var dh = db.DonHang
                    .Include("ChiTietDonHang")
                    .Include("NguoiDung")
                    .FirstOrDefault(x => x.MaDonHang == id);
                if (dh == null) return HttpNotFound();
                return View(dh);
            }
        }

        [HttpPost]
        public JsonResult DoiTrangThai(int id, string trangThai)
        {
            string[] hopLe = new string[] { "ChoXacNhan", "DangChuanBi", "DangGiao", "HoanThanh" };
            if (!hopLe.Contains(trangThai))
            {
                return Json(new { thanhCong = false, thongBao = "Trang thai khong hop le." });
            }
            try
            {
                using (var db = new QL_BanTraSuaEntities())
                {
                    var dh = db.DonHang.FirstOrDefault(x => x.MaDonHang == id);
                    if (dh == null) return Json(new { thanhCong = false });
                    if (dh.TrangThai == "DaHuy")
                    {
                        return Json(new { thanhCong = false, thongBao = "Don da huy, khong the chuyen." });
                    }
                    dh.TrangThai = trangThai;
                    dh.NgayCapNhat = DateTime.Now;
                    db.SaveChanges();
                    return Json(new { thanhCong = true, trangThai = dh.TrangThai });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return Json(new { thanhCong = false, thongBao = "Co loi xay ra." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Huy(int id, string lyDo)
        {
            if (string.IsNullOrWhiteSpace(lyDo))
            {
                TempData["Loi"] = "Vui long nhap ly do huy.";
                return RedirectToAction("ChiTiet", new { id = id });
            }
            try
            {
                using (var db = new QL_BanTraSuaEntities())
                {
                    var dh = db.DonHang.FirstOrDefault(x => x.MaDonHang == id);
                    if (dh == null) return HttpNotFound();
                    dh.TrangThai = "DaHuy";
                    dh.LyDoHuy = lyDo.Trim();
                    dh.NgayCapNhat = DateTime.Now;
                    db.SaveChanges();
                    TempData["ThongBao"] = "Da huy don #" + id + ".";
                    return RedirectToAction("ChiTiet", new { id = id });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                TempData["Loi"] = "Co loi khi huy don.";
                return RedirectToAction("ChiTiet", new { id = id });
            }
        }
    }
}
