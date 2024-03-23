using Microsoft.AspNetCore.Mvc;
using CheckAnalysis.Models;
using System.Diagnostics;
using System.Text.Json;
using System.IO;

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
            foreach (var file in files.Files)
            {
                string targetFileName = $"{AppDomain.CurrentDomain.BaseDirectory}/DownloadsChecks/{file.FileName}";

                using (var stream = new FileStream(targetFileName, FileMode.Create))
                {
                    file.CopyToAsync(stream);
                }

                //Console.WriteLine(s);
                using (StreamReader sr = System.IO.File.OpenText(targetFileName))
                {
                    string s;
                    string jsonCheck = "";
                    int count = 0;
                    while ((s = sr.ReadLine()) != null)
                    {
                        if(s.Contains("{"))
                        {
                            count++;
                            jsonCheck += s.Trim();
                        }
                        else if(s.Trim() == "}" || s.Trim() == "},")
                        {
                            count--;
                            jsonCheck += s.Trim();
                        }
                        else
                        {
                            jsonCheck += s.Trim();
                        }
                        if(count == 0 && s != "[" && s != "]")
                        {
                            jsonCheck = jsonCheck.Trim('[');
                            jsonCheck = jsonCheck.Trim(',');
                            CheckFile check = JsonSerializer.Deserialize<CheckFile>(jsonCheck);
                            jsonCheck = "";
                        }
                    }
                }


            }



            return Json(files.Files.Count);
        }
    }
}