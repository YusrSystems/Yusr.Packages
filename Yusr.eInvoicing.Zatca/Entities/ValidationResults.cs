namespace Yusr.eInvoicing.Zatca.Entities
{
    public class ValidationResults
    {
        public List<ValidationMessage> InfoMessages { get; set; } = [];
        public List<ValidationMessage> WarningMessages { get; set; } = [];
        public List<ValidationMessage> ErrorMessages { get; set; } = [];
        public string Status { get; set; } = string.Empty;
    }
}
