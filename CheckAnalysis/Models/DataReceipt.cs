using Microsoft.AspNetCore.Authentication;

namespace CheckAnalysis.Models
{
    public class DataReceipt
    {
        public string buyerPhoneOrAddress { get; set; }
        public int cashTotalSum { get; set; }
        public int creditSum { get; set; }
        public DateTimeOffset dateTime { get; set; }
        public int ecashTotalSum { get; set; }
        public int fiscalDocumentFormatVer { get; set; }
        public int fiscalDocumentNumber { get; set; }
        public string fiscalDriveNumber { get; set; }
        public ulong fiscalSign { get; set; }
        public string fnsUrl { get; set; }
        public IList<Items> items { get; set; }
        public string kttRegId { get; set; }
        public string machineNumber { get; set; }
        public int nds18 { get; set; }
        public int operationType { get; set; }
        public int prepaidSum { get; set; }
        public Properties properties { get; set; }
        public string propertiesData { get; set; }
        public int provisionSum { get; set; }
        public int requestNumber { get; set; }
        public string retailPlace { get; set; }
        public string retailPlaceAddress { get; set; }
        public string sellerAddress { get; set; }
        public int shiftNumber { get; set; }
        public int taxationType { get; set; }
        public int appliedTaxationType { get; set; }
        public int totalSum { get; set; }
        public string user { get; set; }
        public string userInn { get; set; }

    }
}
