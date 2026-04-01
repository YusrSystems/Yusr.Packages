using System.Xml;
using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Dto;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;
using Yusr.eInvoicing.Abstractions.Enums;
using Yusr.eInvoicing.Abstractions.Services.Api;
using Yusr.eInvoicing.Abstractions.Services.Xml;

namespace Yusr.Infrastructure.eInvoicing.Zatca.Services
{
    public class ComplianceCheckService(IXmlService xmlService, IEInvoiceApiService eInvoiceApiService) : IComplianceCheckService
    {
        private readonly IXmlService _xmlService = xmlService;
        private readonly IEInvoiceApiService _eInvoiceApiService = eInvoiceApiService;

        public async Task<OperationResult<bool>> GenerateFullCheck(IEInvoicingSetting setting, Branch branch, bool Production = false)
        {
            List<(string invoiceTypeName, EInvoiceType type, bool simplified)> invoices = new List<(string invoiceTypeName, EInvoiceType type, bool simplified)>
            {
                ("فاتورة ضريبية", EInvoiceType.Sell, false),
                ("إشعار دائن", EInvoiceType.Credit, false),
                ("إشعار مدين", EInvoiceType.Debit, false),
                ("فاتورة ضريبية مبسطة", EInvoiceType.Sell, true),
                ("إشعار دائن مبسط", EInvoiceType.Credit, true),
                ("إشعار مدين مبسط", EInvoiceType.Debit, true),
            };

            foreach (var invoice in invoices)
            {
                var res = await SendInvoice(setting, branch, invoice.type, invoice.simplified, Production);
                if (!res.Succeeded)
                {
                    return OperationResult<bool>.CopyErrorsFrom(res);
                }
            }

            return OperationResult<bool>.Ok(true);
        }

        public async Task<OperationResult<bool>> SendInvoice(IEInvoicingSetting setting, Branch branch, EInvoiceType type, bool simplified, bool Production = false)
        {
            var res = GenerateInvoice(setting, branch, type, simplified);

            if (!res.Succeeded || res.Result.xmlInvoice == null || res.Result.xmlSignedInvoice == null)
                return OperationResult<bool>.CopyErrorsFrom(res);

            var sendRes = await _eInvoiceApiService.SendComplianceCheckInvoice(res.Result.xmlSignedInvoice, setting.BinarySecurityToken ?? string.Empty, setting.Secret ?? string.Empty, Production);

            if (!sendRes.Succeeded)
                return OperationResult<bool>.CopyErrorsFrom(sendRes);

            return OperationResult<bool>.Ok(true);
        }

        private OperationResult<(XmlDocument xmlInvoice, XmlDocument xmlSignedInvoice)> GenerateInvoice(IEInvoicingSetting setting, Branch branch, EInvoiceType type, bool simplified)
        {
            var now = DateTime.Now;

            EInvoiceDto eInvoice = new EInvoiceDto
            {
                EInvoiceType = type,
                ProfileID = "reporting:1.0",
                ID = 300,
                CurrencyCode = "SAR",
                IssueDate = now,
                IssueTime = now.ToString("HH:mm:ss"),
                DeliveryDate = DateTime.UtcNow,
                InvoiceCounter = 300,
                PreviousInvoiceHash = "NWZlY2ViNjZmZmM4NmYzOGQ5NTI3ODZjNmQ2OTZjNzljMmRiYzIzOWRkNGU5MWI0NjcyOWQ3M2EyN2ZiNTdlOQ==",
                OriginalInvoiceId = type == EInvoiceType.Sell ? null : 200,
                SupplierCRN = setting.Tenant.Crn ?? string.Empty,
                SupplierVatNumber = setting.Tenant.VatNumber ?? string.Empty,
                SupplierName = setting.Tenant.Name ?? string.Empty,
                SupplierAddress = new EInvoiceAddressDto
                {
                    StreetName = branch.Street ?? string.Empty,
                    BuildingNumber = branch.BuildingNumber ?? string.Empty,
                    CitySubdivisionName = branch.District ?? string.Empty,
                    CityName = branch.City?.Name ?? string.Empty,
                    PostalZone = branch.PostalCode ?? string.Empty,
                    CountryCode = "SA"
                },
                ActionAccountId = 3,
                CustomerName = "عميل",
                TaxAmount = 30.15m,
                LineExtensionAmount = 201.00m,
                TaxExclusiveAmount = 201.00m,
                TaxInclusiveAmount = 231.15m,
                RoundingAmount = 0,
                InvoiceAmount = 231.15m,
                InvoiceLines = [
                    new EInvoiceLineDto
                    {
                        Quantity = 33,
                        NoTaxPrice = 3,
                        NoTaxTotalPrice = 99,
                        TaxPrice = 3.45m,
                        TaxTotalPrice = 113.85m,
                        TaxAmount = 14.85m,
                        Name = "كتاب",
                        Taxable = true,
                        TotalTaxPercent = 15m,
                        AllowanceChargeAmount = 0,
                    },
                    new EInvoiceLineDto
                    {
                        Quantity = 3,
                        NoTaxPrice = 34,
                        NoTaxTotalPrice = 102,
                        TaxPrice = 39.1m,
                        TaxTotalPrice = 117.30m,
                        TaxAmount = 15.30m,
                        Name = "قلم",
                        Taxable = true,
                        TotalTaxPercent = 15m,
                        AllowanceChargeAmount = 0,
                    }
                ]
            };

            if (!simplified)
            {
                eInvoice.CustomerVatNumber = "399999999800003";
                eInvoice.CustomerName = "شركة نماذج فاتورة المحدودة";
                eInvoice.CustomerAddress = new EInvoiceAddressDto
                {
                    StreetName = "صلاح الدين",
                    BuildingNumber = "1111",
                    CitySubdivisionName = "المروج",
                    CityName = "الرياض",
                    PostalZone = "12222",
                    CountryCode = "SA"
                };
            }

            return _xmlService.CreateFullXml(eInvoice, setting.CertificateContent ?? string.Empty, setting.PrivateKey ?? string.Empty);
        }
    }
}