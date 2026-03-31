using Yusr.Infrastructure.eInvoicing.Zatca.Services;

namespace Yusr.eInvoicing.Zatca.Entites
{
    public class ComplianceValidationResponse
    {
        public ValidationResults ValidationResults { get; set; } = new ValidationResults();
        public string ReportingStatus { get; set; } = string.Empty;
        public string ClearanceStatus { get; set; } = string.Empty;
        public string ClearedInvoice { get; set; } = string.Empty;
    }
}
