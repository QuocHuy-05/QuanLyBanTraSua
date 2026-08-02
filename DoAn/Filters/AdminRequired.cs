using System.Web.Mvc;
using System.Web.Routing;

namespace DACK_LTW_Nhom4.Filters
{
    /// <summary>
    /// Filter yeu cau quyen Admin. Dat tren toan bo Controller trong Areas/Admin:
    ///   [AdminRequired]
    ///   public class DanhMucController : Controller { ... }
    /// </summary>
    public class AdminRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;

            // Chua dang nhap
            if (session == null || session["MaNguoiDung"] == null)
            {
                filterContext.Controller.TempData["Loi"] = "Vui long dang nhap voi tai khoan Admin.";

                var routeValues = new RouteValueDictionary();
                routeValues["controller"] = "Account";
                routeValues["action"] = "DangNhap";
                routeValues["area"] = "";

                filterContext.Result = new RedirectToRouteResult(routeValues);
                return;
            }

            // Da dang nhap nhung khong phai Admin
            string vaiTro = session["VaiTro"] != null ? session["VaiTro"].ToString() : "";
            if (vaiTro != "Admin")
            {
                filterContext.Controller.TempData["Loi"] = "Ban khong co quyen truy cap khu vuc nay.";

                var routeValues = new RouteValueDictionary();
                routeValues["controller"] = "Home";
                routeValues["action"] = "Index";
                routeValues["area"] = "";

                filterContext.Result = new RedirectToRouteResult(routeValues);
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
