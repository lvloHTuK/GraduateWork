using Microsoft.EntityFrameworkCore;

namespace CheckAnalysis.Models
{
    [Keyless]
    public class MarketBusketData
    {
        public string? CheckID { get; set; }
        public Dictionary<string, int> CategoryFreq { get; set; }

        public MarketBusketData() { }
        public MarketBusketData(string? checkId, Dictionary<string, int> dict)
        {
            CheckID = checkId;
            CategoryFreq = dict;
        }
    }
}
