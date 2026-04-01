using Yusr.Core.Abstractions.Utilities;
using Yusr.eInvoicing.Abstractions.Dto;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;
using Yusr.eInvoicing.Abstractions.Enums;
using Yusr.eInvoicing.Abstractions.Services.Mapper;

namespace Yusr.eInvoicing.Zatca.Services
{
    public class InvoiceMapperService : IInvoiceMapperService
    {
        public EInvoiceDto GetEInvoiceData(IEInvoicingSetting setting, IInvoice invoice, IAccount customer, List<IItem> dbItems, long? lastCounter, string? lastHash)
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

            result.CurrencyCode = setting.Tenant.Currency?.Code ?? "";
            result.SupplierCRN = setting.Tenant.Crn ?? "";
            result.SupplierVatNumber = setting.Tenant.VatNumber ?? "";
            result.SupplierName = setting.Tenant.Name ?? "";
            result.SupplierAddress = new EInvoiceAddressDto
            {
                StreetName = setting.Branch.Street ?? "",
                BuildingNumber = setting.Branch.BuildingNumber ?? "",
                CityName = setting.Branch.City?.Name ?? "",
                PostalZone = setting.Branch.PostalCode ?? "",
                CountryCode = setting.Branch.City?.Country?.Code ?? "",
                CitySubdivisionName = setting.Branch.District ?? ""
            };

            result.CustomerName = customer.Name;
            result.ActionAccountId = customer.Id;
            result.CustomerVatNumber = customer.VatNumber ?? "";
            result.CustomerCRN = customer.Crn ?? "";
            result.CustomerAddress = new EInvoiceAddressDto
            {
                StreetName = customer.Street ?? "",
                BuildingNumber = customer.BuildingNumber ?? "",
                CityName = setting.Branch.City?.Name ?? "",
                PostalZone = customer.PostalCode ?? "",
                CountryCode = setting.Branch.City?.Country?.Code ?? "",
                CitySubdivisionName = customer.District ?? ""
            };

            var dbItemsDic = dbItems.ToDictionary(i => i.Id);

            var lines = new List<EInvoiceLineDto>();

            decimal totalTaxInclusive = 0;
            decimal totalTaxExclusive = 0;
            decimal totalTaxAmount = 0;

            foreach (var itemDto in invoice.InvoiceItems)
            {
                if (!dbItemsDic.TryGetValue(itemDto.Id, out var dbItem)) continue;

                bool isTaxable = dbItem.Taxable;
                decimal taxPerc = itemDto.TotalTaxesPerc;
                decimal price = itemDto.Price;
                decimal discount = itemDto.Discount;
                decimal qty = itemDto.Quantity;
                decimal totalPrice = itemDto.TotalPrice;

                decimal taxFactor = (!isTaxable || taxPerc == 0) ? 1 : YusrMath.GetFactor(taxPerc);

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
                    Name = dbItem.Name,
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
    }
}
