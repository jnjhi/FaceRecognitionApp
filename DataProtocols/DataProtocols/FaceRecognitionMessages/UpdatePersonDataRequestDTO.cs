using DataProtocols;
using Newtonsoft.Json;

[Serializable]
public class UpdatePersonDataRequestDTO : Data
{
    [JsonProperty]
    public int Id;

    [JsonProperty]
    public string GovernmentID;

    [JsonProperty]
    public string FirstName;

    [JsonProperty]
    public string LastName;

    [JsonProperty]
    public int? HeightCm;

    [JsonProperty]
    public string Sex;

    [JsonProperty]
    public string Notes;

    [JsonProperty]
    public float[] FaceEmbedding;

    public UpdatePersonDataRequestDTO()
    {
        DataType = DataType.UpdatePersonDataRequest;
    }

    public UpdatePersonDataRequestDTO(int internalId, string governmentId, string firstName, string lastName, int? heightCm, string sex, string notes, float[] embedding = null)
    {
        DataType = DataType.UpdatePersonDataRequest;
        Id = internalId;
        GovernmentID = governmentId;
        FirstName = firstName;
        LastName = lastName;
        HeightCm = heightCm;
        Sex = sex;
        Notes = notes;
        FaceEmbedding = embedding;
    }
}
