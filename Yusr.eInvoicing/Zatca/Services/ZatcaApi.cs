using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Xml;
using Yusr.Erp.Application.Accounting.DTOs;
using Yusr.Infrastructure.eInvoicing.Zatca.Extensions;
using ZATCA.EInvoice.SDK;
using ZATCA.EInvoice.SDK.Contracts.Models;

namespace Yusr.Infrastructure.eInvoicing.Zatca.Services
{
    public class ComplianceValidationResponse
    {
        public ValidationResults ValidationResults { get; set; } = new ValidationResults();
        public string ReportingStatus { get; set; } = string.Empty;
        public string ClearanceStatus { get; set; } = string.Empty;
        public string ClearedInvoice { get; set; } = string.Empty;
    }

    public class ValidationResults
    {
        public List<ValidationMessage> InfoMessages { get; set; } = new List<ValidationMessage>();
        public List<ValidationMessage> WarningMessages { get; set; } = new List<ValidationMessage>();
        public List<ValidationMessage> ErrorMessages { get; set; } = new List<ValidationMessage>();
        public string Status { get; set; } = string.Empty;
    }

    public class ValidationMessage
    {
        public string Type { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class InvoiceSubmissionResponse
    {
        public string InvoiceHash { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<string> Warnings { get; set; } = new List<string>();
        public List<InvoiceError> Errors { get; set; } = new List<InvoiceError>();
    }

    public class InvoiceError
    {
        public string Category { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class ZatcaApiRespone
    {
        public bool IsValid { get; set; }
        public HttpStatusCode ResultCode { get; set; }
        public string ErrorMessage { get; set; } = "";
        public string WarningMessage { get; set; } = "";
        public string ClearedInvoice { get; set; } = string.Empty;
    }

    public class ZatcaApi
    {
        private static readonly HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://gw-fatoora.zatca.gov.sa/e-invoicing/"),
            Timeout = TimeSpan.FromSeconds(30)
        };

        public static async Task<ZatcaApiRespone> SendComplianceCheckInvoice(XmlDocument signedEInvoice, string binarySecurityToken, string secret, bool Production = false)
        {
            string url = Production ? "core/compliance/invoices" : "simulation/compliance/invoices";
            return await SendInvoice(GenerateRequest(signedEInvoice).ToEInvoiceRequest(), binarySecurityToken, secret, url, addClearanceHeader: false);
        }

        public static async Task<ZatcaApiRespone> SendSimplifiedInvoice(XmlDocument signedEInvoice, string binarySecurityToken, string secret, bool Production = false)
        {
            string url = Production ? "core/invoices/reporting/single" : "simulation/invoices/reporting/single";
            return await SendInvoice(GenerateRequest(signedEInvoice).ToEInvoiceRequest(), binarySecurityToken, secret, url, addClearanceHeader: true);
        }

        public static async Task<ZatcaApiRespone> SendSimplifiedInvoice(EInvoiceRequest invoiceRequest, string binarySecurityToken, string secret, bool Production = false)
        {
            string url = Production ? "core/invoices/reporting/single" : "simulation/invoices/reporting/single";
            return await SendInvoice(invoiceRequest, binarySecurityToken, secret, url, addClearanceHeader: true);
        }

        public static async Task<ZatcaApiRespone> SendStandardInvoice(XmlDocument signedEInvoice, string binarySecurityToken, string secret, bool Production = false)
        {
            string url = Production ? "core/invoices/clearance/single" : "simulation/invoices/clearance/single";
            return await SendInvoice(GenerateRequest(signedEInvoice).ToEInvoiceRequest(), binarySecurityToken, secret, url, addClearanceHeader: true);
        }

        public static async Task<ZatcaApiRespone> SendStandardInvoice(EInvoiceRequest invoiceRequest, string binarySecurityToken, string secret, bool Production = false)
        {
            string url = Production ? "core/invoices/clearance/single" : "simulation/invoices/clearance/single";
            return await SendInvoice(invoiceRequest, binarySecurityToken, secret, url, addClearanceHeader: true);
        }



        private static async Task<ZatcaApiRespone> SendInvoice(EInvoiceRequest invoiceRequest, string binarySecurityToken, string secret, string url, bool addClearanceHeader)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            PrepareRequest(ref request, invoiceRequest, binarySecurityToken, secret, addClearanceHeader);

            HttpResponseMessage response = await httpClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();

            return HandleResult(response, responseContent);
        }

        private static InvoiceRequest GenerateRequest(XmlDocument signedEInvoice)
        {
            RequestGenerator requestGenerator = new RequestGenerator();
            return requestGenerator.GenerateRequest(signedEInvoice).InvoiceRequest;
        }
        private static ZatcaApiRespone HandleResult(HttpResponseMessage response, string responseContent)
        {
            ZatcaApiRespone result = new ZatcaApiRespone();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                result.IsValid = true;
            }

            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                result.IsValid = false;
                result.ErrorMessage = "فشل نظام هيئة الزكاة والضريبة في معالجة الطلب";
                result.ResultCode = HttpStatusCode.InternalServerError;
            }

            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                result.IsValid = false;
                result.ErrorMessage = "غير مصرح";
                result.ResultCode = HttpStatusCode.Unauthorized;
            }

            else if (response.StatusCode == HttpStatusCode.Conflict)
            {
                result.IsValid = true;
                result.ErrorMessage = "تم ارسالها مسبقا";
                result.ResultCode = HttpStatusCode.Conflict;
            }

            else if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.Accepted)
            {
                try
                {
                    result.IsValid = response.StatusCode == HttpStatusCode.Accepted;
                    result.ResultCode = response.StatusCode;

                    ComplianceValidationResponse? complianceValidationResponse = JsonSerializer.Deserialize<ComplianceValidationResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (complianceValidationResponse != null)
                    {
                        StringBuilder error = new StringBuilder();
                        foreach (ValidationMessage err in complianceValidationResponse.ValidationResults.ErrorMessages)
                        {
                            error.AppendLine(err.Message);
                        }
                        result.ErrorMessage = error.ToString();

                        StringBuilder warning = new StringBuilder();
                        foreach (ValidationMessage warn in complianceValidationResponse.ValidationResults.WarningMessages)
                        {
                            warning.AppendLine(warn.Message);
                        }
                        result.WarningMessage = warning.ToString();
                        result.ClearedInvoice = complianceValidationResponse.ClearedInvoice;
                    }
                    else
                    {
                        result.ErrorMessage = "خطأ في إرسال الفاتورة للهيئة";
                    }
                }
                catch
                {
                    result.ErrorMessage = "خطأ في إرسال الفاتورة للهيئة";
                }
            }

            else
            {
                result.IsValid = false;
                result.ErrorMessage = response.StatusCode + " فشل الإرسال";
                result.ResultCode = HttpStatusCode.BadRequest;
            }

            return result;
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
