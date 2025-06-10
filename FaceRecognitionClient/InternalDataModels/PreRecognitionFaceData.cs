using System.Drawing;

namespace FaceRecognitionClient.InternalDataModels
{
    internal class PreRecognitionFaceData
    {
        public float[] Embedding { get; set; }

        public Rectangle Rectangle { get; set; }
    }
}
