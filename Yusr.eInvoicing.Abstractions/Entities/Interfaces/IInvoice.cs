using Yusr.eInvoicing.Abstractions.Enums;

namespace Yusr.eInvoicing.Abstractions.Entities.Interfaces
{
    public interface IInvoice
    {
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
    }
}
