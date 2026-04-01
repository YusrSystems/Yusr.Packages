using System.Xml;
using Yusr.Core.Abstractions.Primitives;
using Yusr.Identity.Abstractions.Primitives;

namespace Yusr.eInvoicing.Abstractions.Services.Signing
{
    public interface ISignService
    {
        OperationResult<XmlDocument> SignInvoice(JwtClaims jwtClaims, XmlDocument xmlInvoice, string certificateContent, string PrivateKey);
    }
}
