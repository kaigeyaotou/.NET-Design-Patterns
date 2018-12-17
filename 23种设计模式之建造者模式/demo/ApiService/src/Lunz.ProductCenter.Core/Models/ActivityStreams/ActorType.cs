using System.Runtime.Serialization;

namespace Lunz.ProductCenter.Core.Models.ActivityStreams
{
    public enum ActorType
    {
        /// <summary>
        /// Person
        /// </summary>
        [EnumMember(Value = "Person")]
        Person,
    }
}