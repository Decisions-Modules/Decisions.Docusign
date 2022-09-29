using DecisionsFramework.ServiceLayer;
using Decisions.Docusign.DSServiceReference;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Decisions.Docusign
{
    public class DSServiceClientFactory
    {

        private const string DEMO_ENDPOINT = "https://demo.docusign.net/api/3.0/dsapi.asmx";
        private const string PROD_ENDPOINT = "api/3.0/dsapi.asmx";

        
        private const string LEGACY_PROD_BASE_URL = "https://www.docusign.net";
        
        public static DocusignSettings DsSettings
        {
            get
            {
                return ModuleSettingsAccessor<DocusignSettings>.GetSettings();
            }
        }

        private static string GetDsAuth(IDocusignCreds creds)
        {
            return "<DocuSignCredentials><Username>" + creds.UserName
                + "</Username><Password>" + creds.Password
                + "</Password><IntegratorKey>" + creds.IntegratorKey
                + "</IntegratorKey></DocuSignCredentials>";
        }

        public static HttpRequestMessageProperty GetAuthHeaderRequestProperty(IDocusignCreds creds)
        {
            var httpRequestProperty = new HttpRequestMessageProperty();
            httpRequestProperty.Headers.Add("X-DocuSign-Authentication", GetDsAuth(creds));
            return httpRequestProperty;
        }

        public static DSAPIServiceSoapClient GetDsClient(IDocusignCreds creds)
        {
            return new DSAPIServiceSoapClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport) { MaxReceivedMessageSize = 167772160 }, new EndpointAddress(GetEndpoint(creds)));
        }

        private static string GetEndpoint(IDocusignCreds creds)
        {
            if (creds.UseDemoEnvironment)
            {
                return DEMO_ENDPOINT;
            }
            else
            {
                string baseUrl = DsSettings.BaseUrl;
                if (string.IsNullOrEmpty(baseUrl))
                {
                    baseUrl = LEGACY_PROD_BASE_URL;
                }
                
                return $"{baseUrl.TrimEnd('/')}/{PROD_ENDPOINT}";
            }
        }
    }
}
