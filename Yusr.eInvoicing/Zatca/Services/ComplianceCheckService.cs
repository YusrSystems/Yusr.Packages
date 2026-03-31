using System.Xml;
using Yusr.Core.Abstractions.Entities;
using Yusr.Erp.Application.Accounting.DTOs;
using Yusr.Erp.Core.Entities.Common;
using Yusr.Erp.Core.Enums;
using Yusr.Identity.Abstractions.Primitives;


namespace Yusr.Infrastructure.eInvoicing.Zatca.Services
{
    public class ComplianceCheckService
    {
        public static async Task<(bool IsValid, string ErrorMessage)> GenerateFullCheck(JwtClaims jwtClaims, Tenant tenant, Setting setting, Branch branch, bool Production = false)
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
                var res = await SendInvoice(jwtClaims, tenant, setting, branch, invoice.type, invoice.simplified, Production);
                if (!res.IsValid)
                {
                    return (false, invoice.invoiceTypeName + " - " + res.ErrorMessage);
                }
            }

            return (true, "");
        }

        public static async Task<(bool IsValid, string ErrorMessage)> SendInvoice(JwtClaims jwtClaims, Tenant tenant, Setting setting, Branch branch, EInvoiceType type, bool simplified, bool Production = false)
        {
            var res = GenerateInvoice(jwtClaims, tenant, setting, branch, type, simplified);

            if (!res.Success || res.xmlInvoice == null || res.xmlSignedInvoice == null)
            {
                return (false, res.ErrorMessage);
            }

            var sendRes = await ZatcaApi.SendComplianceCheckInvoice(res.xmlSignedInvoice, setting.BinarySecurityToken ?? string.Empty, setting.Secret ?? string.Empty, Production);

            return (sendRes.IsValid, sendRes.ErrorMessage);
        }

        private static (bool Success, string ErrorMessage, XmlDocument? xmlInvoice, XmlDocument? xmlSignedInvoice) GenerateInvoice(JwtClaims jwtClaims, Tenant tenant, Setting setting, Branch branch, EInvoiceType type, bool simplified)
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
                SupplierCRN = tenant.Crn ?? string.Empty,
                SupplierVatNumber = tenant.VatNumber ?? string.Empty,
                SupplierName = tenant.Name ?? string.Empty,
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

            return XmlService.CreateFullXml(eInvoice, jwtClaims, setting.CertificateContent ?? string.Empty, setting.PrivateKey ?? string.Empty);
        }
    }
}
