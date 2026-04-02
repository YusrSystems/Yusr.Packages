using Yusr.Core.Abstractions.Utilities;
using Yusr.eInvoicing.Abstractions.Enums;

namespace Yusr.eInvoicing.Abstractions.Entities.Interfaces
{
    public interface IEInvoiceInvoice
    {
        public long Id { get; }
        public InvoiceType InvoiceType { get; }
        public long? InvoiceCounter { get; }
        public long? OriginalInvoiceId { get; }
        public DateTime InvoiceDate { get; }
        public DateTime? DeliveryDate { get; }
        public EInvoiceStatus EInvoiceStatus { get; }
        public decimal FullAmount { get; }
        public string? QR { get; }
        public string? InvoiceHash { get; }
        public string? PreviousHash { get; }
        public string? SignedXml { get; }
        public ICollection<IEInvoiceInvoiceItem> InvoiceItems { get; }

        public static bool IsSendableEInvoice(InvoiceType type, IEInvoiceSetting settings)
        {
            if (type != InvoiceType.Sell && type != InvoiceType.SellReturn)
                return false;

            return IsRegisteredForEInvoicing(settings);
        }

        public bool IsSendableEInvoice(IEInvoiceSetting settings)
        {
            if (InvoiceType != InvoiceType.Sell && InvoiceType != InvoiceType.SellReturn)
                return false;

            return IsRegisteredForEInvoicing(settings);
        }

        public static bool IsRegisteredForEInvoicing(IEInvoiceSetting settings)
        {
            if (settings.EInvoicingEnvironmentType == EInvoicingEnvironmentType.NotRegistered
                || string.IsNullOrEmpty(settings.CertificateContent) || string.IsNullOrEmpty(settings.PrivateKey)
                || string.IsNullOrEmpty(settings.BinarySecurityToken) || string.IsNullOrEmpty(settings.Secret))
                return false;

            return true;
        }

        public IEInvoiceInvoice UpdateEInvoiceInfo(long? invoiceCounter, string? invoiceHash, string? previousHash, string? signedXml, string? qr);

        public IEInvoiceInvoice UpdateEInvoiceStatus(EInvoiceStatus eInvoiceStatus);

        public IEInvoiceInvoice UpdateQr(string? qr);

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