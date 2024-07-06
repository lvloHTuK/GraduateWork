using CheckAnalysis.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
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

        public async Task<List<string?>> GetAllChecks()
        {
            var checks = db.CheckData.Select(x => x.CheckId).Distinct().ToList();
            if (checks != null)
            {
                return checks;
            }

            return null;
        }

        public List<string?> GetDistinctProductsInCheck(string? checkId)
        {
            var products = db.ItemData.Where(x => x.CheckId == checkId).Select(x => x.YandexGPT).Distinct().ToList();
            if (products != null)
            {
                return products;
            }

            return null;
        }

        public async Task<MarketBusketData> GetMarketBusketData(string? checkId)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            using (DbCommand command = db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT * FROM [Checks].[dbo].[MarketBusket] WHERE CheckId = @p1";
                var parameter1 = new SqlParameter("@p1", checkId);
                command.Parameters.Add(parameter1);
                db.Database.OpenConnection();
                var products = await GetAllUniqueProducts();
                var countProduct = products.Count();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            for (int i = 1; i <= countProduct; i++)
                            {
                                var val = reader.GetInt32(i);
                                if (val != 0)
                                {
                                    dict.Add(reader.GetName(i), val);
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                    reader.Close();
                }
            }
            return new MarketBusketData(checkId, dict);
        }

        public async Task InsertNullOnZero()
        {
            var products = await GetAllUniqueProducts();
            using (DbCommand command = db.Database.GetDbConnection().CreateCommand())
            {
                foreach (var item in products)
                {
                    command.CommandText = $"UPDATE [Checks].[dbo].[MarketBusket] SET [{item.Replace(' ', '_').Replace('-', '_')}] = ISNULL([{item.Replace(' ', '_').Replace('-', '_')}], 0)";
                    db.Database.OpenConnection();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateMarketBusket()
        {
            var checks = NewChecks();
            foreach (var check in checks)
            {
                await AddMarketBusket(check);
            }
            await InsertNullOnZero();
        }

        private List<string?> NewChecks()
        {
            List<string?> checks = new List<string?>();
            using (DbCommand command = db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT CheckData.CheckId FROM [CheckData] LEFT JOIN [MarketBusket] ON CheckData.CheckId = MarketBusket.CheckId WHERE MarketBusket.CheckId IS NULL";
                db.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            checks.Add(reader.GetString(0));
                        }
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                    reader.Close();
                }
            }
            return checks;
        }

        public async Task AddMarketBusket(string checkId)
        {
            using (DbCommand command = db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "INSERT INTO MarketBusket ([CheckId]) VALUES (@p1)";
                var parameter1 = new SqlParameter("@p1", checkId);
                command.Parameters.Add(parameter1);
                db.Database.OpenConnection();
                var answer = command.ExecuteNonQuery();
                if (answer == 1)
                {
                    command.CommandText = "SELECT DISTINCT [YandexGPT]\r\n  FROM [Checks].[dbo].[ItemData] WHERE ItemData.CheckId = @p2";
                    parameter1 = new SqlParameter("@p2", checkId);
                    command.Parameters.Add(parameter1);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                AddCountInCheck(checkId, reader.GetString(0));
                            }
                        }
                        else
                        {
                            Console.WriteLine("No rows found.");
                        }
                        reader.Close();
                    }
                }
            }
        }

        public async Task AddCountInCheck(string checkId, string category)
        {
            using (DbCommand command = db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT ItemData.YandexGPT FROM [Checks].[dbo].[ItemData] WHERE ItemData.CheckId = @p3 AND ItemData.YandexGPT = @p4";
                var parameter1 = new SqlParameter("@p3", checkId);
                var parameter2 = new SqlParameter("@p4", category);
                command.Parameters.Add(parameter1);
                command.Parameters.Add(parameter2);
                db.Database.OpenConnection();
                var count = 0;
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            count++;
                        }
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                    reader.Close();
                }
                command.CommandText = $"UPDATE [Checks].[dbo].[MarketBusket] SET [{category.Replace(' ', '_').Replace('-', '_')}] = @p6 WHERE MarketBusket.CheckId = @p7";
                parameter2 = new SqlParameter("@p6", count);
                var parameter3 = new SqlParameter("@p7", checkId);
                command.Parameters.Add(parameter2);
                command.Parameters.Add(parameter3);
                var answer2 = await command.ExecuteNonQueryAsync();
                if (answer2 == 1)
                {
                    Console.WriteLine("OK");
                }
            }
        }

        public int GetFrequency(string lhs, string rhs)
        {
            int result = 0;
            using (DbCommand command = db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = $"SELECT COUNT(*) FROM[Checks].[dbo].[MarketBusket] WHERE [{lhs}] != 0 AND[{rhs}] != 0";
                db.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = reader.GetInt32(0);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                    reader.Close();
                }
            }
            return result;
        }

        public int GetFrequency(string lhs)
        {
            int result = 0;
            using (DbCommand command = db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = $"SELECT COUNT(*) FROM[Checks].[dbo].[MarketBusket] WHERE [{lhs}] != 0";
                db.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = reader.GetInt32(0);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                    reader.Close();
                }
            }
            return result;
        }

        public int GetCountCheck()
        {
            int result = 0;
            using (DbCommand command = db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM [Checks].[dbo].[MarketBusket] ";
                db.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = reader.GetInt32(0);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                    reader.Close();
                }
            }
            return result;
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

        public async Task<List<ProductInfo>> GetInfoCategory(DateTime firstDate, DateTime lastDate)
        {
            List<ProductInfo> products = new List<ProductInfo>();
            using (DbCommand command = db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT SUM(ItemData.quantity), SUM(ItemData.sum), ItemData.Category \r\n" +
                    "FROM [CheckData] LEFT JOIN [ItemData] ON CheckData.CheckId = ItemData.CheckId\r\n" +
                    "WHERE CheckData.dateTime BETWEEN @p1 AND @p2\r\n" +
                    "GROUP BY ItemData.Category";
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
            if (checkId.Count == 0)
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
