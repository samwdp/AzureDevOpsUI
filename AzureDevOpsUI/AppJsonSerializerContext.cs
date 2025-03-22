using System.Text.Json.Serialization;
using AzureDevOpsUi;

[JsonSerializable(typeof(Config))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
