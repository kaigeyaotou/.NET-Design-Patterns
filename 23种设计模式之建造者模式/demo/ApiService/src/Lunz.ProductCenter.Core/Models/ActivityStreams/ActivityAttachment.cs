using System.Runtime.Serialization;

namespace Lunz.ProductCenter.Core.Models.ActivityStreams
{
    [DataContract(Namespace = "")]
    public class ActivityAttachment
    {
        [DataMember(EmitDefaultValue = false)]
        public string Type { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Content { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }
    }
}