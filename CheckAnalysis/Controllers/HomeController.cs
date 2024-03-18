using Microsoft.AspNetCore.Mvc;
using CheckAnalysis.Models;

namespace CheckAnalysis.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UploadFiles(IFormCollection files)
        {
            foreach(var file in files.Files)
            {
                string targetFileName = $"{AppDomain.CurrentDomain.BaseDirectory}/DownloadsChecks/{file.FileName}";

                using (var stream = new FileStream(targetFileName, FileMode.Create))
                {
                    file.CopyToAsync(stream);
                }
            }

            return Json(files.Files.Count);
        }
    }
}