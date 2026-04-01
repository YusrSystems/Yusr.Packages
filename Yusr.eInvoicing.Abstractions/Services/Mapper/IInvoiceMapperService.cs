using Yusr.eInvoicing.Abstractions.Dto;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;

namespace Yusr.eInvoicing.Abstractions.Services.Mapper
{
    public interface IInvoiceMapperService
    {
        public EInvoiceDto GetEInvoiceData(IEInvoicingSetting setting, IInvoice invoice, IAccount customer, List<IItem> dbItems, long? lastCounter, string? lastHash);
    }
}
