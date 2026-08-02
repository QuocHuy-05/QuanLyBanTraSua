using System;
using System.Linq;
using System.Web.Mvc;
using DACK_LTW_Nhom4.Filters;
using DACK_LTW_Nhom4.Models;

namespace DACK_LTW_Nhom4.Areas.Admin.Controllers
{
    [AdminRequired]
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            ViewBag.PageTitle = "Tong quan";
            ViewBag.ActiveMenu = "dashboard";

            using (var db = new QL_BanTraSuaEntities())
            {
                DateTime homNay = DateTime.Today;
                DateTime maiSau = homNay.AddDays(1);
                DateTime dauThang = new DateTime(homNay.Year, homNay.Month, 1);

                int donHangHomNay = db.DonHang.Count(x => x.NgayDat >= homNay && x.NgayDat < maiSau);
                decimal doanhThuThang = db.DonHang
                    .Where(x => x.NgayDat >= dauThang && x.TrangThai == "HoanThanh")
                    .Select(x => (decimal?)x.TongThanhToan)
                    .Sum() ?? 0m;
                int donChoXacNhan = db.DonHang.Count(x => x.TrangThai == "ChoXacNhan");
                int tongKhachHang = db.NguoiDung.Count(x => x.VaiTro == "KhachHang");

                var top5 = db.SanPham
                    .OrderByDescending(x => x.TongLuotMua)
                    .Take(5)
                    .ToList();

                ViewBag.DonHangHomNay = donHangHomNay;
                ViewBag.DoanhThuThang = doanhThuThang;
                ViewBag.DonChoXacNhan = donChoXacNhan;
                ViewBag.TongKhachHang = tongKhachHang;
                ViewBag.Top5 = top5;
            }

            return View();
        }
    }
}
