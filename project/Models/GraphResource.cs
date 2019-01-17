using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoAirlines.Models
{
    public class GraphResource
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Id { get; set; }
    }

    public class Group : GraphResource
    {
        public const string SchemaExtensionName = "YOUR_SCHEMA_EXTENSION_NAME";

        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string[] GroupTypes { get; set; }
        public bool MailEnabled { get; set; }
        public string MailNickname { get; set; }
        public bool SecurityEnabled { get; set; }
        public string Visibility { get; set; }

        [JsonProperty(PropertyName = SchemaExtensionName, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ProvisioningExtension Extension { get; set; }

        [JsonProperty(PropertyName = "members@odata.bind", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Members { get; set; }

        [JsonProperty(PropertyName = "owners@odata.bind", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Owners { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string[] ResourceProvisioningOptions { get; set; }
    }

    public class ProvisioningExtension
    {
        public int SharePointItemId { get; set; }
    }

    public class Team : GraphResource
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string WebUrl { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string DisplayName { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsArchived { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public TeamGuestSettings GuestSettings { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public TeamMemberSettings MemberSettings { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public TeamMessagingSettings MessagingSettings { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public TeamFunSettings FunSettings { get; set; }
    }

    public class TeamGuestSettings
    {
        public bool AllowCreateUpdateChannels { get; set; }
        public bool AllowDeleteChannels { get; set; }
    }

    public class TeamMemberSettings
    {
    }

    public class TeamFunSettings
    {
    }

    public class TeamMessagingSettings
    {
    }

    public class PostMessage
    {
        public RootMessage rootMessage { get; set; }
    }

    public class RootMessage
    {
        public MessageBody body { get; set; }
    }

    public class MessageBody
    {
        public string content { get; set; }
    }

    public class Clone
    {
        public string displayName { get; set; }
        public string description { get; set; }
        public string mailNickName { get; set; }
        public string teamVisibilityType { get; set; }
        public string partsToClone { get; set; } // "apps,members,settings,tabs,channels"
    }


    public class CloneOperationRequest
    {
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "mailNickName")]
        public string MailNickName { get; set; }
        [JsonProperty(PropertyName = "classification")]
        public string Classification { get; set; }
        [JsonProperty("visibility")]
        public string Visibility { get; set; }
    }

    public enum AsyncOperationStatus
    {
        Invalid = 0,
        NotStarted = 1,
        InProgress = 2,
        Succeeded = 3,
        Failed = 4
    }
    public enum AsyncOperationType
    {
        /// <summary>
        /// Invalid value.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// Operation to clone a team.
        /// </summary>
        CloneTeam = 1,

        /// <summary>
        /// Operation to archive a team.
        /// </summary>
        ArchiveTeam = 2,

        /// <summary>
        /// Operation to unarchive a team.
        /// </summary>
        UnArchiveTeam = 3
    }
    public sealed class OperationError
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets an error message for the developer.
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }

    public sealed class TeamsAsyncOperation
    {
        /// <summary>
        /// Gets or sets the identifier that uniquely identifies a specific instance of the async operation.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the type of async operation.
        /// </summary>
        [JsonProperty(PropertyName = "operationType")]
        public AsyncOperationType OperationType { get; set; }

        /// <summary>
        /// Gets or sets the UTC time when the async operation was created.
        /// </summary>
        [JsonProperty(PropertyName = "createdDateTime")]
        public DateTimeOffset CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the latest status of the async operation.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public AsyncOperationStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the UTC time when the async operation was last updated.
        /// </summary>
        public DateTimeOffset lastActionDateTime { get; set; }

        /// <summary>
        /// Gets or sets the number of times the operation was attempted before being marked suceeded/failed.
        /// </summary>
        [JsonProperty(PropertyName = "attemptsCount")]
        public int AttemptsCount { get; set; }

        public string targetResourceId { get; set; }
        public string targetResourceLocation { get; set; }

        /// <summary>
        /// Gets or sets the details of any error that causes the async operation to fail.
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public OperationError Error { get; set; }
    }

    public class TeamsResource
    {
        /// <summary>
        /// Gets or sets the unique id of the resource.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the type of the resource.
        /// </summary>
        [JsonProperty(PropertyName = "resourceType")]
        public TeamsResourceType ResourceType { get; set; }

        /// <summary>
        /// Gets or sets the resource url that can be used for accessing the resource via MS Graph.
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }

    public enum TeamsResourceType
    {
        /// <summary>
        /// Invalid state.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// Represents a resource which is a Team.
        /// </summary>
        Team = 1,

        /// <summary>
        /// Represents a resource which is a channel.
        /// </summary>
        Channel = 2
    }

    public class ResultList<T>
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public T[] value { get; set; }
    }

    //class TeamsAppJsonConverter : JsonConverter
    //{
    //    public override bool CanConvert(Type objectType)
    //    => objectType.IsAssignableFrom(typeof(TeamsApp));
    //    public override bool CanRead => false;
    //    public override bool CanWrite => true;
    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        TeamsApp app = (TeamsApp) value;
    //        writer.WriteValue(app.Id);
    //    }
    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public sealed class TeamsTab
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Name { get; set; } // obsolete
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string TeamsAppId { get; set; }

        [JsonProperty(PropertyName = "teamsApp@odata.bind", DefaultValueHandling = DefaultValueHandling.Ignore)]
        //[JsonConverter(typeof(TeamsAppJsonConverter))]
        public string TeamsApp { get; set; }

        [JsonProperty(PropertyName = "sortOrderIndex")]
        public string SortOrderIndex { get; set; }
        [JsonProperty(PropertyName = "messageId")]
        public string MessageId { get; set; }
        [JsonProperty(PropertyName = "webUrl")]
        public string WebUrl { get; set; }
        [JsonProperty(PropertyName = "configuration")]
        public TeamsTabConfiguration Configuration { get; set; }
    }

    public sealed class TeamsTabConfiguration
    {
        [JsonProperty(PropertyName = "entityId")]
        public string EntityId { get; set; }
        [JsonProperty(PropertyName = "contentUrl")]
        public string ContentUrl { get; set; }
        [JsonProperty(PropertyName = "removeUrl")]
        public string RemoveUrl { get; set; }
        [JsonProperty(PropertyName = "websiteUrl")]
        public string WebsiteUrl { get; set; }
        [JsonProperty(PropertyName = "properties")]
        [JsonExtensionData(ReadData = true, WriteData = true)]
        public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    }

    public class User : GraphResource
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string userPrincipalName { get; set; }
    }

    public class Invitation
    {
        public string InvitedUserEmailAddress { get; set; }
        public string InviteRedirectUrl { get; set; }
        public bool SendInvitationMessage { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public User InvitedUser { get; set; }
    }

    public class AddUserToGroup
    {
        [JsonProperty(PropertyName = "@odata.id")]
        public string UserPath { get; set; }
    }

    public class Channel : GraphResource
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string WebUrl { get; set; }
        public string Email { get; set; }
    }

    public class ChatThread
    {
        public ChatMessage RootMessage { get; set; }
    }

    public class ChatMessage
    {
        public ItemBody Body { get; set; }
    }

    public class ItemBody
    {
        public string Content { get; set; }
    }

    public class TeamsApp
    {
        public string Id { get; set; }
        public string ExternalId { get; set; }
        public string DisplayName { get; set; }
        public TeamsAppDistributionMethod DistributionMethod { get; set; }
        public IEnumerable<TeamsAppDefinition> AppDefinitions { get; set; }
    }
    public class TeamsAppDefinition
    {
        public string Id { get; set; }
        public string TeamsAppId { get; set; }
        public string DisplayName { get; set; }
        public string Version { get; set; }
    }
    public class TeamsAppInstallation
    {
        public string Id { get; set; }
        public TeamsApp TeamsApp { get; set; }
        public TeamsAppDefinition TeamsAppDefinition { get; set; }
    }
    public enum TeamsAppDistributionMethod
    {
        Store = 0,
        Organization = 1,
        Sideloaded = 2,
        UnknownFutureValue = 3
    }

    public class Site : GraphResource
    {
        public SiteCollection SiteCollection { get; set; }
    }

    public class SiteCollection
    {
        public string Hostname { get; set; }
    }

    public class DriveItem : GraphResource
    {
        public ItemReference ParentReference { get; set; }
    }

    public class ItemReference
    {
        public string DriveId { get; set; }
        public string Id { get; set; }
    }

    public class Plan : GraphResource
    {
        public string Title { get; set; }
        public string Owner { get; set; }
    }

    public class Bucket : GraphResource
    {
        public string Name { get; set; }
        public string PlanId { get; set; }
    }

    public class PlannerTask : GraphResource
    {
        public string Title { get; set; }
        public string PlanId { get; set; }
        public string BucketId { get; set; }
        public DateTimeOffset DueDateTime { get; set; }
    }

    public class SharePointList : GraphResource
    {
        public string DisplayName { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ColumnDefinition> Columns { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string WebUrl { get; set; }
    }

    public class ColumnDefinition
    {
        public string Name { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public TextColumn Text { get; set; }

    }

    public class TextColumn
    {
        [JsonProperty(PropertyName = "@odata.type")]
        public string Type { get { return "microsoft.graph.textColumn"; } }
    }

    public class Notification : GraphResource
    {
        public string TargetHostName { get; set; }
        public string AppNotificationId { get; set; }
        public DateTimeOffset ExpirationDateTime { get; set; }
        public NotificationPayload Payload { get; set; }
        public NotificationTargetPolicy TargetPolicy { get; set; }
        public string Priority { get; set; }
        public string GroupName { get; set; }
        public int DisplayTimeToLive { get; set; }
    }

    public class NotificationPayload
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string RawContent { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public NotificationVisualContent VisualContent { get; set; }
    }

    public class NotificationVisualContent
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }

    public class NotificationTargetPolicy
    {
        public string[] PlatformTypes { get; set; }
    }

    public class SharePointPage : GraphResource
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public List<SharePointWebPart> WebParts { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string WebUrl { get; set; }
    }

    public class SharePointWebPart
    {
        public const string ListWebPart = "f92bf067-bc19-489e-a556-7fe95f508720";

        public string Type { get; set; }
        public WebPartData Data { get; set; }
    }

    public class WebPartData
    {
        public string DataVersion { get; set; }
        public object Properties { get; set; }
    }

    public class ListProperties
    {
        public bool IsDocumentLibrary { get; set; }
        public string SelectedListId { get; set; }
        public int WebpartHeightKey { get; set; }
    }

    public class GraphCollection<T>
    {
        public List<T> Value { get; set; }
    }
}
