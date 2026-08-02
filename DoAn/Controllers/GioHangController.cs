using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DACK_LTW_Nhom4.Models;

namespace DACK_LTW_Nhom4.Controllers
{
    /// <summary>
    /// Gio hang luu vao Session["GioHang"] (List<GioHangItem>).
    /// Voucher (neu co) luu vao Session["MaVoucher"], Session["SoTienGiamVoucher"].
    /// </summary>
    public class GioHangController : Controller
    {
        // Helper: lay gio hang hien tai tu Session (tao moi neu chua co)
        private List<GioHangItem> LayGioHang()
        {
            var gio = Session["GioHang"] as List<GioHangItem>;
            if (gio == null)
            {
                gio = new List<GioHangItem>();
                Session["GioHang"] = gio;
            }
            return gio;
        }

        // GET: /GioHang/Xem
        public ActionResult Xem()
        {
            var gio = LayGioHang();
            ViewBag.MaVoucher = Session["MaVoucher"] != null ? Session["MaVoucher"].ToString() : "";
            ViewBag.SoTienGiamVoucher = Session["SoTienGiamVoucher"] != null ? (decimal)Session["SoTienGiamVoucher"] : 0m;
            return View(gio);
        }

        // POST: /GioHang/ThemVao
        // Form post tu trang ChiTiet san pham
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemVao(int maSanPham, string size, string mucDuong, string mucDa, int soLuong, int[] topping)
        {
            if (soLuong < 1) soLuong = 1;
            if (soLuong > 20) soLuong = 20;

            try
            {
                using (var db = new QL_BanTraSuaEntities())
                {
                    var sp = db.SanPham.FirstOrDefault(x => x.MaSanPham == maSanPham);
                    if (sp == null || !sp.TrangThai)
                    {
                        TempData["Loi"] = "San pham khong ton tai hoac da ngung kinh doanh.";
                        return RedirectToAction("Index", "Menu");
                    }

                    decimal donGia;
                    if (size == "L") donGia = sp.GiaL;
                    else if (size == "M") donGia = sp.GiaM;
                    else { size = "S"; donGia = sp.GiaS; }

                    decimal giaTopping = 0;
                    string tenTopping = "";
                    if (topping != null && topping.Length > 0)
                    {
                        var dsTopping = db.Topping
                            .Where(t => topping.Contains(t.MaTopping) && t.TrangThai)
                            .ToList();
                        giaTopping = dsTopping.Sum(t => t.GiaThem);
                        tenTopping = string.Join(", ", dsTopping.Select(t => t.TenTopping));
                    }

                    var item = new GioHangItem();
                    item.MaSanPham = sp.MaSanPham;
                    item.TenSanPham = sp.TenSanPham;
                    item.HinhAnh = sp.HinhAnh;
                    item.Size = size;
                    item.MucDuong = mucDuong;
                    item.MucDa = mucDa;
                    item.DanhSachTopping = tenTopping;
                    item.GiaTopping = giaTopping;
                    item.DonGia = donGia;
                    item.SoLuong = soLuong;

                    var gio = LayGioHang();
                    gio.Add(item);
                    Session["GioHang"] = gio;

                    // Voucher cu khong con phu hop thi xoa
                    XoaVoucherTrongSession();

                    TempData["ThongBao"] = "Da them \"" + sp.TenSanPham + "\" vao gio hang.";
                    return RedirectToAction("Xem");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                TempData["Loi"] = "Co loi khi them vao gio hang. Vui long thu lai.";
                return RedirectToAction("Index", "Menu");
            }
        }

        // POST: /GioHang/CapNhat
        [HttpPost]
        public JsonResult CapNhat(int index, int soLuong)
        {
            var gio = LayGioHang();
            if (index < 0 || index >= gio.Count)
            {
                return Json(new { thanhCong = false, thongBao = "Khong tim thay san pham trong gio." });
            }
            if (soLuong < 1) soLuong = 1;
            if (soLuong > 20) soLuong = 20;

            gio[index].SoLuong = soLuong;
            Session["GioHang"] = gio;

            XoaVoucherTrongSession();

            decimal tamTinh = gio.Sum(x => x.ThanhTien);
            return Json(new
            {
                thanhCong = true,
                thanhTien = gio[index].ThanhTien,
                tamTinh = tamTinh
            });
        }

        // POST: /GioHang/Xoa
        [HttpPost]
        public JsonResult Xoa(int index)
        {
            var gio = LayGioHang();
            if (index < 0 || index >= gio.Count)
            {
                return Json(new { thanhCong = false, thongBao = "Khong tim thay san pham." });
            }
            gio.RemoveAt(index);
            Session["GioHang"] = gio;

            XoaVoucherTrongSession();

            return Json(new
            {
                thanhCong = true,
                tamTinh = gio.Sum(x => x.ThanhTien),
                soLuongDong = gio.Count
            });
        }

        // POST: /GioHang/XoaToan
        [HttpPost]
        public ActionResult XoaToan()
        {
            Session["GioHang"] = new List<GioHangItem>();
            XoaVoucherTrongSession();
            TempData["ThongBao"] = "Da xoa toan bo gio hang.";
            return RedirectToAction("Xem");
        }

        // POST: /GioHang/ApVoucher
        [HttpPost]
        public JsonResult ApVoucher(string maCode)
        {
            if (string.IsNullOrWhiteSpace(maCode))
            {
                return Json(new { thanhCong = false, thongBao = "Vui long nhap ma voucher." });
            }
            maCode = maCode.Trim();

            var gio = LayGioHang();
            if (gio.Count == 0)
            {
                return Json(new { thanhCong = false, thongBao = "Gio hang dang trong." });
            }

            decimal tamTinh = gio.Sum(x => x.ThanhTien);

            try
            {
                using (var db = new QL_BanTraSuaEntities())
                {
                    var vc = db.Voucher.FirstOrDefault(v => v.MaCode == maCode);
                    if (vc == null)
                    {
                        return Json(new { thanhCong = false, thongBao = "Ma voucher khong ton tai." });
                    }
                    if (vc.TrangThai != "ConHieuLuc")
                    {
                        return Json(new { thanhCong = false, thongBao = "Voucher khong con hieu luc." });
                    }
                    DateTime now = DateTime.Now;
                    if (now < vc.NgayBatDau || now > vc.NgayHetHan)
                    {
                        return Json(new { thanhCong = false, thongBao = "Voucher het han hoac chua bat dau." });
                    }
                    if (vc.SoLuongPhatHanh.HasValue && vc.DaDung >= vc.SoLuongPhatHanh.Value)
                    {
                        return Json(new { thanhCong = false, thongBao = "Voucher da het luot su dung." });
                    }
                    if (tamTinh < vc.DonHangToiThieu)
                    {
                        return Json(new
                        {
                            thanhCong = false,
                            thongBao = "Don hang toi thieu " + string.Format("{0:N0}", vc.DonHangToiThieu) + " d de ap voucher nay."
                        });
                    }

                    decimal soTienGiam;
                    if (vc.KieuGiam == "PhanTram")
                    {
                        soTienGiam = tamTinh * (vc.GiaTri / 100m);
                    }
                    else
                    {
                        soTienGiam = vc.GiaTri;
                    }
                    if (soTienGiam > tamTinh) soTienGiam = tamTinh;

                    Session["MaVoucher"] = vc.MaCode;
                    Session["SoTienGiamVoucher"] = soTienGiam;

                    return Json(new
                    {
                        thanhCong = true,
                        soTienGiam = soTienGiam,
                        tongSauGiam = tamTinh - soTienGiam,
                        thongBao = "Ap voucher thanh cong. Giam " + string.Format("{0:N0}", soTienGiam) + " d."
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return Json(new { thanhCong = false, thongBao = "Co loi xay ra. Vui long thu lai." });
            }
        }

        // POST: /GioHang/BoVoucher
        [HttpPost]
        public JsonResult BoVoucher()
        {
            XoaVoucherTrongSession();
            return Json(new { thanhCong = true });
        }

        private void XoaVoucherTrongSession()
        {
            Session["MaVoucher"] = null;
            Session["SoTienGiamVoucher"] = null;
        }
    }
}
