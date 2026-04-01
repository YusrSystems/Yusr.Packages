using System.Text;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;
using Yusr.eInvoicing.Abstractions.Enums;
using Yusr.eInvoicing.Abstractions.Services.Api;
using Yusr.eInvoicing.Abstractions.Services.Csid;
using Yusr.eInvoicing.Abstractions.Services.Csr;
using Yusr.eInvoicing.Abstractions.Services.Entities;
using Yusr.eInvoicing.Abstractions.Services.Registration;
using Yusr.eInvoicing.Zatca.Entities;

namespace Yusr.eInvoicing.Zatca.Services
{
    public class RegistrationService(
        ICsrService<ZatcaCsrResult> csrService,
        ICsidService<ZatcaCsidResult, ZatcaCsrResult> csidService,
        IEInvoiceComplianceCheckService complianceCheckService,
        ICsidStorage csidStorage
    ) : IEInvoiceRegistrationService
    {
        public async Task<OperationResult<bool>> Register(IEInvoicingSetting setting, string otp, EInvoicingEnvironmentType type)
        {
            try
            {
                var generateCsrResult = await csrService.TryGenerateCsrAsync(setting, type);
                if (!generateCsrResult.Succeeded || generateCsrResult.Result == null)
                    return OperationResult<bool>.InternalError("فشل إنشاء طلب الشهادة الرقمية", generateCsrResult.ErrorMessage ?? string.Empty);

                var complianceCsidResult = await csidService.TryRequestComplianceCsidAsync(otp, generateCsrResult.Result, type);
                if (!complianceCsidResult.Succeeded || complianceCsidResult.Result == null)
                    return OperationResult<bool>.InternalError("لم يتم اصدار شهادة الامتثال (SCID) بشكل صحيح", complianceCsidResult.ErrorMessage ?? string.Empty);

                var ComplianceCheckResult = await complianceCheckService.GenerateFullCheck(setting, type);
                if (!ComplianceCheckResult.Succeeded)
                    return OperationResult<bool>.InternalError("لم يتم التحقق من الامتثال بشكل صحيح", ComplianceCheckResult.ErrorMessage ?? string.Empty);

                var productionCsidResult = await csidService.TryRequestProductionCsidAsync(complianceCsidResult.Result!, type);
                if (!productionCsidResult.Succeeded || productionCsidResult.Result == null)
                    return OperationResult<bool>.InternalError("لم يتم اصدار شهادة الإنتاج بشكل صحيح", productionCsidResult.ErrorMessage ?? string.Empty);

                byte[] pcsidCertBytes = Convert.FromBase64String(productionCsidResult.Result.BinarySecurityToken);
                string certificateContent = Encoding.UTF8.GetString(pcsidCertBytes);
                OperationResult<bool> StoreProductionCsidResult = await csidStorage.StoreCsid(productionCsidResult.Result, certificateContent, type);

                return StoreProductionCsidResult;
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.InternalError("لم يتم الحصول على معلومات المؤسسة بشكل صحيح", ex.Message);
            }
        }
    }
}
