using System.Web.Mvc;
using System.Web.Routing;

namespace DACK_LTW_Nhom4.Filters
{
    /// <summary>
    /// Filter yeu cau dang nhap. Dat tren Action hoac toan bo Controller:
    ///   [DangNhapRequired]
    ///   public class GioHangController : Controller { ... }
    /// </summary>
    public class DangNhapRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;

            if (session == null || session["MaNguoiDung"] == null)
            {
                string returnUrl = filterContext.HttpContext.Request.RawUrl;

                filterContext.Controller.TempData["Loi"] = "Vui long dang nhap de tiep tuc.";

                var routeValues = new RouteValueDictionary();
                routeValues["controller"] = "Account";
                routeValues["action"] = "DangNhap";
                routeValues["returnUrl"] = returnUrl;

                filterContext.Result = new RedirectToRouteResult(routeValues);
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
