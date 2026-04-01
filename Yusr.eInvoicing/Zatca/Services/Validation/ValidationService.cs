using Yusr.eInvoicing.Abstractions.Dto;
using Yusr.eInvoicing.Abstractions.Services.Validation;

namespace Yusr.eInvoicing.Zatca.Services.Validation
{
    public class ValidationService : IEInvoiceValidationService
    {
        public (List<string> Errors, List<string> Warnings) ValidateInvoice(EInvoiceDto eInvoice)
        {
            bool simplified = string.IsNullOrEmpty(eInvoice.CustomerVatNumber);
            var errors = new List<string>();
            var warnings = new List<string>();

            CheckErrors(eInvoice, ref errors, simplified);
            CheckWarnings(eInvoice, ref warnings, simplified);
            CheckItems(eInvoice, ref errors, ref warnings, simplified);

            return (errors, warnings);
        }

        private static void CheckWarnings(EInvoiceDto eInvoice, ref List<string> warnings, bool simplified)
        {
            if (!simplified)
            {
                if (string.IsNullOrEmpty(eInvoice.CustomerAddress.CountryCode))
                {
                    warnings.Add("اسم المدينة في عنوان العميل مطلوب/n");
                }

                if (string.IsNullOrEmpty(eInvoice.CustomerAddress.CityName))
                {
                    warnings.Add("اسم المدينة في عنوان العميل مطلوب/n");
                }

                if (string.IsNullOrEmpty(eInvoice.CustomerAddress.StreetName))
                {
                    warnings.Add("اسم الشارع في عنوان العميل مطلوب/n");
                }

                if (string.IsNullOrEmpty(eInvoice.CustomerAddress.CitySubdivisionName))
                {
                    warnings.Add("الحي في عنوان العميل مطلوبة/n");
                }

                if (string.IsNullOrEmpty(eInvoice.CustomerAddress.BuildingNumber))
                {
                    warnings.Add("رقم المبنى في عنوان العميل مطلوب/n");
                }

                if (string.IsNullOrEmpty(eInvoice.CustomerAddress.PostalZone))
                {
                    warnings.Add("الرمز البريدي في عنوان العميل مطلوب/n");
                }
            }
        }

        private static void CheckErrors(EInvoiceDto eInvoice, ref List<string> errors, bool simplified)
        {
            if (eInvoice.IssueDate > DateTime.UtcNow.Date)
            {
                errors.Add("تاريخ الفاتورة لا يمكن أن يكون في المستقبل");
            }
        }

        private static void CheckItems(EInvoiceDto eInvoice, ref List<string> errors, ref List<string> warnings, bool simplified)
        {
            //foreach (var item in InvoiceItems)
            //{
            //    if (string.IsNullOrWhiteSpace(item.name))
            //    {
            //        errors.Add($"يوجد بند برقم {item.ItemId} لا يحتوي على اسم (BR-25).");
            //    }
            //}
        }
    }
}
