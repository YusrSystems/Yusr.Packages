using Yusr.eInvoicing.Abstractions.Entities.Interfaces;
using ZATCA.EInvoice.SDK.Contracts.Models;

namespace Yusr.eInvoicing.Zatca.Entities
{
    public class ZatcaCsrResult(CsrResult csrResult) : ICsrResult
    {
        private readonly CsrResult _csrResult = csrResult;

        public string Csr { get => _csrResult.Csr; set => _csrResult.Csr = value; }
        public string PrivateKey { get => _csrResult.PrivateKey; set => _csrResult.PrivateKey = value; }
    }
}