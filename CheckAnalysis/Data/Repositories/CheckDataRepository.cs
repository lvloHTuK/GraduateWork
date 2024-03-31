using CheckAnalysis.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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

        public async Task Add(CheckFile check)
        {
            CheckData checkData = new CheckData(check);
            await db.CheckData.AddAsync(checkData);
            foreach (var item in check.ticket.document.receipt.items)
            {
                ItemData items = new ItemData(item, checkData.CheckId);
                await db.ItemData.AddAsync(items);
            }
        }

        public async Task Save() => await db.SaveChangesAsync();
    }
}
