using System.Runtime.Serialization;

namespace Lunz.ProductCenter.Core.Models.ActivityStreams
{
    /// <summary>
    /// https://www.w3.org/TR/activitystreams-vocabulary/#activity-types
    /// </summary>
    public enum ActivityType
    {
        /// <summary>
        /// Activity
        /// </summary>
        [EnumMember(Value = "Activity")]
        Activity = 0,

        /// <summary>
        /// Accept
        /// </summary>
        [EnumMember(Value = "Accept")]
        Accept,

        /// <summary>
        /// Add
        /// </summary>
        [EnumMember(Value = "Add")]
        Add,

        /// <summary>
        /// Announce
        /// </summary>
        [EnumMember(Value = "Announce")]
        Announce,

        /// <summary>
        /// Arrive
        /// </summary>
        [EnumMember(Value = "Arrive")]
        Arrive,

        /// <summary>
        /// Block
        /// </summary>
        [EnumMember(Value = "Block")]
        Block,

        /// <summary>
        /// Create
        /// </summary>
        [EnumMember(Value = "Create")]
        Create,

        /// <summary>
        /// Delete
        /// </summary>
        [EnumMember(Value = "Delete")]
        Delete,

        /// <summary>
        /// Dislike
        /// </summary>
        [EnumMember(Value = "Dislike")]
        Dislike,

        /// <summary>
        /// Flag
        /// </summary>
        [EnumMember(Value = "Flag")]
        Flag,

        /// <summary>
        /// Follow
        /// </summary>
        [EnumMember(Value = "Follow")]
        Follow,

        /// <summary>
        /// Ignore
        /// </summary>
        [EnumMember(Value = "Ignore")]
        Ignore,

        /// <summary>
        /// Invite
        /// </summary>
        [EnumMember(Value = "Invite")]
        Invite,

        /// <summary>
        /// Join
        /// </summary>
        [EnumMember(Value = "Join")]
        Join,

        /// <summary>
        /// Leave
        /// </summary>
        [EnumMember(Value = "Leave")]
        Leave,

        /// <summary>
        /// Like
        /// </summary>
        [EnumMember(Value = "Like")]
        Like,

        /// <summary>
        /// Listen
        /// </summary>
        [EnumMember(Value = "Listen")]
        Listen,

        /// <summary>
        /// Move
        /// </summary>
        [EnumMember(Value = "Move")]
        Move,

        /// <summary>
        /// Offer
        /// </summary>
        [EnumMember(Value = "Offer")]
        Offer,

        /// <summary>
        /// Question
        /// </summary>
        [EnumMember(Value = "Question")]
        Question,

        /// <summary>
        /// Reject
        /// </summary>
        [EnumMember(Value = "Reject")]
        Reject,

        /// <summary>
        /// Read
        /// </summary>
        [EnumMember(Value = "Read")]
        Read,

        /// <summary>
        /// Remove
        /// </summary>
        [EnumMember(Value = "Remove")]
        Remove,

        /// <summary>
        /// TentativeReject
        /// </summary>
        [EnumMember(Value = "TentativeReject")]
        TentativeReject,

        /// <summary>
        /// TentativeAccept
        /// </summary>
        [EnumMember(Value = "TentativeAccept")]
        TentativeAccept,

        /// <summary>
        /// Travel
        /// </summary>
        [EnumMember(Value = "Travel")]
        Travel,

        /// <summary>
        /// Undo
        /// </summary>
        [EnumMember(Value = "Undo")]
        Undo,

        /// <summary>
        /// Update
        /// </summary>
        [EnumMember(Value = "Update")]
        Update,

        /// <summary>
        /// View
        /// </summary>
        [EnumMember(Value = "View")]
        View,
    }
}