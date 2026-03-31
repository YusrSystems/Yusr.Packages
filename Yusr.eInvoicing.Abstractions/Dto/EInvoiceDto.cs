using Yusr.eInvoicing.Abstractions.Enums;

namespace Yusr.eInvoicing.Abstractions.Dto
{
    public class EInvoiceDto
    {
        public EInvoiceType EInvoiceType { get; set; }
        public string ProfileID { get; set; } = string.Empty;
        public long ID { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public string IssueTime { get; set; } = string.Empty;
        public DateTime? DeliveryDate { get; set; }
        public long InvoiceCounter { get; set; }
        public string PreviousInvoiceHash { get; set; } = string.Empty;
        public long? OriginalInvoiceId { get; set; }
        public string SupplierCRN { get; set; } = string.Empty;
        public string SupplierVatNumber { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public EInvoiceAddressDto SupplierAddress { get; set; } = new EInvoiceAddressDto();
        public long ActionAccountId { get; set; }
        public string CustomerVatNumber { get; set; } = string.Empty;
        public string CustomerCRN { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public EInvoiceAddressDto CustomerAddress { get; set; } = new EInvoiceAddressDto();
        public decimal TaxAmount { get; set; }
        public decimal LineExtensionAmount { get; set; }
        public decimal TaxExclusiveAmount { get; set; }
        public decimal TaxInclusiveAmount { get; set; }
        public decimal RoundingAmount { get; set; }
        public decimal InvoiceAmount { get; set; }
        public List<EInvoiceLineDto> InvoiceLines { get; set; } = new List<EInvoiceLineDto>();
    }
}
