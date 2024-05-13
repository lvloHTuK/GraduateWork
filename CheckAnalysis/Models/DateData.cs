namespace CheckAnalysis.Models
{

    public class DateData
    {
        public string Name { get; set; }
        public DateTime FirstDate { get; set; }
        public DateTime LastDate { get; set; }
        public double? Value { get; set; }

        public DateData()
        {

        }

        public DateData(string name, DateTime firstDate, DateTime lastDate, double? value)
        {
            Name = name;
            FirstDate = firstDate;
            LastDate = lastDate;
            Value = value;
        }
    }
}
