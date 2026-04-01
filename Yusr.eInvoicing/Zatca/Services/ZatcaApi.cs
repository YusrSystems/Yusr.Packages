using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Xml;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities;
using Yusr.eInvoicing.Abstractions.Services.Api;
using Yusr.eInvoicing.Zatca.Entities;
using Yusr.Infrastructure.eInvoicing.Zatca.Extensions;
using ZATCA.EInvoice.SDK;
using ZATCA.EInvoice.SDK.Contracts.Models;

namespace Yusr.Infrastructure.eInvoicing.Zatca.Services
{

    public class ZatcaApi : IEInvoiceApiService
    {
        private static readonly HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://gw-fatoora.zatca.gov.sa/e-invoicing/"),
            Timeout = TimeSpan.FromSeconds(30)
        };

        public async Task<OperationResult<EInvoicingApiResponse>> SendComplianceCheckInvoice(XmlDocument signedEInvoice, string binarySecurityToken, string secret, bool Production = false)
        {
            string url = Production ? "core/compliance/invoices" : "simulation/compliance/invoices";
            return await SendInvoice(GenerateRequest(signedEInvoice).ToEInvoiceRequest(), binarySecurityToken, secret, url, addClearanceHeader: false);
        }

        public async Task<OperationResult<EInvoicingApiResponse>> SendSimplifiedInvoice(XmlDocument signedEInvoice, string binarySecurityToken, string secret, bool Production = false)
        {
            string url = Production ? "core/invoices/reporting/single" : "simulation/invoices/reporting/single";
            return await SendInvoice(GenerateRequest(signedEInvoice).ToEInvoiceRequest(), binarySecurityToken, secret, url, addClearanceHeader: true);
        }

        public async Task<OperationResult<EInvoicingApiResponse>> SendSimplifiedInvoice(EInvoiceRequest invoiceRequest, string binarySecurityToken, string secret, bool Production = false)
        {
            string url = Production ? "core/invoices/reporting/single" : "simulation/invoices/reporting/single";
            return await SendInvoice(invoiceRequest, binarySecurityToken, secret, url, addClearanceHeader: true);
        }

        public async Task<OperationResult<EInvoicingApiResponse>> SendStandardInvoice(XmlDocument signedEInvoice, string binarySecurityToken, string secret, bool Production = false)
        {
            string url = Production ? "core/invoices/clearance/single" : "simulation/invoices/clearance/single";
            return await SendInvoice(GenerateRequest(signedEInvoice).ToEInvoiceRequest(), binarySecurityToken, secret, url, addClearanceHeader: true);
        }

        public async Task<OperationResult<EInvoicingApiResponse>> SendStandardInvoice(EInvoiceRequest invoiceRequest, string binarySecurityToken, string secret, bool Production = false)
        {
            string url = Production ? "core/invoices/clearance/single" : "simulation/invoices/clearance/single";
            return await SendInvoice(invoiceRequest, binarySecurityToken, secret, url, addClearanceHeader: true);
        }



        private static async Task<OperationResult<EInvoicingApiResponse>> SendInvoice(EInvoiceRequest invoiceRequest, string binarySecurityToken, string secret, string url, bool addClearanceHeader)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            PrepareRequest(ref request, invoiceRequest, binarySecurityToken, secret, addClearanceHeader);

            HttpResponseMessage response = await httpClient.SendAsync(request);
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
                ComplianceValidationResponse? complianceValidationResponse = JsonSerializer.Deserialize<ComplianceValidationResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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
            RequestGenerator requestGenerator = new RequestGenerator();
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
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            string jsonBody = JsonSerializer.Serialize(payload, jsonOptions);
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
    }
}
