namespace FaceRecognitionServer.Services.DataBases.Models
{
    public class AdvancedFaceData : BasicFaceData
    {
        public int? HeightCm { get; set; }
        public string Sex { get; set; }
        public float[] FaceEmbedding { get; set; }
        public string Notes { get; set; }
    }
}
