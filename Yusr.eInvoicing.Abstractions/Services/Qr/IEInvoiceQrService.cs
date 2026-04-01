using System.Xml;

namespace Yusr.eInvoicing.Abstractions.Services.Qr
{
    public interface IEInvoiceQrService
    {
        string ExtractQrValue(XmlDocument xmlInvoice);

        string GenerateQrBase64(string companyName, string companyVatNumber, string timeStamp, string totalWithVat, string vatAmount);
        byte[] GenerateQrCode(string base64Tlv);
    }
}
