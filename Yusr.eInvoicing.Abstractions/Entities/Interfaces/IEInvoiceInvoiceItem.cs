namespace Yusr.eInvoicing.Abstractions.Entities.Interfaces
{
    public interface IEInvoiceInvoiceItem
    {
        public long Id { get; }
        /// <summary>Unit price paid by client, tax-inclusive, excluding settlement.</summary>
        public decimal TaxInclusivePrice { get; }
        public decimal Quantity { get; }
        /// <summary>
        /// Total line amount paid by client, tax-inclusive, after applying per-unit settlement.
        /// Formula: (TaxInclusivePrice + Settlement) * Quantity
        /// </summary>
        public decimal TaxInclusiveTotalPrice { get; }
        public decimal Settlement { get; }
        public bool Taxable { get; }
        public decimal TotalTaxesPerc { get; }
    }
}
