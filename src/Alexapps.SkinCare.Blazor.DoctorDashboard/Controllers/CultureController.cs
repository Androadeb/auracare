using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace ynex.Controllers
{
    [Route("[controller]/[action]")] // إضافة هذا السطر
    public class CultureController : Controller
    {
        [HttpGet]
        public IActionResult SetCulture(string culture, string redirectionUri)
        {
            if (culture != null)
            {
                HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );
            }

            // تنظيف الرابط المرسل من أي تكرار لـ "Culture"
            if (!string.IsNullOrEmpty(redirectionUri) && redirectionUri.Contains("Culture/SetCulture"))
            {
                redirectionUri = "/";
            }

            // نستخدم Redirect عادي بدلاً من LocalRedirect لتجنب تعقيدات الـ Validation
            // ولكن نتأكد أنه يبدأ بـ / لضمان أنه داخل موقعنا
            if (!string.IsNullOrEmpty(redirectionUri) && redirectionUri.StartsWith("/"))
            {
                return Redirect(redirectionUri);
            }

            return Redirect("/"); // العودة للرئيسية كخيار احتياطي
        }
    }
}