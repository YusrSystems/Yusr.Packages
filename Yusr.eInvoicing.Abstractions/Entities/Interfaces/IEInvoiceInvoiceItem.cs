namespace Yusr.eInvoicing.Abstractions.Entities.Interfaces
{
    public interface IEInvoiceInvoiceItem
    {
        public long Id { get; protected set; }
        public decimal Price { get; protected set; }
        public decimal Quantity { get; protected set; }
        public decimal TotalPrice { get; protected set; }
        public decimal Discount { get; protected set; }
        public bool Taxable { get; protected set; }
        public decimal TotalTaxesPerc { get; protected set; }
    }
}
