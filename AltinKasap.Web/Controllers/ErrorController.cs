using Microsoft.AspNetCore.Mvc;

namespace AltinKasap.Web.Controllers;

[Route("error")]
public class ErrorController : Controller
{
    [Route("{statusCode:int}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        ViewBag.StatusCode = statusCode;
        switch (statusCode)
        {
            case 404:
                ViewBag.Title = "Sayfa Bulunamadı";
                ViewBag.Message = "Aradığınız sayfa mevcut değil veya taşınmış olabilir.";
                ViewBag.Icon = "fa-compass";
                break;
            case 403:
                ViewBag.Title = "Erişim Reddedildi";
                ViewBag.Message = "Bu sayfayı görüntüleme yetkiniz yok.";
                ViewBag.Icon = "fa-lock";
                break;
            case 429:
                ViewBag.Title = "Çok Fazla İstek";
                ViewBag.Message = "Çok hızlı istek atıyorsunuz. Lütfen bir dakika sonra tekrar deneyin.";
                ViewBag.Icon = "fa-hourglass-half";
                break;
            case 500:
                ViewBag.Title = "Sunucu Hatası";
                ViewBag.Message = "Bir şeyler ters gitti. Ekibimiz bilgilendirildi.";
                ViewBag.Icon = "fa-triangle-exclamation";
                break;
            default:
                ViewBag.Title = "Hata";
                ViewBag.Message = "Beklenmeyen bir hata oluştu.";
                ViewBag.Icon = "fa-circle-exclamation";
                break;
        }
        return View("Error");
    }

    [Route("")]
    public IActionResult GenericError()
    {
        ViewBag.StatusCode = 500;
        ViewBag.Title = "Sunucu Hatası";
        ViewBag.Message = "Bir şeyler ters gitti. Ekibimiz bilgilendirildi.";
        ViewBag.Icon = "fa-triangle-exclamation";
        return View("Error");
    }
}
