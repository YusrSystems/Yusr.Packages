using QRCoder;
using System.Text;
using System.Xml;
using Yusr.eInvoicing.Abstractions.Services.Qr;

namespace Yusr.eInvoicing.Zatca.Services.Qr
{
    public class QrService : IEInvoiceQrService
    {
        public string ExtractQrValue(XmlDocument xmlInvoice)
        {
            var nsmgr = new XmlNamespaceManager(xmlInvoice.NameTable);
            nsmgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            nsmgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");

            var qrNode = xmlInvoice.SelectSingleNode("//cac:AdditionalDocumentReference[cbc:ID='QR']/cac:Attachment/cbc:EmbeddedDocumentBinaryObject", nsmgr);

            if (qrNode == null)
            {
                return string.Empty;
            }

            return qrNode.InnerText.Trim();
        }

        public string GenerateQrBase64(string companyName, string companyVatNumber, string timeStamp, string totalWithVat, string vatAmount)
        {
            var tlvBytes = new byte[][]
            {
                EncodeTLV(1, companyName),
                EncodeTLV(2, companyVatNumber),
                EncodeTLV(3, timeStamp),
                EncodeTLV(4, totalWithVat),
                EncodeTLV(5, vatAmount)
            };

            var allBytes = CombineArrays(tlvBytes);

            return Convert.ToBase64String(allBytes);
        }
        public byte[] GenerateQrCode(string base64Tlv)
        {
            QRCodeGenerator qrGen = new QRCodeGenerator();
            QRCodeData qrData = qrGen.CreateQrCode(base64Tlv, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrData);
            return qrCode.GetGraphic(20);
        }


        private static byte[] EncodeTLV(int tag, string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            var tlv = new byte[2 + valueBytes.Length];
            tlv[0] = (byte)tag;
            tlv[1] = (byte)valueBytes.Length;
            Buffer.BlockCopy(valueBytes, 0, tlv, 2, valueBytes.Length);
            return tlv;
        }

        private static byte[] CombineArrays(byte[][] arrays)
        {
            int totalLength = 0;
            foreach (var arr in arrays)
            {
                totalLength += arr.Length;
            }

            var result = new byte[totalLength];
            int offset = 0;
            foreach (var arr in arrays)
            {
                Buffer.BlockCopy(arr, 0, result, offset, arr.Length);
                offset += arr.Length;
            }
            return result;
        }
    }
}
