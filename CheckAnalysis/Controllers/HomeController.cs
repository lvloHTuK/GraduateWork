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
                //string targetFileName = $"{AppDomain.CurrentDomain.BaseDirectory}/DownloadsChecks/{file.FileName}";

                /*using (var stream = new FileStream(targetFileName, FileMode.Create))
                {
                    file.CopyToAsync(stream);
                }*/

                //Console.WriteLine(s);
                using (var sr = new StreamReader(file.OpenReadStream()))
                {
                    //await ReadJsonFormat(sr);
                    await ReadOneLine(sr);
                }
            }
            return View("Index");
        }

        private async Task ReadOneLine(StreamReader reader)
        {
            string s;
            int count = 0;
            string jsonCheck = "";
            char prevCh = '_';
            while ((s = reader.ReadLine()) != null)
            {
                foreach(var ch in s)
                {
                    if (ch == '{')
                    {
                        count++;
                        jsonCheck += ch;
                    }
                    else if (ch == '}' || (ch == ',' && prevCh == '{'))
                    {
                        count--;
                        if(ch == ',')
                        {
                            jsonCheck += prevCh;
                            jsonCheck += ch;
                        }
                        else
                        {
                            jsonCheck += ch;
                        }
                    }
                    else
                    {
                        jsonCheck += ch;
                    }
                    if (count == 0 && ch != '[' && ch != ']' && ch != ',')
                    {
                        jsonCheck = jsonCheck.Trim('[');
                        jsonCheck = jsonCheck.Trim(',');
                        Console.WriteLine(jsonCheck);
                        CheckFile check = JsonSerializer.Deserialize<CheckFile>(jsonCheck);
                        await _checkDataRepository.Add(check);
                        jsonCheck = "";
                    }
                    prevCh = ch;
                }
            }
            await _checkDataRepository.Save();
        }

        private async Task ReadJsonFormat(StreamReader reader)
        {
            string s;
            string jsonCheck = "";
            int count = 0;
            while ((s = reader.ReadLine()) != null)
            {
                if (s.Contains("{"))
                {
                    count++;
                    jsonCheck += s.Trim();
                }
                else if (s.Trim() == "}" || s.Trim() == "},")
                {
                    count--;
                    jsonCheck += s.Trim();
                }
                else
                {
                    jsonCheck += s.Trim();
                }
                if (count == 0 && s != "[" && s != "]")
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
}