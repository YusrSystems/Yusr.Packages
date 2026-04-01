using System;
using System.Collections.Generic;
using System.Text;
using Yusr.Infrastructure.eInvoicing.Zatca.Services;

namespace Yusr.eInvoicing.Zatca.Entities
{
    public class ValidationResults
    {
        public List<ValidationMessage> InfoMessages { get; set; } = new List<ValidationMessage>();
        public List<ValidationMessage> WarningMessages { get; set; } = new List<ValidationMessage>();
        public List<ValidationMessage> ErrorMessages { get; set; } = new List<ValidationMessage>();
        public string Status { get; set; } = string.Empty;
    }
}
