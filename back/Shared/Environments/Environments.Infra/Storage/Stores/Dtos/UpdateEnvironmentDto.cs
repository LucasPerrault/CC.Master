using System.Text.Json.Serialization;

namespace Environments.Infra.Storage.Stores.Dtos
{
    public class UpdateEnvironmentDto
    {
        public UpdateEnvironmentDto(string newName)
        {
            NewName = newName;
        }

        [JsonPropertyName("newName")]
        public string NewName { get; init; }
    }
}
