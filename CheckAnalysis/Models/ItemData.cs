using System.ComponentModel.DataAnnotations;

namespace CheckAnalysis.Models
{
    public class ItemData
    {
        [Key]
        public Guid? Id { get; set; }
        public string? CheckId {get; set;}
        public string? name { get; set; }
        public int? nds { get; set; }
        public int? ndsSum { get; set; }
        public int? paymentType { get; set; }
        public int? price { get; set; }
        public double? quantity { get; set; }
        public int? sum { get; set; }
        public string YandexGPT { get; set; }

        public ItemData()
        {

        }
        public ItemData(Items item, string checkId)
        {
            Id = Guid.NewGuid();
            CheckId = checkId;
            name = item.name;
            nds = item.nds;
            ndsSum = item.ndsSum;
            paymentType = item.paymentType;
            price = item.price / 100;
            quantity = item.quantity;
            sum = item.sum / 100;
            CheckId = checkId;
            YandexGPT = item.YandexGPT;
        }

    }
}
