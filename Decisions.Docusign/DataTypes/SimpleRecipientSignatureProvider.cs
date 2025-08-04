using System.Runtime.Serialization;

namespace Decisions.Docusign.DataTypes
{
    public class SimpleRecipientSignatureProvider
    {
        [DataMember]
        public string SignatureProviderName { get; set; }
        
        [DataMember]
        public string Sms { get; set; }
        
        [DataMember]
        public string OneTimePassword { get; set; }
        
        [DataMember]
        public string CPFNumber { get; set; }
        
        [DataMember]
        public string SignerRole { get; set; }
    }
}