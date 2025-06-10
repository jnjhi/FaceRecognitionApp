using DataProtocols.FaceRecognitionMessages;
using DataProtocols.FaceRecognitionMessages.Models;
using FaceRecognitionClient.InternalDataModels;
using FaceRecognitionClient.MVVMStructures.Views;
using FaceRecognitionClient.Services.FaceRecognitionService;
using FaceRecognitionClient.Services.GalleryService;
using FaceRecognitionClient.Utils;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace FaceRecognitionClient.MVVMStructures.Models.FaceRecognition
{
    /// <summary>
    /// Performs face recognition, draws visual overlays, and communicates with the server.
    /// Manages gallery upload and transforms data for UI display.
    /// </summary>
    public class FaceRecognitionModel
    {
        private readonly IFaceProcessor m_FaceProcessor;
        private readonly INetworkFacade m_NetworkFacade;
        private readonly IGalleryService m_GalleryService;
        private readonly Mapper m_Mapper;

        private const int FontSize = 30;
        private const int PenWidth = 8;
        private const int RectangleExpand = 10;
        private const int TextPadding = 5;

        private static readonly Font OverlayFont = new Font("Arial", FontSize, FontStyle.Bold);
        private static readonly Brush OverlayBrush = new SolidBrush(Color.Red);
        private static readonly Pen OverlayPen = new Pen(Color.Green, PenWidth);

        public FaceRecognitionModel(INetworkFacade networkFacade, IGalleryService galleryService, Mapper mapper)
        {
            m_NetworkFacade = networkFacade;
            m_GalleryService = galleryService;
            m_Mapper = mapper;
            m_FaceProcessor = new FaceProcessor();
        }

        /// <summary>
        /// Sends a face recognition request for a captured image, draws results, and saves image to gallery.
        /// </summary>
        public async Task<FaceRecognitionDisplayData> RecognizeAsync(BitmapImage image)
        {
            // Step 1: Get detected face embeddings from the image
            var detectedFaces = m_FaceProcessor.GetFaceEmbedding(image);
            var faceDataList = PrepareResponse(detectedFaces, image);

            /*
            foreach (var face in faceDataList) TODO : get rid of the bag and delete this part of the code 
            {
                var view = new DebugView(ImageProcessingUtils.ConvertBitmapToBitmapImage(ImageProcessingUtils.DecodeBase64ToBitmap(face.ProfilePictureInString64)));
                view.Show();
            }
            */

            var request = new PreRecognitionFaceDataDTO(faceDataList);

            // Step 2: Send to server and get recognition results
            var answer = await m_NetworkFacade.SendRequestAsync<PreRecognitionFaceDataDTO, FaceRecognitionResultDTO>(request);
            var personDataWithProfileImages = m_Mapper.Map<FaceRecognitionResultDTO, List<AdvancedPersonDataWithImage>>(answer);

            /*
            foreach (var person in personDataWithProfileImages) TODO : get rid of the bag and delete this part of the code 
            {
                var view = new DebugView(person.ProfileImage);
                view.Show();
            }
            */
            // Step 4: Draw annotations on the image 
            var bitmap = ImageProcessingUtils.ConvertBitmapImageToBitmap(image);
            var annotatedBitmap = DrawRecognitionOverlay(bitmap, personDataWithProfileImages);
            var finalImage = ImageProcessingUtils.ConvertBitmapToBitmapImage(annotatedBitmap);

            return new FaceRecognitionDisplayData
            {
                AnnotatedImage = finalImage,
                RecognitionData = personDataWithProfileImages
            };
        }

        private List<PreRecognitionDataDTO> PrepareResponse(List<DetectedFace> detectedFaces, BitmapImage image)
        {
            var currentTime = DateTime.Now;
            return detectedFaces.Select(df => new PreRecognitionDataDTO
            {
                CaptureTime = currentTime,
                ProfilePictureInString64 = ImageProcessingUtils.EncodeBitmapToBase64(ImageProcessingUtils.ConvertBitmapImageToBitmap(ImageProcessingUtils.CropFaceToBitmapImage(image,df.BoundingBox))),
                Embedding = df.Embedding.ToArray(),
                Rectangle = new Rectangle(df.BoundingBox.Left, df.BoundingBox.Top, df.BoundingBox.Width, df.BoundingBox.Height)
            }).ToList();
        }

        private Bitmap DrawRecognitionOverlay(Bitmap image, List<AdvancedPersonDataWithImage> results)
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                foreach (var result in results)
                {
                    var rect = result.Rectangle;
                    var expandedRect = new Rectangle(rect.Left - RectangleExpand, rect.Top - RectangleExpand, rect.Width + 2 * RectangleExpand, rect.Height + 2 * RectangleExpand);
                    g.DrawRectangle(OverlayPen, expandedRect);

                    string label = result.FirstName == "UNKNOWN" ? "Unknown" : $"{result.FirstName}\n{result.LastName}";
                    var textSize = g.MeasureString(label, OverlayFont);

                    float textX = expandedRect.Left + (expandedRect.Width - textSize.Width) / 2;
                    float textY = expandedRect.Top - textSize.Height - TextPadding;

                    g.DrawString(label, OverlayFont, OverlayBrush, textX, textY);
                }
            }

            return image;
        }
    }
}
