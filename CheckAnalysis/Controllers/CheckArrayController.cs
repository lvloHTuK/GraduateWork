using CheckAnalysis.Data;
using CheckAnalysis.Data.Repositories;
using CheckAnalysis.Models;
using Microsoft.AspNetCore.Mvc;

namespace CheckAnalysis.Controllers
{
    public class CheckArrayController : Controller
    {
        private readonly CheckDataRepository _checkDataRepository;
        public List<ItemData> ItemData { get; set; }

        public CheckArrayController(CheckDataRepository checkDataRepository)
        {
            _checkDataRepository = checkDataRepository;
            ItemData = new List<ItemData>();
        }

        public async Task<ViewResult> Index()
        {
            foreach(var check in await _checkDataRepository.GetAll())
            {
                ItemData.Add(check);
            }
            ViewBag.Checks = ItemData;
            return View();
        }
    }
}
