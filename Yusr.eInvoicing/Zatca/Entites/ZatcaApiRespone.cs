using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Yusr.eInvoicing.Zatca.Entites
{
    public class ZatcaApiRespone
    {
        public bool IsValid { get; set; }
        public HttpStatusCode ResultCode { get; set; }
        public string ErrorMessage { get; set; } = "";
        public string WarningMessage { get; set; } = "";
        public string ClearedInvoice { get; set; } = string.Empty;
    }
}
