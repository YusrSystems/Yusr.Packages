using Yusr.eInvoicing.Abstractions.Dto;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;

namespace Yusr.eInvoicing.Abstractions.Services.Mapper
{
    public interface IInvoiceMapperService
    {
        public EInvoiceDto GetEInvoiceData(IEInvoiceSetting setting, IEInvoiceInvoice invoice, IEInvoiceAccount customer, List<IEInvoiceItem> dbItems, long? lastCounter, string? lastHash);
    }
}
