using System;
using System.Collections.Generic;
using System.Text;
using Yusr.Infrastructure.eInvoicing.Zatca.Services;

namespace Yusr.eInvoicing.Zatca.Entites
{
    public class InvoiceSubmissionResponse
    {
        public string InvoiceHash { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<string> Warnings { get; set; } = new List<string>();
        public List<InvoiceError> Errors { get; set; } = new List<InvoiceError>();
    }
}
