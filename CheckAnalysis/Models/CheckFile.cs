using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace CheckAnalysis.Models
{
    public class CheckFile
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        
    }
}

