using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Yusr.Core.Abstractions.Primitives;
using Yusr.Erp.Core.Entities.Common;
using Yusr.Identity.Abstractions.Primitives;
using Yusr.Infrastructure.Persistence.Context;
using ZATCA.EInvoice.SDK.Contracts.Models;

namespace Yusr.Infrastructure.eInvoicing.Zatca.Services
{
    public class CsidService(YusrErpDbContext context)
    {
        private readonly YusrErpDbContext _context = context;
        private static readonly HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://gw-fatoora.zatca.gov.sa/e-invoicing/"),
            Timeout = TimeSpan.FromSeconds(30)
        };

        public async Task<(bool IsValid, string? ErrorMessage, CsidResponse? CsidResponse)> TryRequestComplianceCsidAsync(string otp, CsrResult csrResult, bool Production)
        {
            string endpoint = Production
           ? "core/compliance"
           : "simulation/compliance";

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
            {
                return (IsValid: false, ErrorMessage: $"ZATCA Error: {response.StatusCode} - {resultJson}", CsidResponse: null);
            }

            CsidResponse? result = JsonSerializer.Deserialize<CsidResponse>(resultJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null)
            {
                return (IsValid: false, ErrorMessage: "Failed To Parse Json Response", CsidResponse: null);
            }

            return (IsValid: true, ErrorMessage: null, CsidResponse: result);
        }

        public async Task<(bool IsValid, string? ErrorMessage, CsidResponse? CsidResponse)> TryRequestProductionCsidAsync(CsidResponse csidResponse, bool Production)
        {
            string endpoint = Production
           ? "production/csids"
           : "simulation/production/csids";

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);

            // Authorization Header
            string authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{csidResponse.binarySecurityToken}:{csidResponse.secret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            request.Headers.Add("Accept-Version", "V2");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var payload = new { compliance_request_id = csidResponse.requestID };
            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);
            var resultJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return (IsValid: false, ErrorMessage: $"ZATCA Error: {response.StatusCode} - {resultJson}", CsidResponse: null);
            }

            CsidResponse? result = JsonSerializer.Deserialize<CsidResponse>(resultJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null)
                return (IsValid: false, ErrorMessage: "Failed to parse JSON response", CsidResponse: null);

            return (IsValid: true, ErrorMessage: null, CsidResponse: result);
        }

        public async Task<OperationResult<bool>> StoreCsid(JwtClaims jwtClaims, CsidResponse csid, string certificateContent, bool production)
        {
            try
            {
                var setting = await _context.Settings.FirstOrDefaultAsync();
                if (setting == null)
                    return OperationResult<bool>.NotFound();

                setting.UpdateCsid(production ? EInvoicingStatus.Production : EInvoicingStatus.Simulation, csid.requestID, csid.binarySecurityToken, certificateContent, csid.secret);
                await _context.SaveChangesAsync();
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.InternalError("Could not save Csr", ex.Message);
            }
        }
    }

    public class CsidResponse
    {
        public long requestID { get; set; }
        public string dispositionMessage { get; set; } = "";
        public string binarySecurityToken { get; set; } = "";
        public string secret { get; set; } = "";
    }

    public class ZatcaParams
    {
        public byte zatcaStatus { get; set; }
        public string privateKey { get; set; } = "";
        public string binarySecurityToken { get; set; } = "";
        public string certificateContent { get; set; } = "";
        public string secret { get; set; } = "";

        public ZatcaParams() { }

        public ZatcaParams(CsidResponse csid, CsrResult csr)
        {
            byte[] csidCertBytes = Convert.FromBase64String(csid.binarySecurityToken);
            certificateContent = Encoding.UTF8.GetString(csidCertBytes);
            binarySecurityToken = csid.binarySecurityToken;
            secret = csid.secret;
            privateKey = csr.PrivateKey;
            zatcaStatus = 0;
        }
    }
}
