using DecisionsFramework;
using System.Runtime.Serialization;
using DecisionsFramework.Utilities.validation.Rules;

namespace Decisions.Docusign.DataTypes
{
    [DataContract]
    [ValidationRules]
    public class RecipientTabMapping: IValidationSource
    {
        [DataMember]
        [EmptyStringRule]
        public string EmailAddress { get; set; }

        [DataMember]
        public string RecipientName { get; set; }
        
        [DataMember]
        public int RoutingOrder { get; set; }
        
        [DataMember]
        public bool DefaultRecipient { get; set; }

        [DataMember]
        public SimpleTab[] AbsolutePositionTabs { get; set; }       

        [DataMember]
        public SimpleAnchorTab[] AnchorStringTabs { get; set; }

        public override string ToString()
        {
            return EmailAddress;
        }

        public ValidationIssue[] GetValidationIssues()
        {
            return null;
        }
    }
}
