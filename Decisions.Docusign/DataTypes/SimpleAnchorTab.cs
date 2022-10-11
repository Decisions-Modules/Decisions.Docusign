using Decisions.Docusign.DSServiceReference;
using System.Runtime.Serialization;

namespace Decisions.Docusign.DataTypes
{
    [DataContract]
    public class SimpleAnchorTab
    {
        [DataMember]
        public string AnchorTabString { get; set; }

        [DataMember]
        public int XOffset { get; set; }

        [DataMember]
        public int YOffset { get; set; }

        [DataMember]
        public int PageNumber { get; set; }

        [DataMember]
        public TabTypeCode TabType { get; set; }

        [DataMember]
        public int DocumentId { get; set; }
        
        public override string ToString()
        {
            return $"{TabType.ToString()} at {AnchorTabString} on page {PageNumber} document {DocumentId}, offset {XOffset},{YOffset}.";
        }
    }
}
