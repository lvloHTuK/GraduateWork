using CheckAnalysis.Data;
using CheckAnalysis.Data.Repositories;
using CheckAnalysis.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

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

        public async Task<ViewResult> Products()
        {
            var products = await _checkDataRepository.GetAllUniqueProducts();
            ViewBag.Products = products;
            Dictionary<string, double?> popularProducts = new Dictionary<string, double?>();
            foreach(var product in products)
            {
                popularProducts.Add(product, await _checkDataRepository.GetAVGQuantityProduct(product));
            }
            ViewBag.PopularProducts = popularProducts.OrderByDescending(x => x.Value);
            return View();
        }

        public async Task<ViewResult> ProductInformation(ItemData item)
        {
            var avgProduct = await _checkDataRepository.GetAVGQuantityProduct(item.name);
            var sumItem = await _checkDataRepository.GetAllCost(item.name);
            var sumOfMonth = await _checkDataRepository.GetCostOfMonth(item.name);
            ViewBag.AvgProduct = avgProduct;
            ViewBag.SumOfMonth = sumOfMonth;
            ViewBag.Sum = sumItem;
            ViewBag.Item = item;
            return View();
        }

        public async Task<ViewResult> ProductList(ItemData item)
        {
            var items = await _checkDataRepository.GetItemOfName(item.name);
            ViewBag.Items = items;
            return View();
        }

        public async Task<ViewResult> Cost()
        {
            var avgCheck = await _checkDataRepository.GetAverageCheck();
            var allSum = await _checkDataRepository.GetAllCost();
            var sumOfMonth = await _checkDataRepository.GetCostOfMonth();
            ViewBag.AvgCheck = avgCheck;
            ViewBag.AllCost = allSum;
            ViewBag.Cost = sumOfMonth;
            return View();
        }

        public async Task<ViewResult> MonthInfo(DateData dateData)
        {
            //var products = await _checkDataRepository.GetAllUniqueProducts();
            var productsInfo = await _checkDataRepository.GetInfoProduct(dateData.FirstDate, dateData.LastDate);
            /*List<ProductInfo> popularProducts = new List<ProductInfo>();
            foreach (var product in products)
            {
                popularProducts.Add(new ProductInfo(product, await _checkDataRepository.GetAVGQuantityProduct(product, dateData.FirstDate, dateData.LastDate), await _checkDataRepository.GetCostOfMonth(product, dateData.FirstDate, dateData.LastDate)));
            }*/
            ViewBag.PopularProducts = productsInfo;
            return View();
        }
    }
}
