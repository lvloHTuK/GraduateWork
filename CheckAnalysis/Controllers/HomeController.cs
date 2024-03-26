using Microsoft.AspNetCore.Mvc;
using CheckAnalysis.Models;
using System.Diagnostics;
using System.Text.Json;
using System.IO;
using CheckAnalysis.Data.Repositories;

namespace CheckAnalysis.Controllers
{
    public class HomeController : Controller
    {
        private readonly CheckDataRepository _checkDataRepository;

        public HomeController(CheckDataRepository checkDataRepository)
        {
            _checkDataRepository = checkDataRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UploadFiles(IFormCollection files)
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
                            await _checkDataRepository.Add(check);
                            jsonCheck = "";
                        }
                    }
                }
            }
            _checkDataRepository.Save();
            return Json(files.Files.Count);
        }
    }
}