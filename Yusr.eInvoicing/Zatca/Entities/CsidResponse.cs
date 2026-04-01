namespace Yusr.eInvoicing.Zatca.Entities
{

    public class CsidResponse
    {
        public long requestID { get; set; }
        public string dispositionMessage { get; set; } = "";
        public string binarySecurityToken { get; set; } = "";
        public string secret { get; set; } = "";
    }
}
