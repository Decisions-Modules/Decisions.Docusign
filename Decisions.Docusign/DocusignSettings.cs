using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.Design.Properties.Attributes;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.ServiceLayer.Actions;
using DecisionsFramework.ServiceLayer.Actions.Common;
using DecisionsFramework.ServiceLayer.Utilities;

namespace Decisions.Docusign
{
    
    public class DocusignSettings : AbstractModuleSettings, IInitializable, IDocusignCreds
    {
        [ORMField] 
        private string baseUrl;
        
        [ORMField]
        private string userName;

        [ORMField]
        private string accountId;

        [ORMField]
        private string password;

        [ORMField]
        private string loginEmail;

        [ORMField]
        private string integratorKey;

        [ORMField]
        private bool useDemoEnvironment;

        private const string CREDENTIALS_CATEGORY = "Docusign Credentials";
        
        public DocusignSettings()
        {
            this.EntityName = "Docusign Settings";
        }

        [PropertyClassification(0,  "User ID", CREDENTIALS_CATEGORY)]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        [PropertyClassification(10, "Login Email", CREDENTIALS_CATEGORY)]
        public string LoginEmail
        {
            get { return loginEmail; }
            set { loginEmail = value; }
        }

        [PropertyClassification(20, "API Account ID", CREDENTIALS_CATEGORY)]
        public string AccountId
        {
            get { return accountId; }
            set { accountId = value; }
        }

        [PropertyClassification(30, "App Password", CREDENTIALS_CATEGORY)]
        [PasswordText]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        [PropertyClassification(40, "Integrator Key", CREDENTIALS_CATEGORY)]
        [PasswordText]
        public string IntegratorKey
        {
            get { return integratorKey; }
            set { integratorKey = value; }
        }

        [PropertyClassification(50, "Use Demo Environment", CREDENTIALS_CATEGORY)]
        public bool UseDemoEnvironment
        {
            get { return useDemoEnvironment; }
            set { useDemoEnvironment = value; }
        }
                
        [PropertyClassification(60, "Base Url (Production Only)", CREDENTIALS_CATEGORY)]        
        [PropertyHiddenByValue(nameof(UseDemoEnvironment), true, true)]
        public string BaseUrl
        {
            get { return baseUrl; }
            set { baseUrl = value; }
        }

        public override BaseActionType[] GetActions(AbstractUserContext userContext, EntityActionType[] types)
        {
            return new BaseActionType[] { new EditEntityAction(typeof(DocusignSettings), "Edit", "") { IsDefaultGridAction = true } };
        }

        public void Initialize()
        {
            // Create default settings object
            ModuleSettingsAccessor<DocusignSettings>.GetSettings();
        }
    }
}
