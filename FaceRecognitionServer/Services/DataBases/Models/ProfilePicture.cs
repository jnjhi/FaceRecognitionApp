using System.Drawing;

namespace FaceRecognitionServer.Services.DataBases.Models
{
    public class ProfilePicture
    {
        public int IdentifiedPersonId { get; set; }

        public Bitmap Image { get; set; }

        public DateTime CaptureDate { get; set; }

        public ProfilePicture(int identifiedPersonId, Bitmap image, DateTime captureDate)
        {
            IdentifiedPersonId = identifiedPersonId;
            Image = image;
            CaptureDate = captureDate;
        }
    }
}
