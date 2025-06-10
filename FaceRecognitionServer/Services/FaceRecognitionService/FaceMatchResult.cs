using FaceRecognitionServer.Services.DataBases.Models;

namespace FaceRecognitionServer.Services.FaceRecognitionService
{
    public class FaceMatchResult
    {
        public bool IsAMatch { get; set; }
        public AdvancedFaceData FaceRecord { get; set; }
        public double Distance { get; set; }
    }
}
