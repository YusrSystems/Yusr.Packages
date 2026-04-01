using Yusr.Core.Abstractions.Utilities;
using Yusr.eInvoicing.Abstractions.Enums;

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
        public string? QR { get; set; }
        public string? InvoiceHash { get; set; }
        public string? PreviousHash { get; set; }
        public string? SignedXml { get; set; }
        public ICollection<IInvoiceItem> InvoiceItems { get; set; }

        public static bool IsSendableEInvoice(InvoiceType type, IEInvoicingSetting settings)
        {
            if (type != InvoiceType.Sell && type != InvoiceType.SellReturn)
                return false;

            return IsRegisteredForEInvoicing(settings);
        }

        public bool IsSendableEInvoice(IEInvoicingSetting settings)
        {
            if (InvoiceType != InvoiceType.Sell && InvoiceType != InvoiceType.SellReturn)
                return false;

            return IsRegisteredForEInvoicing(settings);
        }

        public static bool IsRegisteredForEInvoicing(IEInvoicingSetting settings)
        {
            if (settings.EInvoicingEnvironmentType == EInvoicingEnvironmentType.NotRegistered
                || string.IsNullOrEmpty(settings.CertificateContent) || string.IsNullOrEmpty(settings.PrivateKey)
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

        public decimal GetTaxAmount()
        {
            return YusrMath.Round(InvoiceItems
                .Select(ii => new
                {
                    ii.TotalPrice,
                    TaxFactor = (!ii.Taxable || ii.TotalTaxesPerc == 0) ? 1 : YusrMath.GetFactor(ii.TotalTaxesPerc)
                })
                .Select(x => new
                {
                    TaxTotalPrice = x.TotalPrice,
                    NoTaxTotalPrice = YusrMath.Round(x.TotalPrice / x.TaxFactor)
                })
                .Sum(x => x.TaxTotalPrice - x.NoTaxTotalPrice));
        }
    }
}