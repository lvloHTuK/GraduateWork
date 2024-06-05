using System.Drawing;

namespace CheckAnalysis.Models
{
    public class ProductInfo
    {
        public string Name { get; set; }
        public double? AvgQuantity { get; set; }
        public double? Sum { get; set; }
        public double? Percent { get; set; }
        public string Dash_Stroke { get; set; }
        public string Color { get; set; }

        public ProductInfo(string name, double? avgQuantity, double? sum)
        {
            Name = name;
            AvgQuantity = avgQuantity;
            Sum = sum;
        }
    }
}
