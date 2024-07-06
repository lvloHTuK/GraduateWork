using CheckAnalysis.Data;
using CheckAnalysis.Data.Repositories;
using CheckAnalysis.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
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
            foreach (var check in await _checkDataRepository.GetAll())
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
            foreach (var product in products)
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
            List<string> pixel = new List<string>();
            var max = sumOfMonth.Max(x => x.Value);
            int percent;
            foreach (var item in sumOfMonth)
            {
                percent = (int)(100 * item.Value / max);
                pixel.Add(String.Concat(percent.ToString(), "px"));
            }
            ViewBag.Px = pixel;
            ViewBag.Cost = sumOfMonth;
            return View();
        }

        public async Task<ViewResult> MonthInfo(DateData dateData)
        {
            Console.WriteLine(dateData.FirstDate.ToString());
            Console.WriteLine(dateData.LastDate.ToString());
            var productsInfo = await _checkDataRepository.GetInfoProduct(dateData.FirstDate, dateData.LastDate);
            var productsInfoForPreviosMonth = await _checkDataRepository.GetInfoProduct(dateData.FirstDate.AddMonths(-1), dateData.LastDate.AddMonths(-1));
            var productCategory = await _checkDataRepository.GetInfoCategory(dateData.FirstDate, dateData.LastDate);
            var random = new Random();
            var allSum = productCategory.Select(x => x.Sum).Sum();
            List<ProductInfo> prod = new List<ProductInfo>();
            double? percentSum = 0;
            foreach (var item in productCategory)
            {
                item.Percent = (item.Sum * 100) / allSum;
                int dashOne = (int)(item.Percent + percentSum);
                int dashTwo = (int)(100 - percentSum);
                item.Dash_Stroke = dashOne.ToString() + " " + dashTwo.ToString();
                prod.Add(item);
                percentSum = prod.Select(x => x.Percent).Sum();
                item.Color = String.Format("#{0:X6}", random.Next(0x1000000));
                Console.WriteLine(item.Name + " " + item.Color + " " + item.Percent.ToString() + " " + item.Dash_Stroke.ToString());
            }
            List<IndexOfCost> listIndexs = new List<IndexOfCost>();
            foreach (var product in productsInfo)
            {
                var productForPreviousMonth = productsInfoForPreviosMonth.FirstOrDefault(x => x.Name == product.Name);
                if (productForPreviousMonth != null)
                {
                    listIndexs.Add(new IndexOfCost(product.Name, productForPreviousMonth.Sum, productForPreviousMonth.AvgQuantity, product.Sum, product.AvgQuantity));
                }
            }
            var sumP0Q0 = listIndexs.Select(x => x.p0q0).Sum();
            var sumP1Q0 = listIndexs.Select(x => x.p1q0).Sum();
            var sumP0Q1 = listIndexs.Select(x => x.p0q1).Sum();
            var sumP1Q1 = listIndexs.Select(x => x.p1q1).Sum();
            var generalIndex = sumP1Q1 / sumP0Q1;
            var consumerPriceIndex = sumP1Q0 / sumP0Q0;
            productCategory.Reverse();
            ViewBag.GeneralIndex = generalIndex;
            ViewBag.ConsumerPriceIndex = consumerPriceIndex;
            ViewBag.IndexsOfCost = listIndexs;
            ViewBag.Month = dateData.Name.ToUpperInvariant();
            ViewBag.Circle = productCategory;
            ViewBag.PopularProducts = productsInfo.OrderByDescending(x => x.Sum);
            return View();
        }
    }
}
