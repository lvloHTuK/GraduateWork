namespace CheckAnalysis.Models
{
    public class IndexOfCost
    {
        public string Name { get; set; }
        public double? p0 { get; set; }
        public double? q0 { get; set; }
        public double? p1 { get; set; }
        public double? q1 { get; set; }
        public double? IndexCost { get; set; }
        public double? p0q0 { get; set; }
        public double? p1q0 { get; set; }
        public double? p0q1 { get; set; }
        public double? p1q1 { get; set; }


        public IndexOfCost(string name, double? p0, double? q0, double? p1, double? q1)
        {
            Name = name;
            this.p0 = p0 / q0;
            this.q0 = q0;
            this.p1 = p1 / q1;
            this.q1 = q1;
            IndexCost = ((this.p1 / this.p0) * 100) - 100;
            this.p0q0 = this.p0 * this.q0;
            this.p1q0 = this.p1 * this.q0;
            this.p0q1 = this.p0 * this.q1;
            this.p1q1 = this.p1 * this.q1;
        }
    }
}
