using DlibDotNet;
using DlibDotNet.Dnn;

public class FaceProcessor : IDisposable
{
    private FrontalFaceDetector _FrontalFaceDetector;
    private ShapePredictor _ShapePredictor;
    private LossMetric _LossMetric;

    public FaceProcessor()
    {
        _FrontalFaceDetector = Dlib.GetFrontalFaceDetector();
        _ShapePredictor = ShapePredictor.Deserialize("C:\\Users\\denis\\source\\repos\\FaceRecognition\\FaceRecognitionTestApp\\Models\\shape_predictor_5_face_landmarks.dat");
        _LossMetric = DlibDotNet.Dnn.LossMetric.Deserialize("C:\\Users\\denis\\source\\repos\\FaceRecognition\\FaceRecognitionTestApp\\Models\\dlib_face_recognition_resnet_model_v1.dat");
    }

    public Matrix<float> GetEmbeddingForStorage(string imagePath) 
    {
        using (var img = Dlib.LoadImageAsMatrix<RgbPixel>(imagePath))
        {
            var faces = _FrontalFaceDetector.Operator(img);
            var face = faces[0];

            if (faces.Length == 0)
            {
                throw new Exception("No faces found in image!");
            }
            else if(faces.Length >= 2)
            {
                throw new Exception("provide a clear photo with no more than one person in it");
            }

            var shape = _ShapePredictor.Detect(img, face);
            var faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
            var faceChip = Dlib.ExtractImageChip<RgbPixel>(img, faceChipDetail);

            var jitterImages = JitterImage(faceChip).ToArray();
            var ret = _LossMetric.Operator(jitterImages);
            using (var m = Dlib.Mat(ret))
            using (var faceDescriptor = Dlib.Mean<float>(m))
            {
                return faceDescriptor.Clone();
            }
        }
    }

    public List<Matrix<float>> GetFaceEmbedding(string imagePath)
    {
        var outPut = new List<Matrix<float>>();

        using (var img = Dlib.LoadImageAsMatrix<RgbPixel>(imagePath))
        using (var win = new ImageWindow(img))
        {
            var faces = new List<Matrix<RgbPixel>>();

            // Detect faces in the image.
            foreach (var face in _FrontalFaceDetector.Operator(img))
            {
                var shape = _ShapePredictor.Detect(img, face);
                var faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                var faceChip = Dlib.ExtractImageChip<RgbPixel>(img, faceChipDetail);
                faces.Add(faceChip);
                win.AddOverlay(face);
            }

            if (!faces.Any())
            {
                throw new Exception("No faces found in image!");
            }

            var faceDescriptors = _LossMetric.Operator(faces);

            //erases all the same faces 
            var edges = new List<SamplePair>();
            for (int i = 0; i < faceDescriptors.Count; i++)
            {
                for (int j = i; j < faceDescriptors.Count; j++)
                {
                    var diff = faceDescriptors[i] - faceDescriptors[j];
                    if (Dlib.Length(diff) < 0.6)
                    {
                        edges.Add(new SamplePair((uint)i, (uint)j));
                    } 
                }
            }
            Dlib.ChineseWhispers(edges, 100, out var numberOfDistinctFaces, out var labels);
            Console.WriteLine($"Number of people found in the image: {numberOfDistinctFaces}");

            foreach (var item in faces)
            {
                var ret = _LossMetric.Operator(item);
                using (var m = Dlib.Mat(ret))
                using (var faceDescriptor = Dlib.Mean<float>(m))
                {
                    outPut.Add(faceDescriptor.Clone());
                }
            }


            foreach (var edge in edges)
                edge.Dispose();

            foreach (var descriptor in faceDescriptors)
                descriptor.Dispose();

            foreach (var face in faces)
                face.Dispose();
        }

        return outPut;
    }

    public void Dispose()
    {
        _ShapePredictor?.Dispose();
        _FrontalFaceDetector?.Dispose();
        _LossMetric?.Dispose();
    }

    private static IEnumerable<Matrix<RgbPixel>> JitterImage(Matrix<RgbPixel> img, int numberOfJitters = 100)
    {
        var rnd = new Rand();
        var crops = new List<Matrix<RgbPixel>>();
        for (var i = 0; i < numberOfJitters; ++i) 
        {
            crops.Add(Dlib.JitterImage(img, rnd)); 
        }
            
        return crops;
    }
}