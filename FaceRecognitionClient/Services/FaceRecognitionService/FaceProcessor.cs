using DlibDotNet;
using DlibDotNet.Dnn;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FaceRecognitionClient.Services.FaceRecognitionService
{
    // This class extracts face embeddings from input images using Dlib.NET models.
    public class FaceProcessor : IFaceProcessor
    {
        private string m_ModelsFilePath = "Services\\FaceRecognitionService\\FaceRecognitionTrainedModels\\"; // Relative path to where the Dlib model files are stored

        private FrontalFaceDetector _FrontalFaceDetector; // Face detector using HOG-based algorithm
        private ShapePredictor _ShapePredictor; // Predicts 5 key face landmarks (eyes, nose, mouth corners)
        private LossMetric _LossMetric; // Dlib deep neural network that converts a face to a 128D embedding

        // Constructor — loads all necessary Dlib models from disk
        public FaceProcessor()
        {
            string basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..")); // Get project root directory
            string modelDirectory = Path.Combine(basePath, m_ModelsFilePath); // Full path to the model folder

            string shapePredictorPath = Path.Combine(modelDirectory, "shape_predictor_5_face_landmarks.dat"); // Full path to shape predictor model
            string lossMetricPath = Path.Combine(modelDirectory, "dlib_face_recognition_resnet_model_v1.dat"); // Full path to face recognition model

            _FrontalFaceDetector = Dlib.GetFrontalFaceDetector(); // Initialize the built-in HOG face detector
            _ShapePredictor = ShapePredictor.Deserialize(shapePredictorPath); // Load landmark model from disk
            _LossMetric = LossMetric.Deserialize(lossMetricPath); // Load the embedding model from disk
        }

        // For a single face in the image: returns its embedding for database storage
        public Matrix<float> GetEmbeddingForStorage(BitmapImage image)
        {
            using (var img = BitmapSourceToMatrix(image)) // Convert WPF BitmapImage to Dlib-compatible matrix
            {
                var faces = _FrontalFaceDetector.Operator(img); // Detect all faces in the image

                var face = faces[0]; // Use the first face detected (we assume there’s only one)

                if (faces.Length == 0) // If no face is found, throw error
                {
                    throw new Exception("No faces found in image!");
                }
                else if (faces.Length >= 2) // If more than one face is found, also throw error
                {
                    throw new Exception("provide a clear photo with no more than one person in it");
                }

                var shape = _ShapePredictor.Detect(img, face); // Predict facial landmarks
                var faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25); // Compute how to align and crop the face
                var faceChip = Dlib.ExtractImageChip<RgbPixel>(img, faceChipDetail); // Extract aligned face chip

                var jitterImages = JitterImage(faceChip).ToArray(); // Generate 100 jittered versions of the face

                var ret = _LossMetric.Operator(jitterImages); // Pass jittered faces through embedding model
                using (var m = Dlib.Mat(ret)) // Create Dlib matrix from result
                using (var faceDescriptor = Dlib.Mean<float>(m)) // Average the 100 outputs into a single embedding
                {
                    return faceDescriptor.Clone(); // Return a deep copy of the embedding (Matrix<float>)
                }
            }
        }

        // For a photo with multiple faces: returns list of face embeddings and bounding boxes
        public List<DetectedFace> GetFaceEmbedding(BitmapImage image)
        {
            var results = new List<DetectedFace>(); // Final list of detected faces

            using (var img = BitmapSourceToMatrix(image)) // Convert image to Dlib matrix
            {
                var faceBoxes = _FrontalFaceDetector.Operator(img); // Detect all face locations

                if (faceBoxes.Length == 0) // If no face found, throw error
                {
                    throw new Exception("No faces found in image!");
                }

                foreach (var face in faceBoxes) // Process each detected face
                {
                    var shape = _ShapePredictor.Detect(img, face); // Predict landmarks
                    var chipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25); // Compute alignment for this face
                    var chip = Dlib.ExtractImageChip<RgbPixel>(img, chipDetail); // Crop and align face

                    var embedding = _LossMetric.Operator(new[] { chip })[0]; // Get 128D embedding for the face

                    results.Add(new DetectedFace // Create DetectedFace record
                    {
                        Embedding = embedding.Clone(), // Store the face embedding
                        BoundingBox = new System.Drawing.Rectangle(face.Left, face.Top, (int)face.Width, (int)face.Height) // Store bounding box
                    });

                    chip.Dispose(); // Free native memory
                    embedding.Dispose(); // Free native memory
                }
            }

            return results; // Return list of face records
        }

        // Clean up all unmanaged resources when done (to prevent memory leaks)
        public void Dispose()
        {
            _ShapePredictor?.Dispose(); // Dispose landmark model
            _FrontalFaceDetector?.Dispose(); // Dispose face detector
            _LossMetric?.Dispose(); // Dispose embedding model
        }

        // Generates slightly distorted versions of a face for robustness (random crop, blur, etc.)
        private static IEnumerable<Matrix<RgbPixel>> JitterImage(Matrix<RgbPixel> img, int numberOfJitters = 100)
        {
            var rnd = new Rand(); // Random seed generator
            var crops = new List<Matrix<RgbPixel>>();

            for (var i = 0; i < numberOfJitters; ++i) // Loop 100 times
            {
                crops.Add(Dlib.JitterImage(img, rnd)); // Add a jittered image to list
            }

            return crops; // Return all variations
        }

        // Converts a WPF BitmapSource (used by camera/upload) into Dlib’s Matrix<RgbPixel> format
        public static Matrix<RgbPixel> BitmapSourceToMatrix(BitmapSource source)
        {
            var formatted = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0); // Convert to BGRA for consistent pixel layout

            int width = formatted.PixelWidth;
            int height = formatted.PixelHeight;
            int stride = width * 4; // 4 bytes per pixel
            byte[] data = new byte[height * stride]; // Flat byte array for raw pixel data

            formatted.CopyPixels(data, stride, 0); // Copy pixel data into array

            var mat = new Matrix<RgbPixel>(height, width); // Create target Dlib matrix

            for (int y = 0; y < height; y++) // Loop over each row
            {
                int rowOffset = y * stride;

                for (int x = 0; x < width; x++) // Loop over each column
                {
                    int i = rowOffset + x * 4;
                    byte b = data[i + 0]; // Blue channel
                    byte g = data[i + 1]; // Green channel
                    byte r = data[i + 2]; // Red channel

                    mat[y, x] = new RgbPixel // Write pixel to Dlib matrix
                    {
                        Red = r,
                        Green = g,
                        Blue = b
                    };
                }
            }

            return mat; // Return final Dlib-compatible image
        }
    }
}
