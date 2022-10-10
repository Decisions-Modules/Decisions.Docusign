using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Decisions.Docusign.DSServiceReference;
using Decisions.Docusign.DataTypes;
using DecisionsFramework.Data.DataTypes;
using DecisionsFramework.Design.Flow.Mapping.InputImpl;

namespace Decisions.Docusign
{
    /// <summary>
    ///     This class was introduced to replace the previous Send Document For Signing.
    ///     Which only could send one document at a time. 
    /// </summary>
    [AutoRegisterStep("Send Documents For Signing", "Integration", "Docusign")]
    [Writable]
    public sealed class SendDocumentsForSigning : ISyncStep, IDataConsumer, IDataProducer, IDefaultInputMappingStep
    {
        #region Constants 
        
        private const string OUTCOME_SENT = "Sent";
        private const string OUTCOME_ERROR = "Error";

        private const string OUTPUT_ENVELOPEID = "EnvelopeID";
        private const string OUTPUT_ERRORMESSAGE = "ErrorMessage";

        private const string INPUT_DOCUMENT = "Document";
        private const string INPUT_RECIPIENTS = "Recipients";
        private const string INPUT_CC = "CC";
        private const string INPUT_SUBJECT = "Subject";
        private const string INPUT_EMAILBLURB = "EmailBlurb";
        private const string INPUT_CREDS = "OverrideCredentials";

        private const string INPUT_RESPECT_SIGNING_ORDER = "RespectSigningOrder";
        private const string INPUT_REMINDERS = "Reminders";

        #endregion
        
        #region ISyncStep Members

        public OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new[] {
                    new OutcomeScenarioData(OUTCOME_SENT, new DataDescription(typeof(string), OUTPUT_ENVELOPEID)),
                    new OutcomeScenarioData(OUTCOME_ERROR, new DataDescription(typeof(string), OUTPUT_ERRORMESSAGE))
                };
            }
        }
        
        public DataDescription[] InputData
        {
            get
            {
                return new DataDescription[]
                {
                    new DataDescription(typeof (FileData), INPUT_DOCUMENT, true),
                    new DataDescription(typeof (RecipientTabMapping), INPUT_RECIPIENTS, true),
                    new DataDescription(typeof(string), INPUT_CC, true),
                    new DataDescription(typeof (string), INPUT_SUBJECT),
                    new DataDescription(typeof (string), INPUT_EMAILBLURB),
                    new DataDescription(typeof (DocusignCredentials), INPUT_CREDS),
                    new DataDescription(typeof(bool), INPUT_RESPECT_SIGNING_ORDER),
                    new DataDescription(typeof(Notification), INPUT_REMINDERS),
                };
            }
        }

        public IInputMapping[] DefaultInputs
        {
            get
            {
                return new IInputMapping[]
                {
                    new SelectValueInputMapping { InputDataName = INPUT_DOCUMENT },
                    new SelectValueInputMapping { InputDataName = INPUT_RECIPIENTS },
                    new IgnoreInputMapping { InputDataName = INPUT_CC },
                    new SelectValueInputMapping { InputDataName = INPUT_SUBJECT },
                    new SelectValueInputMapping { InputDataName = INPUT_EMAILBLURB },
                    new NullInputMapping { InputDataName = INPUT_CREDS },
                    new NullInputMapping { InputDataName = INPUT_REMINDERS },
                };
            }
        }
        
        #endregion

        public ResultData Run(StepStartData data)
        {
            IDocusignCreds credentials;
            if (data.Data.ContainsKey(INPUT_CREDS) && data.Data[INPUT_CREDS] != null)
                credentials = data.Data[INPUT_CREDS] as IDocusignCreds;
            else
                credentials = DSServiceClientFactory.DsSettings;

            var dsClient = DSServiceClientFactory.GetDsClient(credentials);

            // Get Input Values
            var document = (FileData[])data.Data[INPUT_DOCUMENT];
            var recipients = (RecipientTabMapping[])data.Data[INPUT_RECIPIENTS];
            var cc = (string[])data.Data[INPUT_CC];
            var subject = (string)data.Data[INPUT_SUBJECT];
            var emailBlurb = (string)data.Data[INPUT_EMAILBLURB];
            var reminders = (Notification)data.Data[INPUT_REMINDERS];
            
            // Output
            Dictionary<string, object> resultData = new Dictionary<string, object>();

            try
            {
                using (var scope = new System.ServiceModel.OperationContextScope(dsClient.InnerChannel))
                {
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = 
                        DSServiceClientFactory.GetAuthHeaderRequestProperty(credentials);

                    var dsRecipients = new List<Recipient>();
                    var tabs = new List<Tab>();
                    var recipientsList = recipients.ToList();

                    List<Document> documents = new List<Document>();
                    var documentIndex = 1;
                    foreach (FileData doc in document)
                    {
                        documents.Add(new Document
                        {
                            PDFBytes = doc.Contents,
                            ID = documentIndex.ToString(),
                            Name = doc.FileName
                        });
                        documentIndex++;
                    }

                    // Add each Recipient with their Tabs.
                    var recipientIndex = 1;
                    foreach (var rtm in recipientsList)
                    {
                        int routingOrder = rtm.RoutingOrder + 1;// Increment by 1 so order is able to use 0
                        dsRecipients.Add(new Recipient
                        {
                            Email = rtm.EmailAddress,
                            UserName = rtm.RecipientName,
                            Type = RecipientTypeCode.Signer,
                            RoutingOrder = (ushort)routingOrder, 
                            /* RoutingOrder must be positive. And RoutingOrderSpecified cannot be null*/
                            RoutingOrderSpecified = routingOrder > 0 && (data["RespectSigningOrder"] as bool? ?? false),
                            ID = recipientIndex.ToString()
                        });

                        // Absolutely Positioned Tabs
                        if (rtm.AbsolutePositionTabs != null)
                        {
                            foreach (var apt in rtm.AbsolutePositionTabs)
                            {
                                tabs.Add(new Tab
                                {
                                    PageNumber = GetIdentifierOrDefaultOneAsString(apt.PageNumber),
                                    XPosition = apt.XPosition.ToString(),
                                    YPosition = apt.YPosition.ToString(),
                                    Type = apt.TabType,
                                    RecipientID = recipientIndex.ToString(),
                                    Name = rtm.RecipientName,
                                    DocumentID = GetIdentifierOrDefaultOneAsString(apt.DocumentId),
                                });
                            }
                        };
                        // AnchorStringTabs
                        // Docusign will search the document for these string values and attach a Tab at that location.
                        if (rtm.AnchorStringTabs != null)
                        {
                            foreach (var ast in rtm.AnchorStringTabs)
                            {
                                tabs.Add(new Tab
                                {
                                    PageNumber = GetIdentifierOrDefaultOneAsString(ast.PageNumber),
                                    AnchorTabItem = new AnchorTab { AnchorTabString = ast.AnchorTabString, XOffset = ast.XOffset, YOffset = ast.YOffset },
                                    Type = ast.TabType,
                                    Name = recipientIndex.ToString(),
                                    RecipientID = recipientIndex.ToString(),
                                    DocumentID = GetIdentifierOrDefaultOneAsString(ast.DocumentId),
                                });
                            }
                        };
                        recipientIndex++;
                    };
                    
                    // Add CC recipients
                    if (cc != null)
                    {
                        foreach (var ccEmail in cc)
                        {
                            dsRecipients.Add(new Recipient
                            {
                                Email = ccEmail,
                                UserName = ccEmail,
                                Type = RecipientTypeCode.CarbonCopy,
                                ID = recipientIndex.ToString(),
                            });
                            recipientIndex++;
                        }
                    }
                    
                    //construct the envelope and send; return EnvelopeID
                    var envelopeID = dsClient.CreateAndSendEnvelope(new Envelope
                    {
                        Subject = subject,
                        EmailBlurb = emailBlurb,
                        Recipients = dsRecipients.ToArray<Recipient>(),
                        AccountId = credentials.AccountId,
                        Documents = documents.ToArray(),
                        Tabs = tabs.ToArray(),
                        Notification = reminders
                    }).EnvelopeID;

                    resultData.Add(OUTPUT_ENVELOPEID, envelopeID);
                    return new ResultData(OUTCOME_SENT, resultData);
                }
            }
            catch (Exception ex)
            {
                resultData.Add(OUTPUT_ERRORMESSAGE, ex.Message);
                return new ResultData(OUTCOME_ERROR, resultData);
            }
        }

        /// <summary>
        /// This method returns the specified Identifier as a string with a minimum value of 1.
        /// This handles 1-based indexes where we need to sanitize user input or account for older hardcoded values.
        /// </summary>
        /// <returns></returns>
        private string GetIdentifierOrDefaultOneAsString(int id)
        {
            return (id > 0) ? id.ToString() : "1";
        }
    }
}