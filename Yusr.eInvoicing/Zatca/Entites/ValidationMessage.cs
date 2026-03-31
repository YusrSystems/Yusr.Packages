using System;
using System.Collections.Generic;
using System.Text;

namespace Yusr.eInvoicing.Zatca.Entites
{
    public class ValidationMessage
    {
        public string Type { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
