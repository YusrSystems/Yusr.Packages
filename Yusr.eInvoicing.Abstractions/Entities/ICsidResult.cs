namespace Yusr.eInvoicing.Abstractions.Entities
{
    internal interface ICsidResult
    {
        public long RequestId { get; set; }
        public string DispositionMessage { get; set; }
        public string BinarySecurityToken { get; set; }
        public string Secret { get; set; }
    }
}
