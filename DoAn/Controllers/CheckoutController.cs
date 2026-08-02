using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DACK_LTW_Nhom4.Filters;
using DACK_LTW_Nhom4.Models;
using DACK_LTW_Nhom4.ViewModels;

namespace DACK_LTW_Nhom4.Controllers
{
    /// Xac nhan & dat hang. Bat buoc dang nhap.
    [DangNhapRequired]
    public class CheckoutController : Controller
    {
        // GET: /Checkout/XacNhan
        public ActionResult XacNhan()
        {
            var gio = Session["GioHang"] as List<GioHangItem>;
            if (gio == null || gio.Count == 0)
            {
                TempData["Loi"] = "Gio hang dang trong.";
                return RedirectToAction("Index", "Menu");
            }

            int maNguoiDung = (int)Session["MaNguoiDung"];
            var vm = new CheckoutVM();

            using (var db = new QL_BanTraSuaEntities())
            {
                var nd = db.NguoiDung.FirstOrDefault(x => x.MaNguoiDung == maNguoiDung);
                if (nd != null)
                {
                    vm.TenNguoiNhan = nd.HoTen;
                    vm.SoDienThoai = nd.SoDienThoai;
                    vm.DiaChiGiaoHang = nd.DiaChi;
                }
            }

            decimal tamTinh = gio.Sum(x => x.ThanhTien);
            decimal soTienGiam = Session["SoTienGiamVoucher"] != null ? (decimal)Session["SoTienGiamVoucher"] : 0m;
            decimal tongCong = tamTinh - soTienGiam;
            if (tongCong < 0) tongCong = 0;

            ViewBag.GioHang = gio;
            ViewBag.TamTinh = tamTinh;
            ViewBag.SoTienGiam = soTienGiam;
            ViewBag.TongCong = tongCong;
            ViewBag.MaVoucher = Session["MaVoucher"] != null ? Session["MaVoucher"].ToString() : "";

            return View(vm);
        }

        // POST: /Checkout/DatHang
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DatHang(CheckoutVM model)
        {
            var gio = Session["GioHang"] as List<GioHangItem>;
            if (gio == null || gio.Count == 0)
            {
                TempData["Loi"] = "Gio hang dang trong.";
                return RedirectToAction("Index", "Menu");
            }

            if (!ModelState.IsValid)
            {
                decimal tt = gio.Sum(x => x.ThanhTien);
                decimal stg = Session["SoTienGiamVoucher"] != null ? (decimal)Session["SoTienGiamVoucher"] : 0m;
                decimal tc = tt - stg;
                if (tc < 0) tc = 0;
                ViewBag.GioHang = gio;
                ViewBag.TamTinh = tt;
                ViewBag.SoTienGiam = stg;
                ViewBag.TongCong = tc;
                ViewBag.MaVoucher = Session["MaVoucher"] != null ? Session["MaVoucher"].ToString() : "";
                return View("XacNhan", model);
            }

            int maNguoiDung = (int)Session["MaNguoiDung"];
            decimal tamTinh = gio.Sum(x => x.ThanhTien);
            decimal soTienGiam = Session["SoTienGiamVoucher"] != null ? (decimal)Session["SoTienGiamVoucher"] : 0m;
            string maVoucher = Session["MaVoucher"] != null ? Session["MaVoucher"].ToString() : null;

            try
            {
                using (var db = new QL_BanTraSuaEntities())
                {
                    var dh = new DonHang();
                    dh.MaNguoiDung = maNguoiDung;
                    dh.TenNguoiNhan = model.TenNguoiNhan;
                    dh.SoDienThoai = model.SoDienThoai;
                    dh.DiaChiGiaoHang = model.DiaChiGiaoHang;
                    dh.GhiChu = model.GhiChu != null ? model.GhiChu : "";
                    dh.TamTinh = tamTinh;
                    dh.GiamKhuyenMai = 0;
                    dh.GiamVoucher = soTienGiam;
                    dh.TongThanhToan = tamTinh - soTienGiam;
                    if (dh.TongThanhToan < 0) dh.TongThanhToan = 0;
                    dh.MaVoucherDaDung = maVoucher;
                    dh.TrangThai = "ChoXacNhan";
                    dh.LyDoHuy = null;
                    dh.NgayDat = DateTime.Now;
                    dh.NgayCapNhat = DateTime.Now;

                    db.DonHang.Add(dh);
                    db.SaveChanges();

                    // Them ChiTietDonHang
                    foreach (var item in gio)
                    {
                        var ct = new ChiTietDonHang();
                        ct.MaDonHang = dh.MaDonHang;
                        ct.MaSanPham = item.MaSanPham;
                        ct.TenSanPham = item.TenSanPham;
                        ct.Size = item.Size;
                        ct.MucDuong = item.MucDuong;
                        ct.MucDa = item.MucDa;
                        ct.DanhSachTopping = item.DanhSachTopping != null ? item.DanhSachTopping : "";
                        ct.GiaTopping = item.GiaTopping;
                        ct.DonGia = item.DonGia;
                        ct.SoLuong = item.SoLuong;
                        ct.ThanhTien = item.ThanhTien;
                        db.ChiTietDonHang.Add(ct);

                        // Tang luot mua cho san pham
                        var sp = db.SanPham.FirstOrDefault(x => x.MaSanPham == item.MaSanPham);
                        if (sp != null)
                        {
                            sp.TongLuotMua = sp.TongLuotMua + item.SoLuong;
                        }
                    }

                    // Luu voucher su dung
                    if (!string.IsNullOrEmpty(maVoucher) && soTienGiam > 0)
                    {
                        var vc = db.Voucher.FirstOrDefault(x => x.MaCode == maVoucher);
                        if (vc != null)
                        {
                            var lsv = new LichSuVoucher();
                            lsv.MaVoucher = vc.MaVoucher;
                            lsv.MaDonHang = dh.MaDonHang;
                            lsv.MaNguoiDung = maNguoiDung;
                            lsv.SoTienGiam = soTienGiam;
                            lsv.NgayDung = DateTime.Now;
                            db.LichSuVoucher.Add(lsv);

                            vc.DaDung = vc.DaDung + 1;
                        }
                    }

                    db.SaveChanges();

                    // Xoa gio hang & voucher trong session
                    Session["GioHang"] = new List<GioHangItem>();
                    Session["MaVoucher"] = null;
                    Session["SoTienGiamVoucher"] = null;

                    return RedirectToAction("ThanhCong", new { id = dh.MaDonHang });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                TempData["Loi"] = "Co loi khi dat hang. Vui long thu lai.";
                return RedirectToAction("XacNhan");
            }
        }

        // GET: /Checkout/ThanhCong/12
        public ActionResult ThanhCong(int id)
        {
            using (var db = new QL_BanTraSuaEntities())
            {
                int maNguoiDung = (int)Session["MaNguoiDung"];
                var dh = db.DonHang
                    .Include("ChiTietDonHang")
                    .FirstOrDefault(x => x.MaDonHang == id && x.MaNguoiDung == maNguoiDung);

                if (dh == null)
                {
                    return HttpNotFound();
                }
                return View(dh);
            }
        }
    }
}
