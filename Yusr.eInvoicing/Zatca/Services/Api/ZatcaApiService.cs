using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Xml;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities;
using Yusr.eInvoicing.Abstractions.Enums;
using Yusr.eInvoicing.Abstractions.Services.Api;
using Yusr.eInvoicing.Zatca.Entities;
using Yusr.Infrastructure.eInvoicing.Zatca.Extensions;
using ZATCA.EInvoice.SDK;
using ZATCA.EInvoice.SDK.Contracts.Models;

namespace Yusr.eInvoicing.Zatca.Services.Api
{

    public class ZatcaApiService(HttpClient httpClient) : IEInvoiceApiService
    {
        private readonly HttpClient _httpClient = httpClient;
        private static readonly JsonSerializerOptions _serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        private static readonly JsonSerializerOptions _deserializerOptions = new() { PropertyNameCaseInsensitive = true };

        public async Task<OperationResult<EInvoicingApiResponse>> SendComplianceCheckInvoice(XmlDocument signedEInvoice, string binarySecurityToken, string secret, EInvoicingEnvironmentType type)
        {
            string endpoint = type switch
            {
                EInvoicingEnvironmentType.Test => "developer-portal/compliance/invoices",
                EInvoicingEnvironmentType.Simulation => "simulation/compliance/invoices",
                EInvoicingEnvironmentType.Production => "core/compliance/invoices",
                _ => "developer-portal/compliance/invoices"
            };
            return await SendInvoice(GenerateRequest(signedEInvoice).ToEInvoiceRequest(), binarySecurityToken, secret, endpoint, addClearanceHeader: false);
        }

        public async Task<OperationResult<EInvoicingApiResponse>> SendSimplifiedInvoice(XmlDocument signedEInvoice, string binarySecurityToken, string secret, EInvoicingEnvironmentType type)
        {
            string endpoint = type switch
            {
                EInvoicingEnvironmentType.Test => "developer-portal/invoices/reporting/single",
                EInvoicingEnvironmentType.Simulation => "simulation/invoices/reporting/single",
                EInvoicingEnvironmentType.Production => "core/invoices/reporting/single",
                _ => "developer-portal/invoices/reporting/single"
            };
            return await SendInvoice(GenerateRequest(signedEInvoice).ToEInvoiceRequest(), binarySecurityToken, secret, endpoint, addClearanceHeader: true);
        }

        public async Task<OperationResult<EInvoicingApiResponse>> SendSimplifiedInvoice(EInvoiceRequest invoiceRequest, string binarySecurityToken, string secret, EInvoicingEnvironmentType type)
        {
            string endpoint = type switch
            {
                EInvoicingEnvironmentType.Test => "developer-portal/invoices/reporting/single",
                EInvoicingEnvironmentType.Simulation => "simulation/invoices/reporting/single",
                EInvoicingEnvironmentType.Production => "core/invoices/reporting/single",
                _ => "developer-portal/invoices/reporting/single"
            };
            return await SendInvoice(invoiceRequest, binarySecurityToken, secret, endpoint, addClearanceHeader: true);
        }

        public async Task<OperationResult<EInvoicingApiResponse>> SendStandardInvoice(XmlDocument signedEInvoice, string binarySecurityToken, string secret, EInvoicingEnvironmentType type)
        {
            string endpoint = type switch
            {
                EInvoicingEnvironmentType.Test => "developer-portal/invoices/clearance/single",
                EInvoicingEnvironmentType.Simulation => "simulation/invoices/clearance/single",
                EInvoicingEnvironmentType.Production => "core/invoices/clearance/single",
                _ => "developer-portal/invoices/clearance/single"
            };
            return await SendInvoice(GenerateRequest(signedEInvoice).ToEInvoiceRequest(), binarySecurityToken, secret, endpoint, addClearanceHeader: true);
        }

        public async Task<OperationResult<EInvoicingApiResponse>> SendStandardInvoice(EInvoiceRequest invoiceRequest, string binarySecurityToken, string secret, EInvoicingEnvironmentType type)
        {
            string endpoint = type switch
            {
                EInvoicingEnvironmentType.Test => "developer-portal/invoices/clearance/single",
                EInvoicingEnvironmentType.Simulation => "simulation/invoices/clearance/single",
                EInvoicingEnvironmentType.Production => "core/invoices/clearance/single",
                _ => "developer-portal/invoices/clearance/single"
            };
            return await SendInvoice(invoiceRequest, binarySecurityToken, secret, endpoint, addClearanceHeader: true);
        }



        private async Task<OperationResult<EInvoicingApiResponse>> SendInvoice(EInvoiceRequest invoiceRequest, string binarySecurityToken, string secret, string url, bool addClearanceHeader)
        {
            HttpRequestMessage request = new(HttpMethod.Post, url);
            PrepareRequest(ref request, invoiceRequest, binarySecurityToken, secret, addClearanceHeader);

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.InternalServerError)
                return OperationResult<EInvoicingApiResponse>.InternalError("فشل نظام هيئة الزكاة والضريبة في معالجة الطلب", "");

            else if (response.StatusCode == HttpStatusCode.Unauthorized)
                return OperationResult<EInvoicingApiResponse>.InternalError("غير مصرح", "");

            else if (response.StatusCode == HttpStatusCode.Conflict)
                return OperationResult<EInvoicingApiResponse>.Conflict("تم ارسالها مسبقا");

            else if (response.StatusCode == HttpStatusCode.BadRequest)
                return OperationResult<EInvoicingApiResponse>.BadRequest("الطلب غير صحيح");

            if (!response.IsSuccessStatusCode)
                return OperationResult<EInvoicingApiResponse>.InternalError("فشل نظام هيئة الزكاة والضريبة في معالجة الطلب", "");

            try
            {
                ComplianceValidationResponse? complianceValidationResponse = JsonSerializer.Deserialize<ComplianceValidationResponse>(responseContent, _deserializerOptions);

                if (complianceValidationResponse != null)
                {
                    return OperationResult<EInvoicingApiResponse>.Ok(new EInvoicingApiResponse
                    {
                        ClearedInvoice = complianceValidationResponse.ClearedInvoice,
                        ErrorMessages = [.. complianceValidationResponse.ValidationResults.ErrorMessages.Select(x => x.Message)],
                        WarningMessages = [.. complianceValidationResponse.ValidationResults.WarningMessages.Select(x => x.Message)]
                    });
                }
                else
                {
                    return OperationResult<EInvoicingApiResponse>.InternalError("خطأ في إرسال الفاتورة للهيئة", "");
                }
            }
            catch
            {
                return OperationResult<EInvoicingApiResponse>.InternalError("خطأ في إرسال الفاتورة للهيئة", "");
            }
        }

        private static InvoiceRequest GenerateRequest(XmlDocument signedEInvoice)
        {
            RequestGenerator requestGenerator = new();
            return requestGenerator.GenerateRequest(signedEInvoice).InvoiceRequest;
        }

        private static void PrepareRequest(ref HttpRequestMessage request, EInvoiceRequest invoiceRequest, string binarySecurityToken, string secret, bool addClearanceHeader)
        {
            string authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{binarySecurityToken}:{secret}"));

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);
            request.Headers.Add("Accept-Version", "V2");
            request.Headers.Add("Accept-Language", "ar-SA");
            if (addClearanceHeader)
            {
                request.Headers.Add("Clearance-Status", "1");
            }

            var payload = new
            {
                invoiceHash = invoiceRequest.InvoiceHash,
                uuid = invoiceRequest.Uuid,
                invoice = invoiceRequest.Invoice
            };
            string jsonBody = JsonSerializer.Serialize(payload, _serializerOptions);
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
    }
}
