using DataProtocols;
using DataProtocols.FaceRecognitionMessages;
using FaceRecognitionServer.Services;
using FaceRecognitionServer.Services.DataBases.ConnectionToTables;
using FaceRecognitionServer.Services.DataBases.Models;

// Responsible for handling messages related to updating or inserting person face records in the database.
// Implements ITypedMessageHandler for the UpdatePersonDataRequestDTO message type.
public class FaceRecordHandler : ITypedMessageHandler<UpdatePersonDataRequestDTO>
{
    // Used to send responses back to the client
    private readonly ISecureNetworkManager m_NetworkManager;

    // Provides access to the face records table in the database
    private readonly ConnectionToFaceTable m_DatabaseConnection;

    public FaceRecordHandler(ISecureNetworkManager networkManager, ConnectionToFaceTable facesStorageSystem)
    {
        m_NetworkManager = networkManager;
        m_DatabaseConnection = facesStorageSystem;
    }

    // Handles a request to either update an existing person or insert a new one
    public async Task HandleMessageAsync(UpdatePersonDataRequestDTO message, string ip)
    {
        var response = new UpdatePersonDataResponseDTO();

        try
        {
            // Look up the record by its ID
            var existing = m_DatabaseConnection.GetById(message.Id);

            // Validate if the government ID is already used by someone else
            if (m_DatabaseConnection.IsGovernmentIdTaken(existing.GovernmentID, message.Id))
            {
                response.Success = false;
                response.ValidationResult.GovernmentIDError = "this Id is taken";
            }
            else
            {
                if (existing != null)
                {
                    //record exists, Update the existing face record with the new values
                    existing.GovernmentID = message.GovernmentID;
                    existing.FirstName = message.FirstName;
                    existing.LastName = message.LastName;
                    existing.HeightCm = message.HeightCm;
                    existing.Sex = message.Sex;
                    existing.Notes = message.Notes;

                    m_DatabaseConnection.UpdateFaceRecord(existing);
                }
                else
                {
                    // If record doesn’t exist, create a new one (embedding must be provided)
                    if (message.FaceEmbedding == null)
                    {
                        throw new Exception("Cannot add a new person without a face embedding.");
                    }

                    var newFace = new AdvancedFaceData
                    {
                        GovernmentID = message.GovernmentID,
                        FirstName = message.FirstName,
                        LastName = message.LastName,
                        HeightCm = message.HeightCm,
                        Sex = message.Sex,
                        FaceEmbedding = message.FaceEmbedding,
                        Notes = message.Notes
                    };

                    m_DatabaseConnection.AddFaceRecord(newFace);
                }

                response.Success = true;
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
        }

        // Send back the result of the update/insert operation
        var responseJson = ConvertUtils.Serialize(response);
        m_NetworkManager.SendMessage(responseJson, ip);
    }
}
