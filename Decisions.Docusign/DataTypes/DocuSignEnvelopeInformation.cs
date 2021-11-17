using Decisions.Docusign.DSServiceReference;

namespace Decisions.Docusign.DataTypes
{

    [System.Xml.Serialization.XmlRoot(ElementName = "DocuSignEnvelopeInformation")]
    public class DocuSignEnvelopeInformation
    {



        /// <remarks/>
        public EnvelopeStatus EnvelopeStatus { get; set; }

        public DocumentPDF[] DocumentPDFs { get; set; }

        /// <remarks/>
        public string TimeZone { get; set; }


        /// <remarks/>
        public sbyte TimeZoneOffset { get; set; }

    }


}
