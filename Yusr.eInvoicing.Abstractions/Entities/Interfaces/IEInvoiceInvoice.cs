using Yusr.Core.Abstractions.Utilities;
using Yusr.eInvoicing.Abstractions.Enums;

namespace Yusr.eInvoicing.Abstractions.Entities.Interfaces
{
    public interface IEInvoiceInvoice
    {
        public long Id { get; protected set; }
        public InvoiceType InvoiceType { get; protected set; }
        public long? InvoiceCounter { get; protected set; }
        public long? OriginalInvoiceId { get; protected set; }
        public DateTime InvoiceDate { get; protected set; }
        public DateTime? DeliveryDate { get; protected set; }
        public EInvoiceStatus EInvoiceStatus { get; protected set; }
        public decimal FullAmount { get; protected set; }
        public string? QR { get; protected set; }
        public string? InvoiceHash { get; protected set; }
        public string? PreviousHash { get; protected set; }
        public string? SignedXml { get; protected set; }
        public ICollection<IEInvoiceInvoiceItem> InvoiceItems { get; protected set; }

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

        public IEInvoiceInvoice UpdateEInvoiceInfo(long? invoiceCounter, string? invoiceHash, string? previousHash, string? signedXml, string? qr)
        {
            InvoiceCounter = invoiceCounter;
            InvoiceHash = invoiceHash;
            PreviousHash = previousHash;
            SignedXml = signedXml;
            return UpdateQr(qr);
        }

        public IEInvoiceInvoice UpdateEInvoiceStatus(EInvoiceStatus eInvoiceStatus)
        {
            EInvoiceStatus = eInvoiceStatus;
            return this;
        }

        public IEInvoiceInvoice UpdateQr(string? qr)
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