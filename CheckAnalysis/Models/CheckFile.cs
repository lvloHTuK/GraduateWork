using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection.Metadata;
using System.Text.Json;

namespace CheckAnalysis.Models
{
    public class CheckFile
    {
        public string _id { get; set; }
        public DateTimeOffset createdAt { get; set; }
        public CheckDocument ticket { get; set; }
        
    }
}

