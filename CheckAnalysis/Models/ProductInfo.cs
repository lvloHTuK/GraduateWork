namespace CheckAnalysis.Models
{
    public class ProductInfo
    {
        public string Name { get; set; }
        public double? AvgQuantity { get; set; }
        public double? Sum { get; set; }

        public ProductInfo(string name, double? avgQuantity, double? sum)
        {
            Name = name;
            AvgQuantity = avgQuantity;
            Sum = sum;
        }
    }
}
