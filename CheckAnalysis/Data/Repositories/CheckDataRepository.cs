using CheckAnalysis.Models;

namespace CheckAnalysis.Data.Repositories
{
    public class CheckDataRepository
    {
        private readonly CheckDbContext db;

        public CheckDataRepository(CheckDbContext context)
        {
            db = context;
        }


        public async Task Add(CheckFile check)
        {
            CheckData checkData = new CheckData(check);
            foreach(var item in check.ticket.document.receipt.items)
            {
                checkData.AddItem(item);
                await db.CheckData.AddAsync(checkData);
            }
        }

        public async Task Save() => await db.SaveChangesAsync();
    }
}
