using CheckAnalysis.Data.Repositories;

namespace CheckAnalysis.Models
{
    public class MarketBasketAnalyse
    {
        public string Lhs { get; set; }
        public string Rhs { get; set; }
        public int Frequency { get; set; }
        public string Support { get; set; }
        public string Confidence { get; set; }
        public string Lift { get; set; }

        public MarketBasketAnalyse(string lhsRhs, CheckDataRepository repository)
        {
            Lhs = lhsRhs.Split(' ')[0];
            Rhs = lhsRhs.Split(' ')[1];
            Frequency = repository.GetFrequency(this.Lhs, this.Rhs);
            var countChecks = repository.GetCountCheck();
            double sup = this.Frequency / (float)countChecks;
            Console.WriteLine(sup);
            Support = sup.ToString();
            var frequencyLhs = (float)repository.GetFrequency(this.Lhs);
            var frequencyRhs = (float)repository.GetFrequency(this.Rhs);
            Confidence = (this.Frequency / frequencyLhs).ToString();
            float supLhs = frequencyLhs / countChecks;
            float supRhs = frequencyRhs / countChecks;
            var res = sup / (supLhs * supRhs);
            Lift = res.ToString();
        }
    }
}
