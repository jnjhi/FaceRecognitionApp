using System.Drawing;

namespace FaceRecognitionServer.Services.DataBases.Models
{
    public  class AdvancedFaceDataWithProfilePicture : AdvancedFaceData
    {
        public Bitmap profilePicture { get; set; }
        public DateTime CaptureDate { get; set; }
    }
}
