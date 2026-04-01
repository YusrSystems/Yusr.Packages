namespace Yusr.eInvoicing.Zatca.Entities
{
    public class ComplianceValidationResponse
    {
        public ValidationResults ValidationResults { get; set; } = new ValidationResults();
        public string ReportingStatus { get; set; } = string.Empty;
        public string ClearanceStatus { get; set; } = string.Empty;
        public string ClearedInvoice { get; set; } = string.Empty;
    }
}
