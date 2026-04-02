using System.Text;
using Yusr.Core.Abstractions.Enums;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Dto;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;
using Yusr.eInvoicing.Abstractions.Services.Initialization;
using Yusr.eInvoicing.Abstractions.Services.Mapper;
using Yusr.eInvoicing.Abstractions.Services.Qr;
using Yusr.eInvoicing.Abstractions.Services.Validation;
using Yusr.eInvoicing.Abstractions.Services.Xml;
using Yusr.eInvoicing.Zatca.Extensions;
using ZATCA.EInvoice.SDK;

namespace Yusr.eInvoicing.Zatca.Services.Initialization
{
    public class EInvoicingInitializationService(
        IEInvoiceXmlService xmlService,
        IEInvoiceQrService qrService,
        IEInvoiceValidationService validationService,
        IInvoiceMapperService invoiceMapperService
        ) : IEInvoiceInitializationService
    {
        public OperationResult<EInvoicePrepareDto?> Init(IEInvoiceSetting setting, IEInvoiceInvoice invoice, IEInvoiceAccount actionAccount, List<IEInvoiceItem> dbItems, long? lastCounter, string? lastHash, bool ignoreWarnings)
        {
            if (!invoice.IsSendableEInvoice(setting))
                return OperationResult<EInvoicePrepareDto?>.Ok(null);

            var eInvoice = invoiceMapperService.GetEInvoiceData(setting, invoice, actionAccount, dbItems, lastCounter, lastHash);

            var validateRes = ValidateEInvoice(eInvoice, ignoreWarnings);
            if (validateRes.ResultType != ResultType.Ok)
                return OperationResult<EInvoicePrepareDto?>.CopyErrorsFrom(validateRes);

            var eInvoiceXmlResult = xmlService.CreateFullXml(eInvoice, setting.CertificateContent!, setting.PrivateKey!);
            if (!eInvoiceXmlResult.Succeeded || eInvoiceXmlResult.Result.xmlSignedInvoice == null)
                return OperationResult<EInvoicePrepareDto?>.InternalError("لم يتم إنشاء ملف (XML) للفاتورة الإلكترونية بشكل صحيح", eInvoiceXmlResult.ErrorMessage);

            var requestResult = new RequestGenerator().GenerateRequest(eInvoiceXmlResult.Result.xmlSignedInvoice);
            if (!requestResult.IsValid)
                return OperationResult<EInvoicePrepareDto?>.InternalError("لم يتم إصدار الطلب بنجاح", requestResult.ErrorMessages[0]);

            string qrBase64 = qrService.ExtractQrValue(eInvoiceXmlResult.Result.xmlSignedInvoice);

            return OperationResult<EInvoicePrepareDto?>.Ok(new EInvoicePrepareDto
            {
                InvoiceRequest = requestResult.InvoiceRequest.ToEInvoiceRequest(),
                QrBase64 = qrBase64,
                Simplified = string.IsNullOrEmpty(eInvoice.CustomerVatNumber),
            });
        }

        public OperationResult<bool> ValidateEInvoice(EInvoiceDto eInvoice, bool IgnoreWarnings)
        {
            var (Errors, Warnings) = validationService.ValidateInvoice(eInvoice);

            StringBuilder stringBuilder = new();
            if (Errors.Count > 0)
            {
                Errors.ForEach(v => stringBuilder.AppendLine(v));
                return OperationResult<bool>.ValidationError("الفاتورة تحتوي على أخطاء", stringBuilder.ToString());
            }

            if (Warnings.Count > 0 && !IgnoreWarnings)
            {
                Warnings.ForEach(v => stringBuilder.AppendLine(v));
                return OperationResult<bool>.ValidationWarning("الفاتورة تحتوي على تحذيرات", stringBuilder.ToString());
            }

            return OperationResult<bool>.Ok(true);
        }
    }
}
