using Yusr.eInvoicing.Abstractions.Dto;

namespace Yusr.eInvoicing.Abstractions.Services.Validation
{
    public interface IValidationService
    {
        (List<string> Errors, List<string> Warnings) ValidateInvoice(EInvoiceDto eInvoice);
    }
}
