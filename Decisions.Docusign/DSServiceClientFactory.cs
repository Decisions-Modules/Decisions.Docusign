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
        private const string LEGACY_PROD_BASE_URL = "https://www.docusign.net/";

        public static DocusignSettings DsSettings => ModuleSettingsAccessor<DocusignSettings>.GetSettings();

        private static string GetDsAuth(IDocusignCreds credentials)
        {
            return "<DocuSignCredentials><Username>" + credentials.UserName
                + "</Username><Password>" + credentials.Password
                + "</Password><IntegratorKey>" + credentials.IntegratorKey
                + "</IntegratorKey></DocuSignCredentials>";
        }

        public static HttpRequestMessageProperty GetAuthHeaderRequestProperty(IDocusignCreds credentials)
        {
            var httpRequestProperty = new HttpRequestMessageProperty();
            httpRequestProperty.Headers.Add("X-DocuSign-Authentication", GetDsAuth(credentials));
            return httpRequestProperty;
        }

        public static DSAPIServiceSoapClient GetDsClient(IDocusignCreds credentials) =>
            new DSAPIServiceSoapClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport) 
                { MaxReceivedMessageSize = 167772160 }, new EndpointAddress(GetEndpoint(credentials)));
        
        private static string GetEndpoint(IDocusignCreds credentials)
        {
            if (credentials.UseDemoEnvironment)
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
