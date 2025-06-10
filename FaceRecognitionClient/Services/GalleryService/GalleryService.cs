using DataProtocols.GalleryMessages;
using DataProtocols.GalleryMessages.Models;
using FaceRecognitionClient.InternalDataModels;
using FaceRecognitionClient.Utils;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace FaceRecognitionClient.Services.GalleryService
{
    public class GalleryService : IGalleryService
    {
        private readonly INetworkFacade m_NetworkFacade;
        private readonly Mapper m_Mapper;

        public GalleryService(INetworkFacade NetworkFacade, Mapper mapper)
        {
            m_NetworkFacade = NetworkFacade;
            m_Mapper = mapper;
        }

        public async Task<List<GalleryImage>> GetGalleryAsync()
        {
            var request = new GetGalleryRequestDTO();
            var response = await m_NetworkFacade.SendRequestAsync<GetGalleryRequestDTO, GetGalleryResponseDTO>(request);
            var results = new List<GalleryImage>();

            foreach (var person in response.Persons)
            {
                var personData = m_Mapper.Map<FaceRecordDTO, AdvancedPersonData>(person);
                var newPerson = new GalleryImage
                {
                    CaptureTime = person.CaptureTime,
                    Person = new AdvancedPersonDataWithImage(personData, DecodeBase64ToBitmapImage(person.Image))
                };
                results.Add(newPerson);
            }
            return results;
        }


        private static BitmapImage DecodeBase64ToBitmapImage(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64)) 
            {
                return null;
            }

            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64);
                using var ms = new MemoryStream(imageBytes);
                return ImageProcessingUtils.ConvertBitmapToBitmapImage(new Bitmap(ms)); // GDI+ can handle JPEG from stream
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static string EncodeBitmapToBase64(Bitmap bitmap)
        {
            if (bitmap == null)
                return null;

            try
            {
                using var ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Jpeg); // Use JPEG for compact encoding
                byte[] imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
