using Yusr.eInvoicing.Abstractions.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Yusr.eInvoicing.Abstractions.Entities.Interfaces
{
    public interface IInvoice
    {
        public long Id { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public long? InvoiceCounter { get; set; }
        public long? OriginalInvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public EInvoiceStatus EInvoiceStatus { get; set; }
        public decimal FullAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal AddedAmount { get; set; }
        public long StoreId { get; set; }
        public long ActionAccountId { get; set; }
        public string? QR { get; set; }
        public string? InvoiceHash { get; set; }
        public string? PreviousHash { get; set; }
        public string? SignedXml { get; set; }
        public ICollection<IInvoiceItem> InvoiceItems { get; set; }

        public static bool IsSendableEInvoice(InvoiceType type, IEInvoicingSetting settings)
        {
            if (type != InvoiceType.Sell && type != InvoiceType.SellReturn)
                return false;

            return IInvoice.IsRegisteredForEInvoicing(settings);
        }
        public static bool IsRegisteredForEInvoicing(IEInvoicingSetting settings)
        {
            if (settings.EInvoicingStatus == 0
                || string.IsNullOrEmpty(settings.certificateContent) || string.IsNullOrEmpty(settings.PrivateKey)
                || string.IsNullOrEmpty(settings.BinarySecurityToken) || string.IsNullOrEmpty(settings.Secret))
                return false;

            return true;
        }


        public IInvoice UpdateEInvoiceInfo(long? invoiceCounter, string? invoiceHash, string? previousHash, string? signedXml, string? qr)
        {
            InvoiceCounter = invoiceCounter;
            InvoiceHash = invoiceHash;
            PreviousHash = previousHash;
            SignedXml = signedXml;
            return UpdateQr(qr);
        }

        public IInvoice UpdateEInvoiceStatus(EInvoiceStatus eInvoiceStatus)
        {
            EInvoiceStatus = eInvoiceStatus;
            return this;
        }

        public IInvoice UpdateQr(string? qr)
        {
            QR = qr;
            return this;
        }

    }
}
