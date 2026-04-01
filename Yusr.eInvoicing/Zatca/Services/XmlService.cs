using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UBL.Invoice;
using Yusr.Core.Abstractions.Primitives;
using Yusr.Core.Abstractions.Utilities;
using Yusr.eInvoicing.Abstractions.Dto;
using Yusr.eInvoicing.Abstractions.Enums;
using Yusr.eInvoicing.Abstractions.Services.Signing;
using Yusr.eInvoicing.Abstractions.Services.Xml;
using Yusr.Identity.Abstractions.Primitives;

namespace Yusr.Infrastructure.eInvoicing.Zatca.Services
{
    public class XmlService(ISignService signService) : IXmlService
    {
        private readonly ISignService _signService = signService;

        public OperationResult<XmlDocument> GenerateXmlEInvoice(EInvoiceDto eInvoice, JwtClaims jwtClaims, string Certificate, string PrivateKey)
        {
            bool simplified = string.IsNullOrWhiteSpace(eInvoice.CustomerVatNumber) || string.IsNullOrEmpty(eInvoice.CustomerVatNumber);

            // 2: supplier CRN
            PartyIdentificationType[] SupplierIdentification = new PartyIdentificationType[1];
            if (!string.IsNullOrEmpty(eInvoice.SupplierCRN))
            {
                SupplierIdentification[0] = new PartyIdentificationType
                {
                    ID = new IDType
                    {
                        Value = eInvoice.SupplierCRN,
                        schemeID = "CRN"
                    }
                };
            }

            PartyIdentificationType[] CustomerIdentification = new PartyIdentificationType[1];
            if (!string.IsNullOrEmpty(eInvoice.CustomerCRN))
            {
                CustomerIdentification[0] = new PartyIdentificationType
                {
                    ID = new IDType
                    {
                        Value = eInvoice.CustomerCRN,
                        schemeID = "CRN"
                    }
                };
            }
            //else
            //{
            //    CustomerIdentification[0] = new PartyIdentificationType
            //    {
            //        ID = new IDType
            //        {
            //            Value = eInvoice.ActionAccountId.ToString(),
            //            schemeID = "OTH"
            //        }
            //    };
            //}

            PartyTaxSchemeType[] CustomerPartyTaxScheme = new PartyTaxSchemeType[1];
            if (!string.IsNullOrEmpty(eInvoice.CustomerVatNumber))
            {
                CustomerPartyTaxScheme[0] = new PartyTaxSchemeType
                {
                    CompanyID = new CompanyIDType { Value = eInvoice.CustomerVatNumber },
                    TaxScheme = new TaxSchemeType
                    {
                        ID = new IDType { Value = "VAT" }
                    }
                };
            }

            // 3: customer address
            AddressType? CustomerAddress = null;

            if (eInvoice.CustomerAddress != null)
            {
                string?[] addressFields =
                {
                    eInvoice.CustomerAddress.StreetName,
                    eInvoice.CustomerAddress.BuildingNumber,
                    eInvoice.CustomerAddress.CitySubdivisionName,
                    eInvoice.CustomerAddress.CityName,
                    eInvoice.CustomerAddress.PostalZone,
                    eInvoice.CustomerAddress.CountryCode
                };

                if (addressFields.Any(f => !string.IsNullOrEmpty(f)))
                {
                    CustomerAddress = new AddressType();

                    if (!string.IsNullOrEmpty(eInvoice.CustomerAddress.StreetName))
                    {
                        CustomerAddress.StreetName = new StreetNameType { Value = eInvoice.CustomerAddress.StreetName };
                    }
                    if (!string.IsNullOrEmpty(eInvoice.CustomerAddress.BuildingNumber))
                    {
                        CustomerAddress.BuildingNumber = new BuildingNumberType { Value = eInvoice.CustomerAddress.BuildingNumber };
                    }
                    if (!string.IsNullOrEmpty(eInvoice.CustomerAddress.CitySubdivisionName))
                    {
                        CustomerAddress.CitySubdivisionName = new CitySubdivisionNameType { Value = eInvoice.CustomerAddress.CitySubdivisionName };
                    }
                    if (!string.IsNullOrEmpty(eInvoice.CustomerAddress.CityName))
                    {
                        CustomerAddress.CityName = new CityNameType { Value = eInvoice.CustomerAddress.CityName };
                    }
                    if (!string.IsNullOrEmpty(eInvoice.CustomerAddress.PostalZone))
                    {
                        CustomerAddress.PostalZone = new PostalZoneType { Value = eInvoice.CustomerAddress.PostalZone };
                    }
                    if (!string.IsNullOrEmpty(eInvoice.CustomerAddress.CountryCode))
                    {
                        CustomerAddress.Country = new CountryType { IdentificationCode = new IdentificationCodeType { Value = eInvoice.CustomerAddress.CountryCode } };
                    }
                }
            }

            DeliveryType[] Delivery = new DeliveryType[1];
            if (eInvoice.DeliveryDate != null)
            {
                Delivery[0] = new DeliveryType
                {
                    ActualDeliveryDate = new ActualDeliveryDateType
                    {
                        Value = (DateTime)eInvoice.DeliveryDate
                    }
                };
            }

            string InvoiceType = eInvoice.EInvoiceType switch
            {
                EInvoiceType.Sell => "388",
                EInvoiceType.Credit => "381",
                EInvoiceType.Debit => "383",
                _ => ""
            };

            UBL.Invoice.InvoiceType invoice = new UBL.Invoice.InvoiceType
            {
                ProfileID = new ProfileIDType { Value = eInvoice.ProfileID },
                ID = new IDType { Value = eInvoice.ID.ToString() }, // invoiceId from db
                UUID = new UUIDType { Value = Guid.NewGuid().ToString() },
                IssueDate = new IssueDateType { Value = eInvoice.IssueDate }, // yyyy-mm-dd
                IssueTime = new IssueTimeType { Value = eInvoice.IssueTime }, // hh:mm:ss
                InvoiceTypeCode = new InvoiceTypeCodeType { Value = InvoiceType, name = simplified ? "0200000" : "0100000" },
                DocumentCurrencyCode = new DocumentCurrencyCodeType { Value = eInvoice.CurrencyCode },
                TaxCurrencyCode = new TaxCurrencyCodeType { Value = eInvoice.CurrencyCode },


                AdditionalDocumentReference = new DocumentReferenceType[]
                {
                    new DocumentReferenceType
                    {
                        ID = new IDType { Value = "ICV" }, // Invoice Counter Value
                        UUID = new UUIDType { Value = eInvoice.InvoiceCounter.ToString() }
                    },
                    new DocumentReferenceType
                    {
                        ID = new IDType { Value = "PIH"}, // Previous Invoice Hash
                        Attachment = new AttachmentType
                        {
                            EmbeddedDocumentBinaryObject = new EmbeddedDocumentBinaryObjectType
                            {
                                Value = Convert.FromBase64String(eInvoice.PreviousInvoiceHash),
                                mimeCode = "text/plain"
                            }
                        }
                    }

                },

                AccountingSupplierParty = new SupplierPartyType
                {
                    Party = new PartyType
                    {
                        PartyIdentification = SupplierIdentification,

                        PostalAddress = new AddressType
                        {
                            StreetName = new StreetNameType { Value = eInvoice.SupplierAddress.StreetName },
                            BuildingNumber = new BuildingNumberType { Value = eInvoice.SupplierAddress.BuildingNumber },// 4 digits only
                            CitySubdivisionName = new CitySubdivisionNameType { Value = eInvoice.SupplierAddress.CitySubdivisionName },
                            CityName = new CityNameType { Value = eInvoice.SupplierAddress.CityName },
                            PostalZone = new PostalZoneType { Value = eInvoice.SupplierAddress.PostalZone },
                            Country = new CountryType
                            {
                                IdentificationCode = new IdentificationCodeType { Value = eInvoice.SupplierAddress.CountryCode }
                            }
                        },

                        PartyTaxScheme = new PartyTaxSchemeType[]
                        {
                            new PartyTaxSchemeType
                            {
                                CompanyID = new CompanyIDType { Value = eInvoice.SupplierVatNumber }, // الرقم الضريبي
                                TaxScheme = new TaxSchemeType
                                {
                                    ID = new IDType { Value = "VAT" }
                                }
                            }
                        },

                        PartyLegalEntity = new PartyLegalEntityType[]
                        {
                            new PartyLegalEntityType
                            {
                                RegistrationName = new RegistrationNameType { Value = eInvoice.SupplierName }
                            }
                        }

                    }
                },

                AccountingCustomerParty = new CustomerPartyType
                {
                    Party = new PartyType
                    {
                        PartyIdentification = CustomerIdentification,

                        PostalAddress = CustomerAddress,

                        PartyTaxScheme = CustomerPartyTaxScheme,

                        PartyLegalEntity = new PartyLegalEntityType[]
                        {
                            new PartyLegalEntityType
                            {
                                RegistrationName = new RegistrationNameType { Value = eInvoice.CustomerName }
                            }
                        },
                    }
                },

                Delivery = Delivery,

                LegalMonetaryTotal = new MonetaryTotalType
                {
                    LineExtensionAmount = new LineExtensionAmountType { Value = eInvoice.LineExtensionAmount, currencyID = eInvoice.CurrencyCode },
                    TaxExclusiveAmount = new TaxExclusiveAmountType { Value = eInvoice.TaxExclusiveAmount, currencyID = eInvoice.CurrencyCode },
                    TaxInclusiveAmount = new TaxInclusiveAmountType { Value = eInvoice.TaxInclusiveAmount, currencyID = eInvoice.CurrencyCode },
                    PayableRoundingAmount = new PayableRoundingAmountType { Value = eInvoice.RoundingAmount, currencyID = eInvoice.CurrencyCode },
                    PayableAmount = new PayableAmountType { Value = eInvoice.InvoiceAmount, currencyID = eInvoice.CurrencyCode }
                }
            };


            if (eInvoice.OriginalInvoiceId != null)
            {
                invoice.BillingReference = new BillingReferenceType[]
                {
                    new BillingReferenceType
                    {
                        InvoiceDocumentReference = new DocumentReferenceType { ID = new IDType { Value = eInvoice.OriginalInvoiceId.ToString() } }
                    }
                };

                invoice.PaymentMeans = new PaymentMeansType[]
                {
                    new PaymentMeansType
                    {
                        PaymentMeansCode = new PaymentMeansCodeType { Value = "10" },
                        InstructionNote = new InstructionNoteType[]
                        {
                            new InstructionNoteType {Value = "In case of goods or services refund | عند ترجيع السلع أو الخدمات" }
                        }
                    }
                };
            }


            List<InvoiceLineType> invoiceLines = new List<InvoiceLineType>();
            List<TaxSubtotalType> invoiceTaxSubtotals = new List<TaxSubtotalType>();
            List<TaxTotalType> invoiceTaxes = new List<TaxTotalType>();
            List<AllowanceChargeType> invoiceAllowances = new List<AllowanceChargeType>();
            TaxExemptionReasonCodeType? taxExemptionReasonCode = null;
            TaxExemptionReasonType[] taxExemptionReason = new TaxExemptionReasonType[1];

            EInvoiceLineDto invoiceLine;
            string taxCode;
            decimal TotalNoTaxPrice_S = 0;
            decimal TotalTaxAmount_S = 0;
            decimal TotalNoTaxPrice_E = 0;
            string taxExemptionReasonCode_E = string.Empty;
            string taxExemptionReason_E = string.Empty;
            decimal TotalNoTaxPrice_Z = 0;
            string taxExemptionReasonCode_Z = string.Empty;
            string taxExemptionReason_Z = string.Empty;

            for (int i = 0; i < eInvoice.InvoiceLines.Count; i++)
            {
                invoiceLine = eInvoice.InvoiceLines[i];
                taxCode = invoiceLine.Taxable ? invoiceLine.TotalTaxPercent == 0 ? "Z" : "S" : "E";

                if (!string.IsNullOrEmpty(invoiceLine.TaxExemptionReasonCode))
                    taxExemptionReasonCode = new TaxExemptionReasonCodeType { Value = invoiceLine.TaxExemptionReasonCode };

                if (!string.IsNullOrEmpty(invoiceLine.TaxExemptionReason))
                    taxExemptionReason[0] = new TaxExemptionReasonType { Value = invoiceLine.TaxExemptionReason };

                invoiceLines.Add(new InvoiceLineType
                {
                    ID = new IDType { Value = (i + 1).ToString() },
                    InvoicedQuantity = new InvoicedQuantityType { Value = invoiceLine.Quantity },
                    LineExtensionAmount = new LineExtensionAmountType { Value = invoiceLine.NoTaxTotalPrice, currencyID = eInvoice.CurrencyCode },

                    AllowanceCharge = new AllowanceChargeType[]
                    {
                        new AllowanceChargeType
                        {
                            ChargeIndicator = new ChargeIndicatorType{ Value = invoiceLine.AllowanceChargeAmount < 0 },
                            AllowanceChargeReason = new AllowanceChargeReasonType[]
                            {
                                new AllowanceChargeReasonType{ Value = invoiceLine.AllowanceChargeAmount < 0? "مبلغ مضاف موزع" : "خصم موزع" }
                            },
                            Amount = new AmountType2 { currencyID = eInvoice.CurrencyCode, Value = Math.Abs(invoiceLine.AllowanceChargeAmount) },
                        }
                    },

                    TaxTotal = new TaxTotalType[]
                        {
                            new TaxTotalType
                            {
                                TaxAmount = new TaxAmountType { Value = taxCode != "S" ? 0 : invoiceLine.TaxAmount, currencyID = eInvoice.CurrencyCode },
                                RoundingAmount = new RoundingAmountType { Value = invoiceLine.TaxTotalPrice, currencyID = eInvoice.CurrencyCode }
                            }
                        },
                    Item = new UBL.Invoice.ItemType
                    {
                        Name = new NameType1 { Value = invoiceLine.Name },
                        ClassifiedTaxCategory = new TaxCategoryType[]
                            {
                                new TaxCategoryType
                                {
                                    ID = new IDType { Value = taxCode },
                                    Percent = new PercentType1 { Value = taxCode != "S" ? 0 : invoiceLine.TotalTaxPercent },
                                    TaxExemptionReasonCode = taxExemptionReasonCode,
                                    TaxExemptionReason = taxExemptionReason,
                                    TaxScheme = new TaxSchemeType { ID = new IDType { Value = "VAT" } }
                                }
                            }
                    },
                    Price = new PriceType
                    {
                        PriceAmount = new PriceAmountType { Value = invoiceLine.NoTaxPrice, currencyID = eInvoice.CurrencyCode }
                    }
                });

                if (invoiceLine.Taxable && invoiceLine.TotalTaxPercent == 0)
                {
                    TotalNoTaxPrice_Z += invoiceLine.NoTaxTotalPrice;
                    taxExemptionReasonCode_Z = invoiceLine.TaxExemptionReasonCode;
                    taxExemptionReason_Z = invoiceLine.TaxExemptionReason;
                }
                else if (!invoiceLine.Taxable)
                {
                    TotalNoTaxPrice_E += invoiceLine.NoTaxTotalPrice;
                    taxExemptionReasonCode_E = invoiceLine.TaxExemptionReasonCode;
                    taxExemptionReason_E = invoiceLine.TaxExemptionReason;
                }
                else
                {
                    TotalNoTaxPrice_S += invoiceLine.NoTaxTotalPrice;
                    TotalTaxAmount_S += invoiceLine.TaxAmount;
                }
            }

            if (TotalNoTaxPrice_S > 0)
            {
                invoiceTaxSubtotals.Add(new TaxSubtotalType
                {
                    TaxableAmount = new TaxableAmountType
                    {
                        Value = YusrMath.Round(TotalNoTaxPrice_S),
                        currencyID = eInvoice.CurrencyCode
                    },
                    TaxAmount = new TaxAmountType
                    {
                        Value = eInvoice.TaxAmount,
                        currencyID = eInvoice.CurrencyCode
                    },
                    TaxCategory = new TaxCategoryType
                    {
                        ID = new IDType
                        {
                            Value = "S",
                            schemeID = "UN/ECE 5305",
                            schemeAgencyID = "6"
                        },
                        Percent = new PercentType1 { Value = 15 },
                        TaxScheme = new TaxSchemeType
                        {
                            ID = new IDType
                            {
                                Value = "VAT",
                                schemeID = "UN/ECE 5153",
                                schemeAgencyID = "6"
                            }
                        }
                    }
                });
            }

            if (TotalNoTaxPrice_Z > 0)
            {
                invoiceTaxSubtotals.Add(new TaxSubtotalType
                {
                    TaxableAmount = new TaxableAmountType
                    {
                        Value = YusrMath.Round(TotalNoTaxPrice_Z),
                        currencyID = eInvoice.CurrencyCode
                    },
                    TaxAmount = new TaxAmountType
                    {
                        Value = 0,
                        currencyID = eInvoice.CurrencyCode
                    },
                    TaxCategory = new TaxCategoryType
                    {
                        ID = new IDType
                        {
                            Value = "Z",
                            schemeID = "UN/ECE 5305",
                            schemeAgencyID = "6"
                        },
                        Percent = new PercentType1 { Value = 0 },
                        TaxExemptionReasonCode = new TaxExemptionReasonCodeType { Value = taxExemptionReasonCode_Z },
                        TaxExemptionReason = new TaxExemptionReasonType[] { new TaxExemptionReasonType { Value = taxExemptionReason_Z } },
                        TaxScheme = new TaxSchemeType
                        {
                            ID = new IDType
                            {
                                Value = "VAT",
                                schemeID = "UN/ECE 5153",
                                schemeAgencyID = "6"
                            }
                        }
                    }
                });
            }

            if (TotalNoTaxPrice_E > 0)
            {
                invoiceTaxSubtotals.Add(new TaxSubtotalType
                {
                    TaxableAmount = new TaxableAmountType
                    {
                        Value = YusrMath.Round(TotalNoTaxPrice_E),
                        currencyID = eInvoice.CurrencyCode
                    },
                    TaxAmount = new TaxAmountType
                    {
                        Value = 0,
                        currencyID = eInvoice.CurrencyCode
                    },
                    TaxCategory = new TaxCategoryType
                    {
                        ID = new IDType
                        {
                            Value = "E",
                            schemeID = "UN/ECE 5305",
                            schemeAgencyID = "6"
                        },
                        Percent = new PercentType1 { Value = 0 },
                        TaxExemptionReasonCode = new TaxExemptionReasonCodeType { Value = taxExemptionReasonCode_E },
                        TaxExemptionReason = new TaxExemptionReasonType[] { new TaxExemptionReasonType { Value = taxExemptionReason_E } },
                        TaxScheme = new TaxSchemeType
                        {
                            ID = new IDType
                            {
                                Value = "VAT",
                                schemeID = "UN/ECE 5153",
                                schemeAgencyID = "6"
                            }
                        }
                    }
                });
            }

            invoiceTaxes.Add(new TaxTotalType
            {
                TaxAmount = new TaxAmountType
                {
                    Value = eInvoice.TaxAmount,
                    currencyID = eInvoice.CurrencyCode
                }
            });

            invoiceTaxes.Add(new TaxTotalType
            {
                TaxAmount = new TaxAmountType
                {
                    Value = eInvoice.TaxAmount,
                    currencyID = eInvoice.CurrencyCode
                },
                TaxSubtotal = invoiceTaxSubtotals.ToArray()
            });


            invoice.InvoiceLine = invoiceLines.ToArray();
            invoice.TaxTotal = invoiceTaxes.ToArray();
            invoice.AllowanceCharge = invoiceAllowances.ToArray();

            var serializer = new XmlSerializer(typeof(UBL.Invoice.InvoiceType));
            var namespaces = new XmlSerializerNamespaces();

            namespaces.Add("", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");
            namespaces.Add("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            namespaces.Add("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            namespaces.Add("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");



            var xmlDoc = new XmlDocument() { PreserveWhitespace = true };
            using (var memoryStream = new MemoryStream())
            {
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "    ",
                    OmitXmlDeclaration = false,
                    Encoding = Encoding.UTF8,
                };

                using (var writer = XmlWriter.Create(memoryStream, settings))
                {
                    serializer.Serialize(writer, invoice, namespaces);
                }

                memoryStream.Position = 0;
                xmlDoc.Load(memoryStream);
            }

            return OperationResult<XmlDocument>.Ok(xmlDoc);
        }

        public OperationResult<(XmlDocument xmlInvoice, XmlDocument xmlSignedInvoice)> CreateFullXml(EInvoiceDto eInvoice, JwtClaims jwtClaims, string Certificate, string PrivateKey)
        {
            var xmlInvoiceResult = GenerateXmlEInvoice(eInvoice, jwtClaims, Certificate, PrivateKey);

            if (!xmlInvoiceResult.Succeeded || xmlInvoiceResult.Result == null)
                return OperationResult<(XmlDocument xmlInvoice, XmlDocument xmlSignedInvoice)>.CopyErrorsFrom(xmlInvoiceResult);

            var signResult = _signService.SignInvoice(jwtClaims, xmlInvoiceResult.Result, Certificate, PrivateKey);

            if (!signResult.Succeeded || signResult.Result == null)
                return OperationResult<(XmlDocument xmlInvoice, XmlDocument xmlSignedInvoice)>.CopyErrorsFrom(signResult);

            return OperationResult<(XmlDocument xmlInvoice, XmlDocument xmlSignedInvoice)>.Ok((xmlInvoice: xmlInvoiceResult.Result, xmlSignedInvoice: signResult.Result));
        }

        public string? ExtractValue(XmlDocument signedXml, string xpath)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(signedXml.NameTable);
            nsmgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            nsmgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");

            XmlNode? pihNode = signedXml.SelectSingleNode(xpath, nsmgr);

            if (pihNode == null || string.IsNullOrWhiteSpace(pihNode.InnerText))
            {
                return null;
            }

            return pihNode.InnerText;
        }
    }
}
