using Decisions.Docusign.DSServiceReferenceV2;
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

        [DataMember]
        public bool? IgnoreIfNotPresent { get; set; }

        [DataMember]
        public UnitTypeCode? Unit { get; set; }

        [DataMember]
        public bool? Required { get; set; }

        [DataMember]
        public int? Height { get; set; }

        [DataMember]
        public int? Width { get; set; }

        [DataMember]
        public string TabLabel { get; set; }

        [DataMember]
        public bool? Bold { get; set; }

        [DataMember]
        public bool? Italic { get; set; }

        [DataMember]
        public bool? Underline { get; set; }

        [DataMember]
        public string ConditionalParentLabel { get; set; }

        [DataMember]
        public string ConditionalParentValue { get; set; }

        [DataMember]
        public CustomTabType? CustomTabType { get; set; }

        [DataMember]
        public bool? DisableAutoSize { get; set; }

        [DataMember]
        public bool? Locked { get; set; }

        [DataMember]
        public bool? ConcealValueOnDocument { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string ListSelectedValue { get; set; }

        [DataMember]
        public string XPosition { get; set; }

        [DataMember]
        public string YPosition { get; set; }

        [DataMember]
        public bool? RequireAll { get; set; }

        [DataMember]
        public string RadioGroupName { get; set; }

        [DataMember]
        public string ValidationMessage { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Font? Font { get; set; }

        [DataMember]
        public FontSize? FontSize { get; set; }

        [DataMember]
        public FontColor? FontColor { get; set; }

        public override string ToString()
        {
            return $"{TabType.ToString()} at {AnchorTabString} on page {PageNumber} document {DocumentId}, offset {XOffset},{YOffset}.";
        }
    }
}
