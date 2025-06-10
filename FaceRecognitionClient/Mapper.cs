using DataProtocols.FaceRecognitionMessages;
using DataProtocols.FaceRecognitionMessages.Models;
using DataProtocols.GalleryMessages.Models;
using FaceRecognitionClient.InternalDataModels;
using FaceRecognitionClient.Utils;
using System.Drawing;

namespace FaceRecognitionClient
{
    /// <summary>
    /// Mapper is a lightweight, custom object mapping utility.
    /// It allows you to register conversions between types (DTOs ↔ internal models)
    /// and then convert them dynamically during runtime.
    /// </summary>
    public class Mapper
    {
        // A dictionary storing mapping functions between (source type, target type)
        private readonly Dictionary<(Type, Type), Func<object, object>> m_MappingFunctions = new();

        /// <summary>
        /// Constructor initializes default mappings for known DTOs and view models.
        /// </summary>
        public Mapper()
        {
            RegisterDefaultMappings();
        }

        /// <summary>
        /// Registers a mapping function between a source and target type.
        /// </summary>
        public void Register<TSource, TTarget>(Func<TSource, TTarget> mapFunction)
        {
            // Wraps the provided function so it can be stored in a non-generic dictionary
            m_MappingFunctions[(typeof(TSource), typeof(TTarget))] = source => mapFunction((TSource)source);
        }

        /// <summary>
        /// Applies the correct mapping function for the given source object type.
        /// Throws if no mapping has been registered for this source/target pair.
        /// </summary>
        public TTarget Map<TSource, TTarget>(TSource source)
        {
            var key = (typeof(TSource), typeof(TTarget));

            if (!m_MappingFunctions.TryGetValue(key, out var mapper))
            {
                throw new InvalidOperationException($"No mapping registered for {typeof(TSource)} → {typeof(TTarget)}");
            }

            return (TTarget)mapper(source);
        }

        /// <summary>
        /// Registers the default known mappings between DTOs and client-side internal data models.
        /// </summary>
        public void RegisterDefaultMappings()
        {
            // Mapping from server result DTO to internal AdvancedPersonData list
            Register<FaceRecognitionResultDTO, List<AdvancedPersonData>>(dto =>
                dto.Results.ConvertAll(r => new AdvancedPersonData(
                    r.Id,
                    r.GovernmentID,
                    r.FirstName,
                    r.LastName,
                    r.HeightCm,
                    r.Sex,
                    r.FaceEmbedding,
                    r.Notes,
                    r.Rectangle)));

            Register<FaceRecognitionResultDTO, List<AdvancedPersonDataWithImage>>(dto =>
                dto.Results.ConvertAll(r => new AdvancedPersonDataWithImage(
                    r.Id,
                    r.GovernmentID,
                    r.FirstName,
                    r.LastName,
                    r.HeightCm,
                    r.Sex,
                    r.FaceEmbedding,
                    r.Notes,
                    r.Rectangle,
                    ImageProcessingUtils.ConvertBitmapToBitmapImage(ImageProcessingUtils.DecodeBase64ToBitmap(r.ProfilePicture))
                    )));

            // Mapping from DTO with validation errors to internal client-side model
            Register<PersonDataValidationResultDTO, PersonDataValidationResult>(dto =>
                new PersonDataValidationResult
                {
                    FirstNameError = dto.FirstNameError,
                    LastNameError = dto.LastNameError,
                    GovernmentIDError = dto.GovernmentIDError,
                    SexError = dto.SexError
                });

            // Reverse mapping for sending back validation results to the server
            Register<PersonDataValidationResult, PersonDataValidationResultDTO>(model =>
                new PersonDataValidationResultDTO
                {
                    FirstNameError = model.FirstNameError,
                    LastNameError = model.LastNameError,
                    GovernmentIDError = model.GovernmentIDError,
                    SexError = model.SexError
                });

            // Mapping from FaceRecordDTO to AdvancedPersonData
            Register<FaceRecordDTO, AdvancedPersonData>(dto =>
                new AdvancedPersonData(
                    dto.Id,
                    dto.GovernmentID,
                    dto.FirstName,
                    dto.LastName,
                    dto.HeightCm,
                    dto.Sex,
                    dto.FaceEmbedding,
                    dto.Notes,
                    Rectangle.Empty // DTO doesn't contain rectangle info
                ));

            // Mapping from AdvancedPersonData to FaceRecordDTO
            Register<AdvancedPersonData, FaceRecordDTO>(person =>
                new FaceRecordDTO(
                    person.Id,
                    person.GovernmentID,
                    person.FirstName,
                    person.LastName,
                    person.HeightCm,
                    person.Sex,
                    person.FaceEmbedding,
                    person.Notes
                ));

            Register<FaceRecordWithProfilePictureDTO, AdvancedPersonDataWithImage>(dto =>
                new AdvancedPersonDataWithImage(
                    dto.Id,
                    dto.GovernmentID,
                    dto.FirstName,
                    dto.LastName,
                    dto.HeightCm,
                    dto.Sex,
                    dto.FaceEmbedding,
                    dto.Notes,
                    System.Drawing.Rectangle.Empty, // or a real rectangle if you have it
                    ImageProcessingUtils.ConvertBitmapToBitmapImage(ImageProcessingUtils.DecodeBase64ToBitmap(dto.Image))
                ));

        }
    }
}
