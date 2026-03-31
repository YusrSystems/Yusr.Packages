using System.Text;
using System.Xml;
using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Enums;
using Yusr.Core.Abstractions.Primitives;
using Yusr.Erp.Application.Accounting.DTOs;
using Yusr.Erp.Application.Accounting.Interfaces;
using Yusr.Erp.Core.Entities.Accounting;
using Yusr.Erp.Core.Entities.Common;
using Yusr.Erp.Core.Entities.Inventory;
using Yusr.Erp.Core.Enums;
using Yusr.Erp.Core.Interfaces.Common;
using Yusr.Erp.Core.Utilities;
using Yusr.Identity.Abstractions.Primitives;
using Yusr.Infrastructure.eInvoicing.Zatca.Extensions;
using Yusr.Infrastructure.eInvoicing.Zatca.Services;
using ZATCA.EInvoice.SDK;

namespace Yusr.Infrastructure.eInvoicing.Zatca
{
    public class ZatcaService(ISettingsRepository settingsRepo, CsrService csrService, CsidService csidService) : IEInvoicingService
    {
        private readonly ISettingsRepository _settingsRepo = settingsRepo;
        private readonly CsrService _csrService = csrService;
        private readonly CsidService _csidService = csidService;

        public OperationResult<EInvoicePrepareDto> PrepareEInvoice(EInvoiceDto eInvoice, string certificateContent, string privateKey, bool ignoreWarnings, JwtClaims jwtClaims)
        {
            var validateRes = ValidateEInvoice(eInvoice, ignoreWarnings, jwtClaims);
            if (validateRes.ResultType != ResultType.Ok)
                return OperationResult<EInvoicePrepareDto>.CopyErrorsFrom(validateRes);

            var eInvoiceXmlResult = XmlService.CreateFullXml(eInvoice, jwtClaims, certificateContent, privateKey);
            if (!eInvoiceXmlResult.Success || eInvoiceXmlResult.xmlSignedInvoice == null)
                return OperationResult<EInvoicePrepareDto>.InternalError("لم يتم إنشاء ملف (XML) للفاتورة الإلكترونية بشكل صحيح", eInvoiceXmlResult.ErrorMessage);

            var requestResult = new RequestGenerator().GenerateRequest(eInvoiceXmlResult.xmlSignedInvoice);
            if (!requestResult.IsValid)
                return OperationResult<EInvoicePrepareDto>.InternalError("لم يتم إصدار الطلب بنجاح", requestResult.ErrorMessages[0]);

            string qrBase64 = QrService.ExtractQrValue(eInvoiceXmlResult.xmlSignedInvoice);

            return OperationResult<EInvoicePrepareDto>.Ok(new EInvoicePrepareDto
            {
                InvoiceRequest = requestResult.InvoiceRequest.ToEInvoiceRequest(),
                QrBase64 = qrBase64,
                Simplified = string.IsNullOrEmpty(eInvoice.CustomerVatNumber),
            });
        }

        public OperationResult<bool> ValidateEInvoice(EInvoiceDto eInvoice, bool IgnoreWarnings, JwtClaims jwtClaims)
        {
            var validationResult = ValidationService.ValidateInvoice(eInvoice);

            StringBuilder stringBuilder = new StringBuilder();
            if (validationResult.Errors.Count > 0)
            {
                validationResult.Errors.ForEach(v => stringBuilder.AppendLine(v));
                return OperationResult<bool>.ValidationError("الفاتورة تحتوي على أخطاء", stringBuilder.ToString());
            }

            if (validationResult.Warnings.Count > 0 && !IgnoreWarnings)
            {
                validationResult.Warnings.ForEach(v => stringBuilder.AppendLine(v));
                return OperationResult<bool>.ValidationWarning("الفاتورة تحتوي على تحذيرات", stringBuilder.ToString());
            }

            return OperationResult<bool>.Ok(true);
        }

        public async Task<OperationResult<EInvoiceStatus>> SendEInvoice(EInvoicePrepareDto eInvoicePrepareDto, EInvoicingStatus eInvoicingStatus, string binarySecurityToken, string secret, JwtClaims jwtClaims)
        {
            return await SendEInvoice(eInvoicePrepareDto.InvoiceRequest, eInvoicingStatus, binarySecurityToken, secret, eInvoicePrepareDto.Simplified, jwtClaims);
        }

        public async Task<OperationResult<EInvoiceStatus>> SendEInvoice(EInvoiceRequest invoiceRequest, EInvoicingStatus eInvoicingStatus, string binarySecurityToken, string secret, bool isSimplified, JwtClaims jwtClaims)
        {
            EInvoiceStatus eInvoiceStatus = EInvoiceStatus.NotSent;

            ZatcaApiRespone zatcaApiResponse = new ZatcaApiRespone();
            bool production = eInvoicingStatus == EInvoicingStatus.Production;
            if (isSimplified)
                zatcaApiResponse = await ZatcaApi.SendSimplifiedInvoice(invoiceRequest, binarySecurityToken, secret, production);
            else
                zatcaApiResponse = await ZatcaApi.SendStandardInvoice(invoiceRequest, binarySecurityToken, secret, production);

            if (!zatcaApiResponse.IsValid)
                return OperationResult<EInvoiceStatus>.ValidationError("لم ترسل الفاتورة إلى الهيئة بشكل صحيح", $"[الأخطاء]:{zatcaApiResponse.ErrorMessage}, [التحذيرات]: {zatcaApiResponse.WarningMessage}");

            if (!string.IsNullOrEmpty(zatcaApiResponse.ErrorMessage))
                eInvoiceStatus = EInvoiceStatus.NotSent;
            else if (!string.IsNullOrEmpty(zatcaApiResponse.WarningMessage))
                eInvoiceStatus = EInvoiceStatus.SentWithWarnings;
            else
                eInvoiceStatus = EInvoiceStatus.SentCorrectly;

            return OperationResult<EInvoiceStatus>.Ok(eInvoiceStatus);
        }

        public async Task<OperationResult<EInvoiceStatus>> ResendEInvoiceAsync(Invoice invoice, JwtClaims jwtClaims)
        {
            var settings = await _settingsRepo.GetSettingsAsync();
            if (settings == null || settings.BinarySecurityToken == null || settings.Secret == null)
                return OperationResult<EInvoiceStatus>.BadRequest("لم يتم الحصول على الاعدادات بشكل صحيح");

            byte[] xmlBytes = Convert.FromBase64String(invoice.SignedXml ?? string.Empty);
            string xmlString = Encoding.UTF8.GetString(xmlBytes);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            EInvoiceRequest invoiceRequest = new EInvoiceRequest
            {
                Invoice = invoice.SignedXml ?? string.Empty,
                InvoiceHash = invoice.InvoiceHash ?? string.Empty,
                Uuid = ExtractValueFromXml(xmlDoc, "//cbc:UUID") ?? string.Empty,
            };

            string type = ExtractValueFromXml(xmlDoc, "//cbc:InvoiceTypeCode/@name") ?? string.Empty;
            bool isSimplified = type.Equals("0200000");

            return await SendEInvoice(invoiceRequest, settings.EInvoicingStatus, settings.BinarySecurityToken, settings.Secret, isSimplified, jwtClaims);
        }

        public string? GenerateQrBase64(string CompanyName, string CompanyVatNumber, string TimeStamp, string TotalWithVat, string VatAmount)
        {
            return QrService.GenerateQrBase64(CompanyName, CompanyVatNumber, TimeStamp, TotalWithVat, VatAmount);
        }

        public string? ExtractValueFromXml(XmlDocument signedXml, string xpath)
        {
            return XmlService.ExtractValue(signedXml, xpath);
        }

        public byte[] GenerateQrCode(string base64Tlv)
        {
            return QrService.GenerateQrCode(base64Tlv);
        }

        public async Task<OperationResult<bool>> LinkEInvoicing(string OTP, bool Production, JwtClaims jwtClaims)
        {
            try
            {
                Setting? settings = await _settingsRepo.GetSettingsAsync();
                if (settings == null)
                    return OperationResult<bool>.BadRequest("لم يتم الحصول على معلومات المؤسسة بشكل صحيح");

                var generateCsrResult = await _csrService.TryGenerateCsr(jwtClaims, settings.Tenant, settings.Branch!, Production);
                if (!generateCsrResult.IsValid || generateCsrResult.csrResult == null)
                {
                    return OperationResult<bool>.InternalError("فشل إنشاء طلب الشهادة الرقمية", generateCsrResult.ErrorMessage ?? string.Empty);
                }

                var complianceCsidResult = await _csidService.TryRequestComplianceCsidAsync(OTP, generateCsrResult.csrResult, Production);
                if (!complianceCsidResult.IsValid || complianceCsidResult.CsidResponse == null)
                {
                    return OperationResult<bool>.InternalError("لم يتم اصدار شهادة الامتثال (SCID) بشكل صحيح", complianceCsidResult.ErrorMessage ?? string.Empty);
                }

                ZatcaParams zatcaParams = new ZatcaParams(complianceCsidResult.CsidResponse, generateCsrResult.csrResult);

                OperationResult<bool> StoreComplianceCsidResult = await _csidService.StoreCsid(jwtClaims, complianceCsidResult.CsidResponse, zatcaParams.certificateContent, Production);
                if (StoreComplianceCsidResult.ResultType != ResultType.Ok)
                {
                    return OperationResult<bool>.InternalError("لم يتم حفظ شهادة الامتثال بشكل صحيح", StoreComplianceCsidResult.ErrorMessage ?? string.Empty);
                }

                var ComplianceCheckResult = await ComplianceCheckService.GenerateFullCheck(jwtClaims, settings.Tenant, settings, settings.Branch, Production);
                if (!ComplianceCheckResult.IsValid)
                {
                    return OperationResult<bool>.InternalError("لم يتم التحقق من الامتثال بشكل صحيح", ComplianceCheckResult.ErrorMessage ?? string.Empty);
                }

                var productionCsidResult = await _csidService.TryRequestProductionCsidAsync(complianceCsidResult.CsidResponse!, Production);
                if (!productionCsidResult.IsValid || productionCsidResult.CsidResponse == null)
                {
                    return OperationResult<bool>.InternalError("لم يتم اصدار شهادة الإنتاج بشكل صحيح", productionCsidResult.ErrorMessage ?? string.Empty);
                }

                byte[] pcsidCertBytes = Convert.FromBase64String(productionCsidResult.CsidResponse.binarySecurityToken);
                string certificateContent = Encoding.UTF8.GetString(pcsidCertBytes);
                OperationResult<bool> StoreProductionCsidResult = await _csidService.StoreCsid(jwtClaims, productionCsidResult.CsidResponse, certificateContent, Production);

                return StoreProductionCsidResult;
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.InternalError("لم يتم الحصول على معلومات المؤسسة بشكل صحيح", ex.Message);
            }
        }

        public EInvoiceDto GetEInvoiceData(Invoice invoice, Tenant tenant, Branch branch, Account customer, List<Item> dbItems, long? lastCounter, string? lastHash)
        {
            var result = new EInvoiceDto
            {
                ProfileID = "reporting:1.0",
                ID = invoice.Id,
                IssueDate = invoice.InvoiceDate,
                IssueTime = invoice.InvoiceDate.ToString("HH:mm:ss"),
                DeliveryDate = invoice.InvoiceDate,
                OriginalInvoiceId = invoice.OriginalInvoiceId,
                InvoiceAmount = invoice.FullAmount,
                EInvoiceType = invoice.InvoiceType == InvoiceType.SellReturn ? EInvoiceType.Credit : EInvoiceType.Sell
            };

            result.CurrencyCode = tenant.Currency?.Code ?? "";
            result.SupplierCRN = tenant.Crn ?? "";
            result.SupplierVatNumber = tenant.VatNumber ?? "";
            result.SupplierName = tenant.Name ?? "";
            result.SupplierAddress = new EInvoiceAddressDto
            {
                StreetName = branch.Street ?? "",
                BuildingNumber = branch.BuildingNumber ?? "",
                CityName = branch.City?.Name ?? "",
                PostalZone = branch.PostalCode ?? "",
                CountryCode = branch.City?.Country?.Code ?? "",
                CitySubdivisionName = branch.District ?? ""
            };

            result.CustomerName = customer.AccountName;
            result.ActionAccountId = customer.Id;
            result.CustomerVatNumber = customer.VatNumber ?? "";
            result.CustomerCRN = customer.Crn ?? "";
            result.CustomerAddress = new EInvoiceAddressDto
            {
                StreetName = customer.Street ?? "",
                BuildingNumber = customer.BuildingNumber ?? "",
                CityName = branch.City?.Name ?? "",
                PostalZone = customer.PostalCode ?? "",
                CountryCode = branch.City?.Country?.Code ?? "",
                CitySubdivisionName = customer.District ?? ""
            };

            var dbItemsDic = dbItems.ToDictionary(i => i.Id);

            var lines = new List<EInvoiceLineDto>();

            decimal totalTaxInclusive = 0;
            decimal totalTaxExclusive = 0;
            decimal totalTaxAmount = 0;

            foreach (var itemDto in invoice.InvoiceItems)
            {
                if (!dbItemsDic.TryGetValue(itemDto.ItemId, out var dbItem)) continue;

                bool isTaxable = dbItem.Taxable;
                decimal taxPerc = itemDto.TotalTaxesPerc;
                decimal price = itemDto.Price;
                decimal discount = itemDto.Discount;
                decimal qty = itemDto.Quantity;
                decimal totalPrice = itemDto.TotalPrice;

                decimal taxFactor = (!isTaxable || taxPerc == 0) ? 1 : TaxHelper.GetTaxFactor(taxPerc);

                decimal noTaxPrice = YusrMath.Round((price - discount) / taxFactor);
                decimal taxPrice = YusrMath.Round(price - discount);
                decimal noTaxTotalPrice = YusrMath.Round(totalPrice / taxFactor);
                decimal taxTotalPrice = totalPrice;

                decimal lineTaxAmount = YusrMath.Round(taxTotalPrice - noTaxTotalPrice);
                decimal allowanceCharge = YusrMath.Round((noTaxPrice * qty) - noTaxTotalPrice);

                totalTaxInclusive += taxTotalPrice;
                totalTaxExclusive += noTaxTotalPrice;
                totalTaxAmount += lineTaxAmount;

                lines.Add(new EInvoiceLineDto
                {
                    Name = dbItem.ItemName,
                    TaxExemptionReasonCode = dbItem.ExemptionReasonCode ?? "",
                    TaxExemptionReason = dbItem.ExemptionReason ?? "",
                    Taxable = isTaxable,
                    Quantity = qty,
                    TotalTaxPercent = taxPerc,
                    NoTaxPrice = noTaxPrice,
                    TaxPrice = taxPrice,
                    NoTaxTotalPrice = noTaxTotalPrice,
                    TaxTotalPrice = taxTotalPrice,
                    TaxAmount = lineTaxAmount,
                    AllowanceChargeAmount = allowanceCharge
                });
            }

            result.InvoiceLines = lines;

            string initHash = "NWZlY2ViNjZmZmM4NmYzOGQ5NTI3ODZjNmQ2OTZjNzljMmRiYzIzOWRkNGU5MWI0NjcyOWQ3M2EyN2ZiNTdlOQ==";

            result.InvoiceCounter = (lastCounter ?? 0) + 1;
            result.PreviousInvoiceHash = lastHash ?? initHash;

            result.LineExtensionAmount = YusrMath.Round(totalTaxExclusive);
            result.TaxExclusiveAmount = YusrMath.Round(totalTaxExclusive);
            result.TaxInclusiveAmount = YusrMath.Round(totalTaxInclusive);
            result.TaxAmount = YusrMath.Round(totalTaxAmount);
            result.RoundingAmount = YusrMath.Round(invoice.FullAmount - totalTaxInclusive);

            return result;
        }



        public async Task<OperationResult<EInvoicePrepareDto?>> PrepareInvoiceAsync(Tenant tenant, Setting setting, Branch branch, Invoice invoice, Account actionAccount, List<Item> dbItems, long? lastCounter, string? lastHash, bool ignoreWarnings, JwtClaims jwtClaims)
        {
            EInvoicePrepareDto? eInvoicePrepareDto = null;

            if (Invoice.IsSendableEInvoice(invoice.InvoiceType, setting))
            {
                var eInvoice = GetEInvoiceData(invoice, tenant, branch, actionAccount, dbItems, lastCounter, lastHash);
                var prepareRes = PrepareEInvoice(eInvoice, setting.CertificateContent!, setting.PrivateKey!, ignoreWarnings, jwtClaims);
                if (prepareRes.ResultType != ResultType.Ok)
                {
                    return OperationResult<EInvoicePrepareDto?>.CopyErrorsFrom(prepareRes);
                }

                eInvoicePrepareDto = prepareRes.Result;
                invoice.UpdateEInvoiceInfo(eInvoice.InvoiceCounter, eInvoicePrepareDto.InvoiceRequest.InvoiceHash,
                eInvoice.PreviousInvoiceHash, eInvoicePrepareDto.InvoiceRequest.Invoice, eInvoicePrepareDto.QrBase64);
            }
            else
            {
                decimal taxAmount = YusrMath.Round(invoice.InvoiceItems
                    .Select(ii => new
                    {
                        TotalPrice = ii.TotalPrice,
                        TaxFactor = (!ii.Taxable || ii.TotalTaxesPerc == 0) ? 1 : TaxHelper.GetTaxFactor(ii.TotalTaxesPerc)
                    })
                    .Select(x => new
                    {
                        TaxTotalPrice = x.TotalPrice,
                        NoTaxTotalPrice = YusrMath.Round(x.TotalPrice / x.TaxFactor)
                    })
                    .Sum(x => x.TaxTotalPrice - x.NoTaxTotalPrice));

                invoice.UpdateQr(GenerateQrBase64(tenant.Name ?? string.Empty,
                    tenant.VatNumber ?? string.Empty,
                    invoice.InvoiceDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    invoice.FullAmount.ToString(),
                    taxAmount.ToString()
                ));
            }

            return OperationResult<EInvoicePrepareDto?>.Ok(eInvoicePrepareDto);
        }


    }
}
