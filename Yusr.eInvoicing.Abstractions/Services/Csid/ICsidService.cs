using System;
using System.Collections.Generic;
using System.Text;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities;

namespace Yusr.eInvoicing.Abstractions.Services.Csid
{
    internal interface ICsidService
    {
        Task<OperationResult<<CsidResponse?>> TryRequestComplianceCsidAsync(string otp, CsrResult csrResult, bool Production)
    }
}
