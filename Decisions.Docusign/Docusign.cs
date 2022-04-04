using System;
using System.Collections.Generic;
using DecisionsFramework.Data.DataTypes;
using DecisionsFramework.Design.Flow;
using System.ServiceModel;
using System.ServiceModel.Channels;
using DecisionsFramework.Design.Flow.StepImplementations;
using System.Xml.Serialization;
using System.IO;
using Decisions.Docusign.DSServiceReference;

namespace Decisions.Docusign
{
    [AutoRegisterMethodsOnClass(true, "Integration", "Docusign")]
    public static class DocusignSteps
    {        
        //Docusign API dev guide says this method is subject to call limit and should not be used more than once every 15 min per unique envelope ID
        public static string GetDocumentStatus(string envelopeId, [IgnoreMappingDefault] DocusignCredentials overrideCredentials = null)
        {
            IDocusignCreds creds = overrideCredentials as IDocusignCreds ?? DSServiceClientFactory.DsSettings;

            var dsClient = DSServiceClientFactory.GetDsClient(creds);

            using (var scope = new OperationContextScope(dsClient.InnerChannel))
            {                
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = DSServiceClientFactory.GetAuthHeaderRequestProperty(creds);
                
                return dsClient.RequestStatus(envelopeId).Status.ToString();
            }
        }
        
        //Renaming step with AutoRegister since the method name is misspelled DT-032541
        [AutoRegisterMethod("Deserialize Docusign Envelope Information")]  
        public static Docusign.DataTypes.DocuSignEnvelopeInformation DeserialiseDocusignEnvelopeInformation(string XML)
        {
            var result = (DataTypes.DocuSignEnvelopeInformation) 
                new XmlSerializer(typeof(DataTypes.DocuSignEnvelopeInformation), 
                    "http://www.docusign.net/API/3.0").Deserialize(new StringReader(XML));
            return result;
        }

        [Obsolete]
        [ExcludeMethodOnAutoRegister]
        public static FileData GetSignedDocument(string envelopeId, [IgnoreMappingDefault] DocusignCredentials overrideCredentials = null)
        {
            IDocusignCreds creds = overrideCredentials as IDocusignCreds ?? DSServiceClientFactory.DsSettings;

            var dsClient = DSServiceClientFactory.GetDsClient(creds);

            using (var scope = new OperationContextScope(dsClient.InnerChannel))
            {
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = DSServiceClientFactory.GetAuthHeaderRequestProperty(creds);

                var documentsPDFs = dsClient.RequestDocumentPDFs(envelopeId);

                if (documentsPDFs == null || documentsPDFs.DocumentPDF == null || documentsPDFs.DocumentPDF.Length == 0)
                {
                    return null;
                }

                return new FileData(string.Format("{0}.pdf", documentsPDFs.DocumentPDF[0].Name), documentsPDFs.DocumentPDF[0].PDFBytes);
            }
        }
        
        /// <summary>
        /// Returns all the documents within an envelope. With the exception of the summary.pdf file
        /// </summary>
        /// <param name="envelopeId"></param>
        /// <param name="overrideCredentials"></param>
        /// <returns></returns>
        public static FileData[] GetSignedDocuments(string envelopeId, [IgnoreMappingDefault] DocusignCredentials overrideCredentials=null)
        {
            IDocusignCreds creds = overrideCredentials as IDocusignCreds ?? DSServiceClientFactory.DsSettings;
            DSAPIServiceSoapClient dsClient = DSServiceClientFactory.GetDsClient(creds);
			
            // Null check. No good documentation regarding DSAPIServiceSoapClient
            if (dsClient == null)
                return null;
			
            using (var scope = new OperationContextScope(dsClient.InnerChannel))
            {
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] =
                    DSServiceClientFactory.GetAuthHeaderRequestProperty(creds);

                DocumentPDFs documentPDFS = dsClient.RequestDocumentPDFsEx(envelopeId);

                if (documentPDFS == null || documentPDFS.DocumentPDF == null || documentPDFS.DocumentPDF.Length == 0 )
                {
                    return null;
                }
                
                List<FileData> ret = new List<FileData>(); 
                for(int i=0; i<documentPDFS.DocumentPDF.Length; i++)
                {
					// Only add to return array if document is not a summary. 
                    if (!(documentPDFS.DocumentPDF[i].DocumentType == DocumentType.SUMMARY))
                        ret.Add(new FileData(documentPDFS.DocumentPDF[i].Name, documentPDFS.DocumentPDF[i].PDFBytes));
                }

                return ret.ToArray();
            }
        }
        
        public static FileData GetCertificate(string envelopeId, [IgnoreMappingDefault] DocusignCredentials overrideCredentials = null)
        {
            IDocusignCreds creds = overrideCredentials as IDocusignCreds ?? DSServiceClientFactory.DsSettings;

            DSAPIServiceSoapClient dsClient = DSServiceClientFactory.GetDsClient(creds);

            using (OperationContextScope scope = new OperationContextScope(dsClient.InnerChannel))
            {
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = DSServiceClientFactory.GetAuthHeaderRequestProperty(creds);

                var documentsPDFs = dsClient.RequestCertificate(envelopeId);

                if (documentsPDFs == null || documentsPDFs.DocumentPDF == null || documentsPDFs.DocumentPDF.Length == 0)
                {
                    return null;
                }

                return new FileData($"{documentsPDFs.DocumentPDF[0].Name}.pdf", documentsPDFs.DocumentPDF[0].PDFBytes);
            }
        }
    }
}
