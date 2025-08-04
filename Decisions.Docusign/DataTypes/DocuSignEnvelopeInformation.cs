using Decisions.Docusign.DSServiceReferenceV2;

namespace Decisions.Docusign.DataTypes
{
    [System.Xml.Serialization.XmlRoot(ElementName = "DocuSignEnvelopeInformation")]
    public class DocuSignEnvelopeInformation
    {
        public EnvelopeStatus EnvelopeStatus { get; set; }

        public DocumentPDF[] DocumentPDFs { get; set; }
        
        public string TimeZone { get; set; }

        public sbyte TimeZoneOffset { get; set; }
    }
}
