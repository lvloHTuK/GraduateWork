using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CheckAnalysis.Models
{
    public class CheckData
    {
        [Key]
        public Guid Id { get; set; }
        public string? CheckId { get; set; }
        public DateTime createdAt { get; set; }
        public string? buyerPhoneOrAdress { get; set; }
        public int cashTotalSum { get; set; }
        public int creditSum { get; set; }
        public DateTime dateTime { get; set; }
        public int ecashTotalSum { get; set; }
        public int fiscalDocumentFormatVer { get; set; }
        public int fiscalDocumentNumber { get; set; }
        public string? fiscalDriveNumber { get; set; }
        //public ulong fiscalSign { get; set; }
        public string? fnsUrl { get; set; }
        
        public string? kttRegId { get; set; }
        public string? machineNumber { get; set; }
        public int nds18 { get; set; }
        public int operationType { get; set; }
        public int prepaidSum { get; set; }
        public string? propertyName { get; set; }
        public string? propertyValue { get; set; }
        public string? propertiesData { get; set; }
        public int provisionSum { get; set; }
        public int requestNumber { get; set; }
        public string? retailPlace { get; set; }
        public string? retailPlaceAddress { get; set; }
        public string? sellerAddress { get; set; }
        public int shiftNumber { get; set; }
        public int taxationType { get; set; }
        public int appliedTaxationType { get; set; }
        public int totalSum { get; set; }
        public string? user { get; set; }
        public string? userInn { get; set; }

        public CheckData()
        {

        }
        public CheckData(CheckFile checkFile)
        {
            Id = new Guid();
            CheckId = checkFile._id;
            createdAt = checkFile.createdAt.DateTime;
            buyerPhoneOrAdress = checkFile.ticket.document.receipt.buyerPhoneOrAddress;
            cashTotalSum = checkFile.ticket.document.receipt.cashTotalSum;
            creditSum = checkFile.ticket.document.receipt.creditSum;
            dateTime = checkFile.ticket.document.receipt.dateTime.DateTime;
            ecashTotalSum = checkFile.ticket.document.receipt.ecashTotalSum;
            fiscalDocumentFormatVer = checkFile.ticket.document.receipt.fiscalDocumentFormatVer;
            fiscalDocumentNumber = checkFile.ticket.document.receipt.fiscalDocumentNumber;
            fiscalDriveNumber = checkFile.ticket.document.receipt.fiscalDriveNumber;
            //fiscalSign = checkFile.ticket.document.receipt.fiscalSign;
            fnsUrl = checkFile.ticket.document.receipt.fnsUrl;

            kttRegId = checkFile.ticket.document.receipt.kttRegId;
            machineNumber = checkFile.ticket.document.receipt.machineNumber;
            nds18 = checkFile.ticket.document.receipt.nds18;
            operationType = checkFile.ticket.document.receipt.operationType;
            prepaidSum = checkFile.ticket.document.receipt.prepaidSum;
            propertiesData = checkFile.ticket.document.receipt.propertiesData;
            provisionSum = checkFile.ticket.document.receipt.provisionSum;
            requestNumber = checkFile.ticket.document.receipt.requestNumber;
            retailPlace = checkFile.ticket.document.receipt.retailPlace;
            retailPlaceAddress = checkFile.ticket.document.receipt.retailPlaceAddress;
            sellerAddress = checkFile.ticket.document.receipt.sellerAddress;
            shiftNumber = checkFile.ticket.document.receipt.shiftNumber;
            taxationType = checkFile.ticket.document.receipt.taxationType;
            appliedTaxationType = checkFile.ticket.document.receipt.appliedTaxationType;
            totalSum = checkFile.ticket.document.receipt.totalSum / 100;
            user = checkFile.ticket.document.receipt.user;
            userInn = checkFile.ticket.document.receipt.userInn;
        }
    }
}
