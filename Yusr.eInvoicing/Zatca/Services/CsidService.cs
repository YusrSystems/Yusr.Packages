using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Services.Csid;
using Yusr.eInvoicing.Abstractions.Services.Entities;
using Yusr.eInvoicing.Zatca.Entities;

namespace Yusr.Infrastructure.eInvoicing.Zatca.Services
{
    public class CsidService : ICsidService<ZatcaCsidResult, ZatcaCsrResult>
    {
        private static readonly HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://gw-fatoora.zatca.gov.sa/e-invoicing/"),
            Timeout = TimeSpan.FromSeconds(30)
        };

        public async Task<OperationResult<ZatcaCsidResult>> TryRequestComplianceCsidAsync(string otp, ZatcaCsrResult csrResult, bool Production)
        {
            string endpoint = Production ? "core/compliance" : "simulation/compliance";

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Add("OTP", otp);
            request.Headers.Add("Accept-Version", "V2");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var payload = new { csr = csrResult.Csr };
            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);
            var resultJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return OperationResult<ZatcaCsidResult>.InternalError("لم يتم اصدار شهادة الامتثال بشكل صحيح", $"ZATCA Error: {response.StatusCode} - {resultJson}");

            ZatcaCsidResult? result = JsonSerializer.Deserialize<ZatcaCsidResult>(resultJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null)
                return OperationResult<ZatcaCsidResult>.InternalError("لم يتم اصدار شهادة الامتثال بشكل صحيح", "Failed To Parse Json Response");

            return OperationResult<ZatcaCsidResult>.Ok(result);
        }

        public async Task<OperationResult<ZatcaCsidResult>> TryRequestProductionCsidAsync(ZatcaCsidResult csidResponse, bool Production)
        {
            string endpoint = Production ? "production/csids" : "simulation/production/csids";

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);

            // Authorization Header
            string authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{csidResponse.BinarySecurityToken}:{csidResponse.Secret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            request.Headers.Add("Accept-Version", "V2");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var payload = new { compliance_request_id = csidResponse.RequestId };
            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);
            var resultJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return OperationResult<ZatcaCsidResult>.InternalError("لم يتم اصدار شهادة الإنتاج بشكل صحيح", $"ZATCA Error: {response.StatusCode} - {resultJson}");

            ZatcaCsidResult? result = JsonSerializer.Deserialize<ZatcaCsidResult>(resultJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null)
                return OperationResult<ZatcaCsidResult>.InternalError("لم يتم اصدار شهادة الامتثال بشكل صحيح", "Failed To Parse Json Response");

            return OperationResult<ZatcaCsidResult>.Ok(result);
        }
    }
}