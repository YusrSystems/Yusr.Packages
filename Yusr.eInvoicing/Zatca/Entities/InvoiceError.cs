using System;
using System.Collections.Generic;
using System.Text;

namespace Yusr.eInvoicing.Zatca.Entities
{
    public class InvoiceError
    {
        public string Category { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
