namespace Yusr.eInvoicing.Abstractions.Entities.Interfaces
{
    public interface IEInvoiceInvoiceItem
    {
        public long Id { get; }
        public decimal Price { get; }
        public decimal Quantity { get; }
        public decimal TotalPrice { get; }
        public decimal Discount { get; }
        public bool Taxable { get; }
        public decimal TotalTaxesPerc { get; }
    }
}
