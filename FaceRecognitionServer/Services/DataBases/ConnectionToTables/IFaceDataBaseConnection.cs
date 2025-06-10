using FaceRecognitionServer.Services.DataBases.Models;

namespace FaceRecognitionServer.Services.DataBases.ConnectionToTables
{
    public interface IFaceDataBaseConnection : IDisposable
    {
        void AddFaceRecord(string governmentId, string firstName, string lastName, int? heightCm, string sex, float[] faceEmbedding, string notes);

        void UpdateFaceEmbedding(string governmentId, string firstName, string lastName, int? heightCm, string sex, string notes);

        AdvancedFaceData? GetByGovernmentID(string governmentId);

        List<AdvancedFaceData> GetAllFaceRecords();
    }
}