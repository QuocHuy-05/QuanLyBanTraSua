using System;
using System.Linq;
using System.Web.Mvc;
using DACK_LTW_Nhom4.Filters;
using DACK_LTW_Nhom4.Models;

namespace DACK_LTW_Nhom4.Controllers
{
    /// <summary>
    /// Trang khach hang xem don hang cua minh.
    /// </summary>
    [DangNhapRequired]
    public class DonHangController : Controller
    {
        // GET: /DonHang/LichSu
        public ActionResult LichSu()
        {
            int maNguoiDung = (int)Session["MaNguoiDung"];
            using (var db = new QL_BanTraSuaEntities())
            {
                var ds = db.DonHang
                    .Where(x => x.MaNguoiDung == maNguoiDung)
                    .OrderByDescending(x => x.NgayDat)
                    .ToList();
                return View(ds);
            }
        }

        // GET: /DonHang/ChiTiet/12
        public ActionResult ChiTiet(int id)
        {
            int maNguoiDung = (int)Session["MaNguoiDung"];
            using (var db = new QL_BanTraSuaEntities())
            {
                var dh = db.DonHang
                    .Include("ChiTietDonHang")
                    .FirstOrDefault(x => x.MaDonHang == id);

                if (dh == null) return HttpNotFound();

                // Bao mat: chi cho xem don cua minh
                if (dh.MaNguoiDung != maNguoiDung)
                {
                    return new HttpStatusCodeResult(403, "Khong co quyen xem don hang nay.");
                }

                return View(dh);
            }
        }

        // POST: /DonHang/Huy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Huy(int id, string lyDo)
        {
            int maNguoiDung = (int)Session["MaNguoiDung"];
            try
            {
                using (var db = new QL_BanTraSuaEntities())
                {
                    var dh = db.DonHang.FirstOrDefault(x => x.MaDonHang == id);
                    if (dh == null) return HttpNotFound();
                    if (dh.MaNguoiDung != maNguoiDung)
                    {
                        TempData["Loi"] = "Ban khong co quyen huy don nay.";
                        return RedirectToAction("LichSu");
                    }
                    if (dh.TrangThai != "ChoXacNhan")
                    {
                        TempData["Loi"] = "Chi co the huy don dang cho xac nhan.";
                        return RedirectToAction("ChiTiet", new { id = id });
                    }

                    dh.TrangThai = "DaHuy";
                    dh.LyDoHuy = !string.IsNullOrWhiteSpace(lyDo) ? lyDo.Trim() : "Khach hang tu huy";
                    dh.NgayCapNhat = DateTime.Now;
                    db.SaveChanges();

                    TempData["ThongBao"] = "Da huy don hang #" + id + ".";
                    return RedirectToAction("LichSu");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                TempData["Loi"] = "Co loi khi huy don. Vui long thu lai.";
                return RedirectToAction("ChiTiet", new { id = id });
            }
        }
    }
}
