using CheckAnalysis.Data.Repositories;
using CheckAnalysis.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CheckAnalysis.Controllers
{
    public class MarketBusketController : Controller
    {
        private readonly CheckDataRepository _checkDataRepository;

        public MarketBusketController(CheckDataRepository checkDataRepository)
        {
            _checkDataRepository = checkDataRepository;
        }
        public async Task<ActionResult> Index()
        {
            List<MarketBasketAnalyse> marketBusket = new List<MarketBasketAnalyse>();
            await _checkDataRepository.UpdateMarketBusket();
            var checks = await _checkDataRepository.GetAllChecks();
            List<MarketBusketData> marketData = new List<MarketBusketData>();
            Console.WriteLine(checks.Count);
            foreach (var check in checks)
            {
                marketData.Add(await _checkDataRepository.GetMarketBusketData(check));
            }
            foreach (var market in marketData)
            {
                var keys = market.CategoryFreq.Select(x => x.Key).ToList();
                if(keys.Count > 1)
                {
                    var list = GetCombination(keys);
                    foreach (var item in list)
                    {
                        var count = marketBusket.Where(x => x.Lhs == item.Split(' ')[0] && x.Rhs == item.Split(' ')[1]).ToList().Count;
                        if(count == 0)
                        {
                            marketBusket.Add(new MarketBasketAnalyse(item, _checkDataRepository));
                        }
                    }
                }
            }
            ViewBag.MarketBasket = marketBusket;
            return View();
        }

        private List<string> GetCombination(List<string> categorys)
        {
            List<string> result = new List<string>();
            foreach (var category1 in categorys)
            {
                foreach(var category2 in categorys)
                {
                    if (!category1.Equals(category2))
                    {
                        //Console.WriteLine(category1 + " " + category2);
                        result.Add(category1 + " " + category2);
                    }
                }
            }
            return result;
        }
    }
}
