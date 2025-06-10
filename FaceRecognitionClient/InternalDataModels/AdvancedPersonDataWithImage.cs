using System.Drawing;
using System.Windows.Media.Imaging;

namespace FaceRecognitionClient.InternalDataModels
{
    public class AdvancedPersonDataWithImage : AdvancedPersonData
    {
        public BitmapImage ProfileImage { get; set; }

        public AdvancedPersonDataWithImage() : base("", "", "")
        {
        }

        public AdvancedPersonDataWithImage(int id, string governmentId, string firstName, string lastName, int? heightCm, string sex, float[] faceEmbedding, string notes,  Rectangle rectangle, BitmapImage profileImage)
            : base(id, governmentId, firstName, lastName, heightCm, sex, faceEmbedding, notes, rectangle)
        {
            Rectangle = rectangle;
            ProfileImage = profileImage;
        }

        public AdvancedPersonDataWithImage(AdvancedPersonData baseData, BitmapImage profileImage)
            : base(baseData.Id, baseData.GovernmentID, baseData.FirstName, baseData.LastName, baseData.HeightCm, baseData.Sex, baseData.FaceEmbedding, baseData.Notes, baseData.Rectangle)
        {
            ProfileImage = profileImage;
        }

    }
}
