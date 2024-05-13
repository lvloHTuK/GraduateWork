using CheckAnalysis.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CheckAnalysis.Data.Repositories
{
    public class CheckDataRepository
    {
        private readonly CheckDbContext db;

        public CheckDataRepository(CheckDbContext context)
        {
            db = context;
        }

        public async Task<IEnumerable<ItemData>> GetAll()
        {
            var checks = await db.ItemData.ToListAsync();

            if (checks != null)
            {
                return checks;
            }

            return null;
        }

        public async Task<IEnumerable<string>> GetAllUniqueProducts()
        {
            var products = db.ItemData.Select(x => x.YandexGPT).Distinct();
            if (products != null)
            {
                return products;
            }

            return null;
        }

        public async Task<int?> GetCountChecks()
        {
            var avg = db.CheckData.Select(x => x.CheckId).ToList();
            if (avg != null)
            {

                return avg.Count;
            }

            return null;
        }

        public async Task<int?> GetAverageCheck()
        {
            var avg = db.CheckData.Select(x => x.totalSum).ToList();
            var res = avg.Sum() / avg.Count;
            if (avg != null)
            {

                return res;
            }

            return null;
        }

        public async Task<int?> GetAllCost()
        {
            var sum = db.ItemData.Select(x => x.sum).ToList().Sum();
            if (sum != null)
            {
                return sum;
            }

            return null;
        }

        public async Task<int?> GetAllCost(string productName)
        {
            var sum = db.ItemData.Where(x => x.YandexGPT == productName).Select(x => x.sum).ToList().Sum();
            if (sum != null)
            {
                return sum;
            }

            return null;
        }

        public async Task<double?> GetAVGQuantityProduct(string productName)
        {
            var sum = db.ItemData.Where(x => x.YandexGPT == productName).Select(x => x.quantity).ToList().Sum();
            var checks = db.CheckData.Select(x => x.CheckId).ToList();
            if (sum != null && checks != null)
            {
                return sum / checks.Count;
            }

            return null;
        }

        public async Task<List<ProductInfo>> GetInfoProduct(DateTime firstDate, DateTime lastDate)
        {
            List<ProductInfo> products = new List<ProductInfo>();
            using (DbCommand command = db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT SUM(ItemData.quantity), SUM(ItemData.sum), ItemData.YandexGPT \r\n" +
                    "FROM [CheckData] LEFT JOIN [ItemData] ON CheckData.CheckId = ItemData.CheckId\r\n" +
                    "WHERE CheckData.dateTime BETWEEN @p1 AND @p2\r\n" +
                    "GROUP BY ItemData.YandexGPT";
                var parameter1 = new SqlParameter("@p1", firstDate.Date);
                var parameter2 = new SqlParameter("@p2", lastDate.Date);
                command.Parameters.Add(parameter1);
                command.Parameters.Add(parameter2);
                db.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            products.Add(new ProductInfo(reader.GetString(2), reader.GetDouble(0), reader.GetInt32(1)));
                        }
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                    reader.Close();
                }
            }
            return products;
        }

        public async Task<double?> GetAVGQuantityProduct(string productName, DateTime firstDate, DateTime lastDate)
        {
            var checksId = db.CheckData.Where(x => lastDate >= x.dateTime && x.dateTime >= firstDate).Select(x => x.CheckId).ToList();
            double? sum = 0;
            foreach (var check in checksId)
            {
                //var tmp = db.ItemData.Where(x => x.CheckId == check && x.YandexGPT == productName).Select(x => new { x.CheckId, x.quantity, x.sum }).GroupBy(x => x.CheckId);
                var tmpSum = db.ItemData.Where(x => x.CheckId == check && x.YandexGPT == productName).Select(x => x.quantity).ToList().Sum();
                sum += tmpSum;
            }
            if (sum != null && checksId != null)
            {
                return sum / checksId.Count;
            }

            return null;
        }

        public async Task<List<DateData>> GetCostOfMonth()
        {
            DateTime lastDayInMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            DateTime firstDayInMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            List<DateData> sumOfMonths = new List<DateData>();
            for (int i = 0; i < 12; i++)
            {
                var totalSum = db.CheckData.Where(x => lastDayInMonth >= x.dateTime && x.dateTime >= firstDayInMonth).Select(x => x.totalSum).ToList().Sum();
                sumOfMonths.Add(new DateData(lastDayInMonth.ToString("MMMM"), firstDayInMonth, lastDayInMonth, totalSum));
                lastDayInMonth = new DateTime(lastDayInMonth.AddMonths(-1).Year, lastDayInMonth.AddMonths(-1).Month, DateTime.DaysInMonth(DateTime.Now.Year, lastDayInMonth.AddMonths(-1).Month));
                firstDayInMonth = new DateTime(firstDayInMonth.AddMonths(-1).Year, firstDayInMonth.AddMonths(-1).Month, 1);
            }
            return sumOfMonths;
        }

        public async Task<List<DateData>> GetCostOfMonth(string product)
        {
            DateTime lastDayInMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            DateTime firstDayInMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            List<DateData> sumOfMonth = new List<DateData>();
            int? sum = 0;
            for (int i = 0; i < 12; i++)
            {
                var checks = db.CheckData.Where(x => lastDayInMonth >= x.dateTime && x.dateTime >= firstDayInMonth).Select(x => x.CheckId).ToList();
                sum = 0;
                foreach (var check in checks)
                {
                    var tmpSum = db.ItemData.Where(x => x.CheckId == check && x.YandexGPT == product).Select(x => x.sum).ToList().Sum();
                    sum += tmpSum;
                }
                sumOfMonth.Add(new DateData(lastDayInMonth.ToString("MMMM"), firstDayInMonth, lastDayInMonth, sum));
                sum = 0;
                lastDayInMonth = new DateTime(lastDayInMonth.AddMonths(-1).Year, lastDayInMonth.AddMonths(-1).Month, DateTime.DaysInMonth(DateTime.Now.Year, lastDayInMonth.AddMonths(-1).Month));
                firstDayInMonth = new DateTime(firstDayInMonth.AddMonths(-1).Year, firstDayInMonth.AddMonths(-1).Month, 1);
            }
            return sumOfMonth;
        }

        public async Task<double?> GetCostOfMonth(string productName, DateTime firstDate, DateTime lastDate)
        {
            Dictionary<string, int?> sumOfMonths = new Dictionary<string, int?>();
            var checksId = db.CheckData.Where(x => lastDate >= x.dateTime && x.dateTime >= firstDate).Select(x => x.CheckId).ToList();
            double? sum = 0;
            foreach (var check in checksId)
            {
                var tmpSum = db.ItemData.Where(x => x.CheckId == check && x.YandexGPT == productName).Select(x => x.sum).ToList().Sum();
                sum += tmpSum;
            }
            if (sum != null && checksId != null)
            {
                return sum;
            }

            return null;
        }

        public async Task<IEnumerable<ItemData>> GetItemOfName(string product)
        {
            var items = db.ItemData.Where(x => x.YandexGPT == product);

            if (items != null)
            {
                return items;
            }

            return null;
        }

        public async Task Add(CheckFile check)
        {
            CheckData checkData = new CheckData(check);
            var checkId = db.CheckData.Where(x => x.CheckId == check._id).Select(x => x.CheckId).ToList();
            if(checkId.Count == 0)
            {
                Console.WriteLine(check._id);
                await db.CheckData.AddAsync(checkData);
                foreach (var item in check.ticket.document.receipt.items)
                {
                    ItemData items = new ItemData(item, checkData.CheckId);
                    await db.ItemData.AddAsync(items);
                }
            }
        }

        public async Task Save() => await db.SaveChangesAsync();
    }
}
