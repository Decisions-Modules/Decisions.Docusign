using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecisionsFramework.Data.DataTypes;
using DecisionsFramework.Design.Flow;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Decisions.Docusign.DSServiceReference;
using DecisionsFramework.Design.Flow.StepImplementations;
using System.Collections.Generic;

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

            using (var scope = new System.ServiceModel.OperationContextScope(dsClient.InnerChannel))
            {                
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = DSServiceClientFactory.GetAuthHeaderRequestProperty(creds);
                
                return dsClient.RequestStatus(envelopeId).Status.ToString();
            }

        }          

		[Obsolete]
        [ExcludeMethodOnAutoRegister]
        public static FileData GetSignedDocument(string envelopeId, [IgnoreMappingDefault] DocusignCredentials overrideCredentials = null)
        {
            IDocusignCreds creds = overrideCredentials as IDocusignCreds ?? DSServiceClientFactory.DsSettings;

            var dsClient = DSServiceClientFactory.GetDsClient(creds);

            using (var scope = new System.ServiceModel.OperationContextScope(dsClient.InnerChannel))
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
			
            using (var scope = new System.ServiceModel.OperationContextScope(dsClient.InnerChannel))
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
