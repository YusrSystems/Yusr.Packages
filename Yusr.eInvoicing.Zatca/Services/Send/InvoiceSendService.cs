using System.Text;
using System.Xml;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Dto;
using Yusr.eInvoicing.Abstractions.Entities;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;
using Yusr.eInvoicing.Abstractions.Enums;
using Yusr.eInvoicing.Abstractions.Services.Api;
using Yusr.eInvoicing.Abstractions.Services.Send;
using Yusr.eInvoicing.Abstractions.Services.Xml;

namespace Yusr.eInvoicing.Zatca.Services.Send
{
    public class InvoiceSendService(
        IEInvoiceXmlService xmlService,
        IEInvoiceApiService eInvoiceApiService
    ) : IEInvoiceSendService
    {
        public async Task<OperationResult<EInvoiceStatus>> SendEInvoice(EInvoicePrepareDto eInvoicePrepareDto, EInvoicingEnvironmentType eInvoicingEnvironmentType, string binarySecurityToken, string secret)
        {
            return await SendEInvoice(eInvoicePrepareDto.InvoiceRequest, eInvoicingEnvironmentType, binarySecurityToken, secret, eInvoicePrepareDto.Simplified);
        }

        public async Task<OperationResult<EInvoiceStatus>> SendEInvoice(EInvoiceRequest invoiceRequest, EInvoicingEnvironmentType eInvoicingEnvironmentType, string binarySecurityToken, string secret, bool isSimplified)
        {
            OperationResult<EInvoicingApiResponse> zatcaApiResponse;

            if (isSimplified)
                zatcaApiResponse = await eInvoiceApiService.SendSimplifiedInvoice(invoiceRequest, binarySecurityToken, secret, eInvoicingEnvironmentType);
            else
                zatcaApiResponse = await eInvoiceApiService.SendStandardInvoice(invoiceRequest, binarySecurityToken, secret, eInvoicingEnvironmentType);

            if (!zatcaApiResponse.Succeeded)
                return OperationResult<EInvoiceStatus>.ValidationError("لم ترسل الفاتورة إلى الهيئة بشكل صحيح", $"[الأخطاء]:{zatcaApiResponse.ErrorMessage}, [التحذيرات]: {zatcaApiResponse.WarningMessage}");

            EInvoiceStatus eInvoiceStatus;
            if (!string.IsNullOrEmpty(zatcaApiResponse.ErrorMessage))
                eInvoiceStatus = EInvoiceStatus.NotSent;
            else if (!string.IsNullOrEmpty(zatcaApiResponse.WarningMessage))
                eInvoiceStatus = EInvoiceStatus.SentWithWarnings;
            else
                eInvoiceStatus = EInvoiceStatus.SentCorrectly;

            return OperationResult<EInvoiceStatus>.Ok(eInvoiceStatus);
        }

        public async Task<OperationResult<EInvoiceStatus>> ResendEInvoiceAsync(IEInvoiceSetting setting, IEInvoiceInvoice invoice)
        {
            if (!invoice.IsSendableEInvoice(setting))
                return OperationResult<EInvoiceStatus>.ValidationError("هذه الفاتورة غير صالحة للإرسال كفاتورة إلكترونية", "");

            byte[] xmlBytes = Convert.FromBase64String(invoice.SignedXml ?? string.Empty);
            string xmlString = Encoding.UTF8.GetString(xmlBytes);
            XmlDocument xmlDoc = new();
            xmlDoc.LoadXml(xmlString);

            EInvoiceRequest invoiceRequest = new()
            {
                Invoice = invoice.SignedXml ?? string.Empty,
                InvoiceHash = invoice.InvoiceHash ?? string.Empty,
                Uuid = xmlService.ExtractValue(xmlDoc, "//cbc:UUID") ?? string.Empty,
            };

            string type = xmlService.ExtractValue(xmlDoc, "//cbc:InvoiceTypeCode/@name") ?? string.Empty;
            bool isSimplified = type.Equals("0200000");

            return await SendEInvoice(invoiceRequest, setting.EInvoicingEnvironmentType, setting.BinarySecurityToken!, setting.Secret!, isSimplified);
        }
    }
}
