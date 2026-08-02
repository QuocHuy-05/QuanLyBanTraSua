using System;
using System.Linq;
using System.Web.Mvc;
using DACK_LTW_Nhom4.Helpers;
using DACK_LTW_Nhom4.Models;
using DACK_LTW_Nhom4.ViewModels;

namespace DACK_LTW_Nhom4.Controllers
{
    /// Xu ly dang nhap / dang ky / dang xuat.
    /// Dung Session de luu thong tin nguoi dung sau khi dang nhap.
    public class AccountController : Controller
    {
        // ===== DANG NHAP =====

        [HttpGet]
        public ActionResult DangNhap(string returnUrl)
        {
            if (Session["MaNguoiDung"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(new DangNhapVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(DangNhapVM model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            try
            {
                using (var db = new QL_BanTraSuaEntities())
                {
                    string matKhauHash = PasswordHelper.HashMD5(model.MatKhau);

                    var nd = db.NguoiDung.FirstOrDefault(
                        x => x.Email == model.Email && x.MatKhau == matKhauHash);

                    if (nd == null)
                    {
                        ModelState.AddModelError("", "Email hoac mat khau khong dung.");
                        ViewBag.ReturnUrl = returnUrl;
                        return View(model);
                    }

                    if (!nd.TrangThai)
                    {
                        ModelState.AddModelError("", "Tai khoan da bi khoa. Lien he Admin de mo lai.");
                        ViewBag.ReturnUrl = returnUrl;
                        return View(model);
                    }

                    // Luu Session
                    Session["MaNguoiDung"] = nd.MaNguoiDung;
                    Session["HoTen"] = nd.HoTen;
                    Session["VaiTro"] = nd.VaiTro;

                    TempData["ThongBao"] = "Dang nhap thanh cong. Xin chao " + nd.HoTen + "!";

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    if (nd.VaiTro == "Admin")
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ModelState.AddModelError("", "Co loi xay ra. Vui long thu lai.");
                return View(model);
            }
        }

        // ===== DANG KY =====

        [HttpGet]
        public ActionResult DangKy()
        {
            if (Session["MaNguoiDung"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new DangKyVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(DangKyVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                using (var db = new QL_BanTraSuaEntities())
                {
                    bool emailDaTonTai = db.NguoiDung.Any(x => x.Email == model.Email);
                    if (emailDaTonTai)
                    {
                        ModelState.AddModelError("Email", "Email nay da duoc dang ky.");
                        return View(model);
                    }

                    bool sdtDaTonTai = db.NguoiDung.Any(x => x.SoDienThoai == model.SoDienThoai);
                    if (sdtDaTonTai)
                    {
                        ModelState.AddModelError("SoDienThoai", "So dien thoai nay da duoc dang ky.");
                        return View(model);
                    }

                    var nd = new NguoiDung();
                    nd.HoTen = model.HoTen;
                    nd.Email = model.Email;
                    nd.SoDienThoai = model.SoDienThoai;
                    nd.DiaChi = model.DiaChi != null ? model.DiaChi : "";
                    nd.MatKhau = PasswordHelper.HashMD5(model.MatKhau);
                    nd.VaiTro = "KhachHang";
                    nd.TrangThai = true;
                    nd.NgayTao = DateTime.Now;

                    db.NguoiDung.Add(nd);
                    db.SaveChanges();

                    TempData["ThongBao"] = "Dang ky thanh cong. Vui long dang nhap.";
                    return RedirectToAction("DangNhap");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ModelState.AddModelError("", "Co loi xay ra khi dang ky. Vui long thu lai.");
                return View(model);
            }
        }

        // ===== DANG XUAT =====

        public ActionResult DangXuat()
        {
            Session.Clear();
            Session.Abandon();
            TempData["ThongBao"] = "Ban da dang xuat.";
            return RedirectToAction("Index", "Home");
        }
    }
}
