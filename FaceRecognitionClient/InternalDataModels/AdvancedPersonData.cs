using System.Drawing;

namespace FaceRecognitionClient.InternalDataModels
{
    public class AdvancedPersonData : BasicPersonData
    {
        public int Id { get; set; }
        public int? HeightCm { get; set; }
        public string Sex { get; set; }
        public float[] FaceEmbedding { get; set; }
        public string Notes { get; set; }
        public Rectangle Rectangle { get; set; }

        public AdvancedPersonData() : base("", "", "", "")
        {
        }

        public AdvancedPersonData(string governmentId, string firstName, string lastName)
            : base($"{firstName} {lastName}", firstName, lastName, governmentId)
        {
        }

        public AdvancedPersonData(int id, string governmentId, string firstName, string lastName, int? heightCm, string sex, float[] faceEmbedding, string notes, Rectangle rectangle) : base($"{firstName} {lastName}", firstName, lastName, governmentId)
        {
            Id = id;
            HeightCm = heightCm;
            Sex = sex;
            FaceEmbedding = faceEmbedding;
            Notes = notes;
            Rectangle = rectangle;
        }
    }
}
