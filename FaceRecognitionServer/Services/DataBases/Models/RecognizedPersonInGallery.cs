namespace FaceRecognitionServer.Services.DataBases.Models
{
    public class RecognizedPersonInGallery : AdvancedFaceData
    {
        public string Rectangle { get; set; }
        public float? Confidence { get; set; }
        public bool IsUnknown { get; set; }
    }

}
