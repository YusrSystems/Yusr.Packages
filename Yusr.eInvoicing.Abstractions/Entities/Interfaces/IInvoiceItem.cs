using System;
using System.Collections.Generic;
using System.Text;

namespace Yusr.eInvoicing.Abstractions.Entities.Interfaces
{
    public interface IInvoiceItem
    {
        public long TenantId { get; set; }
        public long Id { get; set; }

        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get;  set; }
        public bool Taxable { get;  set; }
        public decimal TotalTaxesPerc { get; set; }
    }
}
