using Decisions.Docusign.DSServiceReference;
using System.Runtime.Serialization;

namespace Decisions.Docusign.DataTypes
{
    [DataContract]
    public class SimpleTab
    {
        [DataMember]
        public int XPosition { get; set; }

        [DataMember]
        public int YPosition { get; set; }

        [DataMember]
        public int PageNumber { get; set; }

        [DataMember]
        public TabTypeCode TabType { get; set; }

        [DataMember]
        public int DocumentId { get; set; }
        
        public override string ToString()
        {
            return $"{TabType.ToString()} on page {PageNumber}, document {DocumentId} at {XPosition},{YPosition}.";
        }
    }
}
